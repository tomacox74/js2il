using Jroc.IR;

namespace Jroc.IL;

internal sealed class LIRStackScheduleValidationException : InvalidOperationException
{
    internal LIRStackScheduleValidationException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Independently validates scheduler ownership, order, region, and persistent
/// evaluation-stack invariants before the plan can reach IL emission.
/// </summary>
internal static class LIRStackScheduleValidator
{
    private const int CatchExceptionMarker = -1;

    internal static LIRStackSchedule ValidateAndAnnotate(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule)
    {
        ArgumentNullException.ThrowIfNull(methodBody);
        ArgumentNullException.ThrowIfNull(schedule);

        ValidateArrayLengths(methodBody, schedule);

        var scheduledLirIndexes = ExpandAndValidateOperationOwnership(
            methodBody,
            schedule);
        var scheduledPositionByLirIndex = BuildScheduledPositionMap(
            methodBody,
            scheduledLirIndexes);

        ValidateEffectOrder(methodBody, scheduledPositionByLirIndex);
        ValidateRegions(methodBody, schedule);
        ValidateIntraRegionDataOrder(
            methodBody,
            schedule,
            scheduledPositionByLirIndex);
        ValidateSchedulerOwnership(methodBody, schedule);

        var carriedDepthBefore = new int[methodBody.Instructions.Count];
        var regionMaxDepths = new int[schedule.Regions.Length];
        var maxStackDepth = SimulatePersistentStack(
            methodBody,
            schedule,
            scheduledLirIndexes,
            carriedDepthBefore,
            regionMaxDepths);

        if (schedule.MaxStackDepth > 0
            && schedule.MaxStackDepth < maxStackDepth)
        {
            Throw(
                $"Schedule declares max stack {schedule.MaxStackDepth}, but "
                + $"validation requires {maxStackDepth}.");
        }

        var annotatedRegions = new ScheduledRegion[schedule.Regions.Length];
        for (var index = 0; index < schedule.Regions.Length; index++)
        {
            if (schedule.Regions[index].MaxStackDepth > 0
                && schedule.Regions[index].MaxStackDepth
                    < regionMaxDepths[index])
            {
                Throw(
                    $"Scheduled region {index} declares max stack "
                    + $"{schedule.Regions[index].MaxStackDepth}, but validation "
                    + $"requires {regionMaxDepths[index]}.");
            }

            annotatedRegions[index] = schedule.Regions[index] with
            {
                MaxStackDepth = regionMaxDepths[index]
            };
        }

        return schedule with
        {
            Regions = annotatedRegions,
            CarriedStackDepthBeforeInstructions = carriedDepthBefore,
            MaxStackDepth = maxStackDepth
        };
    }

    private static void ValidateArrayLengths(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule)
    {
        var tempCount = methodBody.Temps.Count;
        if (schedule.TempResidencies.Length != tempCount)
        {
            Throw(
                $"Temp residency length {schedule.TempResidencies.Length} does not "
                + $"match method temp count {tempCount}.");
        }

        if (schedule.OwnedTemps.Length != tempCount)
        {
            Throw(
                $"Owned-temp length {schedule.OwnedTemps.Length} does not "
                + $"match method temp count {tempCount}.");
        }

        if (schedule.EffectiveLastUses.Length != tempCount)
        {
            Throw(
                $"Effective-last-use length {schedule.EffectiveLastUses.Length} does "
                + $"not match method temp count {tempCount}.");
        }
    }

