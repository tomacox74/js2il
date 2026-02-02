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
    private bool TryLowerThrowStatement(HIRThrowStatement throwStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        if (!TryLowerExpression(throwStmt.Argument, out var argTemp))
        {
            return false;
        }

        argTemp = EnsureObject(argTemp);

        // In async MoveNext with awaits, do not emit CLR throws (they won't be caught by the runtime).
        // Instead reject the deferred promise unless we are inside an async try/catch/finally routing context.
        if (_isAsync
            && _methodBodyIR.AsyncInfo?.HasAwaits == true
            && _asyncTryCatchStack.Count == 0
            && !_methodBodyIR.LeafScopeId.IsNil)
        {
            _methodBodyIR.Instructions.Add(new LIRAsyncReject(argTemp));
            return true;
        }

        if (_asyncTryCatchStack.Count > 0 && !_methodBodyIR.LeafScopeId.IsNil)
        {
            var ctx = _asyncTryCatchStack.Peek();
            var scopeName = _methodBodyIR.LeafScopeId.Name;
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, argTemp));
            _methodBodyIR.Instructions.Add(new LIRBranch(ctx.CatchLabelId));
            return true;
        }

        lirInstructions.Add(new LIRThrow(argTemp));
        return true;
    }
}
