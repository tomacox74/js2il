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
            LIRStackSchedulerMode.ConversionsAndStableLoads =>
                BuildConversionsAndStableLoadsUnvalidated(methodBody),
            LIRStackSchedulerMode.LiteralAndArguments =>
                BuildLiteralAndArgumentsUnvalidated(methodBody),
            LIRStackSchedulerMode.CallResults =>
                BuildCallResultsUnvalidated(methodBody),
            LIRStackSchedulerMode.GeneralRegions =>
                BuildGeneralRegionsUnvalidated(methodBody),
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
                && ((IsTypedUnaryOrComparison(consumer)
                        || IsTypedNumericBinary(consumer))
                    && IsDefinedValueRequired(
                        methodBody,
                        consumer,
                        useCount)
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

    private static LIRStackSchedule BuildConversionsAndStableLoadsUnvalidated(
        MethodBodyIR methodBody)
    {
        var comparisons = BuildTypedComparisonsUnvalidated(methodBody);
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
             regionIndex < comparisons.Regions.Length;
             regionIndex++)
        {
            var region = comparisons.Regions[regionIndex];
            for (var lirIndex = region.StartLirIndex;
                 lirIndex < region.EndLirIndexExclusive;
                 lirIndex++)
            {
                regionByLirIndex[lirIndex] = regionIndex;
            }
        }

        var residencies = comparisons.TempResidencies.ToArray();
        var ownedTemps = comparisons.OwnedTemps.ToArray();
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
            if (!IsConversionConcatOrStableLoad(definition))
            {
                continue;
            }

            var definitionRegion = regionByLirIndex[definitionIndex[tempIndex]];
            if (definitionRegion < 0)
            {
                continue;
            }

            var consumer = methodBody.Instructions[useIndex[tempIndex]];
            var useRegion = regionByLirIndex[useIndex[tempIndex]];
            var region = comparisons.Regions[definitionRegion];
            var boundaryReturn =
                useRegion < 0
                && useIndex[tempIndex] == region.EndLirIndexExclusive
                && IsSupportedTypedNumericTerminal(methodBody, consumer);
            var sameRegionConsumer =
                useRegion == definitionRegion
                && ((IsTypedNumericBinary(consumer)
                        || IsTypedUnaryOrComparison(consumer)
                        || IsConversionConcatOrStableLoad(consumer)
                        || IsSupportedDirectReceiverConsumer(
                            consumer,
                            new TempVariable(tempIndex)))
                    && IsDefinedValueRequired(
                        methodBody,
                        consumer,
                        useCount)
                    || IsSafeBoxingReturnConsumer(
                        methodBody,
                        consumer,
                        useCount,
                        useIndex));
            if (!boundaryReturn && !sameRegionConsumer)
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
        return comparisons with
        {
            Mode = LIRStackSchedulerMode.ConversionsAndStableLoads,
            TempResidencies = residencies,
            OwnedTemps = ownedTemps,
            Metrics = comparisons.Metrics with
            {
                StackResidentTempCount = acceptedCount,
                EliminatedSpillCount = acceptedCount
            }
        };
    }

    private static bool IsConversionConcatOrStableLoad(
        LIRInstruction instruction)
        => instruction is LIRConvertToNumber
            or LIRConvertToObject
            or LIRConcatStrings
            or LIRGetStringLength
            or LIRGetJsArrayLength
            or LIRGetInt32ArrayLength
            or LIRGetJsArrayElement
            or LIRGetInt32ArrayElement
            or LIRLoadLeafScopeField
            or LIRLoadParentScopeField
            or LIRLoadScopeField
            or LIRLoadScopeFieldByName;

    private static LIRStackSchedule BuildLiteralAndArgumentsUnvalidated(
        MethodBodyIR methodBody)
    {
        var conversions = BuildConversionsAndStableLoadsUnvalidated(methodBody);
        var tempCount = methodBody.Temps.Count;
        var definitionIndex = new int[tempCount];
        var definitionCount = new int[tempCount];
        var useIndex = new int[tempCount];
        var useCount = new int[tempCount];
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
                && (uint)defined.Index < (uint)tempCount)
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

        var regionByLirIndex = BuildRegionIndexMap(
            methodBody.Instructions.Count,
            conversions.Regions,
            conversions.Operations);
        var residencies = conversions.TempResidencies.ToArray();
        var ownedTemps = conversions.OwnedTemps.ToArray();
        var movesByInsertionLirIndex = new Dictionary<int, List<int>>();
        var selectedDefinitions = new HashSet<int>();
        var analysisBudget = Math.Max(
            methodBody.Instructions.Count * 8,
            32);

        for (var rootIndex = 0;
             rootIndex < methodBody.Instructions.Count;
             rootIndex++)
        {
            var root = methodBody.Instructions[rootIndex];
            var isConstruction = IsSupportedConstruction(root);
            var isArgumentBundle = IsSupportedArgumentBundle(root);
            if (!isConstruction && !isArgumentBundle)
            {
                continue;
            }

            if (regionByLirIndex[rootIndex] < 0)
            {
                continue;
            }

            selectedDefinitions.Clear();
            var rootRegion = regionByLirIndex[rootIndex];
            var operands = CollectUsedTemps(root);
            TempVariable constructionResult = default;
            if (isConstruction
                && (!LIRInstructionInfo.TryGetDefinedTemp(
                        root,
                        out constructionResult)
                    || definitionCount[constructionResult.Index] != 1
                    || useCount[constructionResult.Index] != 1
                    || constructionResult.Index
                            < methodBody.TempVariableSlots.Count
                        && methodBody.TempVariableSlots[
                            constructionResult.Index] >= 0
                    || !IsSupportedConstructionConsumer(
                        methodBody,
                        constructionResult,
                        useIndex[constructionResult.Index],
                        rootRegion,
                        regionByLirIndex)))
            {
                continue;
            }

            var isSafe = true;
            foreach (var operand in operands)
            {
                if (!TrySelectConstructionProducerTree(
                        methodBody,
                        operand,
                        rootIndex,
                        rootRegion,
                        definitionIndex,
                        definitionCount,
                        useCount,
                        regionByLirIndex,
                        selectedDefinitions,
                        ref analysisBudget))
                {
                    isSafe = false;
                    break;
                }
            }

            if (!isSafe)
            {
                continue;
            }

            if (!TryGetConstructionInsertionIndex(
                    rootIndex,
                    selectedDefinitions,
                    out var insertionLirIndex))
            {
                continue;
            }

            if (!PreservesSelectedEffectOrder(
                    methodBody,
                    BuildSelectedEmissionOrder(
                        methodBody,
                        root,
                        selectedDefinitions)))
            {
                continue;
            }

            foreach (var definitionLirIndex in selectedDefinitions)
            {
                if (!LIRInstructionInfo.TryGetDefinedTemp(
                        methodBody.Instructions[definitionLirIndex],
                        out var producerResult))
                {
                    continue;
                }

                residencies[producerResult.Index] = TempResidency.ScheduledInline;
                ownedTemps[producerResult.Index] = true;
            }

            if (isConstruction)
            {
                residencies[constructionResult.Index] =
                    TempResidency.StackResident;
                ownedTemps[constructionResult.Index] = true;
            }

            if (!movesByInsertionLirIndex.TryGetValue(
                    insertionLirIndex,
                    out var movedRoots))
            {
                movedRoots = new List<int>();
                movesByInsertionLirIndex[insertionLirIndex] = movedRoots;
            }
            movedRoots.Add(rootIndex);
        }

        var operationArray = ReorderConstructionOperations(
            conversions.Operations,
            movesByInsertionLirIndex);
        var acceptedCount = ownedTemps.Count(owned => owned);
        return conversions with
        {
            Mode = LIRStackSchedulerMode.LiteralAndArguments,
            Operations = operationArray,
            Regions = BuildSchedulingRegions(methodBody, operationArray),
            TempResidencies = residencies,
            OwnedTemps = ownedTemps,
            EffectiveLastUses = ComputeScheduledLastUses(
                methodBody,
                operationArray),
            Metrics = conversions.Metrics with
            {
                StackResidentTempCount = residencies.Count(
                    residency => residency == TempResidency.StackResident),
                EliminatedSpillCount = acceptedCount
            }
        };
    }

    private static bool IsSupportedConstruction(LIRInstruction instruction)
        => instruction is LIRBuildArray
            or LIRNewJsArray
            or LIRNewJsObject;

    private static LIRStackSchedule BuildCallResultsUnvalidated(
        MethodBodyIR methodBody)
    {
        var literals = BuildLiteralAndArgumentsUnvalidated(methodBody);
        if (methodBody.IsAsync || methodBody.IsGenerator)
        {
            return literals with { Mode = LIRStackSchedulerMode.CallResults };
        }

        var tempCount = methodBody.Temps.Count;
        var definitionIndex = new int[tempCount];
        var definitionCount = new int[tempCount];
        var useIndex = new int[tempCount];
        var useCount = new int[tempCount];
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
                && (uint)defined.Index < (uint)tempCount)
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

        var regionByLirIndex = BuildRegionIndexMap(
            methodBody.Instructions.Count,
            literals.Regions,
            literals.Operations);
        var residencies = literals.TempResidencies.ToArray();
        var ownedTemps = literals.OwnedTemps.ToArray();
        var analysisBudget = Math.Max(
            methodBody.Instructions.Count * 8,
            32);
        for (var tempIndex = 0; tempIndex < tempCount; tempIndex++)
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

            var definition =
                methodBody.Instructions[definitionIndex[tempIndex]];
            if (!IsSupportedCallResultDefinition(definition))
            {
                continue;
            }

            var consumer = methodBody.Instructions[useIndex[tempIndex]];
            if (IsKnownParamsArrayIntrinsicCall(consumer)
                || !IsSupportedCallResultConsumer(
                    methodBody,
                    new TempVariable(tempIndex),
                    consumer,
                    useCount,
                    useIndex))
            {
                continue;
            }

            var definitionRegion =
                regionByLirIndex[definitionIndex[tempIndex]];
            var useRegion = regionByLirIndex[useIndex[tempIndex]];
            if (definitionRegion < 0
                || useRegion != definitionRegion
                    && !(useRegion < 0
                        && useIndex[tempIndex]
                            == literals.Regions[definitionRegion]
                                .EndLirIndexExclusive
                        && consumer is LIRReturn))
            {
                continue;
            }

            if (definition is LIRCallTypedMember
                && useIndex[tempIndex] != definitionIndex[tempIndex] + 1)
            {
                continue;
            }

            if (!HasOnlySupportedCallInterveningInstructions(
                    methodBody,
                    definitionIndex[tempIndex],
                    useIndex[tempIndex]))
            {
                continue;
            }

            residencies[tempIndex] = TempResidency.StackResident;
            ownedTemps[tempIndex] = true;
        }

        var movesByInsertionLirIndex = new Dictionary<int, List<int>>();
        for (var rootIndex = 0;
             rootIndex < methodBody.Instructions.Count;
             rootIndex++)
        {
            if (methodBody.Instructions[rootIndex]
                    is not LIRCallIntrinsicStatic root
                || !IsKnownParamsArrayIntrinsicCall(root)
                || regionByLirIndex[rootIndex] < 0)
            {
                continue;
            }

            var selectedCallDefinitions = new HashSet<int>();
            var safe = true;
            foreach (var argument in root.Arguments)
            {
                if (!TrySelectCallArgumentProducerTree(
                        methodBody,
                        argument,
                        rootIndex,
                        regionByLirIndex[rootIndex],
                        definitionIndex,
                        definitionCount,
                        useCount,
                        regionByLirIndex,
                        selectedCallDefinitions,
                        ref analysisBudget))
                {
                    safe = false;
                    break;
                }
            }

            if (!safe
                || selectedCallDefinitions.Count == 0
                || !TryGetConstructionInsertionIndex(
                    rootIndex,
                    selectedCallDefinitions,
                    out var insertionLirIndex))
            {
                continue;
            }

            if (!PreservesSelectedEffectOrder(
                    methodBody,
                    BuildSelectedEmissionOrder(
                        methodBody,
                        root,
                        selectedCallDefinitions)))
            {
                continue;
            }

            foreach (var definitionLirIndex in selectedCallDefinitions)
            {
                LIRInstructionInfo.TryGetDefinedTemp(
                    methodBody.Instructions[definitionLirIndex],
                    out var producerResult);
                residencies[producerResult.Index] =
                    TempResidency.ScheduledInline;
                ownedTemps[producerResult.Index] = true;
            }

            if (!movesByInsertionLirIndex.TryGetValue(
                    insertionLirIndex,
                    out var movedRoots))
            {
                movedRoots = new List<int>();
                movesByInsertionLirIndex[insertionLirIndex] = movedRoots;
            }
            movedRoots.Add(rootIndex);
        }

        var operations = ReorderConstructionOperations(
            literals.Operations,
            movesByInsertionLirIndex);
        PruneInvalidTypedNumericResidencies(
            methodBody,
            residencies,
            ownedTemps);
        var acceptedCount = ownedTemps.Count(owned => owned);
        return literals with
        {
            Mode = LIRStackSchedulerMode.CallResults,
            Operations = operations,
            Regions = BuildSchedulingRegions(methodBody, operations),
            TempResidencies = residencies,
            OwnedTemps = ownedTemps,
            EffectiveLastUses = ComputeScheduledLastUses(
                methodBody,
                operations),
            Metrics = literals.Metrics with
            {
                StackResidentTempCount = residencies.Count(
                    residency => residency == TempResidency.StackResident),
                EliminatedSpillCount = acceptedCount
            }
        };
    }

    private static LIRStackSchedule BuildGeneralRegionsUnvalidated(
        MethodBodyIR methodBody)
    {
        var calls = BuildCallResultsUnvalidated(methodBody);
        if (methodBody.IsAsync || methodBody.IsGenerator)
        {
            return calls with
            {
                Mode = LIRStackSchedulerMode.GeneralRegions,
                Metrics = calls.Metrics with
                {
                    ResidualLocalCandidateCount =
                        CountResidualLocalCandidates(calls)
                }
            };
        }

        var tempCount = methodBody.Temps.Count;
        var definitionIndex = new int[tempCount];
        var definitionCount = new int[tempCount];
        var useIndex = new int[tempCount];
        var useCount = new int[tempCount];
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
                && (uint)defined.Index < (uint)tempCount)
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

        var regionByLirIndex = BuildRegionIndexMap(
            methodBody.Instructions.Count,
            calls.Regions,
            calls.Operations);
        var residencies = calls.TempResidencies.ToArray();
        var ownedTemps = calls.OwnedTemps.ToArray();
        var movesByInsertionLirIndex = new Dictionary<int, List<int>>();
        var candidateRegions = new HashSet<int>();
        var acceptedRegions = new HashSet<int>();
        var rejectedDependencyCount = 0;
        var rejectedEffectOrderCount = 0;
        var analysisBudget = Math.Max(
            methodBody.Instructions.Count * 8,
            32);

        for (var rootIndex = methodBody.Instructions.Count - 1;
             rootIndex >= 0;
             rootIndex--)
        {
            var root = methodBody.Instructions[rootIndex];
            var rootRegion = regionByLirIndex[rootIndex];
            if (!IsGeneralSchedulingRoot(root) || rootRegion < 0)
            {
                continue;
            }

            if (LIRInstructionInfo.TryGetDefinedTemp(root, out var existingRoot)
                && residencies[existingRoot.Index]
                    == TempResidency.ScheduledInline)
            {
                continue;
            }

            candidateRegions.Add(rootRegion);
            var selectedDefinitions = new HashSet<int>();
            var emittedDefinitionOrder = new List<int>();
            var safe = true;
            foreach (var operand in CollectUsedTemps(root))
            {
                if (!TrySelectGeneralProducerTree(
                        methodBody,
                        operand,
                        rootIndex,
                        rootRegion,
                        definitionIndex,
                        definitionCount,
                        useCount,
                        regionByLirIndex,
                        selectedDefinitions,
                        emittedDefinitionOrder,
                        ref analysisBudget))
                {
                    safe = false;
                    break;
                }
            }

            if (!safe
                || selectedDefinitions.Count == 0
                || !TryGetConstructionInsertionIndex(
                    rootIndex,
                    selectedDefinitions,
                    out var insertionLirIndex))
            {
                rejectedDependencyCount++;
                continue;
            }

            if (!PreservesSelectedEffectOrder(
                    methodBody,
                    emittedDefinitionOrder))
            {
                rejectedEffectOrderCount++;
                continue;
            }

            foreach (var definitionLirIndex in selectedDefinitions)
            {
                if (!LIRInstructionInfo.TryGetDefinedTemp(
                        methodBody.Instructions[definitionLirIndex],
                        out var producerResult))
                {
                    continue;
                }

                residencies[producerResult.Index] =
                    TempResidency.ScheduledInline;
                ownedTemps[producerResult.Index] = true;
            }

            if (LIRInstructionInfo.TryGetDefinedTemp(
                    root,
                    out var rootResult)
                && IsSupportedConstruction(root)
                && definitionCount[rootResult.Index] == 1
                && useCount[rootResult.Index] == 1
                && (rootResult.Index >= methodBody.TempVariableSlots.Count
                    || methodBody.TempVariableSlots[rootResult.Index] < 0)
                && IsSupportedConstructionConsumer(
                    methodBody,
                    rootResult,
                    useIndex[rootResult.Index],
                    rootRegion,
                    regionByLirIndex))
            {
                residencies[rootResult.Index] = TempResidency.StackResident;
                ownedTemps[rootResult.Index] = true;
            }

            if (!movesByInsertionLirIndex.TryGetValue(
                    insertionLirIndex,
                    out var movedRoots))
            {
                movedRoots = new List<int>();
                movesByInsertionLirIndex[insertionLirIndex] = movedRoots;
            }
            movedRoots.Add(rootIndex);
            acceptedRegions.Add(rootRegion);
        }

        var operations = ReorderConstructionOperations(
            calls.Operations,
            movesByInsertionLirIndex);
        ClaimInlineCallArrays(
            methodBody,
            definitionIndex,
            definitionCount,
            useIndex,
            useCount,
            regionByLirIndex,
            residencies,
            ownedTemps);
        PruneInvalidTypedNumericResidencies(
            methodBody,
            residencies,
            ownedTemps);
        var acceptedCount = ownedTemps.Count(owned => owned);
        var result = calls with
        {
            Mode = LIRStackSchedulerMode.GeneralRegions,
            Operations = operations,
            Regions = BuildSchedulingRegions(methodBody, operations),
            TempResidencies = residencies,
            OwnedTemps = ownedTemps,
            EffectiveLastUses = ComputeScheduledLastUses(
                methodBody,
                operations),
            Metrics = calls.Metrics with
            {
                StackResidentTempCount = residencies.Count(
                    residency => residency == TempResidency.StackResident),
                EliminatedSpillCount = acceptedCount,
                CandidateRegionCount = candidateRegions.Count,
                AcceptedRegionCount = acceptedRegions.Count,
                RejectedRegionCount =
                    candidateRegions.Count - acceptedRegions.Count,
                RejectedDependencyCount = rejectedDependencyCount,
                RejectedEffectOrderCount = rejectedEffectOrderCount
            }
        };
        return result with
        {
            Metrics = result.Metrics with
            {
                ResidualLocalCandidateCount =
                    CountResidualLocalCandidates(result)
            }
        };
    }

    private static bool IsGeneralSchedulingRoot(LIRInstruction instruction)
        => IsSupportedConstruction(instruction)
            || IsSupportedArgumentBundle(instruction);

    private static void ClaimInlineCallArrays(
        MethodBodyIR methodBody,
        int[] definitionIndex,
        int[] definitionCount,
        int[] useIndex,
        int[] useCount,
        int[] regionByLirIndex,
        TempResidency[] residencies,
        bool[] ownedTemps)
    {
        for (var tempIndex = 0; tempIndex < methodBody.Temps.Count; tempIndex++)
        {
            if (ownedTemps[tempIndex]
                || definitionCount[tempIndex] != 1
                || useCount[tempIndex] != 1
                || definitionIndex[tempIndex] < 0
                || useIndex[tempIndex] <= definitionIndex[tempIndex]
                || tempIndex < methodBody.TempVariableSlots.Count
                    && methodBody.TempVariableSlots[tempIndex] >= 0)
            {
                continue;
            }

            var definitionLirIndex = definitionIndex[tempIndex];
            var consumerLirIndex = useIndex[tempIndex];
            var definitionRegion = regionByLirIndex[definitionLirIndex];
            if (definitionRegion < 0
                || regionByLirIndex[consumerLirIndex] != definitionRegion)
            {
                continue;
            }

            var result = new TempVariable(tempIndex);
            var definition = methodBody.Instructions[definitionLirIndex];
            var consumer = methodBody.Instructions[consumerLirIndex];
            var isInlineArgumentArray =
                definition is LIRBuildArray
                && consumerLirIndex == definitionLirIndex + 1
                && IsSupportedInlineArgumentArrayConsumer(
                    consumer,
                    result);
            var isInlineScopesArray =
                definition is LIRBuildScopesArray
                && consumerLirIndex == definitionLirIndex + 1
                && IsSupportedInlineScopesArrayConsumer(
                    consumer,
                    result);
            if (!isInlineArgumentArray && !isInlineScopesArray)
            {
                continue;
            }

            residencies[tempIndex] = TempResidency.ScheduledInline;
            ownedTemps[tempIndex] = true;
        }
    }

    private static bool IsSupportedInlineArgumentArrayConsumer(
        LIRInstruction instruction,
        TempVariable result)
        => instruction switch
        {
            LIRCallIntrinsic call =>
                call.ArgumentsArray.Equals(result),
            LIRCallFunctionWithArgsArray call =>
                call.ArgumentsArray.Equals(result),
            LIRCallFunctionValue call =>
                call.ArgumentsArray.Equals(result),
            LIRCallMember call =>
                call.ArgumentsArray.Equals(result),
            LIRConstructValue construct =>
                construct.ArgumentsArray.Equals(result),
            LIRCallFunctionBaseConstructor call =>
                call.ArgumentsArray.Equals(result),
            _ => false
        };

    private static bool IsSupportedInlineScopesArrayConsumer(
        LIRInstruction instruction,
        TempVariable result)
        => instruction switch
        {
            LIRCallFunction call => call.ScopesArray.Equals(result),
            LIRTailCallFunctionReturn call =>
                call.ScopesArray.Equals(result),
            LIRCallFunctionWithArgsArray call =>
                call.ScopesArray.Equals(result),
            LIRCallFunctionValue call =>
                call.ScopesArray.Equals(result),
            LIRCallFunctionValue0 call =>
                call.ScopesArray.Equals(result),
            LIRCallFunctionValue1 call =>
                call.ScopesArray.Equals(result),
            LIRCallFunctionValue2 call =>
                call.ScopesArray.Equals(result),
            LIRCallFunctionValue3 call =>
                call.ScopesArray.Equals(result),
            LIRCreateBoundArrowFunction create =>
                create.ScopesArray.Equals(result),
            LIRCreateBoundFunctionExpression create =>
                create.ScopesArray.Equals(result),
            _ => false
        };

    private static bool IsSupportedInlineCallArrayRoot(
        LIRInstruction instruction)
        => instruction is LIRCallIntrinsic
            or LIRCallFunction
            or LIRTailCallFunctionReturn
            or LIRCallFunctionWithArgsArray
            or LIRCallFunctionValue
            or LIRCallFunctionValue0
            or LIRCallFunctionValue1
            or LIRCallFunctionValue2
            or LIRCallFunctionValue3
            or LIRCallMember
            or LIRConstructValue
            or LIRCallFunctionBaseConstructor
            or LIRCreateBoundArrowFunction
            or LIRCreateBoundFunctionExpression;

    private static bool TrySelectGeneralProducerTree(
        MethodBodyIR methodBody,
        TempVariable temp,
        int rootIndex,
        int rootRegion,
        int[] definitionIndex,
        int[] definitionCount,
        int[] useCount,
        int[] regionByLirIndex,
        HashSet<int> selectedDefinitions,
        List<int> emittedDefinitionOrder,
        ref int analysisBudget)
    {
        var pending = new Stack<(TempVariable Temp, bool Expanded)>();
        pending.Push((temp, false));
        while (pending.Count > 0)
        {
            if (--analysisBudget < 0)
            {
                return false;
            }

            var (current, expanded) = pending.Pop();
            if ((uint)current.Index >= (uint)definitionIndex.Length)
            {
                return false;
            }

            var producerIndex = definitionIndex[current.Index];
            if (producerIndex < 0)
            {
                continue;
            }

            if (expanded)
            {
                emittedDefinitionOrder.Add(producerIndex);
                continue;
            }

            var producer = methodBody.Instructions[producerIndex];
            if (!IsGeneralInlineProducer(producer))
            {
                if (regionByLirIndex[producerIndex] == rootRegion)
                {
                    return false;
                }
                continue;
            }

            if (producerIndex >= rootIndex
                || definitionCount[current.Index] != 1
                || useCount[current.Index] != 1
                || regionByLirIndex[producerIndex] != rootRegion
                || current.Index < methodBody.TempVariableSlots.Count
                    && methodBody.TempVariableSlots[current.Index] >= 0)
            {
                return false;
            }

            if (!selectedDefinitions.Add(producerIndex))
            {
                continue;
            }

            pending.Push((current, true));
            var operands = CollectUsedTemps(producer);
            for (var index = operands.Count - 1; index >= 0; index--)
            {
                pending.Push((operands[index], false));
            }
        }

        return true;
    }

    private static bool IsGeneralInlineProducer(LIRInstruction instruction)
        => IsSupportedScheduledInlineProducer(instruction)
            || IsSupportedCallResultDefinition(instruction);

    private static bool PreservesSelectedEffectOrder(
        MethodBodyIR methodBody,
        List<int> emittedDefinitionOrder)
    {
        var previousEffectfulLirIndex = -1;
        foreach (var lirIndex in emittedDefinitionOrder)
        {
            var instruction = methodBody.Instructions[lirIndex];
            if (LIRInstructionInfo.GetEffectsForScheduling(instruction)
                    == LIRInstructionEffects.None)
            {
                continue;
            }

            if (lirIndex < previousEffectfulLirIndex)
            {
                return false;
            }

            previousEffectfulLirIndex = lirIndex;
        }

        return true;
    }

    private static List<int> BuildSelectedEmissionOrder(
        MethodBodyIR methodBody,
        LIRInstruction root,
        HashSet<int> selectedDefinitions)
    {
        var order = new List<int>(selectedDefinitions.Count);
        var emitted = new HashSet<int>();
        var selectedDefinitionByTemp = new Dictionary<int, int>(
            selectedDefinitions.Count);
        foreach (var definitionIndex in selectedDefinitions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(
                    methodBody.Instructions[definitionIndex],
                    out var defined))
            {
                selectedDefinitionByTemp[defined.Index] = definitionIndex;
            }
        }

        var stack = new Stack<(LIRInstruction Instruction, bool Expanded)>();
        stack.Push((root, false));
        while (stack.Count > 0)
        {
            var (instruction, expanded) = stack.Pop();
            if (expanded)
            {
                if (LIRInstructionInfo.TryGetDefinedTemp(
                        instruction,
                        out var defined))
                {
                    var definitionIndex =
                        selectedDefinitionByTemp.GetValueOrDefault(
                            defined.Index,
                            -1);
                    if (definitionIndex >= 0
                        && emitted.Add(definitionIndex))
                    {
                        order.Add(definitionIndex);
                    }
                }
                continue;
            }

            stack.Push((instruction, true));
            var uses = CollectUsedTemps(instruction);
            for (var index = uses.Count - 1; index >= 0; index--)
            {
                var definitionIndex =
                    selectedDefinitionByTemp.GetValueOrDefault(
                        uses[index].Index,
                        -1);
                if (definitionIndex >= 0)
                {
                    stack.Push((
                        methodBody.Instructions[definitionIndex],
                        false));
                }
            }
        }

        return order;
    }

    private static int CountResidualLocalCandidates(
        LIRStackSchedule schedule)
    {
        var count = 0;
        for (var tempIndex = 0;
             tempIndex < schedule.TempResidencies.Length;
             tempIndex++)
        {
            if (schedule.TempResidencies[tempIndex]
                == TempResidency.MaterializedLocal)
            {
                count++;
            }
        }

        return count;
    }

    private static bool TrySelectCallArgumentProducerTree(
        MethodBodyIR methodBody,
        TempVariable temp,
        int rootIndex,
        int rootRegion,
        int[] definitionIndex,
        int[] definitionCount,
        int[] useCount,
        int[] regionByLirIndex,
        HashSet<int> selectedDefinitions,
        ref int analysisBudget)
    {
        var pending = new Stack<TempVariable>();
        pending.Push(temp);
        while (pending.Count > 0)
        {
            if (--analysisBudget < 0)
            {
                return false;
            }

            var current = pending.Pop();
            if ((uint)current.Index >= (uint)definitionIndex.Length)
            {
                return false;
            }

            var producerIndex = definitionIndex[current.Index];
            if (producerIndex < 0)
            {
                continue;
            }

            var producer = methodBody.Instructions[producerIndex];
            var isCall = IsSupportedCallResultDefinition(producer);
            var isInlineProducer =
                IsSupportedScheduledInlineProducer(producer);
            if (!isCall && !isInlineProducer)
            {
                if (regionByLirIndex[producerIndex] == rootRegion)
                {
                    return false;
                }
                continue;
            }

            if (producerIndex >= rootIndex
                || definitionCount[current.Index] != 1
                || regionByLirIndex[producerIndex] != rootRegion)
            {
                return false;
            }

            if (useCount[current.Index] != 1
                || current.Index < methodBody.TempVariableSlots.Count
                    && methodBody.TempVariableSlots[current.Index] >= 0)
            {
                if (isCall)
                {
                    return false;
                }
                continue;
            }

            if (!selectedDefinitions.Add(producerIndex))
            {
                continue;
            }

            var operands = CollectUsedTemps(producer);
            for (var index = operands.Count - 1; index >= 0; index--)
            {
                pending.Push(operands[index]);
            }
        }

        return true;
    }

    private static bool IsSupportedCallResultDefinition(
        LIRInstruction instruction)
    {
        if (instruction is LIRCallIntrinsicStatic
            {
                GenericTypeArgument: null
            })
        {
            return true;
        }

        return instruction is LIRCallTypedMember typed
                && typed.ReturnClrType != typeof(void)
            || instruction is LIRCallUserClassInstanceMethod
            {
                HasScopesParameter: false,
                RequiresPrivateBrandCheck: false
            };
    }

    internal static bool IsSupportedScheduledInlineCallProducer(
        LIRInstruction instruction)
        => IsSupportedCallResultDefinition(instruction);

    private static bool IsSupportedCallResultConsumer(
        MethodBodyIR methodBody,
        TempVariable result,
        LIRInstruction consumer,
        int[] useCount,
        int[] useIndex)
        => IsTypedNumericBinary(consumer)
            || IsTypedUnaryOrComparison(consumer)
            || IsConversionConcatOrStableLoad(consumer)
            || IsSafeBoxingReturnConsumer(
                methodBody,
                consumer,
                useCount,
                useIndex)
            || consumer is LIRReturn
            || consumer is LIRCallIntrinsicStatic intrinsic
                && IsSupportedDirectIntrinsicConsumer(intrinsic, result)
            || consumer is LIRCallTypedMember typed
                && typed.Receiver.Equals(result)
            || IsSupportedDirectReceiverConsumer(consumer, result);

    private static bool IsSupportedDirectReceiverConsumer(
        LIRInstruction instruction,
        TempVariable result)
        => instruction switch
        {
            LIRGetItem getItem => getItem.Object.Equals(result),
            LIRGetItemAsNumber getItem =>
                getItem.Object.Equals(result),
            LIRGetItemAsNumberString getItem =>
                getItem.Object.Equals(result),
            LIRSetItem setItem => setItem.Object.Equals(result),
            LIRCallIntrinsic call =>
                call.IntrinsicObject.Equals(result),
            LIRCallInstanceMethod call =>
                call.Receiver.Equals(result),
            LIRCallMember call => call.Receiver.Equals(result),
            LIRCallMember0 call => call.Receiver.Equals(result),
            LIRCallMember1 call => call.Receiver.Equals(result),
            LIRCallMember2 call => call.Receiver.Equals(result),
            LIRCallMember3 call => call.Receiver.Equals(result),
            LIRCallFunctionValue call =>
                call.FunctionValue.Equals(result),
            LIRCallFunctionValue0 call =>
                call.FunctionValue.Equals(result),
            LIRCallFunctionValue1 call =>
                call.FunctionValue.Equals(result),
            LIRCallFunctionValue2 call =>
                call.FunctionValue.Equals(result),
            LIRCallFunctionValue3 call =>
                call.FunctionValue.Equals(result),
            _ => false
        };

    private static bool IsSupportedDirectIntrinsicConsumer(
        LIRCallIntrinsicStatic instruction,
        TempVariable result)
        => string.Equals(
                instruction.IntrinsicName,
                "Math",
                StringComparison.Ordinal)
            && !IsKnownParamsArrayIntrinsicCall(instruction)
            && instruction.Arguments.Count > 0
            && instruction.Arguments[0].Equals(result);

    private static bool IsKnownParamsArrayIntrinsicCall(
        LIRInstruction instruction)
        => instruction is LIRCallIntrinsicStatic
            {
                IntrinsicName: "Math",
                MethodName: "max" or "min" or "hypot",
                GenericTypeArgument: null
            };

    private static bool HasOnlySupportedCallInterveningInstructions(
        MethodBodyIR methodBody,
        int definitionIndex,
        int useIndex)
    {
        for (var index = definitionIndex + 1; index < useIndex; index++)
        {
            var instruction = methodBody.Instructions[index];
            if (IsSupportedCallResultDefinition(instruction)
                || IsSupportedScheduledInlineProducer(instruction)
                || instruction is LIRLoadUserClassInstanceField
                    or LIRLoadUserClassStaticField)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool IsSupportedArgumentBundle(LIRInstruction instruction)
        => instruction is LIRCallIntrinsicStatic
            {
                GenericTypeArgument: null
            };

    internal static bool IsSupportedScheduledInlineRoot(
        LIRInstruction instruction)
        => IsSupportedConstruction(instruction)
            || IsSupportedArgumentBundle(instruction)
            || IsSupportedInlineCallArrayRoot(instruction);

    private static bool IsSupportedConstructionConsumer(
        MethodBodyIR methodBody,
        TempVariable result,
        int consumerIndex,
        int rootRegion,
        int[] regionByLirIndex)
    {
        if ((uint)consumerIndex >= (uint)methodBody.Instructions.Count)
        {
            return false;
        }

        var consumer = methodBody.Instructions[consumerIndex];
        if (consumer is LIRReturn)
        {
            return !methodBody.IsAsync && !methodBody.IsGenerator;
        }

        if (regionByLirIndex[consumerIndex] != rootRegion
            || consumer is not (
                LIRCallIntrinsicStaticWithArgsArray
                or LIRCallIntrinsicStaticVoidWithArgsArray))
        {
            return false;
        }

        var uses = CollectUsedTemps(consumer);
        return uses.Count > 0 && uses[0].Equals(result);
    }

    private static bool TrySelectConstructionProducerTree(
        MethodBodyIR methodBody,
        TempVariable temp,
        int constructionIndex,
        int constructionRegion,
        int[] definitionIndex,
        int[] definitionCount,
        int[] useCount,
        int[] regionByLirIndex,
        HashSet<int> selectedDefinitions,
        ref int analysisBudget)
    {
        var pending = new Stack<TempVariable>();
        pending.Push(temp);
        while (pending.Count > 0)
        {
            if (--analysisBudget < 0)
            {
                return false;
            }

            var current = pending.Pop();
            if ((uint)current.Index >= (uint)definitionIndex.Length)
            {
                return false;
            }

            var producerIndex = definitionIndex[current.Index];
            if (producerIndex < 0)
            {
                continue;
            }

            var producer = methodBody.Instructions[producerIndex];
            if (!IsSupportedScheduledInlineProducer(producer))
            {
                if (regionByLirIndex[producerIndex] == constructionRegion)
                {
                    return false;
                }
                continue;
            }

            if (producerIndex >= constructionIndex
                || definitionCount[current.Index] != 1
                || useCount[current.Index] != 1
                || regionByLirIndex[producerIndex] != constructionRegion
                || current.Index < methodBody.TempVariableSlots.Count
                    && methodBody.TempVariableSlots[current.Index] >= 0)
            {
                return false;
            }

            if (!selectedDefinitions.Add(producerIndex))
            {
                continue;
            }

            var operands = CollectUsedTemps(producer);
            for (var index = operands.Count - 1; index >= 0; index--)
            {
                pending.Push(operands[index]);
            }
        }

        return true;
    }

    internal static bool IsSupportedScheduledInlineProducer(
        LIRInstruction instruction)
        => instruction is LIRConstNumber
            or LIRConstString
            or LIRConstBoolean
            or LIRConstUndefined
            or LIRConstNull
            or LIRLoadParameter
            or LIRLoadThis
            or LIRAddNumber
            or LIRSubNumber
            or LIRMulNumber
            or LIRDivNumber
            or LIRModNumber
            or LIRExpNumber
            or LIRNegateNumber
            or LIRBitwiseNotNumber
            or LIRCompareNumberLessThan
            or LIRCompareNumberGreaterThan
            or LIRCompareNumberLessThanOrEqual
            or LIRCompareNumberGreaterThanOrEqual
            or LIRCompareNumberEqual
            or LIRCompareNumberNotEqual
            or LIRCompareBooleanEqual
            or LIRCompareBooleanNotEqual
            or LIRConvertToNumber
            or LIRConvertToObject
            or LIRConcatStrings
            or LIRGetStringLength
            or LIRGetJsArrayLength
            or LIRGetInt32ArrayLength
            or LIRGetJsArrayElement
            or LIRGetInt32ArrayElement
            or LIRBuildArray
            or LIRBuildScopesArray
            or LIRNewJsArray
            or LIRNewJsObject;

    private static List<TempVariable> CollectUsedTemps(
        LIRInstruction instruction)
    {
        var uses = new List<TempVariable>();
        var visitor = new NumericCollectUseVisitor(uses);
        LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        return uses;
    }

    private static bool TryGetConstructionInsertionIndex(
        int constructionIndex,
        HashSet<int> selectedDefinitions,
        out int insertionLirIndex)
    {
        insertionLirIndex = constructionIndex;
        while (insertionLirIndex > 0
            && selectedDefinitions.Contains(insertionLirIndex - 1))
        {
            insertionLirIndex--;
        }

        return insertionLirIndex < constructionIndex
            && selectedDefinitions.Count
                == constructionIndex - insertionLirIndex;
    }

    private static ScheduledOperation[] ReorderConstructionOperations(
        ScheduledOperation[] operations,
        Dictionary<int, List<int>> movesByInsertionLirIndex)
    {
        if (movesByInsertionLirIndex.Count == 0)
        {
            return operations;
        }

        var movedRootIndexes = movesByInsertionLirIndex.Values
            .SelectMany(static roots => roots)
            .ToHashSet();
        var operationByRootIndex = new Dictionary<int, ScheduledOperation>();
        foreach (var operation in operations)
        {
            if (movedRootIndexes.Contains(operation.StartLirIndex))
            {
                operationByRootIndex[operation.StartLirIndex] = operation;
            }
        }

        var reordered = new List<ScheduledOperation>(operations.Length);
        foreach (var operation in operations)
        {
            if (movesByInsertionLirIndex.TryGetValue(
                    operation.StartLirIndex,
                    out var movedRoots))
            {
                foreach (var rootIndex in movedRoots)
                {
                    if (operationByRootIndex.TryGetValue(
                            rootIndex,
                            out var movedOperation))
                    {
                        reordered.Add(movedOperation);
                    }
                }
            }

            if (!movedRootIndexes.Contains(operation.StartLirIndex))
            {
                reordered.Add(operation);
            }
        }

        return reordered.ToArray();
    }

    private static int[] BuildRegionIndexMap(
        int instructionCount,
        ScheduledRegion[] regions,
        ScheduledOperation[] operations)
    {
        var map = new int[instructionCount];
        Array.Fill(map, -1);
        for (var regionIndex = 0; regionIndex < regions.Length; regionIndex++)
        {
            var region = regions[regionIndex];
            for (var offset = 0; offset < region.OperationCount; offset++)
            {
                var operation = operations[region.StartOperationIndex + offset];
                for (var lirOffset = 0;
                     lirOffset < operation.InstructionCount;
                     lirOffset++)
                {
                    map[operation.GetLirInstructionIndex(lirOffset)] = regionIndex;
                }
            }
        }

        return map;
    }

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

        return (IsTypedNumericBinary(instruction)
                && IsDefinedValueRequired(
                    methodBody,
                    instruction,
                    useCount))
            || IsSupportedTypedNumericTerminal(methodBody, instruction)
            || instruction is LIRStoreParameter;
    }

    private static bool IsDefinedValueRequired(
        MethodBodyIR methodBody,
        LIRInstruction instruction,
        int[] useCount)
    {
        if (!LIRInstructionInfo.TryGetDefinedTemp(
                instruction,
                out var defined)
            || (uint)defined.Index >= (uint)useCount.Length)
        {
            return true;
        }

        return useCount[defined.Index] > 0
            || defined.Index < methodBody.TempVariableSlots.Count
                && methodBody.TempVariableSlots[defined.Index] >= 0;
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
                        or LIRCompareBooleanNotEqual
                        or LIRConvertToNumber
                        or LIRConvertToObject
                        or LIRConcatStrings
                        or LIRGetStringLength
                        or LIRGetJsArrayLength
                        or LIRGetInt32ArrayLength
                        or LIRGetJsArrayElement
                        or LIRGetInt32ArrayElement))
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

    private static int[] ComputeScheduledLastUses(
        MethodBodyIR methodBody,
        ScheduledOperation[] operations)
    {
        var lastUses = new int[methodBody.Temps.Count];
        Array.Fill(lastUses, -1);
        var scheduledPosition = 0;
        foreach (var operation in operations)
        {
            for (var offset = 0; offset < operation.InstructionCount; offset++)
            {
                var visitor = new LastUseVisitor(
                    lastUses,
                    scheduledPosition++);
                LIRInstructionInfo.VisitUsedTemps(
                    methodBody.Instructions[
                        operation.GetLirInstructionIndex(offset)],
                    ref visitor);
            }
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

            var startLirIndex = int.MaxValue;
            var endLirIndexExclusive = -1;
            for (var operationIndex = regionStartOperation;
                 operationIndex < endOperationIndexExclusive;
                 operationIndex++)
            {
                startLirIndex = Math.Min(
                    startLirIndex,
                    operations[operationIndex].StartLirIndex);
                endLirIndexExclusive = Math.Max(
                    endLirIndexExclusive,
                    operations[operationIndex].EndLirIndexExclusive);
            }

            regions[regionCount++] = new ScheduledRegion(
                startLirIndex,
                endLirIndexExclusive,
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
