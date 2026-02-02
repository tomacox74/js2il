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

        // Test condition
        if (!TryLowerExpression(whileStmt.Test, out var conditionTemp))
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

        return true;
    }
}
