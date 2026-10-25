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
        bool IsInFinally,
        int ProtectedControlFlowDepth = 0,
        TempVariable? PendingAbruptTarget = null,
        Dictionary<int, SyncAbruptTarget>? AbruptTargets = null);

    private TempVariable LoadPendingScopeField(string field, Type type)
    {
        var value = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(_methodBodyIR.LeafScopeId.Name, field, value));
        DefineTempStorage(value, new ValueStorage(
            type == typeof(bool) ? ValueStorageKind.UnboxedValue : ValueStorageKind.Reference, type));
        return value;
    }

    private (string Field, TempVariable Value)[] SavePendingCompletions(params (string Field, Type Type)[] fields)
        => fields.Select(field =>
        {
            var value = LoadPendingScopeField(field.Field, field.Type);
            SetTempVariableSlot(value, CreateAnonymousVariableSlot("$saved_" + field.Field, GetTempStorage(value)));
            return (field.Field, value);
        }).ToArray();

    private void RestorePendingCompletions((string Field, TempVariable Value)[] fields)
    {
        foreach (var (field, value) in fields)
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(_methodBodyIR.LeafScopeId.Name, field, value));
    }

    private bool TryEmitAbruptThroughAsyncFinally(int targetLabel, int matchedAbsoluteIndex)
    {
        foreach (var context in _asyncTryFinallyStack)
        {
            if (matchedAbsoluteIndex >= context.ProtectedControlFlowDepth
                || context.PendingAbruptTarget is not TempVariable pendingTarget
                || context.AbruptTargets is null)
                continue;
            var scope = _methodBodyIR.LeafScopeId.Name;
            var cleared = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, cleared));
            DefineTempStorage(cleared, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scope, context.HasPendingReturnFieldName, cleared));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scope, context.HasPendingExceptionFieldName, cleared));
            if (context.IsInFinally)
                continue;
            if (!context.AbruptTargets.TryGetValue(targetLabel, out var target))
            {
                target = new SyncAbruptTarget(context.AbruptTargets.Count + 1d, matchedAbsoluteIndex);
                context.AbruptTargets.Add(targetLabel, target);
            }
            StoreNumberToExistingSlot(pendingTarget, target.Id);
            _methodBodyIR.Instructions.Add(new LIRBranch(context.FinallyEntryLabelId));
            return true;
        }
        return false;
    }
}
