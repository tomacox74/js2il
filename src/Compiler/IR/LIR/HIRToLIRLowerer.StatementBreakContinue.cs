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
    private bool TryLowerBreakStatement(HIRBreakStatement breakStmt)
    {
        if (!TryResolveControlFlowTarget(breakStmt.Label, out var target, out var matchedAbsoluteIndex, isBreak: true))
        {
            return false;
        }

        if (_protectedControlFlowDepthStack.Count > 0 && matchedAbsoluteIndex < _protectedControlFlowDepthStack.Peek())
        {
            _methodBodyIR.Instructions.Add(new LIRLeave(target));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRBranch(target));
        }

        return true;
    }

    private bool TryLowerContinueStatement(HIRContinueStatement continueStmt)
    {
        if (!TryResolveControlFlowTarget(continueStmt.Label, out var target, out var matchedAbsoluteIndex, isBreak: false))
        {
            return false;
        }

        if (_protectedControlFlowDepthStack.Count > 0 && matchedAbsoluteIndex < _protectedControlFlowDepthStack.Peek())
        {
            _methodBodyIR.Instructions.Add(new LIRLeave(target));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRBranch(target));
        }

        return true;
    }
}
