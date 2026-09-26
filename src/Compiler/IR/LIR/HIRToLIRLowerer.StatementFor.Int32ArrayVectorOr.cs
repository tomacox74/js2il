using Jroc.HIR;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryEmitInt32ArrayVectorOrFastPath(
        HIRForStatement loop, HIRInt32ArrayVectorOrPattern pattern, out int endLabel)
    {
        endLabel = default;
        if (UsesDynamicClassInstanceProperties()
            || TryGetStableThisFieldClrType(pattern.ArrayField) != typeof(JavaScriptRuntime.Int32Array)
            || TryGetStableThisFieldClrType(pattern.EndField) != typeof(double)
            || loop.Init is not HIRVariableDeclaration { Name: var indexSymbol }
            || indexSymbol.BindingInfo.IsCaptured
            || UnwrapSingleStatement(loop.Body) is not HIRExpressionStatement
            {
                Expression: HIRIndexAssignmentExpression
            })
        {
            return false;
        }

        if (!TryLowerExpression(
                new HIRPropertyAccessExpression(new HIRThisExpression(), pattern.ArrayField),
                out var array)
            || !TryLowerExpression(new HIRVariableExpression(indexSymbol), out var start)
            || !TryLowerExpression(
                new HIRPropertyAccessExpression(new HIRThisExpression(), pattern.EndField),
                out var end))
        {
            throw new InvalidOperationException("A recognized vector range could not be lowered.");
        }

        endLabel = EmitInt32ArrayVectorOrRange(array, start, end, pattern.Mask);
        return true;
    }

    private int EmitInt32ArrayVectorOrRange(
        TempVariable array, TempVariable start, TempVariable end, double maskValue,
        int? existingFallbackLabel = null)
    {
        var mask = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(maskValue, mask));
        DefineTempStorage(mask, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var applied = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRVectorInt32Range(
            new LIRVectorInt32Load(array, EnsureNumber(start), EnsureNumber(end)),
            new LIRVectorInt32Or(mask),
            new LIRVectorInt32Store(array),
            applied));
        DefineTempStorage(applied, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        var fallbackLabel = existingFallbackLabel ?? CreateLabel();
        var endLabel = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(applied, fallbackLabel));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(fallbackLabel));
        ClearNumericRefinementsAtLabel();
        return endLabel;
    }
}
