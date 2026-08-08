using Jroc.IL;
using Jroc.IR;
using Xunit;

namespace Jroc.Tests;

public sealed class TempLocalAllocatorTests
{
    [Fact]
    public void Allocate_ScheduledLastUseExtendsLifetime_PreventsEarlySlotReuse()
    {
        var body = CreateSequentialCallBody();
        var schedule = LIRStackScheduler.Identity(body);
        var extendedLastUses = schedule.EffectiveLastUses.ToArray();
        extendedLastUses[0] = body.Instructions.Count - 1;
        schedule = schedule with
        {
            Mode = LIRStackSchedulerMode.TypedNumeric,
            EffectiveLastUses = extendedLastUses
        };
        var plan = CreatePlan(body, schedule);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.NotEqual(allocation.TempToSlot[0], allocation.TempToSlot[1]);
    }

    [Fact]
    public void Allocate_NonOverlappingCompatibleTemps_ReuseOneSlot()
    {
        var body = CreateSequentialCallBody();
        var schedule = LIRStackScheduler.Identity(body);
        var plan = CreatePlan(body, schedule);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.Equal(allocation.TempToSlot[0], allocation.TempToSlot[1]);
        Assert.Single(allocation.SlotStorages);
    }

    [Fact]
    public void Allocate_OverlappingCompatibleTemps_UseDifferentSlots()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var right = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("Left", left));
        body.Instructions.Add(Call("Right", right));
        body.Instructions.Add(new LIRAddDynamic(left, right, result));
        body.Instructions.Add(new LIRReturn(result));
        var schedule = LIRStackScheduler.Identity(body);
        var plan = CreatePlan(body, schedule);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.NotEqual(allocation.TempToSlot[left.Index], allocation.TempToSlot[right.Index]);
        Assert.Equal(2, allocation.SlotStorages.Count);
    }

    [Fact]
    public void Allocate_TempUsedTwiceAtLastUse_ReleasesSlotOnlyOnce()
    {
        var body = new MethodBodyIR();
        var repeated = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var doubled = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var firstLater = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var secondLater = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var final = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.Instructions.Add(Call("Repeated", repeated));
        body.Instructions.Add(new LIRAddNumber(repeated, repeated, doubled));
        body.Instructions.Add(Call("FirstLater", firstLater));
        body.Instructions.Add(Call("SecondLater", secondLater));
        body.Instructions.Add(new LIRAddNumber(firstLater, secondLater, final));
        body.Instructions.Add(new LIRReturn(final));
        var schedule = LIRStackScheduler.Identity(body);
        var plan = CreatePlan(body, schedule);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.NotEqual(
            allocation.TempToSlot[firstLater.Index],
            allocation.TempToSlot[secondLater.Index]);
    }

    [Fact]
    public void Allocate_NonOverlappingIncompatibleTemps_DoNotReuseSlot()
    {
        var body = new MethodBodyIR();
        var objectTemp = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var numberTemp = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.Instructions.Add(Call("Object", objectTemp));
        body.Instructions.Add(new LIRStoreParameter(1, objectTemp));
        body.Instructions.Add(Call("Number", numberTemp));
        body.Instructions.Add(new LIRReturn(numberTemp));
        var schedule = LIRStackScheduler.Identity(body);
        var plan = CreatePlan(body, schedule);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.NotEqual(
            allocation.TempToSlot[objectTemp.Index],
            allocation.TempToSlot[numberTemp.Index]);
        Assert.Equal(2, allocation.SlotStorages.Count);
    }

    [Fact]
    public void Allocate_SchedulerOwnedStackTemp_ReceivesNoLocal()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("Value", value));
        body.Instructions.Add(new LIRReturn(value));
        var schedule = WithSchedulerOwnedTemp(
            LIRStackScheduler.Identity(body),
            value,
            TempResidency.StackResident);
        var plan = CreatePlan(body, schedule);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.Equal(-1, allocation.TempToSlot[value.Index]);
        Assert.Empty(allocation.SlotStorages);
        Assert.Equal(TempValueOwner.Scheduler, plan.GetOwner(value.Index));
    }

    [Fact]
    public void MaterializationPlan_ForcesSnapshotResumeAndCatchResults()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var snapshot = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var awaitResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var yieldResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var exception = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(Exception)));
        var unwrapped = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("Source", source));
        body.Instructions.Add(new LIRCopyTemp(source, snapshot));
        body.Instructions.Add(new LIRAwait(source, 0, 1, 10, awaitResult));
        body.Instructions.Add(new LIRYield(source, 2, 20, yieldResult));
        body.Instructions.Add(new LIRStoreException(exception));
        body.Instructions.Add(new LIRUnwrapCatchException(exception, unwrapped));
        var plan = TempMaterializationPlan.Create(
            body,
            schedule: null,
            requiresConstructorResultOverride: _ => false);

        Assert.Equal(TempValueOwner.SnapshotBarrier, plan.GetOwner(snapshot.Index));
        Assert.Equal(TempValueOwner.ResumeResult, plan.GetOwner(awaitResult.Index));
        Assert.Equal(TempValueOwner.ResumeResult, plan.GetOwner(yieldResult.Index));
        Assert.Equal(TempValueOwner.CatchResult, plan.GetOwner(exception.Index));
        Assert.Equal(TempValueOwner.MaterializedLocal, plan.GetOwner(unwrapped.Index));
        Assert.True(plan.ShouldMaterialize(snapshot.Index));
        Assert.True(plan.ShouldMaterialize(awaitResult.Index));
        Assert.True(plan.ShouldMaterialize(yieldResult.Index));
    }

    [Fact]
    public void MaterializationPlan_MultiplyDefinedTemp_PreservesSnapshotBarrier()
    {
        var body = new MethodBodyIR();
        var exception = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(Exception)));
        var source = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var snapshot = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(new LIRUnwrapCatchException(exception, snapshot));
        body.Instructions.Add(new LIRCopyTemp(source, snapshot));

        var plan = TempMaterializationPlan.Create(
            body,
            schedule: null,
            requiresConstructorResultOverride: _ => false);

        Assert.Equal(
            TempValueOwner.SnapshotBarrier,
            plan.GetOwner(snapshot.Index));
        Assert.True(plan.ShouldMaterialize(snapshot.Index));
    }

    [Fact]
    public void MaterializationPlan_RejectsOverlappingRematerializationClaim()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("Value", value));
        body.Instructions.Add(new LIRReturn(value));
        var schedule = WithSchedulerOwnedTemp(
            LIRStackScheduler.Identity(body),
            value,
            TempResidency.StackResident);
        var plan = CreatePlan(body, schedule);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            plan.Claim(
                value.Index,
                TempResidency.Rematerialized,
                TempValueOwner.Rematerialization));

        Assert.Contains(nameof(TempValueOwner.Scheduler), exception.Message);
        Assert.Contains(nameof(TempValueOwner.Rematerialization), exception.Message);
    }

    [Fact]
    public void Rematerialization_DoesNotClaimSchedulerOwnedTemp()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new LIRReturn(value));
        var schedule = WithSchedulerOwnedTemp(
            LIRStackScheduler.Identity(body),
            value,
            TempResidency.StackResident);
        var plan = CreatePlan(body, schedule);

        _ = TempLocalAllocator.Allocate(body, plan, schedule);

        Assert.Equal(TempValueOwner.Scheduler, plan.GetOwner(value.Index));
    }

    [Fact]
    public void BranchFusion_DoesNotClaimSchedulerOwnedComparison()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var right = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var comparison = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        body.Instructions.Add(new LIRConstNumber(1, left));
        body.Instructions.Add(new LIRConstNumber(2, right));
        body.Instructions.Add(new LIRCompareNumberLessThan(left, right, comparison));
        body.Instructions.Add(new LIRBranchIfFalse(comparison, 1));
        body.Instructions.Add(new LIRLabel(1));
        var schedule = WithSchedulerOwnedTemp(
            LIRStackScheduler.Identity(body),
            comparison,
            TempResidency.StackResident);
        var plan = CreatePlan(body, schedule);
        var definitions = BranchConditionOptimizer.BuildTempDefinitionMap(body);

        BranchConditionOptimizer.MarkBranchOnlyComparisonTemps(
            body,
            plan,
            definitions);

        Assert.Equal(TempValueOwner.Scheduler, plan.GetOwner(comparison.Index));
    }

    [Fact]
    public void MaterializationPlan_RejectsSchedulerOwnedVariableSlot()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.TempVariableSlots[value.Index] = 0;
        body.VariableNames.Add("value");
        body.VariableStorages.Add(new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("Value", value));
        body.Instructions.Add(new LIRReturn(value));
        var schedule = WithSchedulerOwnedTemp(
            LIRStackScheduler.Identity(body),
            value,
            TempResidency.StackResident);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            CreatePlan(body, schedule));

        Assert.Contains("Scheduler-owned temp", exception.Message);
        Assert.Contains(nameof(TempValueOwner.VariableSlot), exception.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Allocate_ResumeResultTemp_IsMaterialized(bool isAwait)
    {
        var body = new MethodBodyIR();
        var input = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("Input", input));
        body.Instructions.Add(isAwait
            ? new LIRAwait(input, 0, 1, 10, result)
            : new LIRYield(input, 1, 10, result));
        body.Instructions.Add(new LIRReturn(result));
        var plan = TempMaterializationPlan.Create(
            body,
            schedule: null,
            requiresConstructorResultOverride: _ => false);

        var allocation = TempLocalAllocator.Allocate(body, plan, schedule: null);

        Assert.Equal(TempValueOwner.ResumeResult, plan.GetOwner(result.Index));
        Assert.True(plan.ShouldMaterialize(result.Index));
        Assert.True(allocation.IsMaterialized(result));
    }

    private static MethodBodyIR CreateSequentialCallBody()
    {
        var body = new MethodBodyIR();
        var first = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var second = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(Call("First", first));
        body.Instructions.Add(new LIRStoreParameter(1, first));
        body.Instructions.Add(Call("Second", second));
        body.Instructions.Add(new LIRReturn(second));
        return body;
    }

    private static LIRCallRuntimeServicesStatic Call(
        string name,
        TempVariable result)
        => new(name, System.Array.Empty<TempVariable>(), result);

    private static TempVariable AddTemp(
        MethodBodyIR body,
        ValueStorage storage)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(-1);
        return temp;
    }

    private static TempMaterializationPlan CreatePlan(
        MethodBodyIR body,
        LIRStackSchedule schedule)
        => TempMaterializationPlan.Create(
            body,
            schedule,
            requiresConstructorResultOverride: _ => false);

    private static LIRStackSchedule WithSchedulerOwnedTemp(
        LIRStackSchedule schedule,
        TempVariable temp,
        TempResidency residency)
    {
        var owned = schedule.OwnedTemps.ToArray();
        var residencies = schedule.TempResidencies.ToArray();
        owned[temp.Index] = true;
        residencies[temp.Index] = residency;
        return schedule with
        {
            OwnedTemps = owned,
            TempResidencies = residencies
        };
    }
}
