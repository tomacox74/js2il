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

        for (var operationIndex = 0; operationIndex < operations.Length; operationIndex++)
        {
            var operation = operations[operationIndex];
            var isBoundary = false;
            for (var offset = 0; offset < operation.InstructionCount; offset++)
            {
                var instruction = methodBody.Instructions[
                    operation.GetLirInstructionIndex(offset)];
                if (LIRInstructionInfo.IsSchedulingBoundary(instruction))
                {
                    isBoundary = true;
                    break;
                }
            }

            if (isBoundary)
            {
                AppendRegionBeforeBoundary(operationIndex);
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
}
