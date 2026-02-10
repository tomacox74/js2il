using Js2IL.HIR;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<GeneratorTryCatchFinallyContext> _generatorTryCatchFinallyStack = new();

    private bool TryGetOuterGeneratorTryCatchFinallyContext(out GeneratorTryCatchFinallyContext outerCtx)
    {
        outerCtx = null!;

        if (_generatorTryCatchFinallyStack.Count <= 1)
        {
            return false;
        }

        var enumerator = _generatorTryCatchFinallyStack.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return false;
        }

        // Second element in Stack enumeration is the next "outer" context.
        if (!enumerator.MoveNext())
        {
            return false;
        }

        outerCtx = enumerator.Current;
        return true;
    }

    private sealed record GeneratorTryCatchFinallyContext(
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
