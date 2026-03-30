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

    private static bool ContainsYieldExpression(Acornima.Ast.Node node, Acornima.Ast.Node functionBoundaryNode)
    {
        bool found = false;

        void Walk(Acornima.Ast.Node? n)
        {
            if (n == null || found)
            {
                return;
            }

            if (n is Acornima.Ast.YieldExpression)
            {
                found = true;
                return;
            }

            // Do not traverse into nested function boundaries.
            if (n is Acornima.Ast.FunctionDeclaration or Acornima.Ast.FunctionExpression or Acornima.Ast.ArrowFunctionExpression
                && !ReferenceEquals(n, functionBoundaryNode))
            {
                return;
            }

            foreach (var child in n.ChildNodes)
            {
                Walk(child);
                if (found) return;
            }
        }

        Walk(node);
        return found;
    }
}