    private static int[] ExpandAndValidateOperationOwnership(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule)
    {
        var instructionCount = methodBody.Instructions.Count;
        var scheduledLirIndexes = new int[instructionCount];
        var owned = new bool[instructionCount];
        var writeIndex = 0;

        for (var operationIndex = 0;
             operationIndex < schedule.Operations.Length;
             operationIndex++)
        {
            var operation = schedule.Operations[operationIndex];
            if (operation.InstructionCount <= 0)
            {
                Throw(
                    $"Scheduled operation {operationIndex} has invalid instruction "
                    + $"count {operation.InstructionCount}.");
            }

            if (operation.StartLirIndex < 0
                || operation.EndLirIndexExclusive > instructionCount)
            {
                Throw(
                    $"Scheduled operation {operationIndex} owns LIR range "
                    + $"[{operation.StartLirIndex}, {operation.EndLirIndexExclusive}) "
                    + $"outside method instruction count {instructionCount}.");
            }

            if (operation.InstructionCount > 1
                && operation.Disposition
                    != InstructionDisposition.FusedIntoEmissionUnit)
            {
                Throw(
                    $"Scheduled operation {operationIndex} groups "
                    + $"{operation.InstructionCount} instructions without fused "
                    + "emission ownership.");
            }

            for (var offset = 0; offset < operation.InstructionCount; offset++)
            {
                var lirIndex = operation.GetLirInstructionIndex(offset);
                if (owned[lirIndex])
                {
                    Throw(
                        $"LIR instruction #{lirIndex} is owned by more than one "
                        + "scheduled operation.");
                }

                owned[lirIndex] = true;
                scheduledLirIndexes[writeIndex++] = lirIndex;
            }
        }

        if (writeIndex != instructionCount)
        {
            var missing = Enumerable.Range(0, instructionCount)
                .FirstOrDefault(index => !owned[index]);
            Throw(
                $"Schedule owns {writeIndex} of {instructionCount} LIR instructions; "
                + $"instruction #{missing} is missing.");
        }

        return scheduledLirIndexes;
    }

    private static int[] BuildScheduledPositionMap(
        MethodBodyIR methodBody,
        int[] scheduledLirIndexes)
    {
        var positions = new int[methodBody.Instructions.Count];
        for (var position = 0; position < scheduledLirIndexes.Length; position++)
        {
            positions[scheduledLirIndexes[position]] = position;
        }

        return positions;
    }

    private static void ValidateEffectOrder(
        MethodBodyIR methodBody,
        int[] scheduledPositionByLirIndex)
    {
        var previousScheduledPosition = -1;
        var previousLirIndex = -1;

        for (var lirIndex = 0;
             lirIndex < methodBody.Instructions.Count;
             lirIndex++)
        {
            var instruction = methodBody.Instructions[lirIndex];
            var effects = LIRInstructionInfo.GetEffectsForScheduling(instruction);
            if (effects == LIRInstructionEffects.None
                && !LIRInstructionInfo.IsSchedulingBoundary(instruction))
            {
                continue;
            }

            var scheduledPosition = scheduledPositionByLirIndex[lirIndex];
            if (scheduledPosition < previousScheduledPosition)
            {
                Throw(
                    $"Effect order reversed: LIR instruction #{lirIndex} is scheduled "
                    + $"at position {scheduledPosition} before effectful instruction "
                    + $"#{previousLirIndex} at position {previousScheduledPosition}.");
            }

            previousLirIndex = lirIndex;
            previousScheduledPosition = scheduledPosition;
        }
    }

