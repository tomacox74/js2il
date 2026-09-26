using Jroc.HIR;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryEmitInt32ArrayLocalVectorOrFastPath(
        HIRForStatement loop, HIRInt32ArrayLocalVectorOrPattern pattern, out int endLabel)
    {
        endLabel = default;
        if (loop.Init is not HIRVariableDeclaration { Name: var index }
            || index.BindingInfo.IsCaptured
            || loop.Test is not HIRBinaryExpression
            {
                Operator: Acornima.Operator.LessThan,
                Left: HIRVariableExpression testIndex,
                Right: var limitExpression
            }
            || !ReferenceEquals(testIndex.Name.BindingInfo, index.BindingInfo)
            || !IsStableVectorBound(limitExpression)
            || UnwrapSingleStatement(loop.Body) is not HIRExpressionStatement
            {
                Expression: HIRIndexAssignmentExpression
                {
                    Operator: Acornima.Operator.BitwiseOrAssignment,
                    Object: HIRVariableExpression array,
                    Index: HIRVariableExpression elementIndex,
                    Value: HIRLiteralExpression { Value: double mask }
                }
            }
            || !ReferenceEquals(elementIndex.Name.BindingInfo, index.BindingInfo)
            || array.Name.BindingInfo is not
            {
                Kind: BindingKind.Const,
                IsStableType: true,
                ClrType: var arrayType
            }
            || arrayType != typeof(JavaScriptRuntime.Int32Array)
            || mask != pattern.Mask)
        {
            return false;
        }

        if (!TryLowerExpression(new HIRVariableExpression(index), out var start)
            || !TryLowerExpression(limitExpression, out var limit))
        {
            throw new InvalidOperationException("A recognized local vector range could not be lowered.");
        }

        var numericStart = EnsureNumber(start);
        var numericLimit = EnsureNumber(limit);
        var hasIterations = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThan(
            numericStart, numericLimit, hasIterations));
        DefineTempStorage(hasIterations, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var fallbackLabel = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasIterations, fallbackLabel));
        if (!TryLowerExpression(array, out var words))
        {
            throw new InvalidOperationException("A recognized vector receiver could not be lowered.");
        }

        endLabel = EmitInt32ArrayVectorOrRange(
            words, numericStart, numericLimit, pattern.Mask, fallbackLabel);
        return true;
    }

    private static bool IsStableVectorBound(HIRExpression expression)
        => expression switch
        {
            HIRLiteralExpression { Value: double } => true,
            HIRVariableExpression
            {
                Name.BindingInfo:
                {
                    Kind: BindingKind.Const,
                    IsStableType: true,
                    ClrType: var boundType
                }
            } => boundType == typeof(double),
            _ => false
        };
}
