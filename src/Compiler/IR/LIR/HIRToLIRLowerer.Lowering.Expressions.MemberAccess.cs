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
    private bool TryLowerPropertyAccessExpression(HIRPropertyAccessExpression propAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        if (propAccessExpr.Object is HIRSuperExpression
            && _callableKind == CallableKind.Constructor
            && _isDerivedConstructor
            && !_superConstructorCalled)
        {
            return TryLowerExpression(new HIRThisExpression(), out resultTempVar);
        }

        // Early-bound object-literal member read (phase 4, #1432): when the receiver is a
        // binding whose literal passed phase-1 eligibility, read through the generated
        // typed accessor instead of the dynamic property path.
        if (TryGetInferredObjectLiteralMember(propAccessExpr.Object, propAccessExpr.PropertyName, out var inferredShape, out var inferredMember))
        {
            if (!TryLowerExpression(propAccessExpr.Object, out var inferredReceiver))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRGetInferredMember(
                inferredShape,
                inferredMember.Name,
                EnsureObject(inferredReceiver),
                resultTempVar));
            DefineTempStorage(resultTempVar, GetInferredMemberStorage(inferredMember));
            return true;
        }

        // Intrinsic object property read: support well-known Symbol properties (e.g., Symbol.iterator).
        // We lower this as a static intrinsic call so `Symbol` does not need to be representable
        // as a normal runtime value.
        if (propAccessExpr.Object is HIRVariableExpression intrinsicVar
            && intrinsicVar.Name.Kind == BindingKind.Global
            && string.Equals(intrinsicVar.Name.Name, "Symbol", StringComparison.Ordinal)
            && JavaScriptRuntime.Symbol.IsWellKnown(propAccessExpr.PropertyName))
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

        if (string.Equals(propAccessExpr.PropertyName, "PI", StringComparison.Ordinal)
            && propAccessExpr.Object is HIRVariableExpression mathVar
            && IsStableGlobalMathBinding(mathVar.Name))
        {
            _methodBodyIR.Instructions.Add(new LIRConstNumber(global::System.Math.PI, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // User-defined class instance field access (e.g., this.wordArray).
        // If the receiver is `this` and we know the generated CLR type has a field with this name,
        // lower directly to an instance field load (ldfld) instead of dynamic property access.
        if (propAccessExpr.Object is HIRThisExpression
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null)
        {
            var stableFieldClrType = TryGetStableThisFieldClrType(propAccessExpr.PropertyName);
            var hasRegisteredField = _classRegistry != null
                && _classRegistry.TryGetField(currentClass, propAccessExpr.PropertyName, out _);
            if (!hasRegisteredField && stableFieldClrType == null)
            {
                goto LowerGenericPropertyAccess;
            }

            _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                RegistryClassName: currentClass,
                FieldName: propAccessExpr.PropertyName,
                IsPrivateField: false,
                Result: resultTempVar));
            if (!hasRegisteredField
                || _classRegistry == null
                || !_classRegistry.TryGetFieldClrType(currentClass, propAccessExpr.PropertyName, out var fieldClrType))
            {
                fieldClrType = stableFieldClrType ?? typeof(object);
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

            if (_classRegistry == null
                || !_classRegistry.TryGetStaticField(registryClassName, propAccessExpr.PropertyName, out _))
            {
                goto LowerGenericPropertyAccess;
            }

            _methodBodyIR.Instructions.Add(new LIRLoadUserClassStaticField(
                RegistryClassName: registryClassName,
                FieldName: propAccessExpr.PropertyName,
                Result: resultTempVar));

            if (!_classRegistry.TryGetStaticFieldClrType(registryClassName, propAccessExpr.PropertyName, out var staticFieldClrType))
            {
                staticFieldClrType = typeof(object);
            }
            var storageKind = (staticFieldClrType == typeof(double)
                || staticFieldClrType == typeof(bool)
                || staticFieldClrType == typeof(JavaScriptRuntime.JsNull))
                ? ValueStorageKind.UnboxedValue
                : ValueStorageKind.Reference;
            DefineTempStorage(resultTempVar, new ValueStorage(storageKind, staticFieldClrType));
            return true;
        }

        if (propAccessExpr.Object is HIRSuperExpression
            && TryGetEnclosingBaseClassRegistryName(out var baseClassRegistryName)
            && baseClassRegistryName != null)
        {
            if (!TryLowerExpression(new HIRThisExpression(), out _))
            {
                return false;
            }

            TempVariable baseConstructorTemp;
            if (TryGetEnclosingSuperClassExpression(out var superClassExpression)
                && superClassExpression != null
                && TryLowerExpression(superClassExpression, out baseConstructorTemp))
            {
                baseConstructorTemp = EnsureObject(baseConstructorTemp);
            }
            else
            {
                baseConstructorTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRGetUserClassType(baseClassRegistryName, baseConstructorTemp));
                DefineTempStorage(baseConstructorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
            }

            var prototypeKeyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString("prototype", prototypeKeyTemp));
            DefineTempStorage(prototypeKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var prototypeTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(baseConstructorTemp), EnsureObject(prototypeKeyTemp), prototypeTemp));
            DefineTempStorage(prototypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var propertyKeyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, propertyKeyTemp));
            DefineTempStorage(propertyKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(prototypeTemp), EnsureObject(propertyKeyTemp), resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

    LowerGenericPropertyAccess:
        // Lower the object expression
        if (!TryLowerExpression(propAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        // Length is only safe to intrinsic-lower when the receiver's length semantics are fixed
        // and always numeric. Arguments objects and plain objects expose length as normal
        // configurable properties, so they must use generic property access to preserve cases
        // like delete arguments.length -> undefined.
        if (propAccessExpr.PropertyName == "length")
        {
            var lengthReceiver = objectTemp;
            var receiverStorage = GetTempStorage(lengthReceiver);
            if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(string))
            {
                _methodBodyIR.Instructions.Add(new LIRGetStringLength(lengthReceiver, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }

            // Stable string bindings (e.g., local `t` in dromaeo generateTestStrings) are often
            // represented as object temps at load sites. Use the typed length instruction so IL
            // emission can cast to string without forcing a DotNet2JSConversions.ToString call.
            if (propAccessExpr.Object is HIRVariableExpression receiverVarExpr
                && receiverVarExpr.Name.BindingInfo.IsStableType
                && receiverVarExpr.Name.BindingInfo.ClrType == typeof(string))
            {
                _methodBodyIR.Instructions.Add(new LIRGetStringLength(EnsureObject(lengthReceiver), resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }

            static bool HasIntrinsicNumericLength(Type? clrType)
            {
                if (clrType == null)
                {
                    return false;
                }

                return clrType == typeof(JavaScriptRuntime.Array)
                    || typeof(JavaScriptRuntime.TypedArrayBase).IsAssignableFrom(clrType)
                    || typeof(JavaScriptRuntime.Node.Buffer).IsAssignableFrom(clrType)
                    || typeof(Delegate).IsAssignableFrom(clrType);
            }

            Type? stableReceiverType = null;
            if (propAccessExpr.Object is HIRVariableExpression lengthReceiverVarExpr
                && lengthReceiverVarExpr.Name.BindingInfo.IsStableType)
            {
                stableReceiverType = lengthReceiverVarExpr.Name.BindingInfo.ClrType;
            }

            if ((receiverStorage.Kind == ValueStorageKind.Reference && HasIntrinsicNumericLength(receiverStorage.ClrType))
                || HasIntrinsicNumericLength(stableReceiverType))
            {
                var boxedObject = EnsureObject(lengthReceiver);
                _methodBodyIR.Instructions.Add(new LIRGetLength(boxedObject, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }

            var lengthKeyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, lengthKeyTemp));
            DefineTempStorage(lengthKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var boxedLengthReceiver = EnsureObject(lengthReceiver);
            var boxedLengthKey = EnsureObject(lengthKeyTemp);
            _methodBodyIR.Instructions.Add(new LIRGetItem(boxedLengthReceiver, boxedLengthKey, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // General property access: treat as an item access with a string key (obj[propName]).
        // This enables lowering for cases like `console.log(s.n)` without falling back to legacy.
        var keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var boxedObjectGeneric = EnsureObject(objectTemp);
        var stableThisFieldClrType = propAccessExpr.Object is HIRThisExpression
            ? TryGetStableThisFieldClrType(propAccessExpr.PropertyName)
            : null;
        if (stableThisFieldClrType == typeof(double))
        {
            _methodBodyIR.Instructions.Add(new LIRGetItemAsNumberString(boxedObjectGeneric, keyTemp, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        }
        else
        {
            var boxedKey = EnsureObject(keyTemp);
            _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObjectGeneric, boxedKey, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
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

        var classScope = declaringScope.Kind == ScopeKind.Class
            && string.Equals(declaringScope.Name, classSymbol.Name, StringComparison.Ordinal)
                ? declaringScope
                : declaringScope.Children.FirstOrDefault(s =>
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

        // The conditional operator joins values from two control-flow paths.
        // Representing that join as multiple writes to a single temp can violate SSA assumptions
        // used by later stackification/materialization passes (notably around LIRCopyTemp).
        // Pin the result temp to a dedicated local slot so each branch stores into the same
        // stable location, and post-join reads observe the value from the executed branch.
        var resultStorage = new ValueStorage(ValueStorageKind.BoxedValue, typeof(object));
        DefineTempStorage(resultTempVar, resultStorage);
        var resultSlot = CreateAnonymousVariableSlot("$conditional", resultStorage);
        SetTempVariableSlot(resultTempVar, resultSlot);

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

        return true;
    }

    private bool TryLowerIndexAccessExpression(HIRIndexAccessExpression indexAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        if (indexAccessExpr.Object is HIRSuperExpression
            && _callableKind == CallableKind.Constructor
            && _isDerivedConstructor
            && !_superConstructorCalled)
        {
            return TryLowerExpression(new HIRThisExpression(), out resultTempVar);
        }

        if (indexAccessExpr.Object is HIRSuperExpression
            && TryGetEnclosingBaseClassRegistryName(out var baseClassRegistryName)
            && baseClassRegistryName != null)
        {
            if (!TryLowerExpression(new HIRThisExpression(), out _))
            {
                return false;
            }

            TempVariable baseConstructorTemp;
            if (TryGetEnclosingSuperClassExpression(out var superClassExpression)
                && superClassExpression != null
                && TryLowerExpression(superClassExpression, out baseConstructorTemp))
            {
                baseConstructorTemp = EnsureObject(baseConstructorTemp);
            }
            else
            {
                baseConstructorTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRGetUserClassType(baseClassRegistryName, baseConstructorTemp));
                DefineTempStorage(baseConstructorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
            }

            var prototypeKeyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString("prototype", prototypeKeyTemp));
            DefineTempStorage(prototypeKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var prototypeTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(baseConstructorTemp), EnsureObject(prototypeKeyTemp), prototypeTemp));
            DefineTempStorage(prototypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerExpression(indexAccessExpr.Index, out var superIndexTemp))
            {
                return false;
            }

            var superIndexStorage = GetTempStorage(superIndexTemp);
            TempVariable superIndexForGet = superIndexStorage.Kind == ValueStorageKind.UnboxedValue && superIndexStorage.ClrType == typeof(double)
                ? superIndexTemp
                : EnsureObject(superIndexTemp);

            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(prototypeTemp), superIndexForGet, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // User-defined class instance field access via bracket notation (e.g., this["wordArray"]).
        // If the receiver is `this` and the index is a constant string that matches a known field on the
        // generated CLR type, lower directly to an instance field load (ldfld) instead of dynamic GetItem.
        if (indexAccessExpr.Object is HIRThisExpression
            && indexAccessExpr.Index is HIRLiteralExpression literalIndex
            && literalIndex.Kind == JavascriptType.String
            && literalIndex.Value is string literalFieldName
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null)
        {
            var stableFieldClrType = TryGetStableThisFieldClrType(literalFieldName);
            var hasRegisteredField = _classRegistry != null
                && _classRegistry.TryGetField(currentClass, literalFieldName, out _);
            if (!hasRegisteredField && stableFieldClrType == null)
            {
                goto ContinueWithGenericIndexAccess;
            }

            _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                RegistryClassName: currentClass,
                FieldName: literalFieldName,
                IsPrivateField: false,
                Result: resultTempVar));
            if (!hasRegisteredField
                || _classRegistry == null
                || !_classRegistry.TryGetFieldClrType(currentClass, literalFieldName, out var fieldClrType))
            {
                fieldClrType = stableFieldClrType ?? typeof(object);
            }
            var storageKind = (fieldClrType == typeof(double)
                || fieldClrType == typeof(bool)
                || fieldClrType == typeof(JavaScriptRuntime.JsNull))
                ? ValueStorageKind.UnboxedValue
                : ValueStorageKind.Reference;
            DefineTempStorage(resultTempVar, new ValueStorage(storageKind, fieldClrType));
            return true;
        }

    ContinueWithGenericIndexAccess:
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
        if (indexAccessExpr.Object is HIRThisExpression
            && indexAccessExpr.Index is HIRLiteralExpression { Kind: JavascriptType.String, Value: string literalKey }
            && TryGetStableThisFieldClrType(literalKey) == typeof(double))
        {
            _methodBodyIR.Instructions.Add(new LIRGetItemAsNumberString(boxedObject, EnsureStringKey(indexTemp, literalKey), resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObject, indexForGet, resultTempVar));

        // If the receiver is statically known to be an Int32Array and the index is numeric,
        // lower the result as an unboxed double. This allows IL emission to use the typed
        // `Int32Array.get_Item(double)` fast-path without boxing, and only box later if
        // `EnsureObject` is required by usage.
        var receiverStorage = GetTempStorage(boxedObject);
        Type? stableArrayElementClrType = null;
        if (indexStorage.Kind == ValueStorageKind.UnboxedValue
            && indexStorage.ClrType == typeof(double)
            && indexAccessExpr.Object is HIRVariableExpression receiverVarExpr
            && receiverVarExpr.Name.BindingInfo.IsStableType
            && receiverVarExpr.Name.BindingInfo.ClrType == typeof(JavaScriptRuntime.Array))
        {
            stableArrayElementClrType = receiverVarExpr.Name.BindingInfo.StableElementClrType;
        }

        if (receiverStorage.Kind == ValueStorageKind.Reference
            && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array)
            && indexStorage.Kind == ValueStorageKind.UnboxedValue
            && indexStorage.ClrType == typeof(double))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        }
        else if (stableArrayElementClrType == typeof(string))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        }
        else
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        return true;
    }

    private TempVariable EnsureStringKey(TempVariable keyTemp, string literalKey)
    {
        var keyStorage = GetTempStorage(keyTemp);
        if (keyStorage.Kind == ValueStorageKind.Reference && keyStorage.ClrType == typeof(string))
        {
            return keyTemp;
        }

        var stringKeyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(literalKey, stringKeyTemp));
        DefineTempStorage(stringKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return stringKeyTemp;
    }

    /// <summary>
    /// Loads the current value of a variable, handling both captured and non-captured variables.
    /// </summary>
    private bool TryLoadVariable(BindingInfo binding, out TempVariable result)
    {
        result = default;

        if (binding.Kind == BindingKind.Global)
        {
            var nameTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(binding.Name, nameTemp));
            DefineTempStorage(nameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.GetGlobalBindingValue),
                new[] { EnsureObject(nameTemp) },
                result));
            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _tempBindingOrigin[result] = binding;
            return true;
        }

        // Flow-sensitive numeric refinement: if this binding was previously proven to hold an
        // unboxed double (e.g. via an earlier Number(x) call or EnsureNumber coercion), return
        // that temp directly to avoid a redundant TypeUtilities.ToNumber call at the use site.
        SyncNumericRefinementStateWithLabels();
        if (CanTrackNumericRefinement(binding) && _numericRefinements.TryGetValue(binding, out var refinedTemp))
        {
            result = refinedTemp;
            return true;
        }

        ValueStorage GetPreferredBindingReadStorage(BindingInfo b)
        {
            if (b.Kind == BindingKind.Var
                && !b.DeclaringScope.Parameters.Contains(b.Name)
                && !b.CanUseUnboxedLocal)
            {
                return new ValueStorage(ValueStorageKind.Reference, typeof(object));
            }

            // Only propagate unboxed doubles for stable types. This matches the current
            // typed-scope-field support in TypeGenerator/VariableRegistry.
            if (b.IsStableType && b.ClrType == typeof(double))
            {
                return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
            }

            if (b.IsStableType && b.ClrType == typeof(bool))
            {
                return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
            }

            if (CanUseStringLocalStorage(b))
            {
                return new ValueStorage(ValueStorageKind.Reference, typeof(string));
            }

            return new ValueStorage(ValueStorageKind.Reference, typeof(object));
        }

        TempVariable EmitTemporalDeadZoneReferenceError(BindingInfo b)
        {
            var messageTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString($"Cannot access '{b.Name}' before initialization", messageTemp));
            DefineTempStorage(messageTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var errorTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRNewBuiltInError("ReferenceError", messageTemp, errorTemp));
            DefineTempStorage(errorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRThrow(errorTemp));

            var errorResult = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(errorResult));
            DefineTempStorage(errorResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return errorResult;
        }

        bool IsParameterTemporallyUninitialized(BindingInfo b)
            => _scope?.HasParameterExpressions == true
               && _currentDefaultParameterIndex is int defaultParameterIndex
               && _parameterIndexMap.TryGetValue(b, out var referencedParameterIndex)
               && referencedParameterIndex >= defaultParameterIndex;

        if (IsParameterTemporallyUninitialized(binding))
        {
            result = EmitTemporalDeadZoneReferenceError(binding);
            return true;
        }

        if (_scope?.HasParameterExpressions == true
            && _currentDefaultParameterIndex is int currentDefaultParameterIndex
            && _parameterIndexMap.TryGetValue(binding, out var referencedParameterIndex)
            && referencedParameterIndex < currentDefaultParameterIndex)
        {
            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(referencedParameterIndex, result));
            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
            _tempBindingOrigin[result] = binding;
            return true;
        }

        // Per-iteration environments: if this binding lives in an active materialized scope instance
        // (e.g., for-loop iteration scope), load from that scope temp.
        if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
        {
            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, result));
            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
            result = EmitResolveWithBindingOrDefault(binding, result);
            _tempBindingOrigin[result] = binding;
            return true;
        }

        // Check if this binding is stored in a scope field (captured variable)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage == null && _scope != null && binding.IsCaptured)
            {
                var declaringScope = binding.DeclaringScope;

                if (declaringScope != null)
                {
                    var declaringRegistryName = ScopeNaming.GetRegistryScopeName(declaringScope);
                    var scopeId = new ScopeId(declaringRegistryName);
                    var fieldId = new FieldId(declaringRegistryName, binding.Name);

                    if (!ReferenceEquals(declaringScope, _scope))
                    {
                        var parentIndex = _environmentLayout.ScopeChain.IndexOf(declaringRegistryName);
                        if (parentIndex < 0
                            && declaringScope.Kind == ScopeKind.Global
                            && _environmentLayout.Abi.ScopesSource is ScopesSource.Argument or ScopesSource.ThisField)
                        {
                            parentIndex = 0;
                        }

                        if (parentIndex >= 0)
                        {
                            storage = BindingStorage.ForParentScopeField(fieldId, scopeId, parentIndex);
                        }
                    }
                    else
                    {
                        storage = BindingStorage.ForLeafScopeField(fieldId, scopeId);
                    }
                }
            }
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
                            _tempBindingOrigin[result] = binding;
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
                            result = EmitResolveWithBindingOrDefault(binding, result);
                            _tempBindingOrigin[result] = binding;
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        // Captured variable in parent scope — only loadable when parent scopes are accessible.
                        // Static CLR methods (e.g., property getters/setters) have ScopesSource.None and
                        // cannot load parent scope fields; return false to allow fallback paths.
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil
                            && _environmentLayout?.Abi.ScopesSource is ScopesSource.Argument or ScopesSource.ThisField)
                        {
                            result = CreateTempVariable();
                            var parentIndex = AdjustParentScopeFieldIndexForCurrentMethod(storage.ParentScopeIndex);
                            _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, parentIndex, result));
                            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
                            result = EmitResolveWithBindingOrDefault(binding, result);
                            _tempBindingOrigin[result] = binding;
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
            _tempBindingOrigin[result] = binding;
            return true;
        }

        if (TryMaterializeStringBuilderAccumulator(binding, out result))
        {
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
    /// Matches a property access/assignment receiver against an eligible object-literal
    /// binding (phase 4, #1432). Returns true when the receiver is the binding the shape
    /// was inferred for, the generated CLR type exists, and the member is declared.
    /// Restricted to const/let bindings so the receiver is provably initialized (a `var`
    /// binding could be observed as undefined before initialization, which must keep the
    /// generic path's TypeError semantics).
    /// </summary>
    private static bool TryGetInferredObjectLiteralMember(
        HIRExpression objectExpr,
        string propertyName,
        out ObjectLiteralShapeInfo shape,
        out ObjectLiteralMemberInfo member)
    {
        shape = null!;
        member = null!;

        if (objectExpr is not HIRVariableExpression variableExpr)
        {
            return false;
        }

        var binding = variableExpr.Name.BindingInfo;

        // Const/let object-literal bindings are provably initialized before use. Parameters that
        // were inferred to a literal shape via interprocedural analysis (issue #1434) are also
        // safe: they are assigned their argument on entry, before any member access.
        var isParameter = binding.DeclaringScope.Parameters.Contains(binding.Name);
        if (binding.Kind is not (BindingKind.Const or BindingKind.Let) && !isParameter)
        {
            return false;
        }

        if (binding.ObjectLiteralShape is not { IsEligible: true } candidate
            || candidate.GeneratedClrTypeHandle.IsNil)
        {
            return false;
        }

        if (!candidate.TryGetMember(propertyName, out var candidateMember))
        {
            return false;
        }

        shape = candidate;
        member = candidateMember;
        return true;
    }

    private static ValueStorage GetInferredMemberStorage(ObjectLiteralMemberInfo member)
    {
        var clrType = member.ClrType ?? typeof(object);
        return clrType == typeof(double) || clrType == typeof(bool)
            ? new ValueStorage(ValueStorageKind.UnboxedValue, clrType)
            : new ValueStorage(ValueStorageKind.Reference, clrType);
    }

    /// <summary>
    /// Performs a compound operation (+=, -=, *=, etc.) on two operands.
    /// </summary>
}
