using Jroc.IR;

namespace Jroc.IL;

/// <summary>
/// Cumulative scheduler coverage levels. New levels must include the behavior
/// of every preceding level so adjacent modes remain useful for A/B diagnosis.
/// </summary>
internal enum LIRStackSchedulerMode
{
    Disabled = 0,
    Identity = 1,
    TypedNumeric = 2
}

internal enum TempResidency
{
    MaterializedLocal,
    StackResident,
    Rematerialized
}

internal enum InstructionDisposition
{
    EmitNormally,
    EmitAndDiscardResult,
    ElidePureUnused,
    FusedIntoEmissionUnit
}

/// <summary>
/// One atomic unit in the IL emission order. Identity scheduling uses
/// consecutive source indexes; future schedulers may reorder these operations.
/// </summary>
internal readonly record struct ScheduledOperation(
    int StartLirIndex,
    int InstructionCount,
    InstructionDisposition Disposition)
{
    internal int EndLirIndexExclusive => checked(StartLirIndex + InstructionCount);

    internal int GetLirInstructionIndex(int operationOffset)
    {
        if ((uint)operationOffset >= (uint)InstructionCount)
        {
            throw new ArgumentOutOfRangeException(nameof(operationOffset));
        }

        return StartLirIndex + operationOffset;
    }
}

internal readonly record struct ScheduledRegion(
    int StartLirIndex,
    int EndLirIndexExclusive,
    IReadOnlyList<ScheduledOperation> Operations,
    int MaxStackDepth);

internal readonly record struct LIRStackScheduleMetrics(
    int ScheduledRegionCount,
    int StackResidentTempCount,
    int EliminatedSpillCount);

/// <summary>
/// Immutable scheduler output consumed by local allocation and IL emission.
/// Identity mode deliberately delegates all residency decisions to the legacy
/// pipeline while making emission order and atomic operation ownership explicit.
/// </summary>
internal sealed record LIRStackSchedule(
    LIRStackSchedulerMode Mode,
    IReadOnlyList<ScheduledOperation> Operations,
    IReadOnlyList<ScheduledRegion> Regions,
    IReadOnlyList<TempResidency> TempResidencies,
    IReadOnlyList<bool> OwnedTemps,
    IReadOnlyList<int> EffectiveLastUses,
    int MaxStackDepth,
    LIRStackScheduleMetrics Metrics);

internal readonly record struct LIRStackSchedulerOptions(LIRStackSchedulerMode Mode);
