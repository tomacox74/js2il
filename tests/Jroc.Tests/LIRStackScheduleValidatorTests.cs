using Jroc.IL;
using Jroc.IR;

namespace Jroc.Tests;

public sealed class LIRStackScheduleValidatorTests
{
    [Fact]
    public void Validate_IdentitySchedule_AnnotatesWithoutCarriedStack()
    {
        var body = CreateNumericReturnBody();

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(0, schedule.MaxStackDepth);
        Assert.All(schedule.CarriedStackDepthBeforeInstructions, depth => Assert.Equal(0, depth));
        Assert.All(schedule.Regions, region => Assert.Equal(0, region.MaxStackDepth));
    }

    [Fact]
    public void Validate_DeepStackResidentArrayConstruction_ComputesDepthAboveEight()
    {
        var body = new MethodBodyIR();
        var elements = Enumerable.Range(0, 10)
            .Select(_ => AddTemp(body))
            .ToArray();
        var array = AddTemp(body);

        for (var index = 0; index < elements.Length; index++)
        {
            body.Instructions.Add(new LIRConstNumber(index, elements[index]));
        }
        body.Instructions.Add(new LIRBuildArray(elements, array));
        body.Instructions.Add(new LIRReturn(array));

        var schedule = WithStackResidents(
            LIRStackScheduler.Identity(body),
            elements.Append(array).ToArray());
        var validated = LIRStackScheduleValidator.ValidateAndAnnotate(
            body,
            schedule);

        Assert.Equal(10, validated.MaxStackDepth);
        Assert.Equal(10, Assert.Single(validated.Regions).MaxStackDepth);
        Assert.Equal(10, validated.CarriedStackDepthBeforeInstructions[10]);
        Assert.Equal(1, validated.CarriedStackDepthBeforeInstructions[11]);
    }

    [Fact]
    public void Validate_WrongLifoOrder_IsRejected()
    {
        var body = CreateNumericReturnBody();
        var identity = WithStackResidents(
            LIRStackScheduler.Identity(body),
            body.Temps.ToArray());
        var operations = identity.Operations.ToArray();
        (operations[0], operations[1]) = (operations[1], operations[0]);
        var invalid = identity with { Operations = operations };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("LIFO operand mismatch", exception.Message);
    }

    [Fact]
    public void Validate_IntraRegionUseBeforeDefinition_IsRejected()
    {
        var body = CreateNumericReturnBody();
        var identity = LIRStackScheduler.Identity(body);
        var operations = identity.Operations.ToArray();
        (operations[0], operations[2]) = (operations[2], operations[0]);
        var invalid = identity with { Operations = operations };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("Data dependency reversed", exception.Message);
        Assert.Contains("temp 0", exception.Message);
    }

    [Fact]
    public void Validate_MissingInstructionOwnership_IsRejected()
    {
        var body = CreateNumericReturnBody();
        var identity = LIRStackScheduler.Identity(body);
        var invalid = identity with
        {
            Operations = identity.Operations[..^1]
        };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("is missing", exception.Message);
    }

    [Fact]
    public void Validate_DuplicateInstructionOwnership_IsRejected()
    {
        var body = CreateNumericReturnBody();
        var identity = LIRStackScheduler.Identity(body);
        var operations = identity.Operations.ToArray();
        operations[^1] = operations[0];
        var invalid = identity with { Operations = operations };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("more than one", exception.Message);
    }

    [Fact]
    public void Validate_StackResidentTempWithoutSingleDefinition_IsRejected()
    {
        var body = new MethodBodyIR();
        var missing = AddTemp(body);
        body.Instructions.Add(new LIRReturn(missing));
        var invalid = WithStackResidents(
            LIRStackScheduler.Identity(body),
            missing);

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("one definition and one use", exception.Message);
    }

