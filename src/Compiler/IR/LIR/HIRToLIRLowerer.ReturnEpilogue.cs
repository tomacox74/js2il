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
    private int? _returnEpilogueReturnSlot;
    private TempVariable? _returnEpilogueLoadTemp;
    private bool _needsReturnEpilogueBlock;

    private void EnsureReturnEpilogueStorage()
    {
        if (_returnEpilogueReturnSlot.HasValue && _returnEpilogueLoadTemp.HasValue)
        {
            return;
        }

        // Reserve a stable slot for the return value.
        var slot = CreateAnonymousVariableSlot("$return", new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _returnEpilogueReturnSlot = slot;

        // Create a load temp mapped to the slot so epilogue can return it.
        var loadTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(loadTemp));
        DefineTempStorage(loadTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(loadTemp, slot);
        _returnEpilogueLoadTemp = loadTemp;
    }

    private bool TryEmitReturnViaEpilogue(TempVariable returnValue)
    {
        if (!_methodBodyIR.ReturnEpilogueLabelId.HasValue)
        {
            return false;
        }

        EnsureReturnEpilogueStorage();

        // Store return value into the dedicated slot.
        var storeTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(returnValue, storeTemp));
        DefineTempStorage(storeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(storeTemp, _returnEpilogueReturnSlot!.Value);

        // Leave to epilogue (outside of try/finally so finally executes).
        _methodBodyIR.Instructions.Add(new LIRLeave(_methodBodyIR.ReturnEpilogueLabelId.Value));
        _needsReturnEpilogueBlock = true;
        return true;
    }
}
