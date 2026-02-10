using Js2IL.HIR;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<GeneratorTryFinallyContext> _generatorTryFinallyStack = new();

    private sealed record GeneratorTryFinallyContext(
    bool HasCatch,
    int CatchEntryLabelId,
        int FinallyEntryLabelId,
        int FinallyExitLabelId,
        string PendingExceptionFieldName,
        string HasPendingExceptionFieldName,
        string PendingReturnFieldName,
        string HasPendingReturnFieldName,
    bool IsInFinally,
    bool IsInCatch);
}
