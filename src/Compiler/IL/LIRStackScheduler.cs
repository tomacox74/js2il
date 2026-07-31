using Jroc.IR;

namespace Jroc.IL;

/// <summary>
/// Builds an IL-backend emission plan over LIR. The initial identity mode owns
/// no temp residency decisions and preserves the existing source-order output.
/// </summary>
internal static class LIRStackScheduler
{
    internal static LIRStackSchedule Build(
        MethodBodyIR methodBody,
        LIRStackSchedulerOptions options)
    {
        ArgumentNullException.ThrowIfNull(methodBody);

        var schedule = options.Mode switch
        {
            LIRStackSchedulerMode.Identity => BuildIdentityUnvalidated(methodBody),
            LIRStackSchedulerMode.TypedNumeric =>
                BuildTypedNumericUnvalidated(methodBody),
            LIRStackSchedulerMode.TypedComparisons =>
                BuildTypedComparisonsUnvalidated(methodBody),
            LIRStackSchedulerMode.Disabled => throw new InvalidOperationException(
                "Disabled scheduler mode bypasses schedule construction."),
            _ => throw new NotSupportedException(
                $"LIR stack scheduler mode '{options.Mode}' is not implemented yet.")
        };

        return ValidateOrFallback(methodBody, schedule, options);
    }

    internal static LIRStackSchedule Identity(MethodBodyIR methodBody)
    {
        ArgumentNullException.ThrowIfNull(methodBody);
        return LIRStackScheduleValidator.ValidateAndAnnotate(
            methodBody,
            BuildIdentityUnvalidated(methodBody));
    }

    private static LIRStackSchedule BuildIdentityUnvalidated(
        MethodBodyIR methodBody)
    {
        var operations = BuildIdentityOperations(methodBody);
        var regions = BuildSchedulingRegions(methodBody, operations);
        var tempResidencies = new TempResidency[methodBody.Temps.Count];
        Array.Fill(tempResidencies, TempResidency.MaterializedLocal);

        var ownedTemps = new bool[methodBody.Temps.Count];
        var effectiveLastUses = ComputeIdentityLastUses(methodBody);

        return new LIRStackSchedule(
            LIRStackSchedulerMode.Identity,
            operations,
            regions,
            tempResidencies,
            ownedTemps,
            effectiveLastUses,
            new int[methodBody.Instructions.Count],
            MaxStackDepth: 0,
            new LIRStackScheduleMetrics(
                ScheduledRegionCount: regions.Length,
                StackResidentTempCount: 0,
                EliminatedSpillCount: 0,
                ValidationFallbackCount: 0),
            ValidationFailureReason: null);
    }

