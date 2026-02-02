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
    private int _tempVarCounter = 0;
    private int _labelCounter = 0;

    private int CreateLabel() => _labelCounter++;

    private TempVariable CreateTempVariable()
    {
        var tempVar = new TempVariable(_tempVarCounter);
        _tempVarCounter++;
        _methodBodyIR.Temps.Add(tempVar);
        _methodBodyIR.TempStorages.Add(new ValueStorage(ValueStorageKind.Unknown));
        _methodBodyIR.TempVariableSlots.Add(-1);
        return tempVar;
    }

    private int GetOrCreateVariableSlot(BindingInfo binding, string displayName, ValueStorage storage)
    {
        if (_variableSlots.TryGetValue(binding, out var slot))
        {
            return slot;
        }

        slot = _methodBodyIR.VariableNames.Count;
        _variableSlots[binding] = slot;
        _methodBodyIR.VariableNames.Add(displayName);
        _methodBodyIR.VariableStorages.Add(storage);
        return slot;
    }

    private int CreateAnonymousVariableSlot(string displayName, ValueStorage storage)
    {
        var slot = _methodBodyIR.VariableNames.Count;
        _methodBodyIR.VariableNames.Add(displayName);
        _methodBodyIR.VariableStorages.Add(storage);
        return slot;
    }

    private void SetTempVariableSlot(TempVariable temp, int slot)
    {
        if (temp.Index < 0 || temp.Index >= _methodBodyIR.TempVariableSlots.Count)
        {
            return;
        }
        _methodBodyIR.TempVariableSlots[temp.Index] = slot;
    }

    private int GetTempVariableSlot(TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= _methodBodyIR.TempVariableSlots.Count)
        {
            return -1;
        }

        return _methodBodyIR.TempVariableSlots[temp.Index];
    }

    private TempVariable EnsureTempMappedToSlot(int slot, TempVariable value)
    {
        var currentSlot = GetTempVariableSlot(value);
        if (currentSlot == -1 || currentSlot == slot)
        {
            SetTempVariableSlot(value, slot);
            return value;
        }

        // Avoid retroactively changing earlier IL for this temp by remapping.
        var copy = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(value, copy));
        DefineTempStorage(copy, GetTempStorage(value));
        SetTempVariableSlot(copy, slot);
        return copy;
    }
}
