using Jroc.HIR;
using Jroc.Services;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<SyncTryFinallyContext> _syncTryFinallyStack = new();

    private sealed record SyncAbruptTarget(
        double Id,
        int MatchedAbsoluteIndex);

    private sealed record SyncTryFinallyContext(
        int FinallyEntryLabelId,
        TempVariable HasPendingReturn,
        TempVariable HasPendingException,
        TempVariable PendingException,
        TempVariable PendingAbruptTarget,
        Dictionary<int, SyncAbruptTarget> AbruptTargets,
        int ProtectedControlFlowDepth,
        bool IsInFinally);

    private bool TryEmitReturnThroughSyncFinally(TempVariable returnValue)
    {
        if (_syncTryFinallyStack.Count == 0)
        {
            return false;
        }

        if (!_methodBodyIR.ReturnEpilogueLabelId.HasValue)
        {
            _methodBodyIR.ReturnEpilogueLabelId = CreateLabel();
        }

        returnValue = EnsureObject(returnValue);
        StoreReturnEpilogueValue(returnValue);

        var ctx = _syncTryFinallyStack.Peek();
        if (ctx.IsInFinally)
        {
            return TryEmitReturnThroughEnclosingSyncFinally();
        }

        StoreBooleanToExistingSlot(ctx.HasPendingReturn, true);
        StoreBooleanToExistingSlot(ctx.HasPendingException, false);
        StoreNumberToExistingSlot(ctx.PendingAbruptTarget, 0d);

        _methodBodyIR.Instructions.Add(new LIRLeave(ctx.FinallyEntryLabelId));
        _needsReturnEpilogueBlock = true;
        return true;
    }

    private bool TryEmitAbruptThroughSyncFinally(
        int targetLabelId,
        int matchedAbsoluteIndex)
    {
        if (_syncTryFinallyStack.Count == 0)
        {
            return false;
        }

        if (_syncTryFinallyStack.Peek().IsInFinally)
        {
            return TryEmitAbruptThroughEnclosingSyncFinally(
                targetLabelId,
                matchedAbsoluteIndex);
        }

        if (matchedAbsoluteIndex
                >= _syncTryFinallyStack.Peek().ProtectedControlFlowDepth)
        {
            return false;
        }

        var ctx = _syncTryFinallyStack.Peek();
        if (!ctx.AbruptTargets.TryGetValue(targetLabelId, out var target))
        {
            target = new SyncAbruptTarget(
                ctx.AbruptTargets.Count + 1d,
                matchedAbsoluteIndex);
            ctx.AbruptTargets[targetLabelId] = target;
        }

        StoreBooleanToExistingSlot(ctx.HasPendingReturn, false);
        StoreBooleanToExistingSlot(ctx.HasPendingException, false);
        StoreNumberToExistingSlot(ctx.PendingAbruptTarget, target.Id);
        _methodBodyIR.Instructions.Add(new LIRLeave(ctx.FinallyEntryLabelId));
        return true;
    }

    private bool TryEmitAbruptThroughEnclosingSyncFinally(
        int targetLabelId,
        int matchedAbsoluteIndex)
    {
        var skipCurrent = true;
        foreach (var ctx in _syncTryFinallyStack)
        {
            if (skipCurrent)
            {
                skipCurrent = false;
                continue;
            }

            if (ctx.IsInFinally
                || matchedAbsoluteIndex
                    >= ctx.ProtectedControlFlowDepth)
            {
                continue;
            }

            if (!ctx.AbruptTargets.TryGetValue(
                    targetLabelId,
                    out var target))
            {
                target = new SyncAbruptTarget(
                    ctx.AbruptTargets.Count + 1d,
                    matchedAbsoluteIndex);
                ctx.AbruptTargets[targetLabelId] = target;
            }

            StoreBooleanToExistingSlot(
                ctx.HasPendingReturn,
                false);
            StoreBooleanToExistingSlot(
                ctx.HasPendingException,
                false);
            StoreNumberToExistingSlot(
                ctx.PendingAbruptTarget,
                target.Id);
            _methodBodyIR.Instructions.Add(
                new LIRLeave(ctx.FinallyEntryLabelId));
            return true;
        }

        return false;
    }

    private bool TryEmitReturnThroughEnclosingSyncFinally()
    {
        var skipCurrent = true;
        foreach (var ctx in _syncTryFinallyStack)
        {
            if (skipCurrent)
            {
                skipCurrent = false;
                continue;
            }

            if (ctx.IsInFinally)
            {
                continue;
            }

            StoreBooleanToExistingSlot(
                ctx.HasPendingReturn,
                true);
            StoreBooleanToExistingSlot(
                ctx.HasPendingException,
                false);
            StoreNumberToExistingSlot(
                ctx.PendingAbruptTarget,
                0d);
            _methodBodyIR.Instructions.Add(
                new LIRLeave(ctx.FinallyEntryLabelId));
            return true;
        }

        return false;
    }

    private TempVariable CreateBooleanSlotTemp(string name, bool initialValue)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(initialValue, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        SetTempVariableSlot(temp, CreateAnonymousVariableSlot(name, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));
        return temp;
    }

    private TempVariable CreateObjectSlotTemp(string name)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(temp, CreateAnonymousVariableSlot(name, new ValueStorage(ValueStorageKind.Reference, typeof(object))));
        return temp;
    }

    private TempVariable CreateNumberSlotTemp(string name, double initialValue)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(initialValue, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        SetTempVariableSlot(temp, CreateAnonymousVariableSlot(name, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));
        return temp;
    }

    private void StoreBooleanToExistingSlot(TempVariable slotTemp, bool value)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(value, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var slot = GetTempVariableSlot(slotTemp);
        if (slot >= 0)
        {
            SetTempVariableSlot(temp, slot);
        }
    }

    private void StoreExceptionToExistingSlot(TempVariable slotTemp)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRStoreException(temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(System.Exception)));
        var slot = GetTempVariableSlot(slotTemp);
        if (slot >= 0)
        {
            SetTempVariableSlot(temp, slot);
        }
    }

    private void StoreNumberToExistingSlot(TempVariable slotTemp, double value)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(value, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var slot = GetTempVariableSlot(slotTemp);
        if (slot >= 0)
        {
            SetTempVariableSlot(temp, slot);
        }
    }

    private static bool ContainsAbruptStatement(HIRStatement statement)
    {
        return statement switch
        {
            HIRReturnStatement
                or HIRThrowStatement
                or HIRBreakStatement
                or HIRContinueStatement => true,
            HIRBlock block => block.Statements.Any(ContainsAbruptStatement),
            HIRLabeledStatement labeled => ContainsAbruptStatement(labeled.Body),
            HIRIfStatement ifStmt => ContainsAbruptStatement(ifStmt.Consequent)
                || (ifStmt.Alternate != null && ContainsAbruptStatement(ifStmt.Alternate)),
            HIRTryStatement tryStmt => ContainsAbruptStatement(tryStmt.TryBlock)
                || (tryStmt.CatchBody != null && ContainsAbruptStatement(tryStmt.CatchBody))
                || (tryStmt.FinallyBody != null && ContainsAbruptStatement(tryStmt.FinallyBody)),
            _ => false
        };
    }
}
