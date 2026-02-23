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
    private bool TryLowerVariableDeclaration(HIRVariableDeclaration exprStmt)
    {
        // Variable declarations define a new binding in the current scope.
        TempVariable value;

        if (exprStmt.Initializer != null)
        {
            if (!TryLowerExpression(exprStmt.Initializer, out value))
            {
                IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering variable initializer expression {exprStmt.Initializer.GetType().Name} for '{exprStmt.Name.Name}'");
                return false;
            }
        }
        else
        {
            // No initializer means 'undefined'
            value = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(value));
        }

        // Use BindingInfo as key for correct shadowing behavior
        var binding = exprStmt.Name.BindingInfo;

        bool initializerProvesUnboxedDouble = exprStmt.Initializer != null
            && GetTempStorage(value).Kind == ValueStorageKind.UnboxedValue
            && GetTempStorage(value).ClrType == typeof(double);

        bool initializerProvesUnboxedBool = exprStmt.Initializer != null
            && GetTempStorage(value).Kind == ValueStorageKind.UnboxedValue
            && GetTempStorage(value).ClrType == typeof(bool);

        bool initializerProvesUnboxedInt32 = exprStmt.Initializer != null
            && GetTempStorage(value).Kind == ValueStorageKind.UnboxedValue
            && GetTempStorage(value).ClrType == typeof(int);

        // Per-iteration environments: if this binding lives in an active materialized scope instance
        // (e.g., `for (let/const ...)` loop-head scope), store directly into that scope field.
        if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
        {
            TempVariable fieldValue;
            if ((binding.IsStableType && binding.ClrType == typeof(double))
                || (binding.Kind == BindingKind.Const && initializerProvesUnboxedDouble))
            {
                fieldValue = EnsureNumber(value);
            }
            else if ((binding.IsStableType && binding.ClrType == typeof(bool))
                || (binding.Kind == BindingKind.Const && initializerProvesUnboxedBool))
            {
                fieldValue = EnsureBoolean(value);
            }
            else
            {
                fieldValue = EnsureObject(value);
            }

            _methodBodyIR.Instructions.Add(new LIRStoreScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, fieldValue));
            _variableMap[binding] = fieldValue;
            return true;
        }

        // Check if this binding should be stored in a scope field (captured variable)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            // Captured variable - store to leaf scope field
            if (storage != null &&
                storage.Kind == BindingStorageKind.LeafScopeField &&
                !storage.Field.IsNil &&
                !storage.DeclaringScope.IsNil)
            {
                // Captured variable - store to leaf scope field.
                // Most scope fields are object-typed, but stable inferred primitive fields can be typed.
                // For typed bool fields, do NOT box (stfld bool cannot consume object).
                var fieldValue = value;
                if (!(binding.IsStableType && binding.ClrType == typeof(bool)))
                {
                    fieldValue = EnsureObject(value);
                }

                _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, fieldValue));
                // Also map in SSA for subsequent reads (though they'll use field load)
                _variableMap[binding] = value;
                return true;
            }
        }

        // Non-captured variable - store into a stable local slot.
        // IMPORTANT: declared locals should never store raw unboxed JsNull into an object-typed IL local.
        // Use unboxed locals only for proven-stable primitives (double/bool); otherwise box to object.
        TempVariable slotValue;
        if ((binding.IsStableType && binding.ClrType == typeof(double))
            || (binding.Kind == BindingKind.Const && initializerProvesUnboxedDouble))
        {
            slotValue = EnsureNumber(value);
        }
        else if ((binding.IsStableType && binding.ClrType == typeof(bool))
            || (binding.Kind == BindingKind.Const && initializerProvesUnboxedBool))
        {
            slotValue = EnsureBoolean(value);
        }
        else if (binding.Kind == BindingKind.Const && initializerProvesUnboxedInt32)
        {
            slotValue = value; // keep as int32
        }
        else
        {
            slotValue = EnsureObject(value);
        }

        var storageInfo = GetTempStorage(slotValue);
        var slot = GetOrCreateVariableSlot(binding, exprStmt.Name.Name, storageInfo);
        slotValue = EnsureTempMappedToSlot(slot, slotValue);

        _variableMap[binding] = slotValue;

        // Mark all variable slots as single-assignment initially.
        // This will be removed if the variable is reassigned later.
        // const variables are always single-assignment by definition.
        // let/var variables are single-assignment if never reassigned after initialization.
        _methodBodyIR.SingleAssignmentSlots.Add(slot);
        return true;
    }

    private bool TryLowerDestructuringVariableDeclaration(HIRDestructuringVariableDeclaration destructDecl)
    {
        // PL4.1: Destructuring variable declarators.
        if (!TryLowerExpression(destructDecl.Initializer, out var sourceTemp))
        {
            IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering destructuring initializer expression {destructDecl.Initializer.GetType().Name}");
            return false;
        }

        // Destructuring operates on JS values (boxed as object).
        sourceTemp = EnsureObject(sourceTemp);

        var sourceNameForError = TryGetSimpleSourceNameForDestructuring(destructDecl.Initializer);
        if (!TryLowerDestructuringPattern(destructDecl.Pattern, sourceTemp, DestructuringWriteMode.Declaration, sourceNameForError))
        {
            IRPipelineMetrics.RecordFailureIfUnset("HIR->LIR: failed lowering destructuring variable declaration");
            return false;
        }

        return true;
    }
}
