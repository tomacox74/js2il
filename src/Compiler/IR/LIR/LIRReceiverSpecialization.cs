using Jroc.IL;

namespace Jroc.IR;

internal static class LIRReceiverSpecialization
{
    private const int MinimumLoopDepth = 1;

    public static void Normalize(
        MethodBodyIR methodBody,
        ReceiverTypeFlowDiagnosticTrace? diagnostics = null)
    {
        Dictionary<int, HashSet<Type>>? staticCandidates = null;

        for (var index = 0; index < methodBody.Instructions.Count; index++)
        {
            if (TryNormalizeArrayPropertyAccess(
                    methodBody,
                    index,
                    ref staticCandidates,
                    diagnostics))
            {
                continue;
            }

            if (TryNormalizeStringPropertyAccess(
                    methodBody,
                    index,
                    ref staticCandidates,
                    diagnostics))
            {
                continue;
            }

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

            methodBody.LoopNestingFacts ??=
                LIRLoopNestingAnalysis.Analyze(methodBody);
            var loopDepth =
                methodBody.LoopNestingFacts.GetDepth(index);
            if (loopDepth < MinimumLoopDepth)
            {
                diagnostics?.RecordSpecialization(
                    index,
                    memberName,
                    receiver,
                    receiverType,
                    loopDepth,
                    receiverIsProvenType,
                    "retained-generic(cold)");
                continue;
            }

            diagnostics?.RecordSpecialization(
                index,
                memberName,
                receiver,
                receiverType,
                loopDepth,
                receiverIsProvenType,
                "guarded");
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

    private static bool TryNormalizeArrayPropertyAccess(
        MethodBodyIR methodBody,
        int instructionIndex,
        ref Dictionary<int, HashSet<Type>>? staticCandidates,
        ReceiverTypeFlowDiagnosticTrace? diagnostics)
    {
        TempVariable receiver;
        TempVariable result;
        IReadOnlyList<TempVariable> arguments;
        string memberName;
        string helperName;

        switch (methodBody.Instructions[instructionIndex])
        {
            case LIRGetItem getItem
                when IsUnboxedDouble(
                    methodBody,
                    getItem.Index):
                receiver = getItem.Object;
                result = getItem.Result;
                arguments = [receiver, getItem.Index];
                memberName = "[index]";
                helperName = nameof(
                    JavaScriptRuntime.ObjectRuntime
                        .GetArrayElementWithFallback);
                break;
            case LIRGetItemAsNumber getItemAsNumber
                when TryGetConstantString(
                        methodBody,
                        instructionIndex,
                        getItemAsNumber.Index,
                        out var objectKey)
                    && string.Equals(
                        objectKey,
                        "length",
                        StringComparison.Ordinal):
                receiver = getItemAsNumber.Object;
                result = getItemAsNumber.Result;
                arguments = [receiver];
                memberName = "length";
                helperName = nameof(
                    JavaScriptRuntime.ObjectRuntime
                        .GetArrayLengthWithFallback);
                break;
            case LIRGetItemAsNumberString getItemAsNumber
                when TryGetConstantString(
                        methodBody,
                        instructionIndex,
                        getItemAsNumber.Index,
                        out var stringKey)
                    && string.Equals(
                        stringKey,
                        "length",
                        StringComparison.Ordinal):
                receiver = getItemAsNumber.Object;
                result = getItemAsNumber.Result;
                arguments = [receiver];
                memberName = "length";
                helperName = nameof(
                    JavaScriptRuntime.ObjectRuntime
                        .GetArrayLengthWithFallback);
                break;
            default:
                return false;
        }

        if (!TryClassifyArrayReceiver(
                methodBody,
                instructionIndex,
                receiver,
                ref staticCandidates,
                out var receiverIsProvenArray))
        {
            return false;
        }

        methodBody.LoopNestingFacts ??=
            LIRLoopNestingAnalysis.Analyze(methodBody);
        var loopDepth =
            methodBody.LoopNestingFacts.GetDepth(instructionIndex);
        if (loopDepth < MinimumLoopDepth)
        {
            diagnostics?.RecordSpecialization(
                instructionIndex,
                memberName,
                receiver,
                typeof(JavaScriptRuntime.Array),
                loopDepth,
                receiverIsProvenArray,
                "retained-generic(cold)");
            return false;
        }

        diagnostics?.RecordSpecialization(
            instructionIndex,
            memberName,
            receiver,
            typeof(JavaScriptRuntime.Array),
            loopDepth,
            receiverIsProvenArray,
            "guarded");
        methodBody.Instructions[instructionIndex] =
            new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                helperName,
                arguments,
                result);
        if (string.Equals(
                helperName,
                nameof(JavaScriptRuntime.ObjectRuntime
                    .GetArrayLengthWithFallback),
                StringComparison.Ordinal))
        {
            methodBody.TempStorages[result.Index] =
                new ValueStorage(
                    ValueStorageKind.UnboxedValue,
                    typeof(double));
        }
        else
        {
            SetObjectResultStorage(methodBody, result);
        }
        return true;
    }

    private static bool TryClassifyArrayReceiver(
        MethodBodyIR methodBody,
        int instructionIndex,
        TempVariable receiver,
        ref Dictionary<int, HashSet<Type>>? staticCandidates,
        out bool receiverIsProvenArray)
    {
        receiverIsProvenArray = false;
        if (TryGetStorageReceiverType(
                methodBody,
                receiver,
                out var storageType))
        {
            if (storageType == typeof(JavaScriptRuntime.Array))
            {
                receiverIsProvenArray = true;
                return true;
            }

            if (storageType != typeof(object))
            {
                return false;
            }
        }

        var fact = methodBody.ReceiverTypeFlowFacts?
            .GetTempBefore(instructionIndex, receiver);
        if (fact?.Contains(
                typeof(JavaScriptRuntime.Array)) == true)
        {
            receiverIsProvenArray =
                !fact.IncludesUnknown
                && !fact.IncludesNonCandidate
                && fact.CandidateClrTypes.Count == 1;
            return true;
        }

        staticCandidates ??=
            BuildStaticReceiverCandidates(methodBody);
        return staticCandidates.TryGetValue(
                receiver.Index,
                out var candidates)
            && candidates.Contains(
                typeof(JavaScriptRuntime.Array));
    }

    private static bool TryGetConstantString(
        MethodBodyIR methodBody,
        int instructionIndex,
        TempVariable temp,
        out string value)
    {
        value = string.Empty;
        var current = temp;
        for (var index = instructionIndex - 1;
             index >= 0;
             index--)
        {
            var instruction = methodBody.Instructions[index];
            if (!LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                || defined.Index != current.Index)
            {
                continue;
            }

            switch (instruction)
            {
                case LIRConstString constant:
                    value = constant.Value;
                    return true;
                case LIRCopyTemp copy:
                    current = copy.Source;
                    continue;
                case LIRConvertToObject convert:
                    current = convert.Source;
                    continue;
                default:
                    return false;
            }
        }

        return false;
    }

    private static bool TryNormalizeStringPropertyAccess(
        MethodBodyIR methodBody,
        int instructionIndex,
        ref Dictionary<int, HashSet<Type>>? staticCandidates,
        ReceiverTypeFlowDiagnosticTrace? diagnostics)
    {
        TempVariable receiver;
        TempVariable result;
        IReadOnlyList<TempVariable> arguments;
        string memberName;
        string helperName;

        switch (methodBody.Instructions[instructionIndex])
        {
            case LIRGetItem getItem
                when IsUnboxedDouble(methodBody, getItem.Index):
                receiver = getItem.Object;
                result = getItem.Result;
                arguments = [receiver, getItem.Index];
                memberName = "[index]";
                helperName = nameof(
                    JavaScriptRuntime.ObjectRuntime
                        .GetStringElementWithFallback);
                break;
            default:
                return false;
        }

        if (!TryClassifyStringReceiver(
                methodBody,
                instructionIndex,
                receiver,
                ref staticCandidates,
                out var receiverIsProvenString))
        {
            return false;
        }

        methodBody.LoopNestingFacts ??=
            LIRLoopNestingAnalysis.Analyze(methodBody);
        var loopDepth =
            methodBody.LoopNestingFacts.GetDepth(instructionIndex);
        if (loopDepth < MinimumLoopDepth)
        {
            diagnostics?.RecordSpecialization(
                instructionIndex,
                memberName,
                receiver,
                typeof(string),
                loopDepth,
                receiverIsProvenString,
                "retained-generic(cold)");
            return false;
        }

        diagnostics?.RecordSpecialization(
            instructionIndex,
            memberName,
            receiver,
            typeof(string),
            loopDepth,
            receiverIsProvenString,
            "guarded");
        methodBody.Instructions[instructionIndex] =
            new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                helperName,
                arguments,
                result);
        SetObjectResultStorage(methodBody, result);
        return true;
    }