    private static LIRStackSchedule BuildTypedNumericUnvalidated(
        MethodBodyIR methodBody)
    {
        var identity = BuildIdentityUnvalidated(methodBody);
        var definitionIndex = new int[methodBody.Temps.Count];
        var definitionCount = new int[methodBody.Temps.Count];
        var useIndex = new int[methodBody.Temps.Count];
        var useCount = new int[methodBody.Temps.Count];
        Array.Fill(definitionIndex, -1);
        Array.Fill(useIndex, -1);

        for (var instructionIndex = 0;
             instructionIndex < methodBody.Instructions.Count;
             instructionIndex++)
        {
            var instruction = methodBody.Instructions[instructionIndex];
            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && (uint)defined.Index < (uint)definitionCount.Length)
            {
                definitionCount[defined.Index]++;
                definitionIndex[defined.Index] = instructionIndex;
            }

            var visitor = new NumericUseVisitor(
                useCount,
                useIndex,
                instructionIndex);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        var regionByLirIndex = new int[methodBody.Instructions.Count];
        Array.Fill(regionByLirIndex, -1);
        for (var regionIndex = 0;
             regionIndex < identity.Regions.Length;
             regionIndex++)
        {
            var region = identity.Regions[regionIndex];
            for (var lirIndex = region.StartLirIndex;
                 lirIndex < region.EndLirIndexExclusive;
                 lirIndex++)
            {
                regionByLirIndex[lirIndex] = regionIndex;
            }
        }

        var residencies = identity.TempResidencies.ToArray();
        var ownedTemps = identity.OwnedTemps.ToArray();
        var acceptedCount = 0;
        for (var tempIndex = 0; tempIndex < methodBody.Temps.Count; tempIndex++)
        {
            if (definitionCount[tempIndex] != 1
                || useCount[tempIndex] != 1
                || definitionIndex[tempIndex] < 0
                || useIndex[tempIndex] <= definitionIndex[tempIndex]
                || tempIndex < methodBody.TempVariableSlots.Count
                    && methodBody.TempVariableSlots[tempIndex] >= 0)
            {
                continue;
            }

            var definition = methodBody.Instructions[definitionIndex[tempIndex]];
            if (!IsTypedNumericBinary(definition)
                || !IsSupportedTypedNumericConsumer(
                    methodBody,
                    useIndex[tempIndex],
                    useCount,
                    useIndex))
            {
                continue;
            }

            var definitionRegion = regionByLirIndex[definitionIndex[tempIndex]];
            if (definitionRegion < 0)
            {
                continue;
            }

            var useRegion = regionByLirIndex[useIndex[tempIndex]];
            var region = identity.Regions[definitionRegion];
            var useIsTerminalBoundary =
                useRegion < 0
                && useIndex[tempIndex] == region.EndLirIndexExclusive
                && IsSupportedTypedNumericTerminal(
                    methodBody,
                    methodBody.Instructions[useIndex[tempIndex]]);
            if (useRegion != definitionRegion && !useIsTerminalBoundary)
            {
                continue;
            }

            if (!HasOnlySupportedInterveningInstructions(
                methodBody,
                definitionIndex[tempIndex],
                useIndex[tempIndex]))
            {
                continue;
            }

            residencies[tempIndex] = TempResidency.StackResident;
            ownedTemps[tempIndex] = true;
            acceptedCount++;
        }

        ClaimSafeBoxingTerminals(
            methodBody,
            definitionCount,
            useCount,
            useIndex,
            regionByLirIndex,
            residencies,
            ownedTemps);

        PruneInvalidTypedNumericResidencies(
            methodBody,
            residencies,
            ownedTemps);
        acceptedCount = ownedTemps.Count(owned => owned);

        return identity with
        {
            Mode = LIRStackSchedulerMode.TypedNumeric,
            TempResidencies = residencies,
            OwnedTemps = ownedTemps,
            Metrics = identity.Metrics with
            {
                StackResidentTempCount = acceptedCount,
                EliminatedSpillCount = acceptedCount
            }
        };
    }

    private static void PruneInvalidTypedNumericResidencies(
        MethodBodyIR methodBody,
        TempResidency[] residencies,
        bool[] ownedTemps)
    {
        var stack = new List<int>();
        var uses = new List<TempVariable>();
        for (var instructionIndex = 0;
             instructionIndex < methodBody.Instructions.Count;
             instructionIndex++)
        {
            var instruction = methodBody.Instructions[instructionIndex];
            uses.Clear();
            var visitor = new NumericCollectUseVisitor(uses);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);

            // Remove candidates whose operand position cannot consume an
            // already-carried stack value before ordinary loaded operands.
            var encounteredNonResident = false;
            for (var index = 0; index < uses.Count; index++)
            {
                var use = uses[index];
                var isResident = residencies[use.Index]
                    == TempResidency.StackResident;
                if (isResident && encounteredNonResident)
                {
                    RemoveResidency(use.Index);
                    stack.Remove(use.Index);
                    continue;
                }

                if (!isResident)
                {
                    encounteredNonResident = true;
                }
            }

            var residentPrefixCount = 0;
            while (residentPrefixCount < uses.Count
                && residencies[uses[residentPrefixCount].Index]
                    == TempResidency.StackResident)
            {
                residentPrefixCount++;
            }

            if (residentPrefixCount > stack.Count)
            {
                for (var index = 0; index < residentPrefixCount; index++)
                {
                    RemoveResidency(uses[index].Index);
                    stack.Remove(uses[index].Index);
                }
                residentPrefixCount = 0;
            }

            for (var index = 0; index < residentPrefixCount; index++)
            {
                var expected = uses[index].Index;
                var actual =
                    stack[stack.Count - residentPrefixCount + index];
                if (expected != actual)
                {
                    RemoveResidency(expected);
                    stack.Remove(expected);
                }
            }

            residentPrefixCount = 0;
            while (residentPrefixCount < uses.Count
                && residencies[uses[residentPrefixCount].Index]
                    == TempResidency.StackResident)
            {
                residentPrefixCount++;
            }
            if (residentPrefixCount > 0)
            {
                stack.RemoveRange(
                    stack.Count - residentPrefixCount,
                    residentPrefixCount);
            }

            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && residencies[defined.Index]
                    == TempResidency.StackResident)
            {
                stack.Add(defined.Index);
            }

