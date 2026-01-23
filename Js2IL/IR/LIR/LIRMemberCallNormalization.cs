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
            if (methodBody.Instructions[i] is not LIRCallMember callMember)
            {
                continue;
            }

            // Only normalize when the args array is a temp created by LIRBuildArray.
            var argsTempIndex = callMember.ArgumentsArray.Index;
            if (argsTempIndex < 0)
            {
                continue;
            }

            if (!buildArrays.TryGetValue(argsTempIndex, out var buildInfo))
            {
                continue;
            }

            // Only remove the args array build when the temp is used ONLY by the build itself and this call.
            // If it flows elsewhere, keep existing semantics.
            if (IsTempUsedOutside(methodBody, callMember.ArgumentsArray, ignoreInstructionIndices: new HashSet<int> { buildInfo.DefIndex, i }))
            {
                continue;
            }

            var argCount = buildInfo.Elements.Count;

            // Resolve a uniquely-defined instance method by name + call-site arity.
            if (!classRegistry.TryResolveUniqueInstanceMethod(
                    callMember.MethodName,
                    argCount,
                    out _,
                    out var receiverTypeHandle,
                    out var methodHandle,
                    out var returnClrType,
                    out var maxParamCount))
            {
                continue;
            }

            // Receiver-proven typed case: emit direct early-bound call without runtime-dispatch fallback.
            if (callMember.Receiver.Index >= 0
                && knownUserClassReceiverTypeHandles.TryGetValue(callMember.Receiver.Index, out var knownReceiverHandle)
                && knownReceiverHandle.Equals(receiverTypeHandle))
            {
                methodBody.Instructions[i] = new LIRCallTypedMember(
                    Receiver: callMember.Receiver,
                    ReceiverTypeHandle: receiverTypeHandle,
                    MethodHandle: methodHandle,
                    ReturnClrType: returnClrType,
                    MaxParamCount: maxParamCount,
                    Arguments: buildInfo.Elements,
                    Result: callMember.Result);

                indicesToRemove.Add(buildInfo.DefIndex);
                continue;
            }

            // Otherwise: emit guarded early-bound call with fallback to runtime dispatch.
            methodBody.Instructions[i] = new LIRCallTypedMemberWithFallback(
                Receiver: callMember.Receiver,
                MethodName: callMember.MethodName,
                ReceiverTypeHandle: receiverTypeHandle,
                MethodHandle: methodHandle,
                ReturnClrType: returnClrType,
                MaxParamCount: maxParamCount,
                Arguments: buildInfo.Elements,
                Result: callMember.Result);

            indicesToRemove.Add(buildInfo.DefIndex);
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