    private static void ValidateRegions(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule)
    {
        var operationRegionOwners = new int[schedule.Operations.Length];
        Array.Fill(operationRegionOwners, -1);

        for (var regionIndex = 0;
             regionIndex < schedule.Regions.Length;
             regionIndex++)
        {
            var region = schedule.Regions[regionIndex];
            if (region.OperationCount <= 0
                || region.StartOperationIndex < 0
                || region.StartOperationIndex + region.OperationCount
                    > schedule.Operations.Length)
            {
                Throw(
                    $"Scheduled region {regionIndex} has invalid operation window "
                    + $"[{region.StartOperationIndex}, "
                    + $"{region.StartOperationIndex + region.OperationCount}).");
            }

            for (var operationOffset = 0;
                 operationOffset < region.OperationCount;
                 operationOffset++)
            {
                var operationIndex =
                    region.StartOperationIndex + operationOffset;
                if (operationRegionOwners[operationIndex] >= 0)
                {
                    Throw(
                        $"Scheduled operation {operationIndex} belongs to regions "
                        + $"{operationRegionOwners[operationIndex]} and {regionIndex}.");
                }

                var operation = schedule.Operations[operationIndex];
                for (var lirOffset = 0;
                     lirOffset < operation.InstructionCount;
                     lirOffset++)
                {
                    var lirIndex = operation.GetLirInstructionIndex(lirOffset);
                    if (lirIndex < region.StartLirIndex
                        || lirIndex >= region.EndLirIndexExclusive)
                    {
                        Throw(
                            $"Scheduled region {regionIndex} contains LIR instruction "
                            + $"#{lirIndex} outside declared range "
                            + $"[{region.StartLirIndex}, "
                            + $"{region.EndLirIndexExclusive}).");
                    }

                    if (LIRInstructionInfo.IsSchedulingBoundary(
                        methodBody.Instructions[lirIndex]))
                    {
                        Throw(
                            $"Scheduled region {regionIndex} crosses boundary LIR "
                            + $"instruction #{lirIndex} "
                            + $"({methodBody.Instructions[lirIndex].GetType().Name}).");
                    }
                }

                operationRegionOwners[operationIndex] = regionIndex;
            }
        }

        for (var operationIndex = 0;
             operationIndex < schedule.Operations.Length;
             operationIndex++)
        {
            var operation = schedule.Operations[operationIndex];
            var hasBoundary = false;
            for (var offset = 0; offset < operation.InstructionCount; offset++)
            {
                hasBoundary |= LIRInstructionInfo.IsSchedulingBoundary(
                    methodBody.Instructions[
                        operation.GetLirInstructionIndex(offset)]);
            }

            if (hasBoundary == (operationRegionOwners[operationIndex] >= 0))
            {
                Throw(
                    hasBoundary
                        ? $"Boundary operation {operationIndex} is included in a "
                            + "scheduling region."
                        : $"Non-boundary operation {operationIndex} is not included "
                            + "in a scheduling region.");
            }
        }
    }

    private static void ValidateSchedulerOwnership(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule)
    {
        for (var tempIndex = 0;
             tempIndex < methodBody.Temps.Count;
             tempIndex++)
        {
            if (schedule.TempResidencies[tempIndex] == TempResidency.StackResident
                && !schedule.OwnedTemps[tempIndex])
            {
                Throw(
                    $"Temp {tempIndex} is stack-resident but not scheduler-owned.");
            }

            if (schedule.OwnedTemps[tempIndex]
                && schedule.TempResidencies[tempIndex]
                    == TempResidency.MaterializedLocal)
            {
                Throw(
                    $"Temp {tempIndex} is scheduler-owned but still marked as a "
                    + "materialized local.");
            }
        }
    }

    private static void ValidateIntraRegionDataOrder(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule,
        int[] scheduledPositionByLirIndex)
    {
        var instructionRegion = new int[methodBody.Instructions.Count];
        Array.Fill(instructionRegion, -1);
        for (var regionIndex = 0;
             regionIndex < schedule.Regions.Length;
             regionIndex++)
        {
            var region = schedule.Regions[regionIndex];
            for (var operationOffset = 0;
                 operationOffset < region.OperationCount;
                 operationOffset++)
            {
                var operation = schedule.Operations[
                    region.StartOperationIndex + operationOffset];
                for (var lirOffset = 0;
                     lirOffset < operation.InstructionCount;
                     lirOffset++)
                {
                    instructionRegion[
                        operation.GetLirInstructionIndex(lirOffset)] = regionIndex;
                }
            }
        }

        var uniqueDefinitionByTemp = new int[methodBody.Temps.Count];
        var hasMultipleDefinitions = new bool[methodBody.Temps.Count];
        Array.Fill(uniqueDefinitionByTemp, -1);
        for (var lirIndex = 0;
             lirIndex < methodBody.Instructions.Count;
             lirIndex++)
        {
            if (!LIRInstructionInfo.TryGetDefinedTemp(
                    methodBody.Instructions[lirIndex],
                    out var defined)
                || (uint)defined.Index >= (uint)uniqueDefinitionByTemp.Length)
            {
                continue;
            }

            if (uniqueDefinitionByTemp[defined.Index] >= 0)
            {
                hasMultipleDefinitions[defined.Index] = true;
            }
            else
            {
                uniqueDefinitionByTemp[defined.Index] = lirIndex;
            }
        }

        for (var useLirIndex = 0;
             useLirIndex < methodBody.Instructions.Count;
             useLirIndex++)
        {
            var useRegion = instructionRegion[useLirIndex];
            if (useRegion < 0)
            {
                continue;
            }

            var visitor = new DataOrderVisitor(
                useLirIndex,
                useRegion,
                instructionRegion,
                uniqueDefinitionByTemp,
                hasMultipleDefinitions,
                scheduledPositionByLirIndex);
            LIRInstructionInfo.VisitUsedTemps(
                methodBody.Instructions[useLirIndex],
                ref visitor);
        }
    }