            if (LIRInstructionInfo.IsSchedulingBoundary(instruction)
                && stack.Count > 0)
            {
                foreach (var tempIndex in stack)
                {
                    RemoveResidency(tempIndex);
                }
                stack.Clear();
            }
        }

        void RemoveResidency(int tempIndex)
        {
            residencies[tempIndex] = TempResidency.MaterializedLocal;
            ownedTemps[tempIndex] = false;
        }
    }

    private static LIRStackSchedule BuildTypedComparisonsUnvalidated(
        MethodBodyIR methodBody)
    {
        var numeric = BuildTypedNumericUnvalidated(methodBody);
        var definitionIndex = new int[methodBody.Temps.Count];
        var definitionCount = new int[methodBody.Temps.Count];
        var useIndex = new int[methodBody.Temps.Count];
        var useCount = new int[methodBody.Temps.Count];
        Array.Fill(definitionIndex, -1);
        Array.Fill(useIndex, -1);

        for (var instructionIndex = 0;
             instructionIndex < methodBody.Instructions.Count;
             instructionIndex++)
        {
            var instruction = methodBody.Instructions[instructionIndex];
            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && (uint)defined.Index < (uint)definitionCount.Length)
            {
                definitionCount[defined.Index]++;
                definitionIndex[defined.Index] = instructionIndex;
            }

            var visitor = new NumericUseVisitor(
                useCount,
                useIndex,
                instructionIndex);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        var regionByLirIndex = new int[methodBody.Instructions.Count];
        Array.Fill(regionByLirIndex, -1);
        for (var regionIndex = 0;
             regionIndex < numeric.Regions.Length;
             regionIndex++)
        {
            var region = numeric.Regions[regionIndex];
            for (var lirIndex = region.StartLirIndex;
                 lirIndex < region.EndLirIndexExclusive;
                 lirIndex++)
            {
                regionByLirIndex[lirIndex] = regionIndex;
            }
        }

        var residencies = numeric.TempResidencies.ToArray();
        var ownedTemps = numeric.OwnedTemps.ToArray();
        for (var tempIndex = 0; tempIndex < methodBody.Temps.Count; tempIndex++)
        {
            if (definitionCount[tempIndex] != 1
                || useCount[tempIndex] != 1
                || definitionIndex[tempIndex] < 0
                || useIndex[tempIndex] <= definitionIndex[tempIndex]
                || tempIndex < methodBody.TempVariableSlots.Count
                    && methodBody.TempVariableSlots[tempIndex] >= 0)
            {
                continue;
            }

            var definition = methodBody.Instructions[definitionIndex[tempIndex]];
            var consumer = methodBody.Instructions[useIndex[tempIndex]];
            if (!IsTypedUnaryOrComparison(definition)
                && !(IsTypedNumericBinary(definition)
                    && IsTypedUnaryOrComparison(consumer)))
            {
                continue;
            }

            var definitionRegion = regionByLirIndex[definitionIndex[tempIndex]];
            if (definitionRegion < 0)
            {
                continue;
            }

            var useRegion = regionByLirIndex[useIndex[tempIndex]];
            var region = numeric.Regions[definitionRegion];
            var supportedBoundaryConsumer =
                useRegion < 0
                && useIndex[tempIndex] == region.EndLirIndexExclusive
                && consumer is LIRBranchIfFalse
                    or LIRBranchIfTrue
                    || IsSupportedTypedNumericTerminal(
                        methodBody,
                        consumer);
            var supportedSameRegionConsumer =
                useRegion == definitionRegion
                && (IsTypedUnaryOrComparison(consumer)
                    || IsTypedNumericBinary(consumer)
                    || IsSafeBoxingReturnConsumer(
                        methodBody,
                        consumer,
                        useCount,
                        useIndex));
            if (!supportedBoundaryConsumer && !supportedSameRegionConsumer)
            {
                continue;
            }

            if (!HasOnlySupportedInterveningInstructions(
                methodBody,
                definitionIndex[tempIndex],
                useIndex[tempIndex]))
            {
                continue;
            }

            residencies[tempIndex] = TempResidency.StackResident;
            ownedTemps[tempIndex] = true;
        }

        ClaimSafeBoxingTerminals(
            methodBody,
            definitionCount,
            useCount,
            useIndex,
            regionByLirIndex,
            residencies,
            ownedTemps);
        PruneInvalidTypedNumericResidencies(
            methodBody,
            residencies,
            ownedTemps);
        var acceptedCount = ownedTemps.Count(owned => owned);
        return numeric with
        {
            Mode = LIRStackSchedulerMode.TypedComparisons,
            TempResidencies = residencies,
            OwnedTemps = ownedTemps,
            Metrics = numeric.Metrics with
            {
                StackResidentTempCount = acceptedCount,
                EliminatedSpillCount = acceptedCount
            }
        };
    }

    private static bool IsTypedUnaryOrComparison(LIRInstruction instruction)
        => instruction is LIRNegateNumber
            or LIRBitwiseNotNumber
            or LIRCompareNumberLessThan
            or LIRCompareNumberGreaterThan
            or LIRCompareNumberLessThanOrEqual
            or LIRCompareNumberGreaterThanOrEqual
            or LIRCompareNumberEqual
            or LIRCompareNumberNotEqual
            or LIRCompareBooleanEqual
            or LIRCompareBooleanNotEqual;

    private static bool IsSafeBoxingReturnConsumer(
        MethodBodyIR methodBody,
        LIRInstruction instruction,
        int[] useCount,
        int[] useIndex)
    {
        if (instruction is not LIRConvertToObject convert)
        {
            return false;
        }

        var resultIndex = convert.Result.Index;
        return (uint)resultIndex < (uint)useCount.Length
            && useCount[resultIndex] == 1
            && methodBody.Instructions[useIndex[resultIndex]] is LIRReturn
            && !methodBody.IsAsync
            && !methodBody.IsGenerator;
    }

    private static bool IsTypedNumericBinary(LIRInstruction instruction)
        => instruction is LIRAddNumber
            or LIRSubNumber
            or LIRMulNumber
            or LIRDivNumber
            or LIRModNumber
            or LIRExpNumber;

    private static bool IsSupportedTypedNumericConsumer(
        MethodBodyIR methodBody,
        int consumerIndex,
        int[] useCount,
        int[] useIndex)
    {
        var instruction = methodBody.Instructions[consumerIndex];
        if (instruction is LIRConvertToObject convert)
        {
            var resultIndex = convert.Result.Index;
            return (uint)resultIndex < (uint)useCount.Length
                && useCount[resultIndex] == 1
                && methodBody.Instructions[useIndex[resultIndex]] is LIRReturn
                && !methodBody.IsAsync
                && !methodBody.IsGenerator;
        }

        return IsTypedNumericBinary(instruction)
            || IsSupportedTypedNumericTerminal(methodBody, instruction)
            || instruction is LIRStoreParameter;
    }

    private static void ClaimSafeBoxingTerminals(
        MethodBodyIR methodBody,
        int[] definitionCount,
        int[] useCount,
        int[] useIndex,
        int[] regionByLirIndex,
        TempResidency[] residencies,
        bool[] ownedTemps)
    {
        for (var instructionIndex = 0;
             instructionIndex < methodBody.Instructions.Count;
             instructionIndex++)
        {
            if (methodBody.Instructions[instructionIndex]
                    is not LIRConvertToObject convert
                || residencies[convert.Source.Index]
                    != TempResidency.StackResident
                || definitionCount[convert.Result.Index] != 1
                || useCount[convert.Result.Index] != 1
                || methodBody.Instructions[useIndex[convert.Result.Index]]
                    is not LIRReturn
                || methodBody.IsAsync
                || methodBody.IsGenerator
                || regionByLirIndex[instructionIndex] < 0)
            {
                continue;
            }

            residencies[convert.Result.Index] = TempResidency.StackResident;
            ownedTemps[convert.Result.Index] = true;
        }
    }

    private static bool IsSupportedTypedNumericTerminal(
        MethodBodyIR methodBody,
        LIRInstruction instruction)
        => instruction is LIRReturn
            && !methodBody.IsAsync
            && !methodBody.IsGenerator;

    private static bool HasOnlySupportedInterveningInstructions(
        MethodBodyIR methodBody,
        int definitionIndex,
        int useIndex)
    {
        for (var index = definitionIndex + 1; index < useIndex; index++)
        {
            if (methodBody.Instructions[index] is not (
                    LIRConstNumber
                    or LIRLoadParameter
                    or LIRLoadThis
                    or LIRLoadUserClassInstanceField
                    or LIRLoadUserClassStaticField
                    or LIRAddNumber
                    or LIRSubNumber
                    or LIRMulNumber
                    or LIRDivNumber
                    or LIRModNumber
                    or LIRExpNumber))
                    // Typed unary/comparison definitions are also safe
                    // source-order producers in this cumulative mode.
            {
                if (methodBody.Instructions[index] is not (
                        LIRNegateNumber
                        or LIRBitwiseNotNumber
                        or LIRCompareNumberLessThan
                        or LIRCompareNumberGreaterThan
                        or LIRCompareNumberLessThanOrEqual
                        or LIRCompareNumberGreaterThanOrEqual
                        or LIRCompareNumberEqual
                        or LIRCompareNumberNotEqual
                        or LIRCompareBooleanEqual
                        or LIRCompareBooleanNotEqual))
                {
                    return false;
                }
            }
        }

        return true;
    }

    internal static LIRStackSchedule ValidateOrFallback(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule,
        LIRStackSchedulerOptions options)
    {
        try
        {
            return LIRStackScheduleValidator.ValidateAndAnnotate(
                methodBody,
                schedule);
        }
        catch (LIRStackScheduleValidationException exception)
            when (options.ValidationBehavior
                    == LIRStackScheduleValidationBehavior.FallbackToIdentity
                && schedule.Mode != LIRStackSchedulerMode.Identity)
        {
            IRPipelineMetrics.RecordSchedulerValidationFallback(
                exception.Message);
            var identity = LIRStackScheduleValidator.ValidateAndAnnotate(
                methodBody,
                BuildIdentityUnvalidated(methodBody));
            return identity with
            {
                Metrics = identity.Metrics with
                {
                    ValidationFallbackCount =
                        identity.Metrics.ValidationFallbackCount + 1
                },
                ValidationFailureReason = exception.Message
            };
        }
    }

    private static ScheduledOperation[] BuildIdentityOperations(MethodBodyIR methodBody)
    {
        if (methodBody.Instructions.Count == 0)
        {
            return Array.Empty<ScheduledOperation>();
        }

        var operations = new ScheduledOperation[methodBody.Instructions.Count];
        var operationCount = 0;
        for (var index = 0; index < methodBody.Instructions.Count;)
        {
            if (IsConstructorFieldStoreFusionCandidate(methodBody.Instructions, index))
            {
                operations[operationCount++] = new ScheduledOperation(
                    index,
                    InstructionCount: 2,
                    InstructionDisposition.FusedIntoEmissionUnit);
                index += 2;
                continue;
            }

            operations[operationCount++] = new ScheduledOperation(
                index,
                InstructionCount: 1,
                InstructionDisposition.EmitNormally);
            index++;
        }

        if (operationCount != operations.Length)
        {
            Array.Resize(ref operations, operationCount);
        }

        return operations;
    }

    private static bool IsConstructorFieldStoreFusionCandidate(
        IReadOnlyList<LIRInstruction> instructions,
        int index)
    {
        if (index + 1 >= instructions.Count
            || instructions[index + 1] is not LIRStoreUserClassInstanceField storeField)
        {
            return false;
        }

        return instructions[index] switch
        {
            LIRNewUserClass newUserClass =>
                !newUserClass.IsDerivedConstructor
                && storeField.Value.Equals(newUserClass.Result),
            LIRNewIntrinsicObject newIntrinsic =>
                storeField.Value.Equals(newIntrinsic.Result),
            _ => false
        };
    }

    private static int[] ComputeIdentityLastUses(MethodBodyIR methodBody)
    {
        var lastUses = new int[methodBody.Temps.Count];
        Array.Fill(lastUses, -1);

        for (var instructionIndex = 0;
             instructionIndex < methodBody.Instructions.Count;
             instructionIndex++)
        {
            var visitor = new LastUseVisitor(lastUses, instructionIndex);
             LIRInstructionInfo.VisitUsedTemps(
                 methodBody.Instructions[instructionIndex],
                 ref visitor);
        }

        return lastUses;
    }

    private static ScheduledRegion[] BuildSchedulingRegions(
        MethodBodyIR methodBody,
        ScheduledOperation[] operations)
    {
        if (operations.Length == 0)
        {
            return Array.Empty<ScheduledRegion>();
        }

        var regions = new ScheduledRegion[(operations.Length / 2) + 1];
        var regionCount = 0;
        var regionStartOperation = -1;
        var currentSequencePointIndex = -1;
        Jroc.DebugSymbols.SourceSpan? currentSourceSpan = null;

        for (var operationIndex = 0; operationIndex < operations.Length; operationIndex++)
        {
            var operation = operations[operationIndex];
            var isBoundary = false;
            LIRSequencePoint? sequencePoint = null;
            for (var offset = 0; offset < operation.InstructionCount; offset++)
            {
                var instruction = methodBody.Instructions[
                    operation.GetLirInstructionIndex(offset)];
                sequencePoint ??= instruction as LIRSequencePoint;
                if (LIRInstructionInfo.IsSchedulingBoundary(instruction))
                {
                    isBoundary = true;
                    break;
                }
            }

            if (isBoundary)
            {
                AppendRegionBeforeBoundary(operationIndex);
                if (sequencePoint is not null)
                {
                    currentSequencePointIndex++;
                    currentSourceSpan = sequencePoint.Span;
                }
                continue;
            }

            if (regionStartOperation < 0)
            {
                regionStartOperation = operationIndex;
            }
        }

        AppendRegionBeforeBoundary(operations.Length);

        if (regionCount != regions.Length)
        {
            Array.Resize(ref regions, regionCount);
        }

        return regions;

        void AppendRegionBeforeBoundary(int endOperationIndexExclusive)
        {
            if (regionStartOperation < 0)
            {
                return;
            }

            var firstOperation = operations[regionStartOperation];
            var lastOperation = operations[endOperationIndexExclusive - 1];
            regions[regionCount++] = new ScheduledRegion(
                firstOperation.StartLirIndex,
                lastOperation.EndLirIndexExclusive,
                regionStartOperation,
                endOperationIndexExclusive - regionStartOperation,
                currentSequencePointIndex,
                currentSourceSpan,
                MaxStackDepth: 0);
            regionStartOperation = -1;
        }
    }

    private struct LastUseVisitor : ITempUseVisitor
    {
        private readonly int[] _lastUses;
        private readonly int _instructionIndex;

        internal LastUseVisitor(int[] lastUses, int instructionIndex)
        {
            _lastUses = lastUses;
            _instructionIndex = instructionIndex;
        }

        public void Visit(TempVariable temp)
        {
            if ((uint)temp.Index < (uint)_lastUses.Length)
            {
                _lastUses[temp.Index] = _instructionIndex;
            }
        }
    }

    private struct NumericUseVisitor : ITempUseVisitor
    {
        private readonly int[] _useCount;
        private readonly int[] _useIndex;
        private readonly int _instructionIndex;

        internal NumericUseVisitor(
            int[] useCount,
            int[] useIndex,
            int instructionIndex)
        {
            _useCount = useCount;
            _useIndex = useIndex;
            _instructionIndex = instructionIndex;
        }

        public void Visit(TempVariable temp)
        {
            if ((uint)temp.Index >= (uint)_useCount.Length)
            {
                return;
            }

            _useCount[temp.Index]++;
            _useIndex[temp.Index] = _instructionIndex;
        }
    }

    private struct NumericCollectUseVisitor : ITempUseVisitor
    {
        private readonly List<TempVariable> _uses;

        internal NumericCollectUseVisitor(List<TempVariable> uses)
        {
            _uses = uses;
        }

        public void Visit(TempVariable temp)
        {
            _uses.Add(temp);
        }
    }
}
