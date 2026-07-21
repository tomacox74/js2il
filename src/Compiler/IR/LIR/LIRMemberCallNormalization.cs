using Jroc.IL;
using Jroc.Services;
using System.Reflection.Metadata;

namespace Jroc.IR;

/// <summary>
/// Normalizes generic member calls into explicit early-bound user-class member call instructions
/// when provably safe.
///
/// This pass is IL-agnostic: it rewrites LIR to make intent explicit and to keep LIR->IL emission simpler.
/// </summary>
internal static class LIRMemberCallNormalization
{
    public static void Normalize(MethodBodyIR methodBody, ClassRegistry? classRegistry)
    {
        if (classRegistry == null)
        {
            return;
        }

        // Map: argsArrayTempIndex -> (defInstructionIndex, elements)
        var buildArrays = new Dictionary<int, (int DefIndex, IReadOnlyList<TempVariable> Elements)>();
        var convertToObjectDefs = new Dictionary<int, int>();
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            switch (methodBody.Instructions[i])
            {
                case LIRBuildArray buildArray when buildArray.Result.Index >= 0:
                    buildArrays[buildArray.Result.Index] = (i, buildArray.Elements);
                    break;

                case LIRConvertToObject convertToObject when convertToObject.Result.Index >= 0:
                    convertToObjectDefs[convertToObject.Result.Index] = i;
                    break;
            }
        }

