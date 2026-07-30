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

        return options.Mode switch
        {
            LIRStackSchedulerMode.Identity => Identity(methodBody),
            LIRStackSchedulerMode.Disabled => throw new InvalidOperationException(
                "Disabled scheduler mode bypasses schedule construction."),
            _ => throw new NotSupportedException(
                $"LIR stack scheduler mode '{options.Mode}' is not implemented yet.")
        };
    }

    internal static LIRStackSchedule Identity(MethodBodyIR methodBody)
    {
        ArgumentNullException.ThrowIfNull(methodBody);

        var operations = BuildIdentityOperations(methodBody);
        var tempResidencies = new TempResidency[methodBody.Temps.Count];
        Array.Fill(tempResidencies, TempResidency.MaterializedLocal);

        var ownedTemps = new bool[methodBody.Temps.Count];
        var effectiveLastUses = ComputeIdentityLastUses(methodBody);

        return new LIRStackSchedule(
            LIRStackSchedulerMode.Identity,
            operations,
            Array.Empty<ScheduledRegion>(),
            tempResidencies,
            ownedTemps,
            effectiveLastUses,
            MaxStackDepth: 0,
            new LIRStackScheduleMetrics(
                ScheduledRegionCount: 0,
                StackResidentTempCount: 0,
                EliminatedSpillCount: 0));
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
            VisitIdentityUsedTemps(methodBody.Instructions[instructionIndex], ref visitor);
        }

        return lastUses;
    }

    private static void VisitIdentityUsedTemps<TVisitor>(
        LIRInstruction instruction,
        ref TVisitor visitor)
        where TVisitor : struct, ITempUseVisitor
    {
        // These exception operands are not yet present in the allocator's
        // legacy visitor. Keep identity schedule metadata correct without
        // changing legacy allocation/output in this no-IL-change stage.
        switch (instruction)
        {
            case LIRThrow throwInstruction:
                visitor.Visit(throwInstruction.Value);
                return;
            case LIRUnwrapCatchException unwrapCatch:
                visitor.Visit(unwrapCatch.Exception);
                return;
            default:
                TempLocalAllocator.VisitUsedTemps(instruction, ref visitor);
                return;
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
