using Js2IL.IL;
using Js2IL.Services;
using System.Linq;

namespace Js2IL.IR;

/// <summary>
/// Normalizes generic item access instructions into intrinsic-specific LIR instructions when provably safe.
/// This pass is IL-agnostic: it only rewrites IR to make intent explicit and to reduce fragile pattern-matching
/// in the LIR-to-IL phase.
/// </summary>
internal static class LIRIntrinsicNormalization
{
    public static void Normalize(MethodBodyIR methodBody, ClassRegistry? classRegistry)
    {
        // Normalize intrinsic call patterns that don't require ClassRegistry.
        NormalizeCommonJsRequireCalls(methodBody);

        if (classRegistry == null)
        {
            return;
        }

        // Track temps with proven intrinsic CLR receiver types.
        // Start with strongly typed user-class field loads, then propagate through CopyTemp.
        var knownIntrinsicReceiverClrTypes = new Dictionary<int, Type>();

        // Track temps proven to be constant strings (e.g., property access keys).
        // This allows safe rewrites like `r.promise` -> direct CLR getter when r is known.
        var knownConstStrings = new Dictionary<int, string>();

        // Seed known intrinsic receiver types from temp storage when it is already specific.
        // This covers cases like Promise.withResolvers() which returns a concrete runtime type.
        for (int tempIndex = 0; tempIndex < methodBody.TempStorages.Count; tempIndex++)
        {
            var storage = methodBody.TempStorages[tempIndex];
            if (storage.Kind == ValueStorageKind.Reference
                && storage.ClrType != null
                && storage.ClrType != typeof(object)
                && storage.ClrType.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true)
            {
                knownIntrinsicReceiverClrTypes.TryAdd(tempIndex, storage.ClrType);
            }
        }

        foreach (var instruction in methodBody.Instructions.Where(static ins =>
            ins is LIRConstString
            || ins is LIRLoadUserClassInstanceField
            || ins is LIRCopyTemp))
        {
            switch (instruction)
            {
                case LIRConstString constString:
                    if (constString.Result.Index >= 0)
                    {
                        knownConstStrings[constString.Result.Index] = constString.Value;
                    }
                    break;

                case LIRLoadUserClassInstanceField loadInstanceField:
                    if (loadInstanceField.Result.Index >= 0)
                    {
                        var fieldClrType = GetDeclaredUserClassFieldClrType(
                            classRegistry,
                            loadInstanceField.RegistryClassName,
                            loadInstanceField.FieldName,
                            loadInstanceField.IsPrivateField,
                            isStaticField: false);

                        if (fieldClrType != typeof(object)
                            && fieldClrType.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true)
                        {
                            knownIntrinsicReceiverClrTypes[loadInstanceField.Result.Index] = fieldClrType;
                        }
                    }
                    break;

                case LIRCopyTemp copyTemp:
                    if (copyTemp.Destination.Index >= 0
                        && knownIntrinsicReceiverClrTypes.TryGetValue(copyTemp.Source.Index, out var srcClrType))
                    {
                        knownIntrinsicReceiverClrTypes[copyTemp.Destination.Index] = srcClrType;
                    }

                    if (copyTemp.Destination.Index >= 0
                        && knownConstStrings.TryGetValue(copyTemp.Source.Index, out var srcConstString))
                    {
                        knownConstStrings[copyTemp.Destination.Index] = srcConstString;
                    }
                    break;
            }
        }

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];

