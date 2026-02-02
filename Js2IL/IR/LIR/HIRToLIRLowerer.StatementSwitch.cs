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
    private bool TryLowerSwitchStatement(HIRSwitchStatement switchStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        if (!TryLowerExpression(switchStmt.Discriminant, out var discriminantTemp))
        {
            return false;
        }

        discriminantTemp = EnsureObject(discriminantTemp);

        var endLabel = CreateLabel();
        _controlFlowStack.Push(new ControlFlowContext(endLabel, null, null));

        try
        {
            // Create a label for each case start.
            var caseLabels = new int[switchStmt.Cases.Length];
            for (int i = 0; i < caseLabels.Length; i++)
            {
                caseLabels[i] = CreateLabel();
            }

            int? defaultCaseIndex = null;
            for (int i = 0; i < switchStmt.Cases.Length; i++)
            {
                if (switchStmt.Cases[i].Test == null)
                {
                    defaultCaseIndex = i;
                    break;
                }
            }

            // Dispatch: compare discriminant === caseTest in order.
            for (int i = 0; i < switchStmt.Cases.Length; i++)
            {
                var sc = switchStmt.Cases[i];
                if (sc.Test == null)
                {
                    continue;
                }

                if (!TryLowerExpression(sc.Test, out var testTemp))
                {
                    return false;
                }

                testTemp = EnsureObject(testTemp);
                var cmpTemp = CreateTempVariable();
                lirInstructions.Add(new LIRStrictEqualDynamic(discriminantTemp, testTemp, cmpTemp));
                DefineTempStorage(cmpTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                lirInstructions.Add(new LIRBranchIfTrue(cmpTemp, caseLabels[i]));
            }

            // No match: jump to default if present, else end.
            lirInstructions.Add(new LIRBranch(defaultCaseIndex.HasValue ? caseLabels[defaultCaseIndex.Value] : endLabel));

            // Emit case bodies in order; fallthrough is natural.
            for (int i = 0; i < switchStmt.Cases.Length; i++)
            {
                lirInstructions.Add(new LIRLabel(caseLabels[i]));
                foreach (var cons in switchStmt.Cases[i].Consequent)
                {
                    if (!TryLowerStatement(cons))
                    {
                        return false;
                    }
                }
            }

            // End of switch.
            lirInstructions.Add(new LIRLabel(endLabel));
            return true;
        }
        finally
        {
            _controlFlowStack.Pop();
        }
    }
}
