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
    private bool TryLowerForStatement(HIRForStatement forStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        // For loop structure:
        // init
        // loop_start:
        //   if (!test) goto end
        //   body
        //   update
        //   goto loop_start
        // end:

        var perIterationBindings = GetPerIterationLexicalBindingsForForInit(forStmt.Init);

        int loopStartLabel = CreateLabel();
        int loopUpdateLabel = CreateLabel();
        int loopEndLabel = CreateLabel();

        // Spec: CreatePerIterationEnvironment for for-loops with lexical declarations.
        // When the symbol table models loop-head lexical declarations in a dedicated
        // block scope, materialize that scope as a runtime scope instance and recreate
        // it per iteration (before update) so closures capture the correct binding.
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

                // Create the initial loop environment instance and pin it to a stable slot.
                loopScopeTemp = CreateTempVariable();
                DefineTempStorage(loopScopeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName));
                SetTempVariableSlot(loopScopeTemp, CreateAnonymousVariableSlot($"$for_lexenv_{loopStartLabel}", new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName)));
                _methodBodyIR.Instructions.Add(new LIRCreateScopeInstance(loopScopeId, loopScopeTemp));
                _activeScopeTempsByScopeName[loopScopeName] = loopScopeTemp;
            }
        }

        // Fallback to the legacy guarded leaf-scope recreation when we can't materialize
        // a dedicated loop scope instance.
        bool shouldRecreateLeafScopeEachIteration = !useTempPerIterationScope && CanSafelyRecreateLeafScopeForPerIterationBindings(perIterationBindings);

        try
        {
            // Lower init statement (if present)
            if (forStmt.Init != null && !TryLowerStatement(forStmt.Init))
            {
                return false;
            }

            // Loop start label
            lirInstructions.Add(new LIRLabel(loopStartLabel));
            ClearNumericRefinementsAtLabel();

            // Test condition (if present)
            if (forStmt.Test != null)
            {
                if (!TryLowerExpression(forStmt.Test, out var conditionTemp))
                {
                    return false;
                }

                // If the condition is boxed or is an object reference, convert to boolean using IsTruthy
                var conditionStorage = GetTempStorage(conditionTemp);
                bool needsTruthyCheck = conditionStorage.Kind == ValueStorageKind.BoxedValue ||
                    (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object)) ||
                    (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(double));

                if (needsTruthyCheck)
                {
                    var isTruthyTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIsTruthy(conditionTemp, isTruthyTemp));
                    DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    conditionTemp = isTruthyTemp;
                }

                // Branch to end if condition is false
                lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, loopEndLabel));
            }

            // Loop body
            _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopUpdateLabel, forStmt.Label));
            try
            {
                if (!TryLowerStatement(forStmt.Body))
                {
                    return false;
                }
            }
            finally
            {
                _controlFlowStack.Pop();
            }

            // Continue target (for-loops continue runs update, then loops)
            lirInstructions.Add(new LIRLabel(loopUpdateLabel));
            ClearNumericRefinementsAtLabel();

            // CreatePerIterationEnvironment: ensure the update expression mutates the
            // next iteration's environment, not the previous iteration's.
            if (useTempPerIterationScope)
            {
                EmitRecreatePerIterationScopeFromTemp(loopScopeTemp, loopScopeId, loopScopeName!, perIterationBindings);
            }
            else if (shouldRecreateLeafScopeEachIteration)
            {
                EmitRecreateLeafScopeForPerIterationBindings(perIterationBindings);
            }

            // Update expression (if present)
            if (forStmt.Update != null && !TryLowerExpressionDiscardResult(forStmt.Update))
            {
                return false;
            }
            // Note: Update expression result is discarded (e.g., i++ side effect is what matters)

            // Jump back to loop start
            lirInstructions.Add(new LIRBranch(loopStartLabel));

            // Loop end label
            lirInstructions.Add(new LIRLabel(loopEndLabel));
            ClearNumericRefinementsAtLabel();

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
