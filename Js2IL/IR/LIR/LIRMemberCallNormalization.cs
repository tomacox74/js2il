using Js2IL.IL;
using Js2IL.Services;
using System.Reflection.Metadata;

namespace Js2IL.IR;

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
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is LIRBuildArray buildArray && buildArray.Result.Index >= 0)
            {
                buildArrays[buildArray.Result.Index] = (i, buildArray.Elements);
            }
        }

        // Track temps with proven user-class receiver type handles.
        // Seed from strongly-typed user-class field loads, then propagate through CopyTemp.
        var knownUserClassReceiverTypeHandles = new Dictionary<int, EntityHandle>();
        foreach (var instruction in methodBody.Instructions)
        {
            switch (instruction)
            {
                case LIRNewUserClass newUserClass:
                    // `new C(...)` produces an instance of the generated CLR type *unless* the
                    // constructor has PL5.4a ctor-return override semantics, in which case the
                    // result temp may be overwritten to an arbitrary value.
                    if (newUserClass.Result.Index >= 0
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out var constructedTypeHandle)
                        && !classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__js2il_ctorReturn", out _))
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
                    }
                    break;

                case LIRCopyTemp copyTemp:
                    if (copyTemp.Destination.Index >= 0
                        && knownUserClassReceiverTypeHandles.TryGetValue(copyTemp.Source.Index, out var srcHandle))
                    {
                        knownUserClassReceiverTypeHandles[copyTemp.Destination.Index] = srcHandle;
                    }
                    break;
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
                    out var maxParamCount))
            {
                continue;
            }

            // If the resolved method is known to return the receiver type (i.e. `return this`-style),
            // then a proven-typed receiver implies a proven-typed result as well.
            // This enables early-binding for chained calls without runtime type tests.
            var resolvedReceiverEntityHandle = (EntityHandle)receiverTypeHandle;
            bool resultIsReceiverType = !returnTypeHandle.IsNil && returnTypeHandle.Equals(resolvedReceiverEntityHandle);

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
                    arguments,
                    result);

                if (resultIsReceiverType && result.Index >= 0)
                {
                    knownUserClassReceiverTypeHandles[result.Index] = resolvedReceiverEntityHandle;

                    // Also propagate the proven type handle into the temp's storage so IL emission can avoid
                    // redundant castclass when the result is used as a typed receiver (including stackified temps).
                    if (result.Index < methodBody.TempStorages.Count)
                    {
                        methodBody.TempStorages[result.Index] = new ValueStorage(
                            ValueStorageKind.Reference,
                            typeof(object),
                            resolvedReceiverEntityHandle);
                    }
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
                arguments,
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
}