    private static bool TryClassifyStringReceiver(
        MethodBodyIR methodBody,
        int instructionIndex,
        TempVariable receiver,
        ref Dictionary<int, HashSet<Type>>? staticCandidates,
        out bool receiverIsProvenString)
    {
        receiverIsProvenString = false;
        if (TryGetStorageReceiverType(
                methodBody,
                receiver,
                out var storageType))
        {
            if (storageType == typeof(string))
            {
                receiverIsProvenString = true;
                return true;
            }

            if (storageType != typeof(object))
            {
                return false;
            }
        }

        var fact = methodBody.ReceiverTypeFlowFacts?
            .GetTempBefore(instructionIndex, receiver);
        if (fact?.Contains(typeof(string)) == true)
        {
            receiverIsProvenString =
                !fact.IncludesUnknown
                && !fact.IncludesNonCandidate
                && fact.CandidateClrTypes.Count == 1;
            return true;
        }

        staticCandidates ??=
            BuildStaticReceiverCandidates(methodBody);
        return staticCandidates.TryGetValue(
                receiver.Index,
                out var candidates)
            && candidates.Contains(typeof(string));
    }

    private static Dictionary<int, HashSet<Type>>
        BuildStaticReceiverCandidates(MethodBodyIR methodBody)
    {
        var candidates = new Dictionary<int, HashSet<Type>>();
        foreach (var (tempIndex, summary) in
                 methodBody.ReceiverTempTypeSummaries)
        {
            Add(
                new TempVariable(tempIndex),
                summary.CandidateClrTypes);
        }

        bool changed;
        do
        {
            changed = false;
            foreach (var instruction in methodBody.Instructions)
            {
                if (!LIRInstructionInfo.TryGetDefinedTemp(
                        instruction,
                        out var result)
                    || result.Index < 0)
                {
                    continue;
                }

                if (methodBody.ReceiverTempTypeSummaries.TryGetValue(
                        result.Index,
                        out var summary))
                {
                    changed |= Add(
                        result,
                        summary.CandidateClrTypes);
                }

                switch (instruction)
                {
                    case LIRLoadThis:
                        changed |= Add(
                            result,
                            methodBody.ReceiverThisTypeSummary
                                .CandidateClrTypes);
                        break;
                    case LIRLoadScopeField load:
                        changed |= Add(
                            result,
                            load.Binding.ReceiverCandidateClrTypes);
                        break;
                    case LIRLoadLeafScopeField load:
                        changed |= Add(
                            result,
                            load.Binding.ReceiverCandidateClrTypes);
                        break;
                    case LIRLoadParentScopeField load:
                        changed |= Add(
                            result,
                            load.Binding.ReceiverCandidateClrTypes);
                        break;
                    case LIRCopyTemp copy:
                        changed |= Copy(copy.Source, result);
                        break;
                    case LIRConvertToObject convert:
                        changed |= Copy(convert.Source, result);
                        break;
                    case LIRCallIntrinsicStatic
                    {
                        IntrinsicName:
                            nameof(JavaScriptRuntime.ObjectRuntime),
                        MethodName:
                            nameof(JavaScriptRuntime.ObjectRuntime
                                .RequireObjectCoercible),
                        Arguments: [var source]
                    }:
                        changed |= Copy(source, result);
                        break;
                }
            }
        }
        while (changed);

        return candidates;

        bool Add(TempVariable temp, IEnumerable<Type> types)
        {
            var added = false;
            foreach (var type in types)
            {
                if (!candidates.TryGetValue(
                        temp.Index,
                        out var target))
                {
                    target = [];
                    candidates.Add(temp.Index, target);
                }

                added |= target.Add(type);
            }

            return added;
        }

        bool Copy(TempVariable source, TempVariable target)
        {
            if (candidates.TryGetValue(
                    source.Index,
                    out var sourceCandidates))
            {
                return Add(target, sourceCandidates);
            }

            return false;
        }
    }

