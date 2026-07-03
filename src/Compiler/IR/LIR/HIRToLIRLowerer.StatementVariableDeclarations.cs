using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryApplyInferredNameToDeclarationValue(HIRVariableDeclaration declaration, TempVariable value, out TempVariable namedValue)
    {
        namedValue = value;

        if (string.IsNullOrWhiteSpace(declaration.Name.Name))
        {
            return true;
        }

        string? runtimeMethodName = declaration.Initializer switch
        {
            HIRInitializedUserClassTypeExpression initializedClassExpr
                when initializedClassExpr.ClassScope.AstNode is ClassExpression { Id: null }
                => nameof(JavaScriptRuntime.RuntimeServices.SetClassConstructorInferredName),

            HIRFunctionExpression funcExpr
                when funcExpr.CallableId.AstNode is FunctionExpression { Id: null }
                    && funcExpr.FunctionScope.SyntheticOriginatingNode == null
                => nameof(JavaScriptRuntime.RuntimeServices.SetFunctionInferredName),

            HIRArrowFunctionExpression
                => nameof(JavaScriptRuntime.RuntimeServices.SetFunctionInferredName),

            _ => null
        };

        if (runtimeMethodName == null)
        {
            return true;
        }

        var inferredNameTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(declaration.Name.Name, inferredNameTemp));
        DefineTempStorage(inferredNameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        namedValue = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            MethodName: runtimeMethodName,
            Arguments: new[] { EnsureObject(value), EnsureObject(inferredNameTemp) },
            Result: namedValue));
        DefineTempStorage(namedValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

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

            if (!TryApplyInferredNameToDeclarationValue(exprStmt, value, out value))
            {
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

        // Per-iteration environments: if this binding lives in an active materialized scope instance
        // (e.g., `for (let/const ...)` loop-head scope), store directly into that scope field.
        if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
        {
            TempVariable fieldValue;
            if ((binding.Kind != BindingKind.Var && binding.IsStableType && binding.ClrType == typeof(double))
                || (binding.Kind == BindingKind.Const && initializerProvesUnboxedDouble))
            {
                fieldValue = EnsureNumber(value);
            }
            else if ((binding.Kind != BindingKind.Var && binding.IsStableType && binding.ClrType == typeof(bool))
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
            TryMirrorGlobalVarBinding(binding, fieldValue);
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
                if (binding.Kind != BindingKind.Var && binding.IsStableType && binding.ClrType == typeof(bool))
                {
                    fieldValue = EnsureBoolean(value);
                }
                else
                {
                    fieldValue = EnsureObject(value);
                }

                _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, fieldValue));
                // Also map in SSA for subsequent reads (though they'll use field load)
                _variableMap[binding] = value;
                TryMirrorGlobalVarBinding(binding, fieldValue);
                return true;
            }
        }

        if (TryInitializeStringBuilderAccumulator(binding, exprStmt.Name.Name, exprStmt.Initializer, value))
        {
            TryMirrorGlobalVarBinding(binding, value);
            return true;
        }

        // Non-captured variable - store into a stable local slot.
        // IMPORTANT: declared locals should never store raw unboxed JsNull into an object-typed IL local.
        // Use unboxed locals only for proven-stable primitives (double/bool); otherwise box to object.
        TempVariable slotValue;
        if ((binding.Kind != BindingKind.Var && binding.IsStableType && binding.ClrType == typeof(double))
            || (binding.Kind == BindingKind.Const && initializerProvesUnboxedDouble))
        {
            slotValue = EnsureNumber(value);
        }
        else if ((binding.Kind != BindingKind.Var && binding.IsStableType && binding.ClrType == typeof(bool))
            || (binding.Kind == BindingKind.Const && initializerProvesUnboxedBool))
        {
            slotValue = EnsureBoolean(value);
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
        TryMirrorGlobalVarBinding(binding, slotValue);
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

    private void TryMirrorGlobalVarBinding(BindingInfo binding, TempVariable value)
    {
        if (binding.Kind != BindingKind.Var || binding.DeclaringScope.Kind != ScopeKind.Global)
        {
            return;
        }

        var scope = binding.DeclaringScope;
        while (scope.Parent != null)
        {
            scope = scope.Parent;
        }

        if (!scope.UsesGlobalThisValue)
        {
            return;
        }

        var globalThisTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.GlobalThis),
            nameof(JavaScriptRuntime.GlobalThis.GetGlobalThis),
            global::System.Array.Empty<TempVariable>(),
            globalThisTemp));
        DefineTempStorage(globalThisTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var nameTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(binding.Name, nameTemp));
        DefineTempStorage(nameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var throwOnErrorTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, throwOnErrorTemp));
        DefineTempStorage(throwOnErrorTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        var resultTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.SetProperty),
            new[] { EnsureObject(globalThisTemp), EnsureObject(nameTemp), EnsureObject(value), throwOnErrorTemp },
            resultTemp));
        DefineTempStorage(resultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
    }
}
