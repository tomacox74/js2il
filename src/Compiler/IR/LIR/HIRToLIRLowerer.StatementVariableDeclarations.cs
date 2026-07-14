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
    private void EmitGlobalVarBindingInitializationIfNeeded()
    {
        if (_scope == null || _scope.Kind != ScopeKind.Global || !_scope.UsesGlobalThisValue)
        {
            return;
        }

        foreach (var binding in _scope.Bindings.Values)
        {
            if (binding.Kind != BindingKind.Var)
            {
                continue;
            }

            var nameTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(binding.Name, nameTemp));
            DefineTempStorage(nameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.EnsureGlobalVarBinding),
                new[] { EnsureObject(nameTemp) }));
        }
    }

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
        // Use BindingInfo as key for correct shadowing behavior.
        var binding = exprStmt.Name.BindingInfo;

        // `var` declarations without an initializer are runtime no-ops on redeclaration.
        // Avoid clobbering an existing head-assigned loop value (e.g., for-of `var x` + body `var x;`).
        if (exprStmt.Initializer == null
            && binding.Kind == BindingKind.Var
            && _variableMap.ContainsKey(binding))
        {
            return true;
        }

        // A proven numeric var has no observable read on any path before its first
        // numeric assignment, so its source-level `var x;` declaration is a runtime no-op.
        if (exprStmt.Initializer == null && binding.CanUseUnboxedLocal)
        {
            return true;
        }

        TempVariable value;
        TempVariable? withResolvedVarNameTemp = null;
        TempVariable? withResolvedVarShadowedTemp = null;

        if (exprStmt.Initializer != null
            && binding.Kind == BindingKind.Var
            && _activeWithObjects.Count > 0)
        {
            var preResolvedWithNameTemp = CreateTempVariable();
            withResolvedVarNameTemp = preResolvedWithNameTemp;
            _methodBodyIR.Instructions.Add(new LIRConstString(binding.Name, preResolvedWithNameTemp));
            DefineTempStorage(preResolvedWithNameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var preResolvedShadowedTemp = CreateTempVariable();
            withResolvedVarShadowedTemp = preResolvedShadowedTemp;
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.HasPropertyIn),
                new[] { EnsureObject(preResolvedWithNameTemp), _activeWithObjects.Peek() },
                preResolvedShadowedTemp));
            DefineTempStorage(preResolvedShadowedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        }

        if (exprStmt.Initializer != null)
        {
            var shouldSetPendingAnonymousClassName =
                exprStmt.Initializer is HIRInitializedUserClassTypeExpression initializedClassExpr
                && initializedClassExpr.ClassScope.Name.StartsWith("ClassExpression_", StringComparison.Ordinal);

            var previousPendingAnonymousClassName = _pendingAnonymousClassExpressionInferredName;
            if (shouldSetPendingAnonymousClassName)
            {
                _pendingAnonymousClassExpressionInferredName = exprStmt.Name.Name;
            }

            try
            {
                if (!TryLowerExpression(exprStmt.Initializer, out value))
                {
                    IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering variable initializer expression {exprStmt.Initializer.GetType().Name} for '{exprStmt.Name.Name}'");
                    return false;
                }
            }
            finally
            {
                _pendingAnonymousClassExpressionInferredName = previousPendingAnonymousClassName;
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
            DefineTempStorage(value, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        if (exprStmt.Initializer != null
            && binding.Kind == BindingKind.Var
            && withResolvedVarNameTemp is TempVariable withNameTemp
            && withResolvedVarShadowedTemp is TempVariable shadowedTemp)
        {
            TempVariable fallbackValueTemp;
            if (_variableMap.TryGetValue(binding, out var existingBindingValue))
            {
                fallbackValueTemp = EnsureObject(existingBindingValue);
            }
            else
            {
                fallbackValueTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(fallbackValueTemp));
                DefineTempStorage(fallbackValueTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }

            var withResolvedValueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.ApplyResolvedWithVarInitializer),
                new[] { shadowedTemp, _activeWithObjects.Peek(), EnsureObject(withNameTemp), EnsureObject(value), EnsureObject(fallbackValueTemp) },
                withResolvedValueTemp));
            DefineTempStorage(withResolvedValueTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            value = withResolvedValueTemp;
        }

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
            if ((CanUseStablePrimitiveLocal(binding, typeof(double)))
                || (binding.Kind == BindingKind.Const && initializerProvesUnboxedDouble))
            {
                fieldValue = EnsureNumber(value);
            }
            else if ((CanUseStablePrimitiveLocal(binding, typeof(bool)))
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
        if (CanUseStablePrimitiveLocal(binding, typeof(double))
            || (binding.Kind == BindingKind.Const && initializerProvesUnboxedDouble))
        {
            slotValue = EnsureNumber(value);
        }
        else if (CanUseStablePrimitiveLocal(binding, typeof(bool))
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
