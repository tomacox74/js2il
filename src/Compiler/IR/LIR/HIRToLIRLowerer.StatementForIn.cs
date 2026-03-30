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
    private bool TryLowerForInStatement(HIRForInStatement forInStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        // Desugar for..in:
        // keys = Object.GetEnumerableKeys(rhs)
        // len = keys.length
        // idx = 0
        // loop_start:
        //   if (!(idx < len)) goto end
        //   target = keys[idx]
        //   body
        // loop_update:
        //   idx = idx + 1
        //   goto loop_start
        // end:

        var perIterationBindings = (forInStmt.IsDeclaration && (forInStmt.DeclarationKind is BindingKind.Let or BindingKind.Const))
            ? forInStmt.LoopHeadBindings.Where(b => b.Kind is BindingKind.Let or BindingKind.Const).Where(b => b.IsCaptured).ToList()
            : new List<BindingInfo>();

        bool useTempPerIterationScope = false;
        TempVariable loopScopeTemp = default;
        ScopeId loopScopeId = default;
        string? loopScopeName = null;

        if (perIterationBindings.Count > 0
            && !_methodBodyIR.IsAsync
            && !_methodBodyIR.IsGenerator)
        {
            var declaringScope = perIterationBindings[0].DeclaringScope;
            if (declaringScope != null
                && declaringScope.Kind == ScopeKind.Block
                && perIterationBindings.All(b => b.DeclaringScope == declaringScope))
            {
                useTempPerIterationScope = true;
                loopScopeName = ScopeNaming.GetRegistryScopeName(declaringScope);
                loopScopeId = new ScopeId(loopScopeName);

                loopScopeTemp = CreateTempVariable();
                DefineTempStorage(loopScopeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName));
                SetTempVariableSlot(loopScopeTemp, CreateAnonymousVariableSlot($"$forIn_lexenv", new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName)));
                _methodBodyIR.Instructions.Add(new LIRCreateScopeInstance(loopScopeId, loopScopeTemp));
                _activeScopeTempsByScopeName[loopScopeName] = loopScopeTemp;
            }
        }

        try
        {
            if (!TryLowerExpression(forInStmt.Enumerable, out var rhsTemp))
            {
                return false;
            }

            var rhsBoxed = EnsureObject(rhsTemp);

            // Spec-aligned: for..in uses a For-In Iterator object that re-checks key existence
            // per step (e.g., deletion during enumeration). We model this via a native iterator.
            var iterTemp = CreateTempVariable();
            lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.EnumerateObjectProperties), new[] { rhsBoxed }, iterTemp));
            DefineTempStorage(iterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            // Pin loop-carry temps to stable variable slots (see note in for..of lowering).
            SetTempVariableSlot(iterTemp, CreateAnonymousVariableSlot("$forIn_iter", new ValueStorage(ValueStorageKind.Reference, typeof(object))));

            int loopStartLabel = CreateLabel();
            int loopUpdateLabel = CreateLabel();
            int loopEndLabel = CreateLabel();

            lirInstructions.Add(new LIRLabel(loopStartLabel));

            // result = IteratorNext(iterator)
            var iterResult = CreateTempVariable();
            lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorNext), new[] { EnsureObject(iterTemp) }, iterResult));
            DefineTempStorage(iterResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // done = IteratorResultDone(result)
            var doneBool = CreateTempVariable();
            lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorResultDone), new[] { EnsureObject(iterResult) }, doneBool));
            DefineTempStorage(doneBool, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            lirInstructions.Add(new LIRBranchIfTrue(doneBool, loopEndLabel));

            // key = IteratorResultValue(result)
            var keyTemp = CreateTempVariable();
            lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorResultValue), new[] { EnsureObject(iterResult) }, keyTemp));
            DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var writeMode = (forInStmt.IsDeclaration && (forInStmt.DeclarationKind is BindingKind.Let or BindingKind.Const))
                ? DestructuringWriteMode.ForDeclarationBindingInitialization
                : DestructuringWriteMode.Assignment;

            if (!TryLowerDestructuringPattern(forInStmt.Target, keyTemp, writeMode, sourceNameForError: null))
            {
                return false;
            }

            _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopUpdateLabel, forInStmt.Label));
            try
            {
                if (!TryLowerStatement(forInStmt.Body))
                {
                    return false;
                }
            }
            finally
            {
                _controlFlowStack.Pop();
            }

            lirInstructions.Add(new LIRLabel(loopUpdateLabel));

            if (useTempPerIterationScope)
            {
                EmitRecreatePerIterationScopeFromTemp(loopScopeTemp, loopScopeId, loopScopeName!, perIterationBindings);
            }

            // Continue enumeration.
            lirInstructions.Add(new LIRBranch(loopStartLabel));
            lirInstructions.Add(new LIRLabel(loopEndLabel));
            return true;
        }
        finally
        {
            if (useTempPerIterationScope && loopScopeName != null)
            {
                _activeScopeTempsByScopeName.Remove(loopScopeName);
            }
        }
    }
}
