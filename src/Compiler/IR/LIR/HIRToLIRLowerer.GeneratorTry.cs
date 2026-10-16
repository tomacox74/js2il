using Jroc.HIR;
using Jroc.IL;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<GeneratorTryCatchFinallyContext> _generatorTryCatchFinallyStack = new();
    private int _generatorExpressionProtectionDepth;

    private const string GeneratorPendingExceptionField = nameof(JavaScriptRuntime.GeneratorScope._genPendingException);
    private const string GeneratorHasPendingExceptionField = nameof(JavaScriptRuntime.GeneratorScope._hasGenPendingException);
    private const string GeneratorPendingReturnField = nameof(JavaScriptRuntime.GeneratorScope._genPendingReturnValue);
    private const string GeneratorHasPendingReturnField = nameof(JavaScriptRuntime.GeneratorScope._hasGenPendingReturn);

    private void ProtectGeneratorExpression(int start)
    {
        if (HasActiveSynchronousExceptionHandler
            || _methodBodyIR.Instructions.Skip(start).Any(instruction => instruction is LIRYield or LIRAwait)
            || !_methodBodyIR.Instructions.Skip(start).Any(instruction =>
                (LIRInstructionInfo.GetEffectsForScheduling(instruction)
                    & (LIRInstructionEffects.MayThrow | LIRInstructionEffects.UnsupportedBarrier)) != 0))
        {
            return;
        }

        // Protect synchronous evaluation only; suspension must remain outside CLR EH regions.
        var tryStart = CreateLabel();
        var tryEnd = CreateLabel();
        var catchStart = CreateLabel();
        var catchEnd = CreateLabel();
        var afterExpression = CreateLabel();
        _methodBodyIR.Instructions.Insert(start, new LIRLabel(tryStart));
        _methodBodyIR.Instructions.Add(new LIRLeave(afterExpression));
        _methodBodyIR.Instructions.Add(new LIRLabel(tryEnd));
        _methodBodyIR.Instructions.Add(new LIRLabel(catchStart));

        var exception = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRStoreException(exception));
        DefineTempStorage(exception, new ValueStorage(ValueStorageKind.Reference, typeof(Exception)));
        var value = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRUnwrapCatchException(exception, value));
        DefineTempStorage(value, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        // Route outside the CLR handler so completion routing can use ordinary branches.
        var dispatch = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRLeave(dispatch));
        _methodBodyIR.Instructions.Add(new LIRLabel(catchEnd));
        _methodBodyIR.Instructions.Add(new LIRLabel(dispatch));
        EmitThrowCompletion(value);
        _methodBodyIR.Instructions.Add(new LIRLabel(afterExpression));
        _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
            ExceptionRegionKind.Catch,
            TryStartLabelId: tryStart,
            TryEndLabelId: tryEnd,
            HandlerStartLabelId: catchStart,
            HandlerEndLabelId: catchEnd,
            CatchType: typeof(Exception)));
    }

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

    private static GeneratorTryCatchFinallyContext CreateGeneratorFinallyContext(
        int finallyEntryLabel,
        int finallyExitLabel,
        bool isInFinally = false)
        => new(
            HasCatch: false,
            CatchEntryLabelId: -1,
            FinallyEntryLabelId: finallyEntryLabel,
            FinallyExitLabelId: finallyExitLabel,
            PendingExceptionFieldName: GeneratorPendingExceptionField,
            HasPendingExceptionFieldName: GeneratorHasPendingExceptionField,
            PendingReturnFieldName: GeneratorPendingReturnField,
            HasPendingReturnFieldName: GeneratorHasPendingReturnField,
            IsInFinally: isInFinally,
            IsInCatch: false);

    private void EmitResetGeneratorPendingCompletions(string scopeName)
    {
        var nullTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
        DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, GeneratorPendingExceptionField, nullTemp));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, GeneratorPendingReturnField, nullTemp));

        var falseTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
        DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, GeneratorHasPendingExceptionField, falseTemp));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, GeneratorHasPendingReturnField, falseTemp));
    }

    private void EmitDispatchGeneratorPendingCompletions(string scopeName, int afterLabel)
    {
        GeneratorTryCatchFinallyContext? outerCtx = null;
        if (TryGetOuterGeneratorTryCatchFinallyContext(out var outer))
        {
            outerCtx = outer;
        }

        int? outerHandlerTarget = null;
        if (outerCtx != null)
        {
            if (outerCtx.HasCatch && !outerCtx.IsInCatch && !outerCtx.IsInFinally)
            {
                outerHandlerTarget = outerCtx.CatchEntryLabelId;
            }
            else if (outerCtx.FinallyEntryLabelId != -1)
            {
                outerHandlerTarget = outerCtx.IsInFinally ? outerCtx.FinallyExitLabelId : outerCtx.FinallyEntryLabelId;
            }
        }

        var checkReturnLabel = CreateLabel();
        var hasExTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, GeneratorHasPendingExceptionField, hasExTemp));
        DefineTempStorage(hasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasExTemp, checkReturnLabel));

        var exTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, GeneratorPendingExceptionField, exTemp));
        DefineTempStorage(exTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        if (outerHandlerTarget.HasValue)
        {
            _methodBodyIR.Instructions.Add(new LIRBranch(outerHandlerTarget.Value));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRThrow(exTemp));
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(checkReturnLabel));

        var hasReturnTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, GeneratorHasPendingReturnField, hasReturnTemp));
        DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasReturnTemp, afterLabel));

        var retTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, GeneratorPendingReturnField, retTemp));
        DefineTempStorage(retTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        if (outerHandlerTarget.HasValue)
        {
            _methodBodyIR.Instructions.Add(new LIRBranch(outerHandlerTarget.Value));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRReturn(retTemp));
        }
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
