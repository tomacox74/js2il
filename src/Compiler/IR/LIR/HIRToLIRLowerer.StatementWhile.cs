using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerWhileStatement(HIRWhileStatement whileStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;
        int? fastPathEndLabel = null;
        if (whileStmt.BitsetSearchFieldName is { } fieldName
            && !UsesDynamicClassInstanceProperties()
            && TryGetStableThisFieldClrType(fieldName) == typeof(JavaScriptRuntime.Int32Array)
            && whileStmt.Test is HIRCallExpression
            {
                Arguments.Length: 1,
                Arguments: var arguments
            }
            && arguments[0] is HIRVariableExpression indexVariable
            && indexVariable.Name.BindingInfo is
            {
                IsStableType: true, ClrType: var indexType
            }
            && indexType == typeof(double)
            && _parameterIndexMap.TryGetValue(indexVariable.Name.BindingInfo, out var parameterIndex))
        {
            if (!TryLowerExpression(
                    new HIRPropertyAccessExpression(new HIRThisExpression(), fieldName),
                    out var words)
                || !TryLowerExpression(indexVariable, out var indexValue))
            {
                return false;
            }

            var firstZero = CreateTempVariable();
            lirInstructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.FindFirstZeroBitOrNegative),
                [words, EnsureNumber(indexValue)],
                firstZero));
            DefineTempStorage(firstZero, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var zero = CreateTempVariable();
            lirInstructions.Add(new LIRConstNumber(0, zero));
            DefineTempStorage(zero, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            var hasResult = CreateTempVariable();
            lirInstructions.Add(new LIRCompareNumberGreaterThanOrEqual(firstZero, zero, hasResult));
            DefineTempStorage(hasResult, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            var fallback = CreateLabel();
            fastPathEndLabel = CreateLabel();
            lirInstructions.Add(new LIRBranchIfFalse(hasResult, fallback));
            lirInstructions.Add(new LIRStoreParameter(parameterIndex, firstZero));
            lirInstructions.Add(new LIRBranch(fastPathEndLabel.Value));
            lirInstructions.Add(new LIRLabel(fallback));
            ClearNumericRefinementsAtLabel();
        }

        // While loop structure:
        // loop_start:
        //   if (!test) goto end
        //   body
        //   goto loop_start
        // end:

        int loopStartLabel = CreateLabel();
        int loopEndLabel = CreateLabel();

        // Loop start label
        lirInstructions.Add(new LIRLabel(loopStartLabel));
        // Numeric refinements are invalid at a loop header: values may have changed since the
        // previous iteration's refinement was established.
        ClearNumericRefinementsAtLabel();

        // Test condition
        if (!TryLowerExpression(whileStmt.Test, out var conditionTemp))
        {
            return false;
        }

        conditionTemp = EnsureConditionIsBoolean(conditionTemp);

        // Branch to end if condition is false
        lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, loopEndLabel));

        // Loop body
        _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopStartLabel, whileStmt.Label));
        try
        {
            if (!TryLowerStatement(whileStmt.Body))
            {
                return false;
            }
        }
        finally
        {
            _controlFlowStack.Pop();
        }

        // Jump back to loop start
        lirInstructions.Add(new LIRBranch(loopStartLabel));

        // Loop end label
        lirInstructions.Add(new LIRLabel(loopEndLabel));
        ClearNumericRefinementsAtLabel();
        if (fastPathEndLabel is { } endLabel)
        {
            lirInstructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
        }

        return true;
    }
}
