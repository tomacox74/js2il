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
