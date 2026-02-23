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

        // If the condition is boxed or is an object reference, convert to boolean using IsTruthy
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