            if (instruction is LIRGetLength getLength)
            {
                if (!knownIntrinsicReceiverClrTypes.TryGetValue(getLength.Object.Index, out var receiverType))
                {
                    continue;
                }

                if (receiverType == typeof(JavaScriptRuntime.Array))
                {
                    methodBody.Instructions[i] = new LIRGetJsArrayLength(getLength.Object, getLength.Result);
                    methodBody.TempStorages[getLength.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    continue;
                }

                if (receiverType == typeof(JavaScriptRuntime.Int32Array))
                {
                    methodBody.Instructions[i] = new LIRGetInt32ArrayLength(getLength.Object, getLength.Result);
                    methodBody.TempStorages[getLength.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    continue;
                }

                continue;
            }

            if (instruction is LIRGetItem getItem)
            {
                if (!knownIntrinsicReceiverClrTypes.TryGetValue(getItem.Object.Index, out var receiverType))
                {
                    continue;
                }

                // Int32Array element access (numeric index).
                if (receiverType == typeof(JavaScriptRuntime.Int32Array)
                    && IsUnboxedDouble(methodBody, getItem.Index))
                {
                    // Rewrite: GetItem(receiver, indexDouble, result) -> GetInt32ArrayElement(receiver, indexDouble, result)
                    methodBody.Instructions[i] = new LIRGetInt32ArrayElement(getItem.Object, getItem.Index, getItem.Result);

                    // Ensure result storage is unboxed double.
                    methodBody.TempStorages[getItem.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    continue;
                }

                // Array element access (numeric index).
                if (receiverType == typeof(JavaScriptRuntime.Array)
                    && IsUnboxedDouble(methodBody, getItem.Index))
                {
                    // Rewrite: GetItem(array, indexDouble, result) -> GetJsArrayElement(array, indexDouble, result)
                    methodBody.Instructions[i] = new LIRGetJsArrayElement(getItem.Object, getItem.Index, getItem.Result);
                    continue;
                }

                // Promise.withResolvers().promise access (string key).
                // Rewrite: GetItem(PromiseWithResolvers, "promise", result) -> callvirt PromiseWithResolvers.get_promise()
                if (receiverType == typeof(JavaScriptRuntime.PromiseWithResolvers)
                    && knownConstStrings.TryGetValue(getItem.Index.Index, out var key)
                    && string.Equals(key, "promise", StringComparison.Ordinal))
                {
                    methodBody.Instructions[i] = new LIRCallInstanceMethod(
                        Receiver: getItem.Object,
                        ReceiverClrType: typeof(JavaScriptRuntime.PromiseWithResolvers),
                        MethodName: "get_promise",
                        Arguments: Array.Empty<TempVariable>(),
                        Result: getItem.Result);

                    // Ensure result storage is the concrete Promise type for better downstream codegen.
                    methodBody.TempStorages[getItem.Result.Index] = new ValueStorage(
                        ValueStorageKind.Reference,
                        typeof(JavaScriptRuntime.Promise));
                }

                continue;
            }

            if (instruction is LIRSetItem setItem)
            {
                if (!knownIntrinsicReceiverClrTypes.TryGetValue(setItem.Object.Index, out var receiverType))
                {
                    continue;
                }

                // Array element set (numeric index).
                if (receiverType == typeof(JavaScriptRuntime.Array)
                    && IsUnboxedDouble(methodBody, setItem.Index))
                {
                    // Rewrite: SetItem(array, indexDouble, valueObj, result) -> SetJsArrayElement(array, indexDouble, valueObj, result)
                    methodBody.Instructions[i] = new LIRSetJsArrayElement(setItem.Object, setItem.Index, setItem.Value, setItem.Result);
                    continue;
                }

                // Int32Array element set (numeric index + numeric value).
                if (receiverType == typeof(JavaScriptRuntime.Int32Array)
                    && IsUnboxedDouble(methodBody, setItem.Index)
                    && IsUnboxedDouble(methodBody, setItem.Value))
                {
                    // Rewrite: SetItem(receiver, indexDouble, valueDouble, result) -> SetInt32ArrayElement(receiver, indexDouble, valueDouble, result)
                    methodBody.Instructions[i] = new LIRSetInt32ArrayElement(setItem.Object, setItem.Index, setItem.Value, setItem.Result);

                    // Ensure result storage is unboxed double when materialized.
                    if (setItem.Result.Index >= 0)
                    {
                        methodBody.TempStorages[setItem.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    }
                }
            }
        }
    }

    private static void NormalizeCommonJsRequireCalls(MethodBodyIR methodBody)
    {
        // Map: argsArrayTempIndex -> (defInstructionIndex, elements)
        var buildArrays = new Dictionary<int, (int DefIndex, IReadOnlyList<TempVariable> Elements)>();
        var buildScopesArrays = new Dictionary<int, int>();
        var convertToObjectDefs = new Dictionary<int, int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            switch (methodBody.Instructions[i])
            {
                case LIRBuildArray buildArray when buildArray.Result.Index >= 0:
                    buildArrays[buildArray.Result.Index] = (i, buildArray.Elements);
                    break;

                case LIRBuildScopesArray buildScopes when buildScopes.Result.Index >= 0:
                    buildScopesArrays[buildScopes.Result.Index] = i;
                    break;

                case LIRConvertToObject convert when convert.Result.Index >= 0:
                    convertToObjectDefs[convert.Result.Index] = i;
                    break;
            }
        }

        if (buildArrays.Count == 0)
        {
            // Still worth scanning calls; but without build arrays we can't safely extract the first arg.
        }

        var indicesToRemove = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is not LIRCallFunctionValue call)
            {
                continue;
            }

            if (call.FunctionValue.Index < 0 || call.FunctionValue.Index >= methodBody.TempStorages.Count)
            {
                continue;
            }

            var calleeStorage = methodBody.TempStorages[call.FunctionValue.Index];
            if (calleeStorage.Kind != ValueStorageKind.Reference
                || calleeStorage.ClrType != typeof(global::JavaScriptRuntime.CommonJS.RequireDelegate))
            {
                continue;
            }

            // Extract moduleId = first arg, or undefined when no args.
            TempVariable moduleIdTemp;
            if (call.ArgumentsArray.Index >= 0
                && buildArrays.TryGetValue(call.ArgumentsArray.Index, out var buildInfo))
            {
                if (buildInfo.Elements.Count > 0)
                {
                    moduleIdTemp = buildInfo.Elements[0];
                }
                else
                {
                    moduleIdTemp = CreateTemp(methodBody, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    methodBody.Instructions.Insert(i, new LIRConstUndefined(moduleIdTemp));

                    // We inserted before the call; shift indices and adjust tracked removal indices.
                    ShiftIndicesAfterInsert(indicesToRemove, i);
                    ShiftIndicesAfterInsert(buildArrays, i);
                    ShiftIndicesAfterInsert(buildScopesArrays, i);
                    ShiftIndicesAfterInsert(convertToObjectDefs, i);
                    i++; // call moved one slot forward
                }

                // If the args array temp is only used by its build + this call, remove the build.
                if (!IsTempUsedOutside(methodBody, call.ArgumentsArray, ignoreInstructionIndices: new HashSet<int> { buildInfo.DefIndex, i }))
                {
                    indicesToRemove.Add(buildInfo.DefIndex);

                    // Also remove dead boxing conversions that existed solely to populate the args array.
                    // Keep the first element if it becomes the require(moduleId) argument.
                    for (int argIndex = 1; argIndex < buildInfo.Elements.Count; argIndex++)
                    {
                        var elem = buildInfo.Elements[argIndex];
                        if (elem.Index < 0)
                        {
                            continue;
                        }

                        if (!convertToObjectDefs.TryGetValue(elem.Index, out var defIndex))
                        {
                            continue;
                        }

                        if (!IsTempUsedOutside(methodBody, elem, ignoreInstructionIndices: new HashSet<int> { defIndex, buildInfo.DefIndex, i }))
                        {
                            indicesToRemove.Add(defIndex);
                        }
                    }
                }
            }
            else
            {
                // Unknown args array provenance; stay conservative.
                continue;
            }

            // If the scopes array temp is only used by its build + this call, remove the build.
            if (call.ScopesArray.Index >= 0
                && buildScopesArrays.TryGetValue(call.ScopesArray.Index, out var scopesDefIndex)
                && !IsTempUsedOutside(methodBody, call.ScopesArray, ignoreInstructionIndices: new HashSet<int> { scopesDefIndex, i }))
            {
                indicesToRemove.Add(scopesDefIndex);
            }

            methodBody.Instructions[i] = new LIRCallRequire(call.FunctionValue, moduleIdTemp, call.Result);

            if (call.Result.Index >= 0 && call.Result.Index < methodBody.TempStorages.Count)
            {
                methodBody.TempStorages[call.Result.Index] = new ValueStorage(ValueStorageKind.Reference, typeof(object));
            }
        }

        if (indicesToRemove.Count == 0)
        {
            return;
        }

        // Remove instructions in a single compaction pass.
        var newInstructions = new List<LIRInstruction>(methodBody.Instructions.Count - indicesToRemove.Count);
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (!indicesToRemove.Contains(i))
            {
                newInstructions.Add(methodBody.Instructions[i]);
            }
        }
        methodBody.Instructions.Clear();
        methodBody.Instructions.AddRange(newInstructions);
    }

    private static TempVariable CreateTemp(MethodBodyIR methodBody, ValueStorage storage)
    {
        var index = methodBody.Temps.Count;
        var temp = new TempVariable(index);
        methodBody.Temps.Add(temp);
        methodBody.TempStorages.Add(storage);
        methodBody.TempVariableSlots.Add(-1);
        return temp;
    }

    private static bool IsTempUsedOutside(MethodBodyIR methodBody, TempVariable temp, HashSet<int> ignoreInstructionIndices)
    {
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (ignoreInstructionIndices.Contains(i))
            {
                continue;
            }

            foreach (var used in TempLocalAllocator.EnumerateUsedTemps(methodBody.Instructions[i]))
            {
                if (used.Index == temp.Index)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static void ShiftIndicesAfterInsert(HashSet<int> indices, int insertAt)
    {
        if (indices.Count == 0)
        {
            return;
        }

        var updated = new HashSet<int>();
        foreach (var idx in indices)
        {
            updated.Add(idx >= insertAt ? idx + 1 : idx);
        }

        indices.Clear();
        foreach (var idx in updated)
        {
            indices.Add(idx);
        }
    }

    private static void ShiftIndicesAfterInsert(Dictionary<int, int> indexMap, int insertAt)
    {
        if (indexMap.Count == 0)
        {
            return;
        }

        var keys = indexMap.Keys.ToArray();
        foreach (var k in keys)
        {
            var v = indexMap[k];
            if (v >= insertAt)
            {
                indexMap[k] = v + 1;
            }
        }
    }

    private static void ShiftIndicesAfterInsert(Dictionary<int, (int DefIndex, IReadOnlyList<TempVariable> Elements)> indexMap, int insertAt)
    {
        if (indexMap.Count == 0)
        {
            return;
        }

        var keys = indexMap.Keys.ToArray();
        foreach (var k in keys)
        {
            var (defIndex, elems) = indexMap[k];
            if (defIndex >= insertAt)
            {
                indexMap[k] = (defIndex + 1, elems);
            }
        }
    }

    private static bool IsUnboxedDouble(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var storage = methodBody.TempStorages[temp.Index];
        return storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double);
    }

    private static Type GetDeclaredUserClassFieldClrType(
        ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField)
    {
        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        return classRegistry.TryGetFieldClrType(registryClassName, fieldName, out var t2)
            ? t2
            : typeof(object);
    }
}