    private static int SimulatePersistentStack(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule,
        int[] scheduledLirIndexes,
        int[] carriedDepthBefore,
        int[] regionMaxDepths)
    {
        var stack = new List<int>();
        var maxDepth = 0;
        var catchHandlerLabels = methodBody.ExceptionRegions
            .Where(region => region.Kind == ExceptionRegionKind.Catch)
            .Select(region => region.HandlerStartLabelId)
            .ToHashSet();
        var operationRegionMap = BuildOperationRegionMap(schedule);
        var operationIndexByLirIndex = BuildOperationIndexByLirIndex(
            methodBody,
            schedule);
        var hasStackResidentTemps = Array.Exists(
            schedule.TempResidencies,
            residency => residency == TempResidency.StackResident);
        if (hasStackResidentTemps)
        {
            var definitionCounts = new int[methodBody.Temps.Count];
            var useCounts = new int[methodBody.Temps.Count];
            CountDefinitionsAndUses(
                methodBody,
                definitionCounts,
                useCounts);
            ValidateStackResidentDefUse(
                schedule,
                definitionCounts,
                useCounts);
        }

        foreach (var lirIndex in scheduledLirIndexes)
        {
            var instruction = methodBody.Instructions[lirIndex];
            carriedDepthBefore[lirIndex] = stack.Count;
            var operationIndex = operationIndexByLirIndex[lirIndex];
            var regionIndex = operationRegionMap[operationIndex];

            if (instruction is LIRLabel label)
            {
                RequireEmptyStack(stack, lirIndex, nameof(LIRLabel));
                if (catchHandlerLabels.Contains(label.LabelId))
                {
                    stack.Add(CatchExceptionMarker);
                    UpdateMax(stack.Count);
                }
                continue;
            }

            if (instruction is LIRStoreException storeException)
            {
                if (stack.Count != 1 || stack[0] != CatchExceptionMarker)
                {
                    Throw(
                        $"Catch store at LIR instruction #{lirIndex} expected one "
                        + "implicit exception value on the stack.");
                }

                if (schedule.TempResidencies[storeException.Result.Index]
                    == TempResidency.StackResident)
                {
                    Throw(
                        $"Catch result temp {storeException.Result.Index} must be "
                        + "materialized.");
                }

                stack.Clear();
                continue;
            }

            var uses = hasStackResidentTemps
                ? CollectUsedTemps(instruction)
                : null;
            var residentPrefixCount = 0;
            var encounteredNonResident = false;
            if (uses is not null)
            {
                foreach (var use in uses)
                {
                    var isResident = schedule.TempResidencies[use.Index]
                        == TempResidency.StackResident;
                    if (isResident && encounteredNonResident)
                    {
                        Throw(
                            $"Stack-resident temp {use.Index} appears after a "
                            + $"non-resident operand at LIR instruction #{lirIndex}; "
                            + "the operand order cannot be emitted in LIFO order.");
                    }

                    if (isResident)
                    {
                        residentPrefixCount++;
                    }
                    else
                    {
                        encounteredNonResident = true;
                    }
                }
            }

            if (residentPrefixCount > stack.Count)
            {
                Throw(
                    $"LIR instruction #{lirIndex} consumes "
                    + $"{residentPrefixCount} stack-resident operands but only "
                    + $"{stack.Count} values are carried.");
            }

            for (var index = 0; index < residentPrefixCount; index++)
            {
                var expectedTemp = uses![index].Index;
                var actualTemp =
                    stack[stack.Count - residentPrefixCount + index];
                if (actualTemp != expectedTemp)
                {
                    Throw(
                        $"LIFO operand mismatch at LIR instruction #{lirIndex}: "
                        + $"expected temp {expectedTemp}, found temp {actualTemp}.");
                }
            }

            // Non-resident operand loads and instruction-internal temporaries
            // remain covered by the existing emitter peak estimator. The
            // schedule contributes only persistent carried depth.
            UpdateMax(stack.Count);
            if (residentPrefixCount > 0)
            {
                stack.RemoveRange(
                    stack.Count - residentPrefixCount,
                    residentPrefixCount);
            }

            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && schedule.TempResidencies[defined.Index]
                    == TempResidency.StackResident)
            {
                if (instruction is LIRAwait or LIRYield)
                {
                    Throw(
                        $"Resume-result temp {defined.Index} from "
                        + $"{instruction.GetType().Name} must be materialized.");
                }

                stack.Add(defined.Index);
                UpdateMax(stack.Count);
            }

            if (RequiresEmptyStackAfter(instruction))
            {
                RequireEmptyStack(
                    stack,
                    lirIndex,
                    instruction.GetType().Name);
            }

            void UpdateMax(int depth)
            {
                maxDepth = Math.Max(maxDepth, depth);
                if (regionIndex >= 0)
                {
                    regionMaxDepths[regionIndex] =
                        Math.Max(regionMaxDepths[regionIndex], depth);
                }
            }
        }