    [Fact]
    public void Validate_StackResidentTempCrossingSequencePoint_IsRejected()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new LIRSequencePoint(
            Jroc.DebugSymbols.SourceSpan.Hidden("source.js")));
        body.Instructions.Add(new LIRReturn(value));
        var invalid = WithStackResidents(
            LIRStackScheduler.Identity(body),
            value);

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("expected empty", exception.Message);
        Assert.Contains(nameof(LIRSequencePoint), exception.Message);
    }

    [Fact]
    public void Validate_RegionWithWrongSourceInterval_IsRejected()
    {
        var body = new MethodBodyIR();
        var first = AddTemp(body);
        var second = AddTemp(body);
        body.Instructions.Add(new LIRConstNumber(1, first));
        body.Instructions.Add(new LIRSequencePoint(
            Jroc.DebugSymbols.SourceSpan.Hidden("source.js")));
        body.Instructions.Add(new LIRConstNumber(2, second));
        body.Instructions.Add(new LIRReturn(second));
        var identity = LIRStackScheduler.Identity(body);
        var regions = identity.Regions.ToArray();
        regions[1] = regions[1] with
        {
            SequencePointIndex = -1,
            SourceSpan = null
        };
        var invalid = identity with { Regions = regions };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("outside source interval", exception.Message);
        Assert.Contains("instruction belongs to interval 0", exception.Message);
    }

    [Fact]
    public void Validate_CatchEntry_ConsumesImplicitException()
    {
        var body = new MethodBodyIR();
        var exception = AddTemp(body);
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(new LIRStoreException(exception));
        body.Instructions.Add(new LIRReturn(exception));
        body.ExceptionRegions.Add(new ExceptionRegionInfo(
            ExceptionRegionKind.Catch,
            TryStartLabelId: 1,
            TryEndLabelId: 2,
            HandlerStartLabelId: 10,
            HandlerEndLabelId: 11,
            CatchType: typeof(Exception)));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(1, schedule.MaxStackDepth);
        Assert.Equal(1, schedule.CarriedStackDepthBeforeInstructions[1]);
        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[exception.Index]);
    }

    [Fact]
    public void Validate_CatchEntryWithoutStore_IsRejected()
    {
        var body = new MethodBodyIR();
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());
        body.ExceptionRegions.Add(new ExceptionRegionInfo(
            ExceptionRegionKind.Catch,
            TryStartLabelId: 1,
            TryEndLabelId: 2,
            HandlerStartLabelId: 10,
            HandlerEndLabelId: 11,
            CatchType: typeof(Exception)));

        var unvalidated = CreateUnvalidatedIdentityShape(body);
        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, unvalidated));

        Assert.Contains("expected empty", exception.Message);
    }

    [Fact]
    public void Validate_EffectfulInstructionReordering_IsRejected()
    {
        var body = new MethodBodyIR();
        var first = AddTemp(body);
        var second = AddTemp(body);
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "First",
            System.Array.Empty<TempVariable>(),
            first));
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Second",
            System.Array.Empty<TempVariable>(),
            second));

        var identity = LIRStackScheduler.Identity(body);
        var operations = identity.Operations.Reverse().ToArray();
        var invalid = identity with { Operations = operations };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("Effect order reversed", exception.Message);
    }

    [Fact]
    public void Validate_UndersizedDeclaredMaxStack_IsRejected()
    {
        var body = new MethodBodyIR();
        var elements = Enumerable.Range(0, 3)
            .Select(_ => AddTemp(body))
            .ToArray();
        var array = AddTemp(body);
        foreach (var element in elements)
        {
            body.Instructions.Add(new LIRConstNumber(1, element));
        }
        body.Instructions.Add(new LIRBuildArray(elements, array));
        body.Instructions.Add(new LIRReturn(array));

        var invalid = WithStackResidents(
            LIRStackScheduler.Identity(body),
            elements.Append(array).ToArray()) with
        {
            MaxStackDepth = 2
        };

        var exception = Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduleValidator.ValidateAndAnnotate(body, invalid));

        Assert.Contains("declares max stack 2", exception.Message);
        Assert.Contains("requires 3", exception.Message);
    }

    [Fact]
    public void ValidateOrFallback_InvalidOptimizedPlan_ReturnsAnnotatedIdentity()
    {
        var body = CreateNumericReturnBody();
        var identity = LIRStackScheduler.Identity(body);
        var invalid = identity with
        {
            Mode = LIRStackSchedulerMode.TypedNumeric,
            Operations = identity.Operations[..^1]
        };

        var previousEnabled = IRPipelineMetrics.Enabled;
        try
        {
            IRPipelineMetrics.Enabled = true;
            IRPipelineMetrics.Reset();

            var fallback = LIRStackScheduler.ValidateOrFallback(
                body,
                invalid,
                new LIRStackSchedulerOptions(
                    LIRStackSchedulerMode.TypedNumeric,
                    LIRStackScheduleValidationBehavior.FallbackToIdentity));

            Assert.Equal(LIRStackSchedulerMode.Identity, fallback.Mode);
            Assert.Equal(1, fallback.Metrics.ValidationFallbackCount);
            Assert.NotNull(fallback.ValidationFailureReason);
            Assert.Equal(
                1,
                IRPipelineMetrics.GetStats().SchedulerValidationFallbacks);
        }
        finally
        {
            IRPipelineMetrics.Enabled = previousEnabled;
            IRPipelineMetrics.Reset();
        }
    }

    [Fact]
    public void ValidateOrFallback_StrictMode_Throws()
    {
        var body = CreateNumericReturnBody();
        var identity = LIRStackScheduler.Identity(body);
        var invalid = identity with
        {
            Mode = LIRStackSchedulerMode.TypedNumeric,
            Operations = identity.Operations[..^1]
        };

        Assert.Throws<LIRStackScheduleValidationException>(() =>
            LIRStackScheduler.ValidateOrFallback(
                body,
                invalid,
                new LIRStackSchedulerOptions(
                    LIRStackSchedulerMode.TypedNumeric,
                    LIRStackScheduleValidationBehavior.Throw)));
    }

    private static MethodBodyIR CreateNumericReturnBody()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body);
        var right = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRConstNumber(5, left));
        body.Instructions.Add(new LIRConstNumber(2, right));
        body.Instructions.Add(new LIRSubNumber(left, right, result));
        body.Instructions.Add(new LIRReturn(result));
        return body;
    }

    private static LIRStackSchedule WithStackResidents(
        LIRStackSchedule schedule,
        params TempVariable[] temps)
    {
        var residencies = schedule.TempResidencies.ToArray();
        var owned = schedule.OwnedTemps.ToArray();
        foreach (var temp in temps)
        {
            residencies[temp.Index] = TempResidency.StackResident;
            owned[temp.Index] = true;
        }

        return schedule with
        {
            Mode = LIRStackSchedulerMode.TypedNumeric,
            TempResidencies = residencies,
            OwnedTemps = owned,
            CarriedStackDepthBeforeInstructions =
                new int[schedule.CarriedStackDepthBeforeInstructions.Length],
            MaxStackDepth = 0
        };
    }

    private static LIRStackSchedule CreateUnvalidatedIdentityShape(
        MethodBodyIR body)
    {
        var operations = body.Instructions
            .Select((_, index) => new ScheduledOperation(
                index,
                InstructionCount: 1,
                InstructionDisposition.EmitNormally))
            .ToArray();
        return new LIRStackSchedule(
            LIRStackSchedulerMode.Identity,
            operations,
            System.Array.Empty<ScheduledRegion>(),
            Enumerable.Repeat(
                TempResidency.MaterializedLocal,
                body.Temps.Count).ToArray(),
            new bool[body.Temps.Count],
            Enumerable.Repeat(-1, body.Temps.Count).ToArray(),
            new int[body.Instructions.Count],
            MaxStackDepth: 0,
            new LIRStackScheduleMetrics(
                ScheduledRegionCount: 0,
                StackResidentTempCount: 0,
                EliminatedSpillCount: 0,
                ValidationFallbackCount: 0),
            ValidationFailureReason: null);
    }

    private static TempVariable AddTemp(MethodBodyIR body)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        return temp;
    }
}
