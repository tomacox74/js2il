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

        foreach (var instruction in methodBody.Instructions)
        {
            switch (instruction)
            {
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
                    break;
            }
        }

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];

            if (instruction is LIRGetItem getItem)
            {
                // Only normalize numeric-index accesses.
                if (!IsUnboxedDouble(methodBody, getItem.Index))
                {
                    continue;
                }

                if (knownIntrinsicReceiverClrTypes.TryGetValue(getItem.Object.Index, out var receiverType)
                    && receiverType == typeof(JavaScriptRuntime.Int32Array))
                {
                    // Rewrite: GetItem(receiver, indexDouble, result) -> GetInt32ArrayElement(receiver, indexDouble, result)
                    methodBody.Instructions[i] = new LIRGetInt32ArrayElement(getItem.Object, getItem.Index, getItem.Result);

                    // Ensure result storage is unboxed double.
                    methodBody.TempStorages[getItem.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                }

                continue;
            }

            if (instruction is LIRSetItem setItem)
            {
                // Only normalize numeric-index and numeric-value accesses.
                if (!IsUnboxedDouble(methodBody, setItem.Index) || !IsUnboxedDouble(methodBody, setItem.Value))
                {
                    continue;
                }

                if (knownIntrinsicReceiverClrTypes.TryGetValue(setItem.Object.Index, out var receiverType)
                    && receiverType == typeof(JavaScriptRuntime.Int32Array))
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
