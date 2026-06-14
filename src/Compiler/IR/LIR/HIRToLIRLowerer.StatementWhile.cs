using Acornima.Ast;
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

        return true;
    }
}
