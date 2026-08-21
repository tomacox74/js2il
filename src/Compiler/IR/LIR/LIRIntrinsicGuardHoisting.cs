using Jroc.IL;
using Jroc.SymbolTables;

namespace Jroc.IR;

internal static class LIRIntrinsicGuardHoisting
{
    private const int MinimumCallsForPreheaderHoist = 1;
    private const int MinimumCallsForInLoopRegion = 2;

    public static void Normalize(MethodBodyIR methodBody)
    {
        if (methodBody.ExceptionRegions.Count > 0)
        {
            return;
        }

        if (!methodBody.Instructions.Any(
                static instruction =>
                    instruction is
                        LIRCallGuardedStringIntrinsic
                        or LIRCallGuardedIntrinsicMember))
        {
            return;
        }

        var loopFacts = methodBody.LoopNestingFacts ??=
            LIRLoopNestingAnalysis.Analyze(methodBody);
        if (loopFacts.NaturalLoops.Count == 0)
        {
            return;
        }

        var claimedCalls = new HashSet<int>();
        var plans = new List<HoistPlan>();
        var definitions = BuildDefinitions(methodBody);
        foreach (var loop in loopFacts.NaturalLoops)
        {
            foreach (var family in Enum.GetValues<
                         JavaScriptRuntime.IntrinsicPrototypeFamily>())
            {
                var calls = loop.InstructionIndices
                    .Where(index => !claimedCalls.Contains(index))
                    .Where(index => IsHoistableGuardedCall(
                        methodBody,
                        methodBody.Instructions[index],
                        definitions))
                    .Where(index => GetGuardedFamily(
                        methodBody.Instructions[index]) == family)
                    .OrderBy(static index => index)
                    .ToArray();
                if (calls.Length < MinimumCallsForPreheaderHoist
                    || !IsSafeRegion(
                        methodBody,
                        loop,
                        family,
                        calls.ToHashSet(),
                        definitions))
                {
                    continue;
                }

                var assumption = CreateBooleanTemp(methodBody);
                foreach (var callIndex in calls)
                {
                    methodBody.Instructions[callIndex] =
                        AttachAssumption(
                            methodBody.Instructions[callIndex],
                            assumption);
                    claimedCalls.Add(callIndex);
                }

                plans.Add(new HoistPlan(
                    loop.PreheaderInsertionIndex,
                    family,
                    assumption));
            }

            PlanInLoopRegions(
                methodBody,
                loop,
                claimedCalls,
                plans,
                definitions);
        }

        foreach (var plan in plans
                     .OrderByDescending(static plan => plan.InsertionIndex)
                     .ThenByDescending(static plan => plan.Family))
        {
            methodBody.Instructions.Insert(
                plan.InsertionIndex,
                new LIRCaptureIntrinsicPrototypeAssumption(
                    plan.Family,
                    plan.Assumption));
        }

        if (plans.Count > 0)
        {
            methodBody.LoopNestingFacts = null;
        }
    }

    private static void PlanInLoopRegions(
        MethodBodyIR methodBody,
        LIRNaturalLoopRegion loop,
        HashSet<int> claimedCalls,
        List<HoistPlan> plans,
        IReadOnlyDictionary<int, LIRInstruction> definitions)
    {
        var graph = LIRControlFlowGraph.Build(methodBody);
        foreach (var block in graph.Blocks)
        {
            var blockIndices = Enumerable
                .Range(block.Start, block.End - block.Start)
                .Where(loop.InstructionIndices.Contains)
                .ToArray();
            if (blockIndices.Length == 0)
            {
                continue;
            }

            foreach (var family in Enum.GetValues<
                         JavaScriptRuntime.IntrinsicPrototypeFamily>())
            {
                var regionCalls = new List<int>();
                foreach (var instructionIndex in blockIndices)
                {
                    if (claimedCalls.Contains(instructionIndex))
                    {
                        FlushRegion();
                        continue;
                    }

                    var instruction =
                        methodBody.Instructions[instructionIndex];
                    if (GetGuardedFamily(instruction) == family
                        && IsHoistableGuardedCall(
                            methodBody,
                            instruction,
                            definitions))
                    {
                        regionCalls.Add(instructionIndex);
                        continue;
                    }

                    if (IsEffectBarrier(
                            methodBody,
                            instruction,
                            family,
                            definitions))
                    {
                        FlushRegion();
                    }
                }

                FlushRegion();

                void FlushRegion()
                {
                    if (regionCalls.Count
                        >= MinimumCallsForInLoopRegion)
                    {
                        var assumption = CreateBooleanTemp(methodBody);
                        foreach (var callIndex in regionCalls)
                        {
                            methodBody.Instructions[callIndex] =
                                AttachAssumption(
                                    methodBody.Instructions[callIndex],
                                    assumption);
                            claimedCalls.Add(callIndex);
                        }

                        plans.Add(new HoistPlan(
                            regionCalls[0],
                            family,
                            assumption));
                    }

                    regionCalls.Clear();
                }
            }
        }
    }

