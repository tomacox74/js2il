using Js2IL.Services;

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

        foreach (var instruction in methodBody.Instructions)
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
                // Only rewrite when the index is an unboxed double AND the value is not an unboxed double,
                // so we don't regress the existing non-boxing SetItem(object, double, double) fast path.
                if (receiverType == typeof(JavaScriptRuntime.Array)
                    && IsUnboxedDouble(methodBody, setItem.Index)
                    && !IsUnboxedDouble(methodBody, setItem.Value))
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
