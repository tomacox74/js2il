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