    private static bool IsSafeRegion(
        MethodBodyIR methodBody,
        LIRNaturalLoopRegion loop,
        JavaScriptRuntime.IntrinsicPrototypeFamily family,
        IReadOnlySet<int> guardedCalls,
        IReadOnlyDictionary<int, LIRInstruction> definitions)
    {
        foreach (var instructionIndex in loop.InstructionIndices)
        {
            var instruction = methodBody.Instructions[instructionIndex];
            if (guardedCalls.Contains(instructionIndex))
            {
                continue;
            }

            if (IsEffectBarrier(
                    methodBody,
                    instruction,
                    family,
                    definitions))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsEffectBarrier(
        MethodBodyIR methodBody,
        LIRInstruction instruction,
        JavaScriptRuntime.IntrinsicPrototypeFamily family,
        IReadOnlyDictionary<int, LIRInstruction> definitions)
    {
        if (GetGuardedFamily(instruction) is { } guardedFamily)
        {
            return guardedFamily != family
                || !IsHoistableGuardedCall(
                    methodBody,
                    instruction,
                    definitions);
        }

        if (TryGetKnownCallable(instruction, out var callable))
        {
            return !methodBody.IntrinsicGuardEffectSummaries.TryGetValue(
                    callable,
                    out var summary)
                || !summary.IsGuardHoistSafe;
        }

        if (IsSafeDirectHeapRead(instruction)
            || IsSafeRuntimeCheck(instruction)
            || IsSafePrimitiveBoxing(methodBody, instruction))
        {
            return false;
        }

        var effects =
            LIRInstructionInfo.GetEffectsForScheduling(instruction);
        return (effects
                & (LIRInstructionEffects.Calls
                    | LIRInstructionEffects.ReadsHeap
                    | LIRInstructionEffects.WritesHeap
                    | LIRInstructionEffects.Suspension
                    | LIRInstructionEffects.UnsupportedBarrier
                    | LIRInstructionEffects.ScopeReplacement
                    | LIRInstructionEffects.EmitsInternalControlFlow))
            != 0;
    }

    private static bool IsHoistableGuardedCall(
        MethodBodyIR methodBody,
        LIRInstruction instruction,
        IReadOnlyDictionary<int, LIRInstruction> definitions)
    {
        IReadOnlyList<TempVariable> arguments;
        switch (instruction)
        {
            case LIRCallGuardedStringIntrinsic guarded:
                arguments = guarded.Arguments;
                break;
            case LIRCallGuardedIntrinsicMember
            {
                PrototypeFamily:
                    JavaScriptRuntime.IntrinsicPrototypeFamily.TypedArray
            } guarded:
                arguments = guarded.Arguments;
                break;
            default:
                return false;
        }

        return arguments.All(
            argument => IsProvenPrimitiveValue(
                methodBody,
                argument,
                definitions,
                []));
    }

    private static bool IsProvenPrimitiveValue(
        MethodBodyIR methodBody,
        TempVariable value,
        IReadOnlyDictionary<int, LIRInstruction> definitions,
        HashSet<int> visiting)
    {
        if (value.Index < 0
            || value.Index >= methodBody.TempStorages.Count
            || !visiting.Add(value.Index))
        {
            return false;
        }

        var storage = methodBody.TempStorages[value.Index];
        if (storage.Kind == ValueStorageKind.UnboxedValue
            || storage.Kind == ValueStorageKind.Reference
                && storage.ClrType == typeof(string))
        {
            return true;
        }

        if (!definitions.TryGetValue(value.Index, out var definition))
        {
            return false;
        }

        return definition switch
        {
            LIRConstNumber
                or LIRConstString
                or LIRConstBoolean
                or LIRConstUndefined
                or LIRConstNull => true,
            LIRConvertToObject conversion =>
                IsProvenPrimitiveValue(
                    methodBody,
                    conversion.Source,
                    definitions,
                    visiting),
            LIRCopyTemp copy =>
                IsProvenPrimitiveValue(
                    methodBody,
                    copy.Source,
                    definitions,
                    visiting),
            _ => false
        };
    }

    private static IReadOnlyDictionary<int, LIRInstruction>
        BuildDefinitions(MethodBodyIR methodBody)
    {
        var definitions = new Dictionary<int, LIRInstruction>();
        foreach (var instruction in methodBody.Instructions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined))
            {
                definitions.TryAdd(defined.Index, instruction);
            }
        }

        return definitions;
    }

