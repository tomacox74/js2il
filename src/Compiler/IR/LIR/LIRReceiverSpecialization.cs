using Jroc.IL;

namespace Jroc.IR;

internal static class LIRReceiverSpecialization
{
    public static void Normalize(MethodBodyIR methodBody)
    {
        for (var index = 0; index < methodBody.Instructions.Count; index++)
        {
            if (!TryGetMemberCall(
                    methodBody.Instructions[index],
                    out var receiver,
                    out var memberName,
                    out var arguments,
                    out var result)
                || !TrySelectReceiverType(
                    methodBody,
                    index,
                    receiver,
                    memberName,
                    arguments.Count,
                    out var receiverType,
                    out var family,
                    out var receiverIsProvenType))
            {
                continue;
            }

            methodBody.Instructions[index] =
                new LIRCallGuardedIntrinsicMember(
                    receiver,
                    receiverType,
                    family,
                    memberName,
                    receiverIsProvenType,
                    arguments,
                    result);
            SetObjectResultStorage(methodBody, result);
        }
    }

    private static void SetObjectResultStorage(
        MethodBodyIR methodBody,
        TempVariable result)
    {
        if (result.Index < 0
            || result.Index >= methodBody.TempStorages.Count)
        {
            return;
        }

        var storage = new ValueStorage(
            ValueStorageKind.Reference,
            typeof(object));
        methodBody.TempStorages[result.Index] = storage;
        if (result.Index >= methodBody.TempVariableSlots.Count)
        {
            return;
        }

        var slot = methodBody.TempVariableSlots[result.Index];
        if (slot >= 0 && slot < methodBody.VariableStorages.Count)
        {
            methodBody.VariableStorages[slot] = storage;
        }
    }

    private static bool TrySelectReceiverType(
        MethodBodyIR methodBody,
        int instructionIndex,
        TempVariable receiver,
        string memberName,
        int argumentCount,
        out Type receiverType,
        out JavaScriptRuntime.IntrinsicPrototypeFamily family,
        out bool receiverIsProvenType)
    {
        receiverType = null!;
        family = default;
        receiverIsProvenType = false;

        if (TryGetStorageReceiverType(
                methodBody,
                receiver,
                out var storageType))
        {
            if (storageType == typeof(JavaScriptRuntime.Array))
            {
                return false;
            }

            if (TryClassifyEligibleType(
                    storageType,
                    memberName,
                    argumentCount,
                    out family))
            {
                receiverType = storageType;
                receiverIsProvenType = true;
                return true;
            }

            if (storageType != typeof(object))
            {
                return false;
            }
        }

        var fact = methodBody.ReceiverTypeFlowFacts?
            .GetTempBefore(instructionIndex, receiver);
        if (fact == null)
        {
            return false;
        }

        foreach (var candidate in fact.CandidateClrTypes
                     .OrderBy(GetCandidatePriority)
                     .ThenBy(
                         static type => type.FullName,
                         StringComparer.Ordinal))
        {
            if (!TryClassifyEligibleType(
                    candidate,
                    memberName,
                    argumentCount,
                    out family))
            {
                continue;
            }

            receiverType = candidate;
            receiverIsProvenType =
                !fact.IncludesUnknown
                && !fact.IncludesNonCandidate
                && fact.CandidateClrTypes.Count == 1;
            return true;
        }

        return false;
    }

    private static int GetCandidatePriority(Type type)
        => type == typeof(JavaScriptRuntime.Array) ? 0 : 1;

    private static bool TryGetStorageReceiverType(
        MethodBodyIR methodBody,
        TempVariable receiver,
        out Type receiverType)
    {
        receiverType = null!;
        if (receiver.Index < 0
            || receiver.Index >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var storage = methodBody.TempStorages[receiver.Index];
        if (storage.Kind != ValueStorageKind.Reference
            || storage.ClrType == null
            || storage.ClrType == typeof(object))
        {
            return false;
        }

        receiverType = storage.ClrType;
        return true;
    }

    private static bool TryClassifyEligibleType(
        Type receiverType,
        string memberName,
        int argumentCount,
        out JavaScriptRuntime.IntrinsicPrototypeFamily family)
    {
        family = default;
        if (receiverType == typeof(JavaScriptRuntime.Array))
        {
            if (!IsEligibleArrayMember(memberName, argumentCount))
            {
                return false;
            }

            family = JavaScriptRuntime.IntrinsicPrototypeFamily.Array;
        }
        else if (typeof(JavaScriptRuntime.TypedArrayBase)
                 .IsAssignableFrom(receiverType))
        {
            if (!IsEligibleTypedArrayMember(memberName, argumentCount))
            {
                return false;
            }

            family =
                JavaScriptRuntime.IntrinsicPrototypeFamily.TypedArray;
        }
        else
        {
            return false;
        }

        return LIRToILCompiler.ResolveTypedInstanceMethodOverload(
            receiverType,
            memberName,
            argumentCount) != null;
    }

    private static bool IsEligibleArrayMember(
        string memberName,
        int argumentCount)
        => argumentCount is >= 0 and <= 5
            && IsGuardedArrayMemberName(memberName);

    private static bool IsGuardedArrayMemberName(string memberName)
        => memberName is
            "push"
            or "unshift"
            or "pop"
            or "shift"
            or "slice"
            or "splice";

    private static bool IsEligibleTypedArrayMember(
        string memberName,
        int argumentCount)
        => memberName switch
        {
            "at" => argumentCount is 0 or 1,
            "includes" or "indexOf" or "lastIndexOf" =>
                argumentCount is >= 0 and <= 2,
            "join" => argumentCount is 0 or 1,
            "reverse" => argumentCount == 0,
            _ => false
        };

    private static bool TryGetMemberCall(
        LIRInstruction instruction,
        out TempVariable receiver,
        out string memberName,
        out IReadOnlyList<TempVariable> arguments,
        out TempVariable result)
    {
        receiver = default;
        memberName = string.Empty;
        arguments = [];
        result = default;

        switch (instruction)
        {
            case LIRCallMember0 call:
                receiver = call.Receiver;
                memberName = call.MethodName;
                result = call.Result;
                return true;
            case LIRCallMember1 call:
                receiver = call.Receiver;
                memberName = call.MethodName;
                arguments = [call.A0];
                result = call.Result;
                return true;
            case LIRCallMember2 call:
                receiver = call.Receiver;
                memberName = call.MethodName;
                arguments = [call.A0, call.A1];
                result = call.Result;
                return true;
            case LIRCallMember3 call:
                receiver = call.Receiver;
                memberName = call.MethodName;
                arguments = [call.A0, call.A1, call.A2];
                result = call.Result;
                return true;
            case LIRCallMember4 call:
                receiver = call.Receiver;
                memberName = call.MethodName;
                arguments = [call.A0, call.A1, call.A2, call.A3];
                result = call.Result;
                return true;
            case LIRCallMember5 call:
                receiver = call.Receiver;
                memberName = call.MethodName;
                arguments =
                    [call.A0, call.A1, call.A2, call.A3, call.A4];
                result = call.Result;
                return true;
            default:
                return false;
        }
    }
}
