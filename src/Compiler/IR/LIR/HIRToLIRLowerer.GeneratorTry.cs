using Js2IL.HIR;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<GeneratorTryCatchFinallyContext> _generatorTryCatchFinallyStack = new();

    private const string GeneratorPendingExceptionField = nameof(JavaScriptRuntime.GeneratorScope._genPendingException);
    private const string GeneratorHasPendingExceptionField = nameof(JavaScriptRuntime.GeneratorScope._hasGenPendingException);
    private const string GeneratorPendingReturnField = nameof(JavaScriptRuntime.GeneratorScope._genPendingReturnValue);
    private const string GeneratorHasPendingReturnField = nameof(JavaScriptRuntime.GeneratorScope._hasGenPendingReturn);

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
