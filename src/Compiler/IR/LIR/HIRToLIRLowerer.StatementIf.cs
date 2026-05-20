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

        // Apply JavaScript truthiness semantics before branching so strings, boxed values,
        // numbers, null/undefined, and object references all behave like ECMAScript tests.
        conditionTemp = EnsureConditionIsBoolean(conditionTemp);

        // Branch to else if condition is false
        lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, elseLabel));

        // Clear refinements conservatively at branch entry; pre-branch refinements are still
        // valid in each arm, but dropping them here keeps branch-handling behavior uniform and
        // avoids carrying arm-specific refinements across later join points.
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
