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
    private bool TryLowerPropertyAccessExpression(HIRPropertyAccessExpression propAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Intrinsic object property read: support well-known Symbol properties (e.g., Symbol.iterator).
        // We lower this as a static intrinsic call so `Symbol` does not need to be representable
        // as a normal runtime value.
        if (propAccessExpr.Object is HIRVariableExpression intrinsicVar
            && intrinsicVar.Name.Kind == BindingKind.Global
            && string.Equals(intrinsicVar.Name.Name, "Symbol", StringComparison.Ordinal)
            && JavaScriptRuntime.IntrinsicObjectRegistry.Get("Symbol") != null)
        {
            var intrinsicKeyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, intrinsicKeyTemp));
            DefineTempStorage(intrinsicKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                IntrinsicName: "Symbol",
                MethodName: "GetWellKnown",
                Arguments: new[] { EnsureObject(intrinsicKeyTemp) },
                Result: resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // User-defined class instance field access (e.g., this.wordArray).
        // If the receiver is `this` and we know the generated CLR type has a field with this name,
        // lower directly to an instance field load (ldfld) instead of dynamic property access.
        if (_classRegistry != null
            && propAccessExpr.Object is HIRThisExpression
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, propAccessExpr.PropertyName, out _))
        {
            _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                RegistryClassName: currentClass,
                FieldName: propAccessExpr.PropertyName,
                IsPrivateField: false,
                Result: resultTempVar));
            if (!_classRegistry.TryGetFieldClrType(currentClass, propAccessExpr.PropertyName, out var fieldClrType))
            {
                fieldClrType = typeof(object);
            }
            var storageKind = (fieldClrType == typeof(double)
                || fieldClrType == typeof(bool)
                || fieldClrType == typeof(JavaScriptRuntime.JsNull))
                ? ValueStorageKind.UnboxedValue
                : ValueStorageKind.Reference;
            DefineTempStorage(resultTempVar, new ValueStorage(storageKind, fieldClrType));
            return true;
        }

        // User-defined class static field access (e.g., Greeter.message).
        // Classes are compiled as .NET types, and static class fields are emitted as CLR static fields.
        // When the receiver is the class identifier, lower directly to a static field load.
        if (propAccessExpr.Object is HIRVariableExpression classVarExpr &&
            classVarExpr.Name.BindingInfo.DeclarationNode is ClassDeclaration)
        {
            if (!TryGetRegistryClassNameForClassSymbol(classVarExpr.Name, out var registryClassName))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRLoadUserClassStaticField(
                RegistryClassName: registryClassName,
                FieldName: propAccessExpr.PropertyName,
                Result: resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Lower the object expression
        if (!TryLowerExpression(propAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        // Currently we only support the 'length' property
        if (propAccessExpr.PropertyName == "length")
        {
            var boxedObject = EnsureObject(objectTemp);
            _methodBodyIR.Instructions.Add(new LIRGetLength(boxedObject, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // General property access: treat as an item access with a string key (obj[propName]).
        // This enables lowering for cases like `console.log(s.n)` without falling back to legacy.
        var keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var boxedObjectGeneric = EnsureObject(objectTemp);
        var boxedKey = EnsureObject(keyTemp);
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObjectGeneric, boxedKey, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryGetRegistryClassNameForClassSymbol(Symbol classSymbol, out string registryClassName)
    {
        registryClassName = string.Empty;

        if (_scope == null)
        {
            return false;
        }

        var declaringScope = FindDeclaringScope(classSymbol.BindingInfo);
        if (declaringScope == null)
        {
            return false;
        }

        // The class scope is expected to be a child scope of the declaring scope.
        var classScope = declaringScope.Children.FirstOrDefault(s =>
            s.Kind == ScopeKind.Class &&
            string.Equals(s.Name, classSymbol.Name, StringComparison.Ordinal));

        if (classScope == null)
        {
            return false;
        }

        // Match TwoPhaseCompilationCoordinator registry naming (namespace + type name).
        var ns = classScope.DotNetNamespace ?? "Classes";
        var typeName = classScope.DotNetTypeName ?? classScope.Name;
        registryClassName = $"{ns}.{typeName}";
        return true;
    }

    private bool TryLowerConditionalExpression(HIRConditionalExpression conditionalExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Evaluate the test condition, then branch to either consequent or alternate.
        if (!TryLowerExpression(conditionalExpr.Test, out var conditionTemp))
        {
            return false;
        }

        // If the test is already a boolean, we can branch directly.
        // Otherwise, apply JS truthiness semantics via Operators.IsTruthy(object).
        TempVariable boolConditionTemp;
        var conditionStorage = GetTempStorage(conditionTemp);
        if (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(bool))
        {
            boolConditionTemp = conditionTemp;
        }
        else
        {
            var conditionBoxed = EnsureObject(conditionTemp);
            var isTruthyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(conditionBoxed, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            boolConditionTemp = isTruthyTemp;
        }

        int elseLabel = CreateLabel();
        int endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(boolConditionTemp, elseLabel));

        // Consequent branch
        if (!TryLowerExpression(conditionalExpr.Consequent, out var consequentTemp))
        {
            return false;
        }

        // For now, always box the result so branches can join safely.
        var consequentBoxed = EnsureObject(consequentTemp);
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(consequentBoxed, resultTempVar));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        // Alternate branch
        _methodBodyIR.Instructions.Add(new LIRLabel(elseLabel));

        if (!TryLowerExpression(conditionalExpr.Alternate, out var alternateTemp))
        {
            return false;
        }

        var alternateBoxed = EnsureObject(alternateTemp);
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(alternateBoxed, resultTempVar));

        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
        return true;
    }

    private bool TryLowerIndexAccessExpression(HIRIndexAccessExpression indexAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // User-defined class instance field access via bracket notation (e.g., this["wordArray"]).
        // If the receiver is `this` and the index is a constant string that matches a known field on the
        // generated CLR type, lower directly to an instance field load (ldfld) instead of dynamic GetItem.
        if (_classRegistry != null
            && indexAccessExpr.Object is HIRThisExpression
            && indexAccessExpr.Index is HIRLiteralExpression literalIndex
            && literalIndex.Kind == JavascriptType.String
            && literalIndex.Value is string literalFieldName
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, literalFieldName, out _))
        {
            _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                RegistryClassName: currentClass,
                FieldName: literalFieldName,
                IsPrivateField: false,
                Result: resultTempVar));
            if (!_classRegistry.TryGetFieldClrType(currentClass, literalFieldName, out var fieldClrType))
            {
                fieldClrType = typeof(object);
            }
            var storageKind = (fieldClrType == typeof(double)
                || fieldClrType == typeof(bool)
                || fieldClrType == typeof(JavaScriptRuntime.JsNull))
                ? ValueStorageKind.UnboxedValue
                : ValueStorageKind.Reference;
            DefineTempStorage(resultTempVar, new ValueStorage(storageKind, fieldClrType));
            return true;
        }

        // Lower the object expression
        if (!TryLowerExpression(indexAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        // Lower the index expression
        if (!TryLowerExpression(indexAccessExpr.Index, out var indexTemp))
        {
            return false;
        }

        var boxedObject = EnsureObject(objectTemp);
        var indexStorage = GetTempStorage(indexTemp);
        TempVariable indexForGet = indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double)
            ? indexTemp
            : EnsureObject(indexTemp);
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObject, indexForGet, resultTempVar));

        // If the receiver is statically known to be an Int32Array and the index is numeric,
        // lower the result as an unboxed double. This allows IL emission to use the typed
        // `Int32Array.get_Item(double)` fast-path without boxing, and only box later if
        // `EnsureObject` is required by usage.
        var receiverStorage = GetTempStorage(boxedObject);
        if (receiverStorage.Kind == ValueStorageKind.Reference
            && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array)
            && indexStorage.Kind == ValueStorageKind.UnboxedValue
            && indexStorage.ClrType == typeof(double))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        }
        else
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        return true;
    }

    /// <summary>
    /// Loads the current value of a variable, handling both captured and non-captured variables.
    /// </summary>
    private bool TryLoadVariable(BindingInfo binding, out TempVariable result)
    {
        result = default;

        static ValueStorage GetPreferredBindingReadStorage(BindingInfo b)
        {
            // Only propagate unboxed doubles for stable types. This matches the current
            // typed-scope-field support in TypeGenerator/VariableRegistry.
            if (b.IsStableType && b.ClrType == typeof(double))
            {
                return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
            }

            return new ValueStorage(ValueStorageKind.Reference, typeof(object));
        }

        // Per-iteration environments: if this binding lives in an active materialized scope instance
        // (e.g., for-loop iteration scope), load from that scope temp.
        if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
        {
            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, result));
            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
            return true;
        }

        // Check if this binding is stored in a scope field (captured variable)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null)
            {
                switch (storage.Kind)
                {
                    case BindingStorageKind.IlArgument:
                        // Non-captured parameter
                        if (storage.JsParameterIndex >= 0)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadParameter(storage.JsParameterIndex, result));
                            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                            return true;
                        }
                        break;

                    case BindingStorageKind.LeafScopeField:
                        // Captured variable in current scope
                        if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.Field, storage.DeclaringScope, result));
                            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        // Captured variable in parent scope
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, result));
                            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlLocal:
                        // Non-captured local - use SSA map
                        break;
                }
            }
        }

        // Fallback: Check parameter index map
        if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
        {
            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(paramIndex, result));
            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Non-captured local: look up in SSA map
        if (_variableMap.TryGetValue(binding, out result))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs a compound operation (+=, -=, *=, etc.) on two operands.
    /// </summary>
}
