using Jroc.HIR;
using Jroc.Services;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryEmitBitsetCountFastPath(
        HIRForStatement loop, HIRBitsetCountPattern pattern, out int endLabel)
    {
        endLabel = default;
        if (UsesDynamicClassInstanceProperties()
            || !TryGetEnclosingClassRegistryName(out var className)
            || className is null
            || _classRegistry is null
            || !_classRegistry.TryGetFieldTypeHandle(className, pattern.ReceiverField, out var receiverTypeHandle)
            || receiverTypeHandle.IsNil
            || TryGetStableThisFieldClrType(pattern.EndField) != typeof(double)
            || loop.Init is not HIRVariableDeclaration { Name: var indexSymbol }
            || indexSymbol.BindingInfo.IsCaptured
            || UnwrapSingleStatement(loop.Body) is not HIRIfStatement conditional
            || UnwrapSingleStatement(conditional.Consequent) is not HIRExpressionStatement
            {
                Expression: HIRUpdateExpression
                {
                    Argument: HIRVariableExpression totalVariable
                }
            }
            || totalVariable.Name.Name != pattern.TotalName
            || !CanUseStablePrimitiveLocal(totalVariable.Name.BindingInfo, typeof(double)))
        {
            return false;
        }

        var instructions = _methodBodyIR.Instructions;
        if (!TryLowerExpression(
                new HIRPropertyAccessExpression(new HIRThisExpression(), pattern.ReceiverField),
                out var receiver)
            || !TryLowerExpression(
                new HIRVariableExpression(indexSymbol), out var start)
            || !TryLowerExpression(
                new HIRPropertyAccessExpression(new HIRThisExpression(), pattern.EndField),
                out var limit)
            || !TryLowerExpression(totalVariable, out var initialTotal))
        {
            throw new InvalidOperationException("A recognized bitset count could not be lowered.");
        }

        var receiverStorage = new ValueStorage(
            ValueStorageKind.Reference, typeof(object), receiverTypeHandle);
        DefineTempStorage(receiver, receiverStorage);

        var fieldName = CreateTempVariable();
        instructions.Add(new LIRConstString(pattern.WordsField, fieldName));
        DefineTempStorage(fieldName, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var clearBits = CreateTempVariable();
        instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.CountZeroBitsInCompiledBitsetOrNegative),
            [receiver, fieldName, EnsureNumber(start), EnsureNumber(limit)],
            clearBits,
            GenericTypeArgument: receiverStorage));
        DefineTempStorage(clearBits, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var zero = CreateTempVariable();
        instructions.Add(new LIRConstNumber(0, zero));
        DefineTempStorage(zero, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var canUseResult = CreateTempVariable();
        instructions.Add(new LIRCompareNumberGreaterThanOrEqual(clearBits, zero, canUseResult));
        DefineTempStorage(canUseResult, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var fallbackLabel = CreateLabel();
        endLabel = CreateLabel();
        instructions.Add(new LIRBranchIfFalse(canUseResult, fallbackLabel));

        var adjustedTotal = CreateTempVariable();
        instructions.Add(new LIRAddNumber(EnsureNumber(initialTotal), clearBits, adjustedTotal));
        DefineTempStorage(adjustedTotal, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var totalBinding = totalVariable.Name.BindingInfo;
        var hadPrior = _variableMap.TryGetValue(totalBinding, out var priorValue);
        if (!TryStoreToBinding(totalBinding, adjustedTotal, out _))
        {
            throw new InvalidOperationException("The bitset count result could not be stored.");
        }
        if (hadPrior)
        {
            _variableMap[totalBinding] = priorValue;
        }
        else
        {
            _variableMap.Remove(totalBinding);
        }

        instructions.Add(new LIRBranch(endLabel));
        instructions.Add(new LIRLabel(fallbackLabel));
        ClearNumericRefinementsAtLabel();
        return true;
    }

    private static HIRStatement UnwrapSingleStatement(HIRStatement statement)
    {
        while (statement is HIRBlock block)
        {
            var meaningful = block.Statements
                .Where(item => item is not HIRSequencePointStatement)
                .ToArray();
            if (meaningful.Length != 1)
            {
                break;
            }
            statement = meaningful[0];
        }
        return statement;
    }
}
