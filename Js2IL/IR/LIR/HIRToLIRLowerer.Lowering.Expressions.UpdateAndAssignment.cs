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
    private bool TryLowerUpdateExpression(HIRUpdateExpression updateExpr, out TempVariable resultTempVar, bool resultUsed = true)
    {
        resultTempVar = default;

        // If the result is unused (expression statement, for-loop update clause), we can skip
        // materializing the postfix return value.
        var needsPostfixValue = resultUsed && !updateExpr.Prefix;

        if (updateExpr.Operator != Acornima.Operator.Increment && updateExpr.Operator != Acornima.Operator.Decrement)
        {
            return false;
        }

        var isIncrement = updateExpr.Operator == Acornima.Operator.Increment;

        // Support ++/-- on identifiers, property access, and index access.
        // This is needed for common JS patterns like `obj.prop++` and `obj[idx]++`.
        if (updateExpr.Argument is HIRPropertyAccessExpression propAccessExpr)
        {
            return TryLowerUpdatePropertyAccessExpression(
                propAccessExpr,
                isIncrement,
                updateExpr.Prefix,
                needsPostfixValue,
                out resultTempVar);
        }

        if (updateExpr.Argument is HIRIndexAccessExpression indexAccessExpr)
        {
            return TryLowerUpdateIndexAccessExpression(
                indexAccessExpr,
                isIncrement,
                updateExpr.Prefix,
                needsPostfixValue,
                out resultTempVar);
        }

        if (updateExpr.Argument is not HIRVariableExpression updateVarExpr)
        {
            return false;
        }

        var updateBinding = updateVarExpr.Name.BindingInfo;

        // Updating a const is a runtime TypeError.
        if (updateBinding.Kind == BindingKind.Const)
        {
            _methodBodyIR.Instructions.Add(new LIRThrowNewTypeError("Assignment to constant variable."));
            resultTempVar = CreateTempVariable();
            return true;
        }

        // Load the current value (handles both captured and non-captured variables)
        if (!TryLoadVariable(updateBinding, out var currentValue))
        {
            return false;
        }

        // Environment-stored update path:
        // - Captured variables live in scope fields
        // - Parameters live in IL arguments
        // Do not rely on the temp storage kind here, since other lowering steps may propagate
        // stable unboxed types for captured fields.
        var isActiveScopeStored = TryGetActiveScopeFieldStorage(updateBinding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId);
        var updateStorage = isActiveScopeStored ? null : _environmentLayout?.GetStorage(updateBinding);
        var isEnvironmentStored = isActiveScopeStored || (updateStorage != null && updateStorage.Kind != BindingStorageKind.IlLocal);

        // Implement numeric coercion via runtime TypeUtilities.ToNumber(object?) and then store
        // the boxed updated value back to the appropriate storage location.
        if (isEnvironmentStored)
        {
            // Note: if we're in this branch and not using an active scope temp, updateStorage must be non-null.

            var currentNumber = EnsureNumber(currentValue);

            // For postfix, we must capture the old (ToNumber-coerced) value before the store happens.
            // Use LIRCopyTemp so Stackify will materialize the captured value.
            TempVariable? originalSnapshotForPostfix = null;
            if (needsPostfixValue)
            {
                var snapshotValue = EnsureObject(currentNumber);
                var snapshot = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(snapshotValue, snapshot));
                DefineTempStorage(snapshot, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                originalSnapshotForPostfix = snapshot;
            }

            var deltaOneTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, deltaOneTemp));
            DefineTempStorage(deltaOneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var updatedNumber = CreateTempVariable();
            if (isIncrement)
            {
                _methodBodyIR.Instructions.Add(new LIRAddNumber(currentNumber, deltaOneTemp, updatedNumber));
            }
            else
            {
                _methodBodyIR.Instructions.Add(new LIRSubNumber(currentNumber, deltaOneTemp, updatedNumber));
            }
            DefineTempStorage(updatedNumber, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var updatedBoxed = EnsureObject(updatedNumber);

            if (isActiveScopeStored)
            {
                _methodBodyIR.Instructions.Add(new LIRStoreScopeField(activeScopeTemp, updateBinding, activeFieldId, activeScopeId, updatedBoxed));
            }
            else
            {
                switch (updateStorage!.Kind)
                {
                    case BindingStorageKind.IlArgument:
                        if (updateStorage.JsParameterIndex < 0)
                        {
                            return false;
                        }
                        _methodBodyIR.Instructions.Add(new LIRStoreParameter(updateStorage.JsParameterIndex, updatedBoxed));
                        break;

                    case BindingStorageKind.LeafScopeField:
                        if (updateStorage.Field.IsNil || updateStorage.DeclaringScope.IsNil)
                        {
                            return false;
                        }
                        _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(updateBinding, updateStorage.Field, updateStorage.DeclaringScope, updatedBoxed));
                        break;

                    case BindingStorageKind.ParentScopeField:
                        if (updateStorage.ParentScopeIndex < 0 || updateStorage.Field.IsNil || updateStorage.DeclaringScope.IsNil)
                        {
                            return false;
                        }
                        {
                            var parentIndex = updateStorage.ParentScopeIndex;
                            if ((_methodBodyIR.IsAsync && _methodBodyIR.AsyncInfo?.HasAwaits == true)
                                || (_methodBodyIR.IsGenerator && (_methodBodyIR.GeneratorInfo?.YieldPointCount ?? 0) > 0))
                            {
                                parentIndex += 1;
                            }
                            _methodBodyIR.Instructions.Add(new LIRStoreParentScopeField(updateBinding, updateStorage.Field, updateStorage.DeclaringScope, parentIndex, updatedBoxed));
                        }
                        break;

                    default:
                        // Not a captured storage - fall back to local update path.
                        return false;
                }
            }

            // Update SSA map for subsequent reads.
            _variableMap[updateBinding] = updatedBoxed;
            _numericRefinements.Remove(updateBinding);

            if (updateExpr.Prefix)
            {
                resultTempVar = updatedBoxed;
                return true;
            }

            resultTempVar = needsPostfixValue ? originalSnapshotForPostfix!.Value : updatedBoxed;
            return true;
        }

        // For non-captured locals, support both numeric (double) and boxed/object paths.
        // Only use the unboxed-double fast path for bindings inferred as stable double.
        // Otherwise, even if the current value temp is unboxed double (due to propagation), we must
        // treat the variable as object-typed and store the boxed result back to its slot.
        var isStableDoubleBinding = updateBinding.IsStableType && updateBinding.ClrType == typeof(double);

        if (!isStableDoubleBinding)
        {
            // Boxed/local update path (e.g., object-typed locals).
            var currentNumber = EnsureNumber(currentValue);

            TempVariable? originalSnapshotForPostfix = null;
            if (needsPostfixValue)
            {
                originalSnapshotForPostfix = EnsureObject(currentNumber);
            }

            var boxedDeltaTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, boxedDeltaTemp));
            DefineTempStorage(boxedDeltaTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var boxedUpdatedNumber = CreateTempVariable();
            if (isIncrement)
            {
                _methodBodyIR.Instructions.Add(new LIRAddNumber(currentNumber, boxedDeltaTemp, boxedUpdatedNumber));
            }
            else
            {
                _methodBodyIR.Instructions.Add(new LIRSubNumber(currentNumber, boxedDeltaTemp, boxedUpdatedNumber));
            }
            DefineTempStorage(boxedUpdatedNumber, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var updatedBoxed = EnsureObject(boxedUpdatedNumber);

            if (!TryStoreToBinding(updateBinding, updatedBoxed, out var storedValue))
            {
                return false;
            }

            if (updateExpr.Prefix)
            {
                resultTempVar = EnsureObject(storedValue);
                return true;
            }

            resultTempVar = needsPostfixValue ? originalSnapshotForPostfix!.Value : EnsureObject(storedValue);
            return true;
        }

        // Stable-double non-captured local variable.
        // Get or create a variable slot for this binding.
        // Note: Captured variables are rejected earlier (Reference/object check), so we only reach here
        // for IlLocal bindings or when there's no environment layout.
        var slot = GetOrCreateVariableSlot(updateBinding, updateVarExpr.Name.Name, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        // In SSA: ++/-- produces a new value and updates the variable binding.
        // Prefix returns updated value; postfix returns original value.
        // Ensure we operate on a true unboxed double even if the current value has been boxed.
        var originalTemp = EnsureNumber(currentValue);

        // For postfix, capture/box the original value *before* we emit the update that overwrites
        // the stable variable local slot. Otherwise, later loads of originalTemp would observe the
        // updated value.
        TempVariable? boxedOriginalForPostfix = null;
        if (needsPostfixValue)
        {
            boxedOriginalForPostfix = EnsureObject(originalTemp);
        }

        var deltaTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, deltaTemp));
        this.DefineTempStorage(deltaTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var updatedTemp = CreateTempVariable();
        if (isIncrement)
        {
            _methodBodyIR.Instructions.Add(new LIRAddNumber(originalTemp, deltaTemp, updatedTemp));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRSubNumber(originalTemp, deltaTemp, updatedTemp));
        }
        this.DefineTempStorage(updatedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        // Store back to the appropriate location.
        // Note: Captured variables (LeafScopeField, ParentScopeField) are rejected earlier at line ~877
        // because they load as Reference/object type. Only IlLocal and no-environment-layout cases reach here.
        //
        // IMPORTANT: In loops (e.g., `for (...; ...; i--)`), the updated value must be materialized
        // into the stable variable slot at the update point so it survives the back-edge.
        // Relying on slot mapping alone can allow later materialization/stackification to elide the
        // store when source and destination share the same slot.
        var storeTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(updatedTemp, storeTemp));
        DefineTempStorage(storeTemp, GetTempStorage(updatedTemp));
        SetTempVariableSlot(storeTemp, slot);

        _variableMap[updateBinding] = storeTemp;
        _numericRefinements.Remove(updateBinding);
        // Remove it from the single-assignment set to prevent incorrect inlining.
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);

        if (updateExpr.Prefix)
        {
            // Prefix returns the updated value, boxed to object so we can store/emit without extra locals.
            resultTempVar = EnsureObject(storeTemp);
            return true;
        }

        // Postfix returns the original value.
        resultTempVar = needsPostfixValue ? boxedOriginalForPostfix!.Value : EnsureObject(storeTemp);
        return true;
    }

    private bool TryLowerUpdatePropertyAccessExpression(
        HIRPropertyAccessExpression propAccessExpr,
        bool isIncrement,
        bool prefix,
        bool needsPostfixValue,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(propAccessExpr.Object, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        var keyTemp = EmitConstString(propAccessExpr.PropertyName);
        var boxedKey = EnsureObject(keyTemp);

        var current = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, boxedKey, current));
        DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var currentNumber = EnsureNumber(current);

        TempVariable? originalSnapshotForPostfix = null;
        if (needsPostfixValue)
        {
            var snapshotValue = EnsureObject(currentNumber);
            var snapshot = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(snapshotValue, snapshot));
            DefineTempStorage(snapshot, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            originalSnapshotForPostfix = snapshot;
        }

        var deltaOneTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, deltaOneTemp));
        DefineTempStorage(deltaOneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var updatedNumber = CreateTempVariable();
        if (isIncrement)
        {
            _methodBodyIR.Instructions.Add(new LIRAddNumber(currentNumber, deltaOneTemp, updatedNumber));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRSubNumber(currentNumber, deltaOneTemp, updatedNumber));
        }
        DefineTempStorage(updatedNumber, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var valueForSet = updatedNumber;
        var updatedBoxed = EnsureObject(updatedNumber);

        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, boxedKey, valueForSet, setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        if (prefix)
        {
            resultTempVar = updatedBoxed;
            return true;
        }

        resultTempVar = needsPostfixValue ? originalSnapshotForPostfix!.Value : updatedBoxed;
        return true;
    }

    private bool TryLowerUpdateIndexAccessExpression(
        HIRIndexAccessExpression indexAccessExpr,
        bool isIncrement,
        bool prefix,
        bool needsPostfixValue,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(indexAccessExpr.Object, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        if (!TryLowerExpression(indexAccessExpr.Index, out var indexTemp))
        {
            return false;
        }

        TempVariable? boxedIndex = null;
        var indexStorageForGet = GetTempStorage(indexTemp);
        TempVariable indexForGet;
        if (indexStorageForGet.Kind == ValueStorageKind.UnboxedValue && indexStorageForGet.ClrType == typeof(double))
        {
            indexForGet = indexTemp;
        }
        else
        {
            boxedIndex = EnsureObject(indexTemp);
            indexForGet = boxedIndex.Value;
        }

        var current = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, indexForGet, current));
        DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var currentNumber = EnsureNumber(current);

        TempVariable? originalSnapshotForPostfix = null;
        if (needsPostfixValue)
        {
            var snapshotValue = EnsureObject(currentNumber);
            var snapshot = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(snapshotValue, snapshot));
            DefineTempStorage(snapshot, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            originalSnapshotForPostfix = snapshot;
        }

        var deltaOneTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, deltaOneTemp));
        DefineTempStorage(deltaOneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var updatedNumber = CreateTempVariable();
        if (isIncrement)
        {
            _methodBodyIR.Instructions.Add(new LIRAddNumber(currentNumber, deltaOneTemp, updatedNumber));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRSubNumber(currentNumber, deltaOneTemp, updatedNumber));
        }
        DefineTempStorage(updatedNumber, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        // Use typed SetItem when possible (avoids boxing updatedNumber for the store).
        var indexStorage = GetTempStorage(indexTemp);
        bool canUseNumericSetItem = indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double);
        bool canUseStringNumericSetItem = indexStorage.Kind == ValueStorageKind.Reference && indexStorage.ClrType == typeof(string);

        TempVariable indexForSet;
        if (canUseNumericSetItem)
        {
            indexForSet = indexTemp;
        }
        else
        {
            boxedIndex ??= EnsureObject(indexTemp);
            indexForSet = boxedIndex.Value;
        }

        TempVariable valueForSet;
        TempVariable updatedBoxed;
        if (canUseNumericSetItem || canUseStringNumericSetItem)
        {
            valueForSet = updatedNumber;
            updatedBoxed = EnsureObject(updatedNumber);
        }
        else
        {
            updatedBoxed = EnsureObject(updatedNumber);
            valueForSet = updatedBoxed;
        }

        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, indexForSet, valueForSet, setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        if (prefix)
        {
            resultTempVar = updatedBoxed;
            return true;
        }

        resultTempVar = needsPostfixValue ? originalSnapshotForPostfix!.Value : updatedBoxed;
        return true;
    }

    private TempVariable EnsureNumber(TempVariable tempVar)
    {
        SyncNumericRefinementStateWithLabels();
        var storage = GetTempStorage(tempVar);

        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            _tempBindingOrigin.Remove(tempVar);
            return tempVar;
        }

        // Fast path for common numeric hot loops:
        // collapse a just-emitted dynamic '+' followed by ToNumber into one runtime call.
        if (TryRewriteLatestDynamicAddToNumber(tempVar, out var fusedNumberTemp))
        {
            if (_tempBindingOrigin.Remove(tempVar, out var fusedSourceBinding) && CanTrackNumericRefinement(fusedSourceBinding))
            {
                _numericRefinements[fusedSourceBinding] = fusedNumberTemp;
            }
            return fusedNumberTemp;
        }

        // Dynamic numeric coercion: object -> double via runtime helper.
        var numberTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConvertToNumber(EnsureObject(tempVar), numberTempVar));
        DefineTempStorage(numberTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        // Flow-sensitive refinement: if tempVar originated from a variable load, record that the
        // binding is now proven double so subsequent loads can return the coerced temp directly.
        if (_tempBindingOrigin.Remove(tempVar, out var sourceBinding) && CanTrackNumericRefinement(sourceBinding))
        {
            _numericRefinements[sourceBinding] = numberTempVar;
        }

        return numberTempVar;
    }

    // Invalidate or update the numeric refinement for a binding after an assignment.
    // Call this whenever a binding is written so stale refinements cannot be used.
    // Pass the new value temp so we can carry the refinement forward when the newly
    // assigned value is itself an unboxed double (e.g. after x = Number(x)).
    private void InvalidateNumericRefinement(BindingInfo binding, TempVariable newValue)
    {
        SyncNumericRefinementStateWithLabels();
        if (!CanTrackNumericRefinement(binding))
        {
            _numericRefinements.Remove(binding);
            return;
        }

        var storage = GetTempStorage(newValue);
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            _numericRefinements[binding] = newValue;
        }
        else
        {
            _numericRefinements.Remove(binding);
        }
    }

    private bool TryRewriteLatestDynamicAddToNumber(TempVariable tempVar, out TempVariable numberTempVar)
    {
        numberTempVar = default;

        if (_methodBodyIR.Instructions.Count == 0)
        {
            return false;
        }

        var lastIndex = _methodBodyIR.Instructions.Count - 1;
        var lastInstruction = _methodBodyIR.Instructions[lastIndex];

        TempVariable left;
        TempVariable right;
        switch (lastInstruction)
        {
            case LIRAddDynamic addDynamic when addDynamic.Result == tempVar:
                left = addDynamic.Left;
                right = addDynamic.Right;
                break;
            case LIRAddDynamicDoubleObject addDynamicDoubleObject when addDynamicDoubleObject.Result == tempVar:
                left = addDynamicDoubleObject.LeftDouble;
                right = addDynamicDoubleObject.RightObject;
                break;
            case LIRAddDynamicObjectDouble addDynamicObjectDouble when addDynamicObjectDouble.Result == tempVar:
                left = addDynamicObjectDouble.LeftObject;
                right = addDynamicObjectDouble.RightDouble;
                break;
            default:
                return false;
        }

        _methodBodyIR.Instructions.RemoveAt(lastIndex);

        numberTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRAddAndToNumber(left, right, numberTempVar));
        DefineTempStorage(numberTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        return true;
    }

    private TempVariable EnsureBoolean(TempVariable tempVar)
    {
        var storage = GetTempStorage(tempVar);

        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
        {
            return tempVar;
        }

        var boolTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(EnsureObject(tempVar), boolTempVar));
        DefineTempStorage(boolTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        return boolTempVar;
    }

    private TempVariable CoerceToVariableSlotStorage(int slot, TempVariable value)
    {
        if (slot < 0 || slot >= _methodBodyIR.VariableStorages.Count)
        {
            return value;
        }

        var slotStorage = _methodBodyIR.VariableStorages[slot];
        if (slotStorage.Kind == ValueStorageKind.UnboxedValue && slotStorage.ClrType == typeof(double))
        {
            return EnsureNumber(value);
        }

        if (slotStorage.Kind == ValueStorageKind.UnboxedValue && slotStorage.ClrType == typeof(bool))
        {
            return EnsureBoolean(value);
        }

        if (slotStorage.Kind is ValueStorageKind.Reference or ValueStorageKind.BoxedValue)
        {
            return EnsureObject(value);
        }

        return value;
    }

    private bool TryStoreToBinding(BindingInfo binding, TempVariable valueToStore, out TempVariable storedValue)
    {
        storedValue = default;

        var lirInstructions = _methodBodyIR.Instructions;

        // Per-iteration environments: if this binding lives in an active materialized scope instance
        // (e.g., for-loop iteration scope), store into that scope temp.
        if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
        {
            var boxedValue = EnsureObject(valueToStore);
            lirInstructions.Add(new LIRStoreScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, boxedValue));
            _variableMap[binding] = boxedValue;
            InvalidateNumericRefinement(binding, valueToStore);
            storedValue = boxedValue;
            return true;
        }

        // Store via environment layout (captured vars, parameters)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null)
            {
                switch (storage.Kind)
                {
                    case BindingStorageKind.LeafScopeField:
                        if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxedValue));
                            _variableMap[binding] = boxedValue;
                            InvalidateNumericRefinement(binding, valueToStore);
                            storedValue = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            {
                                var parentIndex = storage.ParentScopeIndex;
                                if ((_methodBodyIR.IsAsync && _methodBodyIR.AsyncInfo?.HasAwaits == true)
                                    || (_methodBodyIR.IsGenerator && (_methodBodyIR.GeneratorInfo?.YieldPointCount ?? 0) > 0))
                                {
                                    parentIndex += 1;
                                }
                                lirInstructions.Add(new LIRStoreParentScopeField(binding, storage.Field, storage.DeclaringScope, parentIndex, boxedValue));
                            }
                            _variableMap[binding] = boxedValue;
                            InvalidateNumericRefinement(binding, valueToStore);
                            storedValue = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlArgument:
                        if (storage.JsParameterIndex >= 0)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreParameter(storage.JsParameterIndex, boxedValue));
                            InvalidateNumericRefinement(binding, valueToStore);
                            storedValue = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlLocal:
                        // Non-captured local - fall through to local-slot behavior
                        break;
                }
            }
        }

        // Fallback parameter index map (when environment layout isn't present)
        if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
        {
            var boxedValue = EnsureObject(valueToStore);
            lirInstructions.Add(new LIRStoreParameter(paramIndex, boxedValue));
            InvalidateNumericRefinement(binding, valueToStore);
            storedValue = boxedValue;
            return true;
        }

        // Non-captured variable - use a stable variable slot.
        // IMPORTANT: do not derive the slot storage from the RHS temp storage.
        // For example, `null` is represented as unboxed JsNull and must never force an
        // object-typed JS variable into an unboxed local slot.
        TempVariable slotValue;
        ValueStorage slotStorage;
        if (binding.IsStableType && binding.ClrType == typeof(double))
        {
            slotValue = EnsureNumber(valueToStore);
            slotStorage = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
        }
        else if (binding.IsStableType && binding.ClrType == typeof(bool))
        {
            slotValue = EnsureBoolean(valueToStore);
            slotStorage = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
        }
        else
        {
            slotValue = EnsureObject(valueToStore);
            slotStorage = new ValueStorage(ValueStorageKind.Reference, typeof(object));
        }

        var slot = GetOrCreateVariableSlot(binding, binding.Name, slotStorage);
        slotValue = CoerceToVariableSlotStorage(slot, slotValue);

        // IMPORTANT: locals are not truly SSA across loops/back-edges.
        // Always materialize an assignment into the variable slot *at the assignment point*
        // so subsequent iterations/branches observe the updated value.
        //
        // Do NOT rely on the source temp's slot mapping: if the source temp already maps to
        // the same IL local slot as the destination, LIRCopyTemp can be optimized away.
        // Copy through an intermediate temp (no slot) to force an actual store.
        var sourceCopy = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(slotValue, sourceCopy));
        DefineTempStorage(sourceCopy, GetTempStorage(slotValue));

        var storeTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(sourceCopy, storeTemp));
        DefineTempStorage(storeTemp, GetTempStorage(sourceCopy));
        SetTempVariableSlot(storeTemp, slot);

        _variableMap[binding] = storeTemp;
        InvalidateNumericRefinement(binding, storeTemp);
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);
        storedValue = storeTemp;
        return true;
    }

    private List<BindingInfo> GetPerIterationLexicalBindingsForForInit(HIRStatement? init)
    {
        var result = new List<BindingInfo>();

        void CollectFromStatement(HIRStatement? st)
        {
            if (st == null)
            {
                return;
            }

            if (st is HIRVariableDeclaration vd)
            {
                var binding = vd.Name.BindingInfo;
                if (binding.Kind is BindingKind.Let or BindingKind.Const && binding.IsCaptured)
                {
                    result.Add(binding);
                }
                return;
            }

            if (st is HIRBlock block)
            {
                foreach (var inner in block.Statements)
                {
                    CollectFromStatement(inner);
                }
            }
        }

        CollectFromStatement(init);
        return result;
    }

    private bool CanSafelyRecreateLeafScopeForPerIterationBindings(IReadOnlyList<BindingInfo> bindings)
    {
        if (bindings.Count == 0)
        {
            return false;
        }

        // Only supported for non-resumables.
        if (_methodBodyIR.IsAsync || _methodBodyIR.IsGenerator)
        {
            return false;
        }

        if (!_methodBodyIR.NeedsLeafScopeLocal || _methodBodyIR.LeafScopeId.IsNil)
        {
            return false;
        }

        if (_environmentLayout == null)
        {
            return false;
        }

        // Ensure all per-iteration bindings live in the leaf scope.
        foreach (var storage in bindings.Select(_environmentLayout.GetStorage))
        {
            if (storage == null || storage.Kind != BindingStorageKind.LeafScopeField)
            {
                return false;
            }

            if (storage.DeclaringScope.IsNil || storage.DeclaringScope != _methodBodyIR.LeafScopeId)
            {
                return false;
            }
        }

        // Guard: leaf scope must not contain other captured fields.
        // Otherwise, recreating the leaf scope would change closure semantics for those bindings.
        var leafScopeFieldBindings = _environmentLayout.StorageByBinding
            .Where(kvp => kvp.Value.Kind == BindingStorageKind.LeafScopeField && kvp.Value.DeclaringScope == _methodBodyIR.LeafScopeId)
            .Select(kvp => kvp.Key)
            .ToHashSet();

        return leafScopeFieldBindings.SetEquals(bindings);
    }

    private void EmitRecreateLeafScopeForPerIterationBindings(IReadOnlyList<BindingInfo> bindings)
    {
        if (_environmentLayout == null)
        {
            throw new InvalidOperationException("Cannot recreate leaf scope without environment layout.");
        }

        // Load current values before overwriting leaf local.
        var temps = new Dictionary<BindingInfo, TempVariable>();
        foreach (var binding in bindings)
        {
            var storage = _environmentLayout.GetStorage(binding)
                ?? throw new InvalidOperationException("Missing storage for per-iteration binding.");

            var temp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.Field, storage.DeclaringScope, temp));
            DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            temps[binding] = temp;
        }

        // Overwrite leaf scope local with a new instance.
        _methodBodyIR.Instructions.Add(new LIRCreateLeafScopeInstance(_methodBodyIR.LeafScopeId));

        // Store values into the new leaf scope instance.
        foreach (var binding in bindings)
        {
            var storage = _environmentLayout.GetStorage(binding)
                ?? throw new InvalidOperationException("Missing storage for per-iteration binding.");
            var valueTemp = temps[binding];
            _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, EnsureObject(valueTemp)));
        }
    }

    private void EmitRecreatePerIterationScopeFromTemp(TempVariable scopeInstanceTemp, ScopeId scopeId, string scopeName, IReadOnlyList<BindingInfo> bindings)
    {
        // Load current values before overwriting the loop scope instance.
        var valueTemps = new Dictionary<BindingInfo, TempVariable>();
        foreach (var binding in bindings)
        {
            var temp = CreateTempVariable();
            var fieldId = new FieldId(scopeName, binding.Name);
            _methodBodyIR.Instructions.Add(new LIRLoadScopeField(scopeInstanceTemp, binding, fieldId, scopeId, temp));
            DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            valueTemps[binding] = temp;
        }

        // Create a new scope instance for the next iteration.
        var newScopeTemp = CreateTempVariable();
        DefineTempStorage(newScopeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: scopeName));
        _methodBodyIR.Instructions.Add(new LIRCreateScopeInstance(scopeId, newScopeTemp));

        // Copy values into the new scope instance.
        foreach (var binding in bindings)
        {
            var fieldId = new FieldId(scopeName, binding.Name);
            var valueTemp = valueTemps[binding];
            _methodBodyIR.Instructions.Add(new LIRStoreScopeField(newScopeTemp, binding, fieldId, scopeId, EnsureObject(valueTemp)));
        }

        // Update the loop's current scope instance (backed by a stable variable slot).
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(newScopeTemp, scopeInstanceTemp));
    }

    private bool TryLowerAssignmentExpression(HIRAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        var binding = assignExpr.Target.BindingInfo;
        var lirInstructions = _methodBodyIR.Instructions;

        // Assigning to a const is a runtime TypeError.
        if (binding.Kind == BindingKind.Const)
        {
            lirInstructions.Add(new LIRThrowNewTypeError("Assignment to constant variable."));
            resultTempVar = CreateTempVariable();
            return true;
        }

        // For compound assignment (+=, -=, etc.), we need to load the current value first
        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            // Simple assignment: x = expr
            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }
        }
        else
        {
            // Compound assignment: x += expr, x -= expr, etc.
            // First, load the current value of the variable
            TempVariable currentValue;
            if (!TryLoadVariable(binding, out currentValue))
            {
                return false;
            }

            // Lower the RHS expression
            if (!TryLowerExpression(assignExpr.Value, out var rhsValue))
            {
                return false;
            }

            // Perform the compound operation
            if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhsValue, out valueToStore))
            {
                return false;
            }
        }

        // Store the value to the appropriate location
        // Per-iteration environments: if this binding lives in an active materialized scope instance,
        // store into that scope temp.
        if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
        {
            var boxedValue = EnsureObject(valueToStore);
            lirInstructions.Add(new LIRStoreScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, boxedValue));
            _variableMap[binding] = boxedValue;
            InvalidateNumericRefinement(binding, valueToStore);
            resultTempVar = boxedValue;
            return true;
        }

        // Check if this binding should be stored in a scope field (captured variable)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null)
            {
                switch (storage.Kind)
                {
                    case BindingStorageKind.LeafScopeField:
                        // Captured variable in current scope - store to leaf scope field
                        if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxedValue));
                            // Also update SSA map for subsequent reads
                            _variableMap[binding] = boxedValue;
                            InvalidateNumericRefinement(binding, valueToStore);
                            resultTempVar = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        // Captured variable in parent scope - store to parent scope field
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            {
                                var parentIndex = storage.ParentScopeIndex;
                                if ((_methodBodyIR.IsAsync && _methodBodyIR.AsyncInfo?.HasAwaits == true)
                                    || (_methodBodyIR.IsGenerator && (_methodBodyIR.GeneratorInfo?.YieldPointCount ?? 0) > 0))
                                {
                                    parentIndex += 1;
                                }
                                lirInstructions.Add(new LIRStoreParentScopeField(binding, storage.Field, storage.DeclaringScope, parentIndex, boxedValue));
                            }
                            // Also update SSA map for subsequent reads, mirroring leaf-scope behavior
                            _variableMap[binding] = boxedValue;
                            InvalidateNumericRefinement(binding, valueToStore);
                            resultTempVar = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlArgument:
                        // Storing to a parameter
                        if (storage.JsParameterIndex >= 0)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreParameter(storage.JsParameterIndex, boxedValue));
                            InvalidateNumericRefinement(binding, valueToStore);
                            resultTempVar = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlLocal:
                        // Non-captured local - use SSA temp (fall through to default behavior)
                        break;
                }
            }
        }

        // Check parameter index map for parameters (fallback)
        if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
        {
            var boxedValue = EnsureObject(valueToStore);
            lirInstructions.Add(new LIRStoreParameter(paramIndex, boxedValue));
            InvalidateNumericRefinement(binding, valueToStore);
            resultTempVar = boxedValue;
            return true;
        }

        // Non-captured local variable - update SSA map.
        // IMPORTANT: do not derive the slot storage from the RHS temp storage.
        // `null` is represented as unboxed JsNull and must never force object variables
        // into an unboxed local slot.
        TempVariable slotValue;
        ValueStorage slotStorage;
        if (binding.IsStableType && binding.ClrType == typeof(double))
        {
            slotValue = EnsureNumber(valueToStore);
            slotStorage = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
        }
        else if (binding.IsStableType && binding.ClrType == typeof(bool))
        {
            slotValue = EnsureBoolean(valueToStore);
            slotStorage = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
        }
        else
        {
            slotValue = EnsureObject(valueToStore);
            slotStorage = new ValueStorage(ValueStorageKind.Reference, typeof(object));
        }

        var slot = GetOrCreateVariableSlot(binding, assignExpr.Target.Name, slotStorage);
        slotValue = CoerceToVariableSlotStorage(slot, slotValue);

        // IMPORTANT: locals are not truly SSA across loops/back-edges.
        // Always materialize an assignment into the variable slot *at the assignment point*
        // so subsequent iterations/branches observe the updated value.
        //
        // Do NOT rely on the source temp's slot mapping: if the source temp already maps to
        // the same IL local slot as the destination, LIRCopyTemp can be optimized away.
        // Copy through an intermediate temp (no slot) to force an actual store.
        var sourceCopy = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(slotValue, sourceCopy));
        DefineTempStorage(sourceCopy, GetTempStorage(slotValue));

        var storeTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(sourceCopy, storeTemp));
        DefineTempStorage(storeTemp, GetTempStorage(sourceCopy));
        SetTempVariableSlot(storeTemp, slot);

        _variableMap[binding] = storeTemp;
        InvalidateNumericRefinement(binding, storeTemp);
        resultTempVar = storeTemp;

        // This is a reassignment (not initial declaration), so the variable is not single-assignment.
        // Remove it from the single-assignment set to prevent incorrect inlining.
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);
        return true;
    }

    private bool DoesClassNeedParentScopes(ClassDeclaration classDecl, Scope classScope)
    {
        if (classScope.ReferencesParentScopeVariables)
        {
            return true;
        }

        // Match ClassesGenerator's heuristic for when a class must capture parent scopes:
        // if any constructor/method contains nested functions or news a class that itself
        // requires parent scopes.
        var ctor = classDecl.Body.Body.OfType<MethodDefinition>()
            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

        if (ctor?.Value is FunctionExpression ctorExpr && MethodBodyRequiresParentScopes(ctorExpr.Body, classScope))
        {
            return true;
        }

        foreach (var funcExpr in classDecl.Body.Body
            .OfType<MethodDefinition>()
            .Where(m => (m.Key as Identifier)?.Name != "constructor")
            .Select(m => m.Value)
            .OfType<FunctionExpression>())
        {
            if (MethodBodyRequiresParentScopes(funcExpr.Body, classScope))
            {
                return true;
            }
        }

        return false;
    }

    private bool MethodBodyRequiresParentScopes(Node? body, Scope classScope)
    {
        bool found = false;

        void Walk(Node? n)
        {
            if (n == null || found) return;

            switch (n)
            {
                case BlockStatement bs:
                    foreach (var st in bs.Body) Walk(st);
                    break;
                case ExpressionStatement es:
                    Walk(es.Expression);
                    break;
                case VariableDeclaration vd:
                    foreach (var d in vd.Declarations)
                    {
                        Walk(d.Init as Node);
                    }
                    break;
                case IfStatement ifs:
                    Walk(ifs.Test);
                    Walk(ifs.Consequent);
                    Walk(ifs.Alternate);
                    break;
                case WhileStatement ws:
                    Walk(ws.Test);
                    Walk(ws.Body);
                    break;
                case DoWhileStatement dws:
                    Walk(dws.Body);
                    Walk(dws.Test);
                    break;
                case ForStatement fs:
                    Walk(fs.Init as Node);
                    Walk(fs.Test);
                    Walk(fs.Update);
                    Walk(fs.Body);
                    break;
                case ForInStatement fi:
                    Walk(fi.Left as Node);
                    Walk(fi.Right as Node);
                    Walk(fi.Body);
                    break;
                case ForOfStatement fof:
                    Walk(fof.Left as Node);
                    Walk(fof.Right as Node);
                    Walk(fof.Body);
                    break;
                case ReturnStatement rs:
                    Walk(rs.Argument);
                    break;
                case AssignmentExpression ae:
                    Walk(ae.Left);
                    Walk(ae.Right);
                    break;
                case CallExpression ce:
                    Walk(ce.Callee);
                    foreach (var a in ce.Arguments) Walk(a as Node);
                    break;
                case NewExpression ne:
                    if (ne.Callee is Identifier classId)
                    {
                        var foundClassScope = FindClassScopeByName(classScope, classId.Name);
                        if (foundClassScope != null && foundClassScope.ReferencesParentScopeVariables)
                        {
                            found = true;
                            return;
                        }
                    }
                    foreach (var a in ne.Arguments) Walk(a as Node);
                    break;

                case FunctionDeclaration:
                case FunctionExpression:
                case ArrowFunctionExpression:
                    found = true;
                    return;

                default:
                    // Conservative: keep walking common container nodes we know about.
                    break;
            }
        }

        Walk(body);
        return found;
    }

    private static Scope? FindClassScopeByName(Scope startScope, string className)
    {
        var current = startScope;
        while (current != null)
        {
            foreach (var child in current.Children.Where(child =>
                         child.Kind == ScopeKind.Class
                         && string.Equals(child.Name, className, StringComparison.Ordinal)))
            {
                return child;
            }

            current = current.Parent;
        }

        return null;
    }

}
