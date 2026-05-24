using Js2IL.HIR;
using Js2IL.Services;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly Stack<SyncTryFinallyContext> _syncTryFinallyStack = new();

    private sealed record SyncTryFinallyContext(
        int FinallyEntryLabelId,
        TempVariable HasPendingReturn,
        TempVariable HasPendingException,
        TempVariable PendingException,
        bool IsInFinally);

    private bool TryEmitReturnThroughSyncFinally(TempVariable returnValue)
    {
        if (_syncTryFinallyStack.Count == 0 || _syncTryFinallyStack.Peek().IsInFinally)
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

        StoreBooleanToExistingSlot(ctx.HasPendingReturn, true);
        StoreBooleanToExistingSlot(ctx.HasPendingException, false);

        _methodBodyIR.Instructions.Add(new LIRLeave(ctx.FinallyEntryLabelId));
        _needsReturnEpilogueBlock = true;
        return true;
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
        _methodBodyIR.Instructions.Add(new LIRConstNull(temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(temp, CreateAnonymousVariableSlot(name, new ValueStorage(ValueStorageKind.Reference, typeof(object))));
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

    private static bool ContainsAbruptStatement(HIRStatement statement)
    {
        return statement switch
        {
            HIRReturnStatement or HIRThrowStatement => true,
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
