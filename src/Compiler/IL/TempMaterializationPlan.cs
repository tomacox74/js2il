using Jroc.IR;

namespace Jroc.IL;

internal enum TempValueOwner
{
    MaterializedLocal,
    Scheduler,
    BranchConditionFusion,
    Rematerialization,
    VariableSlot,
    SnapshotBarrier,
    ResumeResult,
    CatchResult,
    ConstructorResultOverride
}

/// <summary>
/// Single source of truth for which compiler component owns each temp's
/// residency decision. Claims are exclusive and forced materialization is
/// established before optional optimization owners run.
/// </summary>
internal sealed class TempMaterializationPlan
{
    private readonly TempResidency[] _residencies;
    private readonly TempValueOwner[] _owners;

    private TempMaterializationPlan(int tempCount)
    {
        _residencies = new TempResidency[tempCount];
        Array.Fill(_residencies, TempResidency.MaterializedLocal);
        _owners = new TempValueOwner[tempCount];
        Array.Fill(_owners, TempValueOwner.MaterializedLocal);
    }

    internal int Count => _residencies.Length;

    internal IReadOnlyList<TempResidency> Residencies => _residencies;

    internal IReadOnlyList<TempValueOwner> Owners => _owners;

    internal static TempMaterializationPlan Create(
        MethodBodyIR methodBody,
        LIRStackSchedule? schedule,
        Func<LIRNewUserClass, bool> requiresConstructorResultOverride)
    {
        ArgumentNullException.ThrowIfNull(methodBody);
        ArgumentNullException.ThrowIfNull(requiresConstructorResultOverride);

        var plan = new TempMaterializationPlan(methodBody.Temps.Count);
        if (schedule is not null)
        {
            for (var tempIndex = 0;
                 tempIndex < methodBody.Temps.Count;
                 tempIndex++)
            {
                if (!schedule.OwnedTemps[tempIndex])
                {
                    continue;
                }

                plan.Claim(
                    tempIndex,
                    schedule.TempResidencies[tempIndex],
                    TempValueOwner.Scheduler);
            }
        }

        for (var tempIndex = 0;
             tempIndex < methodBody.TempVariableSlots.Count;
             tempIndex++)
        {
            if (methodBody.TempVariableSlots[tempIndex] >= 0)
            {
                plan.ForceMaterialized(tempIndex, TempValueOwner.VariableSlot);
            }
        }

        var firstDefinitionByTemp = new LIRInstruction?[methodBody.Temps.Count];
        foreach (var instruction in methodBody.Instructions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && (uint)defined.Index
                    < (uint)firstDefinitionByTemp.Length
                && firstDefinitionByTemp[defined.Index] is null)
            {
                firstDefinitionByTemp[defined.Index] = instruction;
            }

            switch (instruction)
            {
                case LIRStoreException storeException:
                    plan.ForceMaterialized(
                        storeException.Result.Index,
                        TempValueOwner.CatchResult);
                    break;
                case LIRNewUserClass newUserClass
                    when requiresConstructorResultOverride(newUserClass):
                    plan.ForceMaterialized(
                        newUserClass.Result.Index,
                        TempValueOwner.ConstructorResultOverride);
                    break;
            }
        }

        for (var tempIndex = 0;
             tempIndex < firstDefinitionByTemp.Length;
             tempIndex++)
        {
            switch (firstDefinitionByTemp[tempIndex])
            {
                case LIRCopyTemp:
                    plan.ForceMaterialized(
                        tempIndex,
                        TempValueOwner.SnapshotBarrier);
                    break;
                case LIRAwait:
                case LIRYield:
                    plan.ForceMaterialized(
                        tempIndex,
                        TempValueOwner.ResumeResult);
                    break;
            }
        }

        return plan;
    }

    internal TempResidency GetResidency(int tempIndex)
        => _residencies[tempIndex];

    internal TempValueOwner GetOwner(int tempIndex)
        => _owners[tempIndex];

    internal bool ShouldMaterialize(int tempIndex)
        => _residencies[tempIndex] == TempResidency.MaterializedLocal;

    internal bool IsClaimedByScheduler(int tempIndex)
        => _owners[tempIndex] == TempValueOwner.Scheduler;

    internal bool TryClaim(
        int tempIndex,
        TempResidency residency,
        TempValueOwner owner)
    {
        ValidateIndex(tempIndex);
        if (residency == TempResidency.MaterializedLocal)
        {
            throw new ArgumentException(
                "Non-materialized claims require stack, scheduled-inline, or "
                + "rematerialized residency.",
                nameof(residency));
        }

        if (_owners[tempIndex] != TempValueOwner.MaterializedLocal)
        {
            return false;
        }

        _residencies[tempIndex] = residency;
        _owners[tempIndex] = owner;
        return true;
    }

    internal void Claim(
        int tempIndex,
        TempResidency residency,
        TempValueOwner owner)
    {
        if (!TryClaim(tempIndex, residency, owner))
        {
            throw new InvalidOperationException(
                $"Temp {tempIndex} is already owned by {_owners[tempIndex]} and "
                + $"cannot also be claimed by {owner}.");
        }
    }

    internal void ForceMaterialized(int tempIndex, TempValueOwner owner)
    {
        ValidateIndex(tempIndex);
        var currentOwner = _owners[tempIndex];
        if (currentOwner == TempValueOwner.Scheduler)
        {
            throw new InvalidOperationException(
                $"Scheduler-owned temp {tempIndex} cannot be forced to materialize "
                + $"by {owner}.");
        }

        _residencies[tempIndex] = TempResidency.MaterializedLocal;
        _owners[tempIndex] = owner;
    }

    internal void ValidateAgainstSchedule(
        MethodBodyIR methodBody,
        LIRStackSchedule? schedule)
    {
        if (methodBody.Temps.Count != Count)
        {
            throw new InvalidOperationException(
                $"Materialization plan has {Count} temps, but method has "
                + $"{methodBody.Temps.Count}.");
        }

        for (var tempIndex = 0; tempIndex < Count; tempIndex++)
        {
            var schedulerOwns = schedule?.OwnedTemps[tempIndex] == true;
            if (schedulerOwns != IsClaimedByScheduler(tempIndex))
            {
                throw new InvalidOperationException(
                    $"Scheduler ownership mismatch for temp {tempIndex}: schedule="
                    + $"{schedulerOwns}, materialization plan="
                    + $"{IsClaimedByScheduler(tempIndex)}.");
            }

            if (_owners[tempIndex] == TempValueOwner.Scheduler
                && _residencies[tempIndex] == TempResidency.MaterializedLocal)
            {
                throw new InvalidOperationException(
                    $"Scheduler-owned temp {tempIndex} cannot be materialized.");
            }

            if (_owners[tempIndex] != TempValueOwner.Scheduler
                && schedulerOwns)
            {
                throw new InvalidOperationException(
                    $"Temp {tempIndex} is owned by both scheduler and "
                    + $"{_owners[tempIndex]}.");
            }
        }
    }

    private void ValidateIndex(int tempIndex)
    {
        if ((uint)tempIndex >= (uint)_residencies.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(tempIndex));
        }
    }
}
