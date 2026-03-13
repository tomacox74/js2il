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
    private readonly Stack<AsyncTryCatchContext> _asyncTryCatchStack = new();
    private readonly Stack<AsyncTryFinallyContext> _asyncTryFinallyStack = new();

    private sealed record AsyncTryCatchContext(int CatchStateId, int CatchLabelId, string PendingExceptionFieldName);

    private sealed record AsyncTryFinallyContext(
        int FinallyEntryLabelId,
        int FinallyExitLabelId,
        string PendingExceptionFieldName,
        string HasPendingExceptionFieldName,
        string PendingReturnFieldName,
        string HasPendingReturnFieldName,
        bool IsInFinally);
}
