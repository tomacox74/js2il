using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private void EmitGeneratorStateSwitchIfNeeded()
    {
        if (!_isGenerator)
        {
            return;
        }

        // Async generators need a combined dispatch that prioritizes resumption from await
        // (via _asyncState) over resumption from yield (via _genState). That dispatch is emitted
        // in IL at LIRCreateLeafScopeInstance time.
        if (_isAsync)
        {
            return;
        }

        if (_methodBodyIR.GeneratorInfo == null)
        {
            return;
        }

        // Emit a dispatch at method entry so resume calls jump to the right label.
        // State 0 falls through to the normal entry path.
        var startLabel = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRGeneratorStateSwitch(_methodBodyIR.GeneratorInfo.ResumeLabels, startLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(startLabel));
    }

}