    private static bool IsUnboxedDouble(
        MethodBodyIR methodBody,
        TempVariable temp)
        => temp.Index >= 0
            && temp.Index < methodBody.TempStorages.Count
            && methodBody.TempStorages[temp.Index] is
            {
                Kind: ValueStorageKind.UnboxedValue,
                ClrType: not null
            } storage
            && storage.ClrType == typeof(double);

    internal static bool TryGetPotentialReceiver(
        MethodBodyIR methodBody,
        LIRInstruction instruction,
        out TempVariable receiver)
    {
        if (instruction is LIRGetItem getItem
            && IsUnboxedDouble(methodBody, getItem.Index))
        {
            receiver = getItem.Object;
            return true;
        }

        if (instruction is LIRGetItemAsNumber
                or LIRGetItemAsNumberString)
        {
            receiver = instruction switch
            {
                LIRGetItemAsNumber numericGetItem =>
                    numericGetItem.Object,
                LIRGetItemAsNumberString stringGetItem =>
                    stringGetItem.Object,
                _ => default
            };
            return true;
        }

        if (TryGetMemberCall(
                instruction,
                out receiver,
                out var memberName,
                out var arguments,
                out _)
            && (IsEligibleArrayMember(
                    memberName,
                    arguments.Count)
                || IsEligibleTypedArrayMember(
                    memberName,
                    arguments.Count)))
        {
            return true;
        }

        receiver = default;
        return false;
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