    private static bool IsSafeDirectHeapRead(LIRInstruction instruction)
        => instruction is
            LIRGetStringLength
            or LIRGetJsArrayLength
            or LIRGetInt32ArrayLength
            or LIRGetInt32ArrayElement;

    private static bool IsSafeRuntimeCheck(LIRInstruction instruction)
        => instruction is LIRCallIntrinsicStatic
        {
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: nameof(
                JavaScriptRuntime.ObjectRuntime
                    .RequireObjectCoercible)
        };

    private static bool IsSafePrimitiveBoxing(
        MethodBodyIR methodBody,
        LIRInstruction instruction)
    {
        if (instruction is not LIRConvertToObject conversion
            || conversion.Source.Index < 0
            || conversion.Source.Index
                >= methodBody.TempStorages.Count)
        {
            return false;
        }

        var source = methodBody.TempStorages[
            conversion.Source.Index];
        return source.Kind == ValueStorageKind.UnboxedValue
            && source.ClrType is { IsValueType: true };
    }

    private static bool TryGetKnownCallable(
        LIRInstruction instruction,
        out Services.TwoPhaseCompilation.CallableId callable)
    {
        switch (instruction)
        {
            case LIRCallFunction { CallableId: { } target }:
                callable = target;
                return true;
            case LIRCallFunctionWithArgsArray
                {
                    CallableId: { } target
                }:
                callable = target;
                return true;
            case LIRCallDeclaredCallable declared:
                callable = declared.CallableId;
                return true;
            default:
                callable = null!;
                return false;
        }
    }

    private static JavaScriptRuntime.IntrinsicPrototypeFamily?
        GetGuardedFamily(LIRInstruction instruction)
        => instruction switch
        {
            LIRCallGuardedStringIntrinsic =>
                JavaScriptRuntime.IntrinsicPrototypeFamily.String,
            LIRCallGuardedIntrinsicMember guarded =>
                guarded.PrototypeFamily,
            _ => null
        };

    private static LIRInstruction AttachAssumption(
        LIRInstruction instruction,
        TempVariable assumption)
        => instruction switch
        {
            LIRCallGuardedStringIntrinsic guarded =>
                guarded with { PrototypeAssumption = assumption },
            LIRCallGuardedIntrinsicMember guarded =>
                guarded with { PrototypeAssumption = assumption },
            _ => throw new InvalidOperationException(
                "Only guarded intrinsic calls can use a hoisted assumption.")
        };

    private static TempVariable CreateBooleanTemp(MethodBodyIR methodBody)
    {
        var temp = new TempVariable(methodBody.Temps.Count);
        methodBody.Temps.Add(temp);
        methodBody.TempStorages.Add(
            new ValueStorage(
                ValueStorageKind.UnboxedValue,
                typeof(bool)));
        methodBody.TempVariableSlots.Add(-1);
        methodBody.PinnedTempIndices.Add(temp.Index);
        return temp;
    }

    private readonly record struct HoistPlan(
        int InsertionIndex,
        JavaScriptRuntime.IntrinsicPrototypeFamily Family,
        TempVariable Assumption);
}
