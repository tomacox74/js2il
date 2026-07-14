using Jroc.IL;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;
using System.Linq;
using System.Reflection.Metadata;

namespace Jroc.IR;

/// <summary>
/// Normalizes generic item access instructions into intrinsic-specific LIR instructions when provably safe.
/// This pass is IL-agnostic: it only rewrites IR to make intent explicit and to reduce fragile pattern-matching
/// in the LIR-to-IL phase.
/// </summary>
internal static class LIRIntrinsicNormalization
{
    public static void Normalize(MethodBodyIR methodBody, ClassRegistry? classRegistry, ICallableDeclarationReader? callableReader = null)
    {
        NormalizeDirectDeclaredFunctionCalls(methodBody, callableReader);

        // Normalize intrinsic call patterns that don't require ClassRegistry.
        NormalizeCommonJsRequireCalls(methodBody);
        NormalizeIntrinsicCallArityExpansion(methodBody);

        if (classRegistry == null)
        {
            return;
        }

        // Track temps with proven specialized CLR receiver types.
        // "Specialized" primarily means JavaScriptRuntime.* types; string is included as a special-case
        // because we can safely early-bind a constrained subset of string member calls.
        // Start with strongly typed user-class field loads, then propagate through CopyTemp.
        var knownSpecializedReceiverClrTypes = new Dictionary<int, Type>();

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
                && (storage.ClrType == typeof(string)
                    || storage.ClrType.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true))
            {
                knownSpecializedReceiverClrTypes.TryAdd(tempIndex, storage.ClrType);
            }
        }

        foreach (var instruction in methodBody.Instructions.Where(static ins =>
            ins is LIRConstString
            || ins is LIRLoadUserClassInstanceField
            || ins is LIRLoadScopeField
            || ins is LIRLoadLeafScopeField
            || ins is LIRLoadParentScopeField
            || ins is LIRCallIntrinsicStatic
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
                            knownSpecializedReceiverClrTypes[loadInstanceField.Result.Index] = fieldClrType;
                        }
                    }
                    break;

                case LIRLoadScopeField loadScopeField:
                    if (loadScopeField.Result.Index >= 0
                        && IsSpecializedReceiverClrType(loadScopeField.Binding.ClrType))
                    {
                        knownSpecializedReceiverClrTypes[loadScopeField.Result.Index] = loadScopeField.Binding.ClrType!;
                    }
                    break;

                case LIRLoadLeafScopeField loadLeafScopeField:
                    if (loadLeafScopeField.Result.Index >= 0
                        && IsSpecializedReceiverClrType(loadLeafScopeField.Binding.ClrType))
                    {
                        knownSpecializedReceiverClrTypes[loadLeafScopeField.Result.Index] = loadLeafScopeField.Binding.ClrType!;
                    }
                    break;

                case LIRLoadParentScopeField loadParentScopeField:
                    if (loadParentScopeField.Result.Index >= 0
                        && IsSpecializedReceiverClrType(loadParentScopeField.Binding.ClrType))
                    {
                        knownSpecializedReceiverClrTypes[loadParentScopeField.Result.Index] = loadParentScopeField.Binding.ClrType!;
                    }
                    break;

                case LIRCallIntrinsicStatic
                    {
                        IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
                        MethodName: nameof(JavaScriptRuntime.ObjectRuntime.RequireObjectCoercible)
                    } requireObjectCoercible:
                    if (requireObjectCoercible.Result.Index >= 0
                        && requireObjectCoercible.Arguments.Count == 1
                        && knownSpecializedReceiverClrTypes.TryGetValue(requireObjectCoercible.Arguments[0].Index, out var coercedReceiverType))
                    {
                        knownSpecializedReceiverClrTypes[requireObjectCoercible.Result.Index] = coercedReceiverType;
                    }
                    break;

                case LIRCopyTemp copyTemp:
                    if (copyTemp.Destination.Index >= 0
                        && knownSpecializedReceiverClrTypes.TryGetValue(copyTemp.Source.Index, out var srcClrType))
                    {
                        knownSpecializedReceiverClrTypes[copyTemp.Destination.Index] = srcClrType;
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

            if (instruction is LIRCopyTemp copyTemp)
            {
                if (copyTemp.Destination.Index >= 0
                    && knownSpecializedReceiverClrTypes.TryGetValue(copyTemp.Source.Index, out var copiedReceiverType))
                {
                    knownSpecializedReceiverClrTypes[copyTemp.Destination.Index] = copiedReceiverType;
                }

                if (copyTemp.Destination.Index >= 0
                    && knownConstStrings.TryGetValue(copyTemp.Source.Index, out var copiedConstString))
                {
                    knownConstStrings[copyTemp.Destination.Index] = copiedConstString;
                }
            }

            if (instruction is LIRCallIntrinsicStatic
                {
                    IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
                    MethodName: nameof(JavaScriptRuntime.ObjectRuntime.RequireObjectCoercible)
                } requireObjectCoercible
                && requireObjectCoercible.Result.Index >= 0
                && requireObjectCoercible.Arguments.Count == 1
                && knownSpecializedReceiverClrTypes.TryGetValue(requireObjectCoercible.Arguments[0].Index, out var coercedReceiverType))
            {
                knownSpecializedReceiverClrTypes[requireObjectCoercible.Result.Index] = coercedReceiverType;
            }

            if (instruction is LIRGetLength getLength)
            {
                if (!knownSpecializedReceiverClrTypes.TryGetValue(getLength.Object.Index, out var receiverType))
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

                if (receiverType == typeof(string))
                {
                    methodBody.Instructions[i] = new LIRGetStringLength(getLength.Object, getLength.Result);
                    methodBody.TempStorages[getLength.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    continue;
                }

                if (receiverType == typeof(JavaScriptRuntime.Node.Buffer))
                {
                    methodBody.Instructions[i] = new LIRCallInstanceMethod(
                        Receiver: getLength.Object,
                        ReceiverClrType: typeof(JavaScriptRuntime.Node.Buffer),
                        MethodName: "get_length",
                        Arguments: Array.Empty<TempVariable>(),
                        Result: getLength.Result);
                    methodBody.TempStorages[getLength.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    continue;
                }

                continue;
            }

            if (instruction is LIRGetItem getItem)
            {
                if (!knownSpecializedReceiverClrTypes.TryGetValue(getItem.Object.Index, out var receiverType))
                {
                    continue;
                }

                // Int32Array element access (numeric index).
                if (receiverType == typeof(JavaScriptRuntime.Int32Array)
                    && IsNumericDouble(methodBody, getItem.Index))
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

            if (instruction is LIRGetItemAsNumber getItemAsNumber)
            {
                if (knownSpecializedReceiverClrTypes.TryGetValue(getItemAsNumber.Object.Index, out var receiverType)
                    && receiverType == typeof(JavaScriptRuntime.Int32Array)
                    && IsNumericDouble(methodBody, getItemAsNumber.Index))
                {
                    methodBody.Instructions[i] = new LIRGetInt32ArrayElement(
                        getItemAsNumber.Object,
                        getItemAsNumber.Index,
                        getItemAsNumber.Result);
                    methodBody.TempStorages[getItemAsNumber.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                    continue;
                }

                if (IsTempStringReference(methodBody, getItemAsNumber.Index))
                {
                    methodBody.Instructions[i] = new LIRGetItemAsNumberString(
                        getItemAsNumber.Object,
                        getItemAsNumber.Index,
                        getItemAsNumber.Result);
                }

                continue;
            }

            if (instruction is LIRSetItem setItem)
            {
                if (!knownSpecializedReceiverClrTypes.TryGetValue(setItem.Object.Index, out var receiverType))
                {
                    continue;
                }

                // Array length set (string key "length").
                if (receiverType == typeof(JavaScriptRuntime.Array)
                    && knownConstStrings.TryGetValue(setItem.Index.Index, out var setItemKey)
                    && string.Equals(setItemKey, "length", StringComparison.Ordinal))
                {
                    // Rewrite: SetItem(array, "length", value, result) -> SetJsArrayLength(array, value, result)
                    methodBody.Instructions[i] = new LIRSetJsArrayLength(setItem.Object, setItem.Value, setItem.Result);
                    continue;
                }

                // Int32Array element set (numeric index + numeric value).
                if (receiverType == typeof(JavaScriptRuntime.Int32Array)
                    && IsNumericDouble(methodBody, setItem.Index)
                    && IsNumericDouble(methodBody, setItem.Value))
                {
                    // Rewrite: SetItem(receiver, indexDouble, valueDouble, result) -> SetInt32ArrayElement(receiver, indexDouble, valueDouble, result)
                    methodBody.Instructions[i] = new LIRSetInt32ArrayElement(setItem.Object, setItem.Index, setItem.Value, setItem.Result);

                    // Ensure result storage is unboxed double when materialized.
                    if (setItem.Result.Index >= 0)
                    {
                        if (IsUnboxedDouble(methodBody, setItem.Value))
                        {
                            methodBody.TempStorages[setItem.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                        }
                    }
                }
            }

            if (instruction is LIRCallMember0 callMember0)
            {
                if (TryNormalizeArrayMemberCall(
                        methodBody,
                        i,
                        callMember0.Receiver,
                        callMember0.MethodName,
                        Array.Empty<TempVariable>(),
                        callMember0.Result,
                        knownSpecializedReceiverClrTypes))
                {
                    continue;
                }

                if (!knownSpecializedReceiverClrTypes.TryGetValue(callMember0.Receiver.Index, out var receiverType)
                    || receiverType != typeof(string)
                    || !TryResolveSafeStringIntrinsicReturnClrType(methodBody, callMember0.MethodName, Array.Empty<TempVariable>(), out var returnClrType))
                {
                    continue;
                }

                methodBody.Instructions[i] = new LIRCallIntrinsicStatic(
                    IntrinsicName: "String",
                    MethodName: callMember0.MethodName,
                    Arguments: new[] { callMember0.Receiver },
                    Result: callMember0.Result);

                if (callMember0.Result.Index >= 0)
                {
                    ApplyResolvedIntrinsicReturnStorage(methodBody, callMember0.Result, returnClrType);
                }

                continue;
            }

            if (instruction is LIRCallMember1 callMember1)
            {
                if (TryNormalizeArrayMemberCall(
                        methodBody,
                        i,
                        callMember1.Receiver,
                        callMember1.MethodName,
                        new[] { callMember1.A0 },
                        callMember1.Result,
                        knownSpecializedReceiverClrTypes))
                {
                    continue;
                }

                if (knownSpecializedReceiverClrTypes.TryGetValue(callMember1.Receiver.Index, out var stringReceiverType)
                    && stringReceiverType == typeof(string)
                    && TryResolveSafeStringIntrinsicReturnClrType(methodBody, callMember1.MethodName, new[] { callMember1.A0 }, out var stringReturnClrType))
                {
                    methodBody.Instructions[i] = new LIRCallIntrinsicStatic(
                        IntrinsicName: "String",
                        MethodName: callMember1.MethodName,
                        Arguments: new[] { callMember1.Receiver, callMember1.A0 },
                        Result: callMember1.Result);

                    if (callMember1.Result.Index >= 0)
                    {
                        ApplyResolvedIntrinsicReturnStorage(methodBody, callMember1.Result, stringReturnClrType);
                    }

                    continue;
                }

                if (!knownSpecializedReceiverClrTypes.TryGetValue(callMember1.Receiver.Index, out var receiverType)
                    || receiverType != typeof(JavaScriptRuntime.RegExp)
                    || !string.Equals(callMember1.MethodName, "test", StringComparison.Ordinal))
                {
                    continue;
                }

                methodBody.Instructions[i] = new LIRCallInstanceMethod(
                    Receiver: callMember1.Receiver,
                    ReceiverClrType: typeof(JavaScriptRuntime.RegExp),
                    MethodName: callMember1.MethodName,
                    Arguments: new[] { callMember1.A0 },
                    Result: callMember1.Result);

                continue;
            }

            if (instruction is LIRCallMember2 callMember2)
            {
                if (TryNormalizeArrayMemberCall(
                        methodBody,
                        i,
                        callMember2.Receiver,
                        callMember2.MethodName,
                        new[] { callMember2.A0, callMember2.A1 },
                        callMember2.Result,
                        knownSpecializedReceiverClrTypes))
                {
                    continue;
                }

                if (!knownSpecializedReceiverClrTypes.TryGetValue(callMember2.Receiver.Index, out var receiverType)
                    || receiverType != typeof(string)
                    || !TryResolveSafeStringIntrinsicReturnClrType(methodBody, callMember2.MethodName, new[] { callMember2.A0, callMember2.A1 }, out var returnClrType))
                {
                    continue;
                }

                methodBody.Instructions[i] = new LIRCallIntrinsicStatic(
                    IntrinsicName: "String",
                    MethodName: callMember2.MethodName,
                    Arguments: new[] { callMember2.Receiver, callMember2.A0, callMember2.A1 },
                    Result: callMember2.Result);

                if (callMember2.Result.Index >= 0)
                {
                    ApplyResolvedIntrinsicReturnStorage(methodBody, callMember2.Result, returnClrType);
                }

                continue;
            }

            if (instruction is LIRCallMember3 callMember3)
            {
                TryNormalizeArrayMemberCall(
                    methodBody,
                    i,
                    callMember3.Receiver,
                    callMember3.MethodName,
                    new[] { callMember3.A0, callMember3.A1, callMember3.A2 },
                    callMember3.Result,
                    knownSpecializedReceiverClrTypes);
            }
        }

        FuseCharCodeAtWithConvertToNumber(methodBody);
        FuseGetItemWithConvertToNumber(methodBody);
    }

    public static void NormalizeLateNumericMemberCalls(MethodBodyIR methodBody)
    {
        FuseCharCodeAtWithConvertToNumber(methodBody);
    }

    private static void NormalizeDirectDeclaredFunctionCalls(MethodBodyIR methodBody, ICallableDeclarationReader? callableReader)
    {
        if (callableReader == null)
        {
            return;
        }

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is not LIRCallFunction callFunction || callFunction.CallableId is not { } callableId)
            {
                continue;
            }

            // Preserve semantic paths that require full runtime argument context.
            if (callableId.NeedsArgumentsObject || callableId.HasRestParameters)
            {
                continue;
            }

            if (!callableReader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
            {
                continue;
            }

            var signature = callableReader.GetSignature(callableId);
            if (signature?.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.SingleScope)
            {
                continue;
            }

            bool requiresScopes = signature?.RequiresScopesParameter ?? true;
            int jsParamCount = callableId.JsParamCount;
            int argsToPass = Math.Min(callFunction.Arguments.Count, jsParamCount);

            var declaredArgs = new List<TempVariable>((requiresScopes ? 1 : 0) + 1 + jsParamCount);
            var prelude = new List<LIRInstruction>(1 + Math.Max(0, jsParamCount - argsToPass));

            if (requiresScopes)
            {
                declaredArgs.Add(callFunction.ScopesArray);
            }

            // Normal function call path: new.target is undefined (modeled as null).
            var newTargetTemp = CreateTemp(methodBody, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            prelude.Add(new LIRConstUndefined(newTargetTemp));
            declaredArgs.Add(newTargetTemp);

            for (int argIndex = 0; argIndex < argsToPass; argIndex++)
            {
                declaredArgs.Add(callFunction.Arguments[argIndex]);
            }

            // Pad missing args with undefined to preserve JS call semantics.
            for (int argIndex = argsToPass; argIndex < jsParamCount; argIndex++)
            {
                var undefinedArgTemp = CreateTemp(methodBody, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                prelude.Add(new LIRConstUndefined(undefinedArgTemp));
                declaredArgs.Add(undefinedArgTemp);
            }

            if (prelude.Count > 0)
            {
                methodBody.Instructions.InsertRange(i, prelude);
                i += prelude.Count;
            }

            methodBody.Instructions[i] = new LIRCallDeclaredCallable(callableId, declaredArgs, callFunction.Result);

            if (callFunction.Result.Index >= 0 && callFunction.Result.Index < methodBody.TempStorages.Count)
            {
                methodBody.TempStorages[callFunction.Result.Index] = GetDeclaredCallResultStorage(signature?.ReturnClrType);
            }
        }
    }

    private static ValueStorage GetDeclaredCallResultStorage(Type? returnClrType)
        => returnClrType switch
        {
            Type type when type == typeof(double) => new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)),
            Type type when type == typeof(bool) => new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)),
            Type type when type == typeof(string) => new ValueStorage(ValueStorageKind.Reference, typeof(string)),
            Type type when type == typeof(JavaScriptRuntime.Array) => new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)),
            _ => new ValueStorage(ValueStorageKind.Reference, typeof(object))
        };

    /// <summary>
    /// Peephole pass: fuses LIRGetItem(obj, index, result) immediately followed by
    /// LIRConvertToNumber(result, numResult) into a single LIRGetItemAsNumber(obj, index, numResult)
    /// when the intermediate result temp is used only by that LIRConvertToNumber.
    /// This avoids boxing the GetItem return value when the consumer expects a number.
    /// </summary>
    private static void FuseGetItemWithConvertToNumber(MethodBodyIR methodBody)
    {
        // Maps temp index to the instruction index of the single LIRConvertToNumber that consumes it.
        // Temps consumed by more than one LIRConvertToNumber are tracked in ineligibleTempIndices.
        var singleConvertToNumberConsumerByTempIndex = new Dictionary<int, int>();
        var ineligibleTempIndices = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is LIRConvertToNumber conv && conv.Source.Index >= 0)
            {
                var srcIdx = conv.Source.Index;
                if (!ineligibleTempIndices.Contains(srcIdx))
                {
                    if (singleConvertToNumberConsumerByTempIndex.TryGetValue(srcIdx, out _))
                    {
                        // Second consumer found - mark as ineligible and remove from eligible map.
                        ineligibleTempIndices.Add(srcIdx);
                        singleConvertToNumberConsumerByTempIndex.Remove(srcIdx);
                    }
                    else
                    {
                        singleConvertToNumberConsumerByTempIndex[srcIdx] = i;
                    }
                }
            }
        }

        var indicesToRemove = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is not LIRGetItem getItem)
            {
                continue;
            }

            var resultIdx = getItem.Result.Index;
            if (resultIdx < 0)
            {
                continue;
            }

            // Check that result storage is object (not already an unboxed double from normalization).
            var resultStorage = methodBody.TempStorages[resultIdx];
            if (resultStorage.Kind == ValueStorageKind.UnboxedValue)
            {
                continue;
            }

            // Find the single LIRConvertToNumber that consumes this result.
            if (!singleConvertToNumberConsumerByTempIndex.TryGetValue(resultIdx, out var convIdx))
            {
                continue;
            }

            var conv = (LIRConvertToNumber)methodBody.Instructions[convIdx];

            // Verify the result temp is not used by any other instruction (besides the GetItem def and the ConvertToNumber).
            if (IsTempUsedOutside(methodBody, getItem.Result, new HashSet<int> { i, convIdx }))
            {
                continue;
            }

            if (convIdx == i + 1)
            {
                // With no intervening evaluation, access and coercion can remain fused directly.
                methodBody.Instructions[i] = new LIRGetItemAsNumber(getItem.Object, getItem.Index, conv.Result);
                indicesToRemove.Add(convIdx);

                if (conv.Result.Index >= 0 && conv.Result.Index < methodBody.TempStorages.Count)
                {
                    methodBody.TempStorages[conv.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                }
            }
            else if (convIdx > i + 1)
            {
                if (methodBody.Instructions
                    .Skip(i + 1)
                    .Take(convIdx - i - 1)
                    .Any(static instruction => instruction is
                        LIRLabel
                        or LIRBranch
                        or LIRLeave
                        or LIRBranchIfFalse
                        or LIRBranchIfTrue
                        or LIRReturn
                        or LIRReturnUndefinedImmediate
                        or LIRTailCallFunctionReturn
                        or LIRThrow
                        or LIRThrowNewTypeError
                        or LIREndFinally
                        or LIRAwait
                        or LIRYield
                        or LIRAsyncStateSwitch
                        or LIRGeneratorStateSwitch))
                {
                    continue;
                }

                // Preserve the original access and coercion positions. Numeric values travel in a
                // value type; object coercion remains deferred until the existing conversion.
                methodBody.TempStorages[resultIdx] = new ValueStorage(
                    ValueStorageKind.UnboxedValue,
                    typeof(JavaScriptRuntime.NumericIndexedValue));
            }
        }

        if (indicesToRemove.Count == 0)
        {
            return;
        }

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

    private static void FuseCharCodeAtWithConvertToNumber(MethodBodyIR methodBody)
    {
        var singleConvertToNumberConsumerByTempIndex = new Dictionary<int, int>();
        var ineligibleTempIndices = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is LIRConvertToNumber conv && conv.Source.Index >= 0)
            {
                var srcIdx = conv.Source.Index;
                if (!ineligibleTempIndices.Contains(srcIdx))
                {
                    if (singleConvertToNumberConsumerByTempIndex.TryGetValue(srcIdx, out _))
                    {
                        ineligibleTempIndices.Add(srcIdx);
                        singleConvertToNumberConsumerByTempIndex.Remove(srcIdx);
                    }
                    else
                    {
                        singleConvertToNumberConsumerByTempIndex[srcIdx] = i;
                    }
                }
            }
        }

        var indicesToRemove = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is not LIRCallMember1 callMember
                || !string.Equals(callMember.MethodName, "charCodeAt", StringComparison.Ordinal)
                || callMember.Result.Index < 0)
            {
                continue;
            }

            if (!singleConvertToNumberConsumerByTempIndex.TryGetValue(callMember.Result.Index, out var convIdx))
            {
                continue;
            }

            var canMove = convIdx > i && CanMoveNumberCoercionAcrossInterveningInstructions(methodBody, i + 1, convIdx);
            if (!canMove)
            {
                continue;
            }

            if (IsTempUsedOutside(methodBody, callMember.Result, new HashSet<int> { i, convIdx }))
            {
                continue;
            }

            var conv = (LIRConvertToNumber)methodBody.Instructions[convIdx];
            methodBody.Instructions[i] = new LIRCallIntrinsicStatic(
                IntrinsicName: "String",
                MethodName: nameof(JavaScriptRuntime.String.CharCodeAtAsNumber),
                Arguments: new[] { callMember.Receiver, callMember.A0 },
                Result: conv.Result);
            indicesToRemove.Add(convIdx);

            if (conv.Result.Index >= 0 && conv.Result.Index < methodBody.TempStorages.Count)
            {
                methodBody.TempStorages[conv.Result.Index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
            }
        }

        if (indicesToRemove.Count == 0)
        {
            return;
        }

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

    private static bool TryResolveSafeStringIntrinsicReturnClrType(
        MethodBodyIR methodBody,
        string methodName,
        IReadOnlyList<TempVariable> arguments,
        out Type returnClrType)
    {
        returnClrType = null!;
        var argCount = arguments.Count;

        if (!IsStringMethodEligibleForEarlyBind(methodName, argCount))
        {
            return false;
        }

        // Early-bind signatures where the receiver is the first `string` parameter and
        // each JS argument either keeps runtime coercion (`object`) or is already proven
        // to be a string at compile time.
        var safe = typeof(JavaScriptRuntime.String)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
            .Where(mi =>
            {
                var ps = mi.GetParameters();
                if (ps.Length != argCount + 1)
                {
                    return false;
                }

                if (ps[0].ParameterType != typeof(string))
                {
                    return false;
                }

                for (int i = 1; i < ps.Length; i++)
                {
                    var parameterType = ps[i].ParameterType;
                    if (parameterType == typeof(object))
                    {
                        continue;
                    }

                    if (parameterType == typeof(string)
                        && IsTempStringReference(methodBody, arguments[i - 1]))
                    {
                        continue;
                    }

                    if (parameterType == typeof(bool)
                        && IsUnboxedBool(methodBody, arguments[i - 1]))
                    {
                        continue;
                    }

                    if (parameterType == typeof(double)
                        && IsUnboxedDouble(methodBody, arguments[i - 1]))
                    {
                        continue;
                    }

                    if (parameterType != typeof(object))
                    {
                        return false;
                    }
                }

                return true;
            })
            .OrderBy(mi => mi.ToString(), StringComparer.Ordinal)
            .FirstOrDefault();

        if (safe == null)
        {
            return false;
        }

        returnClrType = safe.ReturnType;
        return true;
    }

    private static bool IsStringMethodEligibleForEarlyBind(string methodName, int argCount)
    {
        return argCount switch
        {
            0 => methodName is "charAt" or "charCodeAt" or "trim" or "trimStart" or "trimLeft" or "trimEnd" or "trimRight" or "toLowerCase" or "toUpperCase",
            1 => methodName is "charAt" or "charCodeAt" or "substring" or "substr" or "slice" or "indexOf" or "lastIndexOf" or "startsWith" or "endsWith" or "includes",
            2 => methodName is "substring" or "substr" or "slice" or "indexOf" or "lastIndexOf" or "startsWith" or "endsWith" or "includes",
            _ => false
        };
    }

    private static void ApplyResolvedIntrinsicReturnStorage(MethodBodyIR methodBody, TempVariable result, Type returnClrType)
    {
        if (result.Index < 0 || result.Index >= methodBody.TempStorages.Count)
        {
            return;
        }

        ValueStorage storage;
        if (returnClrType == typeof(bool))
        {
            storage = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
        }
        else if (returnClrType == typeof(double))
        {
            storage = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
        }
        else if (returnClrType == typeof(string))
        {
            storage = new ValueStorage(ValueStorageKind.Reference, typeof(string));
        }
        else if (returnClrType == typeof(JavaScriptRuntime.Array))
        {
            storage = new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array));
        }
        else
        {
            storage = new ValueStorage(ValueStorageKind.Reference, typeof(object));
        }

        methodBody.TempStorages[result.Index] = storage;
        ApplySingleAssignmentSlotStorage(methodBody, result, storage);
    }

    private static void ApplySingleAssignmentSlotStorage(MethodBodyIR methodBody, TempVariable result, ValueStorage storage)
    {
        if (result.Index < 0 || result.Index >= methodBody.TempVariableSlots.Count)
        {
            return;
        }

        var slot = methodBody.TempVariableSlots[result.Index];
        if (slot < 0
            || slot >= methodBody.VariableStorages.Count
            || !methodBody.SingleAssignmentSlots.Contains(slot))
        {
            return;
        }

        var current = methodBody.VariableStorages[slot];
        if (current.Kind == ValueStorageKind.Reference && current.ClrType == typeof(object))
        {
            methodBody.VariableStorages[slot] = storage;
        }
    }

    private static bool TryNormalizeArrayMemberCall(
        MethodBodyIR methodBody,
        int instructionIndex,
        TempVariable receiver,
        string methodName,
        IReadOnlyList<TempVariable> arguments,
        TempVariable result,
        IDictionary<int, Type> knownSpecializedReceiverClrTypes)
    {
        if (!knownSpecializedReceiverClrTypes.TryGetValue(receiver.Index, out var receiverType)
            || receiverType != typeof(JavaScriptRuntime.Array)
            || !TryResolveArrayMemberReturnClrType(methodName, arguments.Count, out var returnClrType))
        {
            return false;
        }

        methodBody.Instructions[instructionIndex] = new LIRCallInstanceMethod(
            Receiver: receiver,
            ReceiverClrType: typeof(JavaScriptRuntime.Array),
            MethodName: methodName,
            Arguments: arguments,
            Result: result);

        ApplyResolvedIntrinsicReturnStorage(methodBody, result, returnClrType);
        if (result.Index >= 0 && IsSpecializedReceiverClrType(returnClrType))
        {
            knownSpecializedReceiverClrTypes[result.Index] = returnClrType;
        }

        return true;
    }

    private static bool TryResolveArrayMemberReturnClrType(string methodName, int argCount, out Type returnClrType)
    {
        returnClrType = typeof(object);

        switch (methodName)
        {
            case "push" when argCount is 0 or 1:
            case "unshift" when argCount is 0 or 1:
                returnClrType = typeof(double);
                return true;

            case "pop" when argCount == 0:
            case "shift" when argCount == 0:
                returnClrType = typeof(object);
                return true;

            case "slice" when argCount is >= 0 and <= 2:
            case "splice" when argCount is >= 0 and <= 3:
                returnClrType = typeof(JavaScriptRuntime.Array);
                return true;

            default:
                return false;
        }
    }

    private static bool IsNumericDouble(MethodBodyIR methodBody, TempVariable temp)
    {
        return IsUnboxedDouble(methodBody, temp) || IsBoxedDouble(methodBody, temp);
    }

    private static bool IsBoxedDouble(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var storage = methodBody.TempStorages[temp.Index];
        return storage.Kind == ValueStorageKind.BoxedValue && storage.ClrType == typeof(double);
    }

    private static bool IsTempStringReference(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var storage = methodBody.TempStorages[temp.Index];
        return storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string);
    }

    private static bool IsUnboxedBool(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var storage = methodBody.TempStorages[temp.Index];
        return storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool);
    }

    /// <summary>
    /// Rewrites <see cref="LIRCallIntrinsic"/> instructions whose argument array is a provably-small
    /// <see cref="LIRBuildArray"/> (≤3 elements) to <see cref="LIRCallInstanceMethod"/> with explicit
    /// element temps.  This lets the IL emitter select arity-specific method overloads (e.g.,
    /// <c>Console.log(a0, a1)</c>) instead of the variadic <c>object[]</c> form, and removes the
    /// type-dispatch peephole that previously lived in <c>LIRToILCompiler</c>.
    /// </summary>
    private static void NormalizeIntrinsicCallArityExpansion(MethodBodyIR methodBody)
    {
        // Index build-array definitions by their result temp so we can look them up from a call site.
        var buildArrays = new Dictionary<int, (int DefIndex, IReadOnlyList<TempVariable> Elements)>();
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is LIRBuildArray buildArray && buildArray.Result.Index >= 0)
            {
                buildArrays[buildArray.Result.Index] = (i, buildArray.Elements);
            }
        }

        if (buildArrays.Count == 0)
        {
            return;
        }

        var indicesToRemove = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is not LIRCallIntrinsic callIntrinsic)
            {
                continue;
            }

            if (callIntrinsic.ArgumentsArray.Index < 0
                || !buildArrays.TryGetValue(callIntrinsic.ArgumentsArray.Index, out var buildInfo)
                || buildInfo.Elements.Count > 3)
            {
                continue;
            }

            // Rewrite to LIRCallInstanceMethod so the IL emitter can use arity-specific overloads
            // instead of the variadic object[] form.
            methodBody.Instructions[i] = new LIRCallInstanceMethod(
                Receiver: callIntrinsic.IntrinsicObject,
                ReceiverClrType: typeof(JavaScriptRuntime.Console),
                MethodName: callIntrinsic.Name,
                Arguments: buildInfo.Elements,
                Result: callIntrinsic.Result);

            // If the args array temp is used only by its build instruction and this call, remove the build.
            if (!IsTempUsedOutside(methodBody, callIntrinsic.ArgumentsArray,
                    ignoreInstructionIndices: new HashSet<int> { buildInfo.DefIndex, i }))
            {
                indicesToRemove.Add(buildInfo.DefIndex);
            }
        }

        if (indicesToRemove.Count == 0)
        {
            return;
        }

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
            TempVariable requireValueTemp;
            TempVariable scopesArrayTemp;
            TempVariable moduleIdTemp;
            TempVariable callResultTemp;

            switch (methodBody.Instructions[i])
            {
                case LIRCallFunctionValue call:
                    if (!IsRequireDelegateTemp(methodBody, call.FunctionValue))
                    {
                        continue;
                    }

                    // Extract moduleId = first arg, or undefined when no args.
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

                    requireValueTemp = call.FunctionValue;
                    scopesArrayTemp = call.ScopesArray;
                    callResultTemp = call.Result;
                    break;

                case LIRCallFunctionValue0 call0:
                    if (!IsRequireDelegateTemp(methodBody, call0.FunctionValue))
                    {
                        continue;
                    }

                    moduleIdTemp = CreateTemp(methodBody, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    methodBody.Instructions.Insert(i, new LIRConstUndefined(moduleIdTemp));

                    // We inserted before the call; shift indices and adjust tracked removal indices.
                    ShiftIndicesAfterInsert(indicesToRemove, i);
                    ShiftIndicesAfterInsert(buildArrays, i);
                    ShiftIndicesAfterInsert(buildScopesArrays, i);
                    ShiftIndicesAfterInsert(convertToObjectDefs, i);
                    i++; // call moved one slot forward

                    requireValueTemp = call0.FunctionValue;
                    scopesArrayTemp = call0.ScopesArray;
                    callResultTemp = call0.Result;
                    break;

                case LIRCallFunctionValue1 call1:
                    if (!IsRequireDelegateTemp(methodBody, call1.FunctionValue))
                    {
                        continue;
                    }

                    requireValueTemp = call1.FunctionValue;
                    scopesArrayTemp = call1.ScopesArray;
                    moduleIdTemp = call1.A0;
                    callResultTemp = call1.Result;
                    break;

                case LIRCallFunctionValue2 call2:
                    if (!IsRequireDelegateTemp(methodBody, call2.FunctionValue))
                    {
                        continue;
                    }

                    requireValueTemp = call2.FunctionValue;
                    scopesArrayTemp = call2.ScopesArray;
                    moduleIdTemp = call2.A0;
                    callResultTemp = call2.Result;
                    break;

                case LIRCallFunctionValue3 call3:
                    if (!IsRequireDelegateTemp(methodBody, call3.FunctionValue))
                    {
                        continue;
                    }

                    requireValueTemp = call3.FunctionValue;
                    scopesArrayTemp = call3.ScopesArray;
                    moduleIdTemp = call3.A0;
                    callResultTemp = call3.Result;
                    break;

                default:
                    continue;
            }

            // If the scopes array temp is only used by its build + this call, remove the build.
            if (scopesArrayTemp.Index >= 0
                && buildScopesArrays.TryGetValue(scopesArrayTemp.Index, out var scopesDefIndex)
                && !IsTempUsedOutside(methodBody, scopesArrayTemp, ignoreInstructionIndices: new HashSet<int> { scopesDefIndex, i }))
            {
                indicesToRemove.Add(scopesDefIndex);
            }

            methodBody.Instructions[i] = new LIRCallRequire(requireValueTemp, moduleIdTemp, callResultTemp);

            if (callResultTemp.Index >= 0 && callResultTemp.Index < methodBody.TempStorages.Count)
            {
                methodBody.TempStorages[callResultTemp.Index] = new ValueStorage(ValueStorageKind.Reference, typeof(object));
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

    private static bool CanMoveNumberCoercionAcrossInterveningInstructions(MethodBodyIR methodBody, int startInclusive, int endExclusive)
    {
        for (int i = startInclusive; i < endExclusive; i++)
        {
            if (!IsPureInterveningInstruction(methodBody.Instructions[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPureInterveningInstruction(LIRInstruction instruction)
        => instruction is LIRCopyTemp
            or LIRConstNumber
            or LIRConstString
            or LIRConstBoolean
            or LIRConstUndefined
            or LIRConstNull;

    private static bool IsRequireDelegateTemp(MethodBodyIR methodBody, TempVariable callee)
    {
        if (callee.Index < 0 || callee.Index >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var calleeStorage = methodBody.TempStorages[callee.Index];
        return calleeStorage.Kind == ValueStorageKind.Reference
            && calleeStorage.ClrType == typeof(global::JavaScriptRuntime.CommonJS.RequireDelegate);
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

    private static bool IsSpecializedReceiverClrType(Type? clrType)
    {
        return clrType != null
            && clrType != typeof(object)
            && !clrType.IsValueType
            && (clrType == typeof(string)
                || clrType.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true);
    }
}
