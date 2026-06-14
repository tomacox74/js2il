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
    private bool TryLowerDoWhileStatement(HIRDoWhileStatement doWhileStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        // Do/while loop structure:
        // loop_start:
        //   body
        // loop_test:
        //   if (!test) goto end
        //   goto loop_start
        // end:

        int loopStartLabel = CreateLabel();
        int loopTestLabel = CreateLabel();
        int loopEndLabel = CreateLabel();

        // Loop start label
        lirInstructions.Add(new LIRLabel(loopStartLabel));
        ClearNumericRefinementsAtLabel();

        // Loop body (always executes at least once)
        _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopTestLabel, doWhileStmt.Label));
        try
        {
            if (!TryLowerStatement(doWhileStmt.Body))
            {
                return false;
            }
        }
        finally
        {
            _controlFlowStack.Pop();
        }

        // Continue target (do/while continue should skip remainder of body and go to test)
        lirInstructions.Add(new LIRLabel(loopTestLabel));
        ClearNumericRefinementsAtLabel();

        // Test condition
        if (!TryLowerExpression(doWhileStmt.Test, out var conditionTemp))
        {
            return false;
        }

        conditionTemp = EnsureConditionIsBoolean(conditionTemp);

        // Branch to end if condition is false
        lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, loopEndLabel));

        // Jump back to loop start
        lirInstructions.Add(new LIRBranch(loopStartLabel));

        // Loop end label
        lirInstructions.Add(new LIRLabel(loopEndLabel));
        ClearNumericRefinementsAtLabel();

        return true;
    }
}
