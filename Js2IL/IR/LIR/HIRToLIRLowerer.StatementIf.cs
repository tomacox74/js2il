using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerIfStatement(HIRIfStatement ifStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        // Evaluate the test condition
        if (!TryLowerExpression(ifStmt.Test, out var conditionTemp))
        {
            IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering if test expression {ifStmt.Test.GetType().Name}");
            return false;
        }

        int elseLabel = CreateLabel();

        // If the condition is boxed or is an object reference, we need to
        // convert it to a boolean using IsTruthy before branching.
        // This is because brfalse on a boxed boolean checks for null, not false,
        // and JavaScript has different truthiness rules (0, "", null, undefined, NaN are falsy).
        var conditionStorage = GetTempStorage(conditionTemp);
        bool needsTruthyCheck = conditionStorage.Kind == ValueStorageKind.BoxedValue ||
            (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object)) ||
            (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(double));

        if (needsTruthyCheck)
        {
            var isTruthyTemp = CreateTempVariable();
            lirInstructions.Add(new LIRCallIsTruthy(conditionTemp, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            conditionTemp = isTruthyTemp;
        }

        // Branch to else if condition is false
        lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, elseLabel));

        // Numeric refinements from before the branch are no longer valid inside the branch
        // body since either the then- or else-path may be taken at runtime.
        ClearNumericRefinementsAtLabel();

        // Consequent block (then)
        if (!TryLowerStatement(ifStmt.Consequent))
        {
            IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering if consequent {ifStmt.Consequent.GetType().Name}");
            return false;
        }

        // Alternate block (else) - if present
        if (ifStmt.Alternate != null)
        {
            // Jump over else block
            int endLabel = CreateLabel();
            lirInstructions.Add(new LIRBranch(endLabel));

            // Else label
            lirInstructions.Add(new LIRLabel(elseLabel));
            ClearNumericRefinementsAtLabel();

            if (!TryLowerStatement(ifStmt.Alternate))
            {
                IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering if alternate {ifStmt.Alternate.GetType().Name}");
                return false;
            }

            // End label
            lirInstructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
        }
        else
        {
            // No else block - just emit the else label (which is effectively the end)
            lirInstructions.Add(new LIRLabel(elseLabel));
            ClearNumericRefinementsAtLabel();
        }

        return true;
    }
}