        // Track temps with proven user-class receiver type handles.
        // Seed from strongly-typed user-class field loads, then propagate through CopyTemp.
        var knownUserClassReceiverTypeHandles = new Dictionary<int, EntityHandle>();
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];
            switch (instruction)
            {
                case LIRNewUserClass newUserClass:
                    // `new C(...)` produces an instance of the generated CLR type *unless* the
                    // constructor has PL5.4a ctor-return override semantics, in which case the
                    // result temp may be overwritten to an arbitrary value.
                    if (newUserClass.Result.Index >= 0
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out var constructedTypeHandle)
                        && !classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__jroc_ctorReturn", out _))
                    {
                        knownUserClassReceiverTypeHandles[newUserClass.Result.Index] = constructedTypeHandle;
                    }
                    break;

                case LIRLoadUserClassInstanceField loadInstanceField:
                    if (loadInstanceField.Result.Index >= 0
                        && TryGetDeclaredUserClassFieldTypeHandle(
                            classRegistry,
                            loadInstanceField.RegistryClassName,
                            loadInstanceField.FieldName,
                            loadInstanceField.IsPrivateField,
                            isStaticField: false,
                            out var fieldTypeHandle))
                    {
                        knownUserClassReceiverTypeHandles[loadInstanceField.Result.Index] = fieldTypeHandle;
                        SetTempStorage(
                            methodBody,
                            loadInstanceField.Result,
                            new ValueStorage(ValueStorageKind.Reference, typeof(object), fieldTypeHandle));
                    }
                    break;

                case LIRCopyTemp copyTemp:
                    if (copyTemp.Destination.Index >= 0
                        && knownUserClassReceiverTypeHandles.TryGetValue(copyTemp.Source.Index, out var srcHandle)
                        && GetTempStorage(methodBody, copyTemp.Source).TypeHandle == srcHandle)
                    {
                        knownUserClassReceiverTypeHandles[copyTemp.Destination.Index] = srcHandle;
                        SetTempStorage(
                            methodBody,
                            copyTemp.Destination,
                            new ValueStorage(ValueStorageKind.Reference, typeof(object), srcHandle));
                    }
                    break;

                case LIRCallIntrinsicStatic
                {
                    IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
                    MethodName: nameof(JavaScriptRuntime.ObjectRuntime.RequireObjectCoercible),
                    Arguments.Count: 1
                } requireObjectCoercible
                    when requireObjectCoercible.Result.Index >= 0
                         && knownUserClassReceiverTypeHandles.TryGetValue(
                             requireObjectCoercible.Arguments[0].Index,
                             out var coercedTypeHandle)
                         && GetTempStorage(methodBody, requireObjectCoercible.Arguments[0]).TypeHandle
                             == coercedTypeHandle:
                {
                    var typedStorage = new ValueStorage(
                        ValueStorageKind.Reference,
                        typeof(object),
                        coercedTypeHandle);
                    methodBody.Instructions[i] = requireObjectCoercible with
                    {
                        GenericTypeArgument = typedStorage
                    };
                    SetTempStorage(methodBody, requireObjectCoercible.Result, typedStorage);
                    knownUserClassReceiverTypeHandles[requireObjectCoercible.Result.Index] = coercedTypeHandle;
                    break;
                }
            }
        }

        var indicesToRemove = new HashSet<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (!TryGetMemberCallSite(
                    methodBody,
                    i,
                    buildArrays,
                    out var receiver,
                    out var methodName,
                    out var arguments,
                    out var result,
                    out var buildArrayDefIndex))
            {
                continue;
            }

            var argCount = arguments.Count;

            // Resolve a uniquely-defined instance method by name + call-site arity.
            if (!classRegistry.TryResolveUniqueInstanceMethod(
                    methodName,
                    argCount,
                    out _,
                    out var receiverTypeHandle,
                    out var methodHandle,
                    out var returnClrType,
                    out var returnTypeHandle,
                    out var hasScopesParam,
                    out var maxParamCount,
                    out var parameterClrTypes))
            {
                continue;
            }

            // If the resolved method is known to return the receiver type (i.e. `return this`-style),
            // then a proven-typed receiver implies a proven-typed result as well.
            // This enables early-binding for chained calls without runtime type tests.
            var resolvedReceiverEntityHandle = (EntityHandle)receiverTypeHandle;
            bool resultIsReceiverType = !returnTypeHandle.IsNil && returnTypeHandle.Equals(resolvedReceiverEntityHandle);
            var normalizedArguments = ForwardBoxedArgumentsToTypedSources(
                methodBody,
                arguments,
                parameterClrTypes,
                maxParamCount,
                convertToObjectDefs,
                buildArrayDefIndex,
                i,
                indicesToRemove);

            // Receiver-proven typed case: emit direct early-bound call without runtime-dispatch fallback.
            if (receiver.Index >= 0
                && knownUserClassReceiverTypeHandles.TryGetValue(receiver.Index, out var knownReceiverHandle)
                && knownReceiverHandle.Equals(receiverTypeHandle))
            {
                methodBody.Instructions[i] = new LIRCallTypedMember(
                    receiver,
                    receiverTypeHandle,
                    methodHandle,
                    hasScopesParam,
                    returnClrType,
                    maxParamCount,
                    parameterClrTypes,
                    normalizedArguments,
                    result);

                if (result.Index >= 0)
                {
                    var resultStorage = GetPreferredTypedCallResultStorage(returnClrType, returnTypeHandle);
                    if (CanRetypeResultTemp(methodBody, result, resultStorage))
                    {
                        SetTempStorage(methodBody, result, resultStorage);
                    }
                }

                if (resultIsReceiverType && result.Index >= 0)
                {
                    knownUserClassReceiverTypeHandles[result.Index] = resolvedReceiverEntityHandle;
                }

                if (buildArrayDefIndex >= 0)
                {
                    indicesToRemove.Add(buildArrayDefIndex);
                }
                continue;
            }

            // Otherwise: emit guarded early-bound call with fallback to runtime dispatch.
            methodBody.Instructions[i] = new LIRCallTypedMemberWithFallback(
                receiver,
                methodName,
                receiverTypeHandle,
                methodHandle,
                hasScopesParam,
                returnClrType,
                maxParamCount,
                parameterClrTypes,
                normalizedArguments,
                result);

            if (buildArrayDefIndex >= 0)
            {
                indicesToRemove.Add(buildArrayDefIndex);
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

    private static bool TryGetMemberCallSite(
        MethodBodyIR methodBody,
        int instructionIndex,
        Dictionary<int, (int DefIndex, IReadOnlyList<TempVariable> Elements)> buildArrays,
        out TempVariable receiver,
        out string methodName,
        out IReadOnlyList<TempVariable> arguments,
        out TempVariable result,
        out int buildArrayDefIndex)
    {
        receiver = default;
        methodName = string.Empty;
        arguments = Array.Empty<TempVariable>();
        result = default;
        buildArrayDefIndex = -1;

        switch (methodBody.Instructions[instructionIndex])
        {
            case LIRCallMember callMember:
                {
                    // Only normalize when the args array is a temp created by LIRBuildArray.
                    var argsTempIndex = callMember.ArgumentsArray.Index;
                    if (argsTempIndex < 0)
                    {
                        return false;
                    }

                    if (!buildArrays.TryGetValue(argsTempIndex, out var buildInfo))
                    {
                        return false;
                    }

                    // Only remove the args array build when the temp is used ONLY by the build itself and this call.
                    // If it flows elsewhere, keep existing semantics.
                    if (IsTempUsedOutside(methodBody, callMember.ArgumentsArray, ignoreInstructionIndices: new HashSet<int> { buildInfo.DefIndex, instructionIndex }))
                    {
                        return false;
                    }

                    receiver = callMember.Receiver;
                    methodName = callMember.MethodName;
                    arguments = buildInfo.Elements;
                    result = callMember.Result;
                    buildArrayDefIndex = buildInfo.DefIndex;
                    return true;
                }

            case LIRCallMember0 callMember0:
                receiver = callMember0.Receiver;
                methodName = callMember0.MethodName;
                result = callMember0.Result;
                return true;

            case LIRCallMember1 callMember1:
                receiver = callMember1.Receiver;
                methodName = callMember1.MethodName;
                arguments = new[] { callMember1.A0 };
                result = callMember1.Result;
                return true;

            case LIRCallMember2 callMember2:
                receiver = callMember2.Receiver;
                methodName = callMember2.MethodName;
                arguments = new[] { callMember2.A0, callMember2.A1 };
                result = callMember2.Result;
                return true;

            case LIRCallMember3 callMember3:
                receiver = callMember3.Receiver;
                methodName = callMember3.MethodName;
                arguments = new[] { callMember3.A0, callMember3.A1, callMember3.A2 };
                result = callMember3.Result;
                return true;

            default:
                return false;
        }
    }

    private static bool TryGetDeclaredUserClassFieldTypeHandle(
        ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField,
        out EntityHandle typeHandle)
    {
        typeHandle = default;

        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldTypeHandle(registryClassName, fieldName, out typeHandle);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldTypeHandle(registryClassName, fieldName, out typeHandle);
        }

        return classRegistry.TryGetFieldTypeHandle(registryClassName, fieldName, out typeHandle);
    }

    private static ValueStorage GetPreferredTypedCallResultStorage(Type returnClrType, EntityHandle returnTypeHandle)
    {
        if (!returnTypeHandle.IsNil)
        {
            return new ValueStorage(ValueStorageKind.Reference, typeof(object), returnTypeHandle);
        }

        if (returnClrType == typeof(double) || returnClrType == typeof(bool) || returnClrType == typeof(JavaScriptRuntime.JsNull))
        {
            return new ValueStorage(ValueStorageKind.UnboxedValue, returnClrType);
        }

        if (returnClrType != typeof(object))
        {
            return new ValueStorage(ValueStorageKind.Reference, returnClrType);
        }

        return new ValueStorage(ValueStorageKind.Reference, typeof(object));
    }

    private static void SetTempStorage(MethodBodyIR methodBody, TempVariable temp, ValueStorage storage)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            methodBody.TempStorages[temp.Index] = storage;
        }
    }

    private static bool CanRetypeResultTemp(MethodBodyIR methodBody, TempVariable temp, ValueStorage desiredStorage)
    {
        var slot = GetTempVariableSlot(methodBody, temp);
        if (slot < 0 || slot >= methodBody.VariableStorages.Count)
        {
            return true;
        }

        return AreCompatiblePinnedSlotStorages(methodBody.VariableStorages[slot], desiredStorage);
    }

    private static bool AreCompatiblePinnedSlotStorages(ValueStorage slotStorage, ValueStorage desiredStorage)
        => ValueStorageFacts.IsSameRuntimeRepresentation(slotStorage, desiredStorage);

    private static int GetTempVariableSlot(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            return methodBody.TempVariableSlots[temp.Index];
        }

        return -1;
    }

    private static IReadOnlyList<TempVariable> ForwardBoxedArgumentsToTypedSources(
        MethodBodyIR methodBody,
        IReadOnlyList<TempVariable> arguments,
        IReadOnlyList<Type?> parameterClrTypes,
        int maxParamCount,
        IReadOnlyDictionary<int, int> convertToObjectDefs,
        int buildArrayDefIndex,
        int callInstructionIndex,
        HashSet<int> indicesToRemove)
    {
        if (arguments.Count == 0 || convertToObjectDefs.Count == 0)
        {
            return arguments;
        }

        TempVariable[]? rewritten = null;

        for (int argIndex = 0; argIndex < arguments.Count; argIndex++)
        {
            if (argIndex >= maxParamCount || argIndex >= parameterClrTypes.Count)
            {
                continue;
            }

            var parameterClrType = parameterClrTypes[argIndex];
            if (parameterClrType == null || parameterClrType == typeof(object))
            {
                continue;
            }

            var argument = arguments[argIndex];
            if (argument.Index < 0
                || !convertToObjectDefs.TryGetValue(argument.Index, out var convertDefIndex)
                || methodBody.Instructions[convertDefIndex] is not LIRConvertToObject convertToObject
                || !CanForwardConvertToObjectSource(methodBody, convertToObject, parameterClrType, convertDefIndex, callInstructionIndex))
            {
                continue;
            }

            rewritten ??= arguments.ToArray();
            rewritten[argIndex] = convertToObject.Source;

            if (GetTempVariableSlot(methodBody, convertToObject.Result) >= 0)
            {
                continue;
            }

            var ignoredInstructions = new HashSet<int> { convertDefIndex, callInstructionIndex };
            if (buildArrayDefIndex >= 0)
            {
                ignoredInstructions.Add(buildArrayDefIndex);
            }

            if (!IsTempUsedOutside(methodBody, convertToObject.Result, ignoredInstructions))
            {
                indicesToRemove.Add(convertDefIndex);
            }
        }

        return rewritten ?? arguments;
    }

    private static bool CanForwardConvertToObjectSource(
        MethodBodyIR methodBody,
        LIRConvertToObject convertToObject,
        Type parameterClrType,
        int convertInstructionIndex,
        int callInstructionIndex)
    {
        var sourceStorage = GetTempStorage(methodBody, convertToObject.Source);
        var targetStorage = parameterClrType.IsValueType
            ? new ValueStorage(ValueStorageKind.UnboxedValue, parameterClrType)
            : new ValueStorage(ValueStorageKind.Reference, parameterClrType);

        if (!ValueStorageFacts.CanFlowTo(sourceStorage, targetStorage))
        {
            return false;
        }

        // Keep this rewrite to stable, already-evaluated values. The old LIRBuildArray
        // materialized arguments before dispatch; forwarding an effectful source could
        // otherwise move evaluation into the direct/fallback call branches.
        return IsStableAlreadyEvaluatedTemp(methodBody, convertToObject.Source, convertInstructionIndex, callInstructionIndex);
    }

    private static bool IsStableAlreadyEvaluatedTemp(
        MethodBodyIR methodBody,
        TempVariable temp,
        int snapshotInstructionIndex,
        int useInstructionIndex)
    {
        var variableSlot = GetTempVariableSlot(methodBody, temp);
        if (variableSlot >= 0)
        {
            return methodBody.SingleAssignmentSlots.Contains(variableSlot)
                || !IsVariableSlotWrittenBetween(methodBody, variableSlot, snapshotInstructionIndex, useInstructionIndex);
        }

        return TryFindDefInstruction(methodBody, temp) switch
        {
            LIRConstNumber or LIRConstString or LIRConstBoolean or LIRConstUndefined or LIRConstNull => true,
            LIRLoadParameter or LIRLoadThis => true,
            LIRCopyTemp copyTemp => IsStableAlreadyEvaluatedTemp(methodBody, copyTemp.Source, snapshotInstructionIndex, useInstructionIndex),
            _ => false
        };
    }

    private static bool IsVariableSlotWrittenBetween(MethodBodyIR methodBody, int variableSlot, int startInstructionIndex, int endInstructionIndex)
    {
        if (startInstructionIndex >= endInstructionIndex)
        {
            return true;
        }

        for (int i = startInstructionIndex + 1; i < endInstructionIndex; i++)
        {
            if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[i], out var defined)
                && GetTempVariableSlot(methodBody, defined) == variableSlot)
            {
                return true;
            }
        }

        return false;
    }

    private static LIRInstruction? TryFindDefInstruction(MethodBodyIR methodBody, TempVariable temp)
    {
        foreach (var instruction in methodBody.Instructions)
        {
            if (TryGetDefinedTemp(instruction, out var defined) && defined.Index == temp.Index)
            {
                return instruction;
            }
        }

        return null;
    }

    private static bool TryGetDefinedTemp(LIRInstruction instruction, out TempVariable defined)
    {
        switch (instruction)
        {
            case LIRConstNumber x: defined = x.Result; return true;
            case LIRConstString x: defined = x.Result; return true;
            case LIRConstBoolean x: defined = x.Result; return true;
            case LIRConstUndefined x: defined = x.Result; return true;
            case LIRConstNull x: defined = x.Result; return true;
            case LIRLoadParameter x: defined = x.Result; return true;
            case LIRLoadThis x: defined = x.Result; return true;
            case LIRCopyTemp x: defined = x.Destination; return true;
            default:
                defined = default;
                return false;
        }
    }

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        var variableSlot = GetTempVariableSlot(methodBody, temp);
        if (variableSlot >= 0 && variableSlot < methodBody.VariableStorages.Count)
        {
            return methodBody.VariableStorages[variableSlot];
        }

        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }

        return new ValueStorage(ValueStorageKind.Unknown);
    }

    private static bool IsTempUsedOutside(MethodBodyIR methodBody, TempVariable temp, HashSet<int> ignoreInstructionIndices)
    {
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (ignoreInstructionIndices.Contains(i))
            {
                continue;
            }

            if (TempLocalAllocator.UsesTemp(methodBody.Instructions[i], temp))
            {
                return true;
            }
        }

        return false;
    }
}