        RequireEmptyStack(stack, methodBody.Instructions.Count, "method end");
        return maxDepth;
    }

    private static int[] BuildOperationRegionMap(LIRStackSchedule schedule)
    {
        var map = new int[schedule.Operations.Length];
        Array.Fill(map, -1);
        for (var regionIndex = 0;
             regionIndex < schedule.Regions.Length;
             regionIndex++)
        {
            var region = schedule.Regions[regionIndex];
            for (var offset = 0; offset < region.OperationCount; offset++)
            {
                map[region.StartOperationIndex + offset] = regionIndex;
            }
        }

        return map;
    }

    private static int[] BuildOperationIndexByLirIndex(
        MethodBodyIR methodBody,
        LIRStackSchedule schedule)
    {
        var map = new int[methodBody.Instructions.Count];
        for (var operationIndex = 0;
             operationIndex < schedule.Operations.Length;
             operationIndex++)
        {
            var operation = schedule.Operations[operationIndex];
            for (var offset = 0; offset < operation.InstructionCount; offset++)
            {
                map[operation.GetLirInstructionIndex(offset)] = operationIndex;
            }
        }

        return map;
    }

    private static void CountDefinitionsAndUses(
        MethodBodyIR methodBody,
        int[] definitionCounts,
        int[] useCounts)
    {
        foreach (var instruction in methodBody.Instructions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && (uint)defined.Index < (uint)definitionCounts.Length)
            {
                definitionCounts[defined.Index]++;
            }

            var visitor = new CountUseVisitor(useCounts);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }
    }

    private static void ValidateStackResidentDefUse(
        LIRStackSchedule schedule,
        int[] definitionCounts,
        int[] useCounts)
    {
        for (var tempIndex = 0;
             tempIndex < schedule.TempResidencies.Length;
             tempIndex++)
        {
            if (schedule.TempResidencies[tempIndex]
                != TempResidency.StackResident)
            {
                continue;
            }

            if (definitionCounts[tempIndex] != 1 || useCounts[tempIndex] != 1)
            {
                Throw(
                    $"Stack-resident temp {tempIndex} must have exactly one "
                    + $"definition and one use; found "
                    + $"{definitionCounts[tempIndex]} definition(s) and "
                    + $"{useCounts[tempIndex]} use(s).");
            }
        }
    }

    private static List<TempVariable> CollectUsedTemps(
        LIRInstruction instruction)
    {
        var uses = new List<TempVariable>();
        var visitor = new CollectUseVisitor(uses);
        LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        return uses;
    }

    private static bool RequiresEmptyStackAfter(LIRInstruction instruction)
        => instruction is LIRSequencePoint
            or LIRBranch
            or LIRBranchIfFalse
            or LIRBranchIfTrue
            or LIRLeave
            or LIREndFinally
            or LIRReturn
            or LIRReturnUndefinedImmediate
            or LIRTailCallFunctionReturn
            or LIRThrow
            or LIRThrowNewTypeError
            or LIRAwait
            or LIRYield
            or LIRGeneratorStateSwitch
            or LIRAsyncInitialize
            or LIRAsyncCallMoveNext
            or LIRAsyncReturnPromise
            or LIRAsyncLoadState
            or LIRAsyncStoreState
            or LIRAsyncResolve
            or LIRAsyncReject
            or LIRAsyncStateSwitch
            or LIRAsyncStoreAwaitedResult
            or LIRAsyncLoadAwaitedResult
            or LIRCreateLeafScopeInstance
            or LIRCreateScopeInstance
            or LIRUnwrapCatchException;

    private static void RequireEmptyStack(
        List<int> stack,
        int lirIndex,
        string boundary)
    {
        if (stack.Count != 0)
        {
            Throw(
                $"Evaluation stack depth is {stack.Count} at {boundary} "
                + $"boundary near LIR instruction #{lirIndex}; expected empty.");
        }
    }

    private static void Throw(string message)
        => throw new LIRStackScheduleValidationException(message);

    private struct CountUseVisitor : ITempUseVisitor
    {
        private readonly int[] _counts;

        internal CountUseVisitor(int[] counts)
        {
            _counts = counts;
        }

        public void Visit(TempVariable temp)
        {
            if ((uint)temp.Index < (uint)_counts.Length)
            {
                _counts[temp.Index]++;
            }
        }
    }

    private struct CollectUseVisitor : ITempUseVisitor
    {
        private readonly List<TempVariable> _uses;

        internal CollectUseVisitor(List<TempVariable> uses)
        {
            _uses = uses;
        }

        public void Visit(TempVariable temp)
        {
            _uses.Add(temp);
        }
    }

    private readonly struct DataOrderVisitor : ITempUseVisitor
    {
        private readonly int _useLirIndex;
        private readonly int _useRegion;
        private readonly int[] _instructionRegion;
        private readonly int[] _uniqueDefinitionByTemp;
        private readonly bool[] _hasMultipleDefinitions;
        private readonly int[] _scheduledPositionByLirIndex;

        internal DataOrderVisitor(
            int useLirIndex,
            int useRegion,
            int[] instructionRegion,
            int[] uniqueDefinitionByTemp,
            bool[] hasMultipleDefinitions,
            int[] scheduledPositionByLirIndex)
        {
            _useLirIndex = useLirIndex;
            _useRegion = useRegion;
            _instructionRegion = instructionRegion;
            _uniqueDefinitionByTemp = uniqueDefinitionByTemp;
            _hasMultipleDefinitions = hasMultipleDefinitions;
            _scheduledPositionByLirIndex = scheduledPositionByLirIndex;
        }

        public void Visit(TempVariable temp)
        {
            if ((uint)temp.Index >= (uint)_uniqueDefinitionByTemp.Length
                || _hasMultipleDefinitions[temp.Index])
            {
                return;
            }

            var definitionLirIndex = _uniqueDefinitionByTemp[temp.Index];
            if (definitionLirIndex < 0
                || _instructionRegion[definitionLirIndex] != _useRegion)
            {
                return;
            }

            if (_scheduledPositionByLirIndex[definitionLirIndex]
                >= _scheduledPositionByLirIndex[_useLirIndex])
            {
                Throw(
                    $"Data dependency reversed in region {_useRegion}: temp "
                    + $"{temp.Index} is used by LIR instruction #{_useLirIndex} "
                    + $"before its definition at instruction "
                    + $"#{definitionLirIndex}.");
            }
        }
    }
}
