using Jroc.DebugSymbols;
using Jroc.IL;
using Jroc.IR;
using Jroc.Services.TwoPhaseCompilation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Jroc.Tests.Utilities;

namespace Jroc.Tests;

public sealed class LIRStackSchedulerTests
{
    [Fact]
    public void Identity_EmptyBody_ReturnsEmptyNonOwningPlan()
    {
        var schedule = LIRStackScheduler.Identity(new MethodBodyIR());

        Assert.Equal(LIRStackSchedulerMode.Identity, schedule.Mode);
        Assert.Empty(schedule.Operations);
        Assert.Empty(schedule.Regions);
        Assert.Empty(schedule.TempResidencies);
        Assert.Empty(schedule.OwnedTemps);
        Assert.Empty(schedule.EffectiveLastUses);
        Assert.Equal(0, schedule.MaxStackDepth);
        Assert.Equal(default, schedule.Metrics);
    }

    [Fact]
    public void Identity_StraightLineBody_PreservesOrderAndRawLastUses()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body);
        var right = AddTemp(body);
        var result = AddTemp(body);

        body.Instructions.Add(new LIRLoadParameter(1, left));
        body.Instructions.Add(new LIRConstNumber(2, right));
        body.Instructions.Add(new LIRMulNumber(left, right, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(
            new[] { 0, 1, 2, 3 },
            schedule.Operations.Select(operation => operation.StartLirIndex));
        Assert.All(schedule.Operations, operation =>
        {
            Assert.Equal(1, operation.InstructionCount);
            Assert.Equal(InstructionDisposition.EmitNormally, operation.Disposition);
        });
        Assert.Equal(
            new[] { TempResidency.MaterializedLocal, TempResidency.MaterializedLocal, TempResidency.MaterializedLocal },
            schedule.TempResidencies);
        Assert.All(schedule.OwnedTemps, Assert.False);
        Assert.Equal(new[] { 2, 2, 3 }, schedule.EffectiveLastUses);
        var region = Assert.Single(schedule.Regions);
        Assert.Equal(0, region.StartLirIndex);
        Assert.Equal(3, region.EndLirIndexExclusive);
        Assert.Equal(0, region.StartOperationIndex);
        Assert.Equal(3, region.OperationCount);
        Assert.Equal(1, schedule.Metrics.ScheduledRegionCount);
    }

    [Fact]
    public void Identity_ControlFlowBody_PreservesEverySourceIndex()
    {
        var body = new MethodBodyIR();
        var condition = AddTemp(body);
        var value = AddTemp(body);

        body.Instructions.Add(new LIRLoadParameter(1, condition));
        body.Instructions.Add(new LIRBranchIfFalse(condition, 1));
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new LIRReturn(value));
        body.Instructions.Add(new LIRLabel(1));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(
            Enumerable.Range(0, body.Instructions.Count),
            schedule.Operations.Select(operation => operation.StartLirIndex));
        Assert.Collection(
            schedule.Regions,
            region =>
            {
                Assert.Equal(0, region.StartLirIndex);
                Assert.Equal(1, region.EndLirIndexExclusive);
            },
            region =>
            {
                Assert.Equal(2, region.StartLirIndex);
                Assert.Equal(3, region.EndLirIndexExclusive);
            });
    }

    [Fact]
    public void Identity_SequencePointAndScopeCreation_SplitRegions()
    {
        var body = new MethodBodyIR();
        var first = AddTemp(body);
        var second = AddTemp(body);

        body.Instructions.Add(new LIRConstNumber(1, first));
        body.Instructions.Add(new LIRSequencePoint(SourceSpan.Hidden("source.js")));
        body.Instructions.Add(new LIRConstNumber(2, second));
        body.Instructions.Add(new LIRCreateLeafScopeInstance(new ScopeId("block")));
        body.Instructions.Add(new LIRCopyTemp(second, first));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Collection(
            schedule.Regions,
            region =>
            {
                Assert.Equal(0, region.StartLirIndex);
                Assert.Equal(1, region.EndLirIndexExclusive);
                Assert.Equal(-1, region.SequencePointIndex);
                Assert.Null(region.SourceSpan);
            },
            region =>
            {
                Assert.Equal(2, region.StartLirIndex);
                Assert.Equal(3, region.EndLirIndexExclusive);
                Assert.Equal(0, region.SequencePointIndex);
                Assert.Equal(
                    SourceSpan.Hidden("source.js"),
                    region.SourceSpan);
            },
            region =>
            {
                Assert.Equal(4, region.StartLirIndex);
                Assert.Equal(5, region.EndLirIndexExclusive);
                Assert.Equal(0, region.SequencePointIndex);
                Assert.Equal(
                    SourceSpan.Hidden("source.js"),
                    region.SourceSpan);
            });
    }

    [Fact]
    public void Identity_InternalControlFlowAndUnknownInstructions_AreOpaqueBoundaries()
    {
        var body = new MethodBodyIR();
        var exception = AddTemp(body);
        var result = AddTemp(body);
        var value = AddTemp(body);

        body.Instructions.Add(new LIRConstUndefined(exception));
        body.Instructions.Add(new LIRUnwrapCatchException(exception, result));
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new UnknownInstruction());
        body.Instructions.Add(new LIRCopyTemp(value, result));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Collection(
            schedule.Regions,
            region => Assert.Equal((0, 1), (region.StartLirIndex, region.EndLirIndexExclusive)),
            region => Assert.Equal((2, 3), (region.StartLirIndex, region.EndLirIndexExclusive)),
            region => Assert.Equal((4, 5), (region.StartLirIndex, region.EndLirIndexExclusive)));
    }

    [Fact]
    public void Identity_ExceptionOperands_ContributeToRawLastUses()
    {
        var body = new MethodBodyIR();
        var exception = AddTemp(body);
        var value = AddTemp(body);

        body.Instructions.Add(new LIRUnwrapCatchException(exception, value));
        body.Instructions.Add(new LIRThrow(value));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(new[] { 0, 1 }, schedule.EffectiveLastUses);
    }

    [Fact]
    public void Identity_IntrinsicConstructorFieldStore_GroupsAtomicFusionCandidate()
    {
        var body = new MethodBodyIR();
        var result = AddTemp(body);

        body.Instructions.Add(new LIRNewIntrinsicObject(
            "Int32Array",
            System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Example",
            "buffer",
            IsPrivateField: false,
            result));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Collection(
            schedule.Operations,
            operation =>
            {
                Assert.Equal(0, operation.StartLirIndex);
                Assert.Equal(2, operation.InstructionCount);
                Assert.Equal(InstructionDisposition.FusedIntoEmissionUnit, operation.Disposition);
                Assert.Equal(0, operation.GetLirInstructionIndex(0));
                Assert.Equal(1, operation.GetLirInstructionIndex(1));
            },
            operation =>
            {
                Assert.Equal(2, operation.StartLirIndex);
                Assert.Equal(1, operation.InstructionCount);
                Assert.Equal(InstructionDisposition.EmitNormally, operation.Disposition);
            });
    }

    [Fact]
    public void Identity_UserConstructorFieldStore_GroupsOnlyEligibleStructuralCandidate()
    {
        var body = new MethodBodyIR();
        var newTarget = AddTemp(body);
        var result = AddTemp(body);
        var callable = new CallableId
        {
            Kind = CallableKind.ClassConstructor,
            DeclaringScopeName = "module",
            Name = "Child",
            JsParamCount = 0
        };

        body.Instructions.Add(new LIRNewUserClass(
            "Child",
            "Child",
            callable,
            NewTarget: newTarget,
            NeedsScopes: false,
            ScopesArray: null,
            MinArgCount: 0,
            MaxArgCount: 0,
            IsDerivedConstructor: false,
            ParameterClrTypes: System.Array.Empty<Type?>(),
            Arguments: System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Parent",
            "child",
            IsPrivateField: false,
            result));

        var schedule = LIRStackScheduler.Identity(body);

        var operation = Assert.Single(schedule.Operations);
        Assert.Equal(2, operation.InstructionCount);
        Assert.Equal(InstructionDisposition.FusedIntoEmissionUnit, operation.Disposition);
    }

    [Fact]
    public void Identity_DerivedUserConstructor_DoesNotGroupFusionCandidate()
    {
        var body = new MethodBodyIR();
        var newTarget = AddTemp(body);
        var result = AddTemp(body);
        var callable = new CallableId
        {
            Kind = CallableKind.ClassConstructor,
            DeclaringScopeName = "module",
            Name = "Derived",
            JsParamCount = 0
        };

        body.Instructions.Add(new LIRNewUserClass(
            "Derived",
            "Derived",
            callable,
            NewTarget: newTarget,
            NeedsScopes: false,
            ScopesArray: null,
            MinArgCount: 0,
            MaxArgCount: 0,
            IsDerivedConstructor: true,
            ParameterClrTypes: System.Array.Empty<Type?>(),
            Arguments: System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Parent",
            "child",
            IsPrivateField: false,
            result));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(2, schedule.Operations.Length);
        Assert.All(
            schedule.Operations,
            operation => Assert.Equal(InstructionDisposition.EmitNormally, operation.Disposition));
    }

    [Fact]
    public void Build_DisabledMode_RejectsScheduleConstruction()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            LIRStackScheduler.Build(
                new MethodBodyIR(),
                new LIRStackSchedulerOptions(LIRStackSchedulerMode.Disabled)));

        Assert.Contains("bypasses schedule construction", exception.Message);
    }

    [Fact]
    public void Build_TypedNumericMode_ClaimsLeftAssociativeChain()
    {
        var body = new MethodBodyIR();
        var factor = AddTemp(body);
        var two = AddTemp(body);
        var product = AddTemp(body);
        var one = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, factor));
        body.Instructions.Add(new LIRConstNumber(2, two));
        body.Instructions.Add(new LIRMulNumber(factor, two, product));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddNumber(product, one, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedNumeric));

        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[product.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[result.Index]);
        Assert.True(schedule.OwnedTemps[product.Index]);
        Assert.True(schedule.OwnedTemps[result.Index]);
        Assert.Equal(2, schedule.Metrics.StackResidentTempCount);
        Assert.Equal(2, schedule.Metrics.EliminatedSpillCount);
        Assert.Equal(1, schedule.MaxStackDepth);
        Assert.Equal(1, schedule.CarriedStackDepthBeforeInstructions[3]);
        Assert.Equal(1, schedule.CarriedStackDepthBeforeInstructions[4]);
        Assert.Equal(1, schedule.CarriedStackDepthBeforeInstructions[5]);
    }

    [Fact]
    public void Build_TypedNumericMode_ClaimsIndependentProductsInOperandOrder()
    {
        var body = new MethodBodyIR();
        var a = AddTemp(body);
        var b = AddTemp(body);
        var left = AddTemp(body);
        var c = AddTemp(body);
        var d = AddTemp(body);
        var right = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, a));
        body.Instructions.Add(new LIRLoadParameter(2, b));
        body.Instructions.Add(new LIRMulNumber(a, b, left));
        body.Instructions.Add(new LIRLoadParameter(3, c));
        body.Instructions.Add(new LIRLoadParameter(4, d));
        body.Instructions.Add(new LIRMulNumber(c, d, right));
        body.Instructions.Add(new LIRAddNumber(left, right, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedNumeric));

        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[left.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[right.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[result.Index]);
        Assert.Equal(3, schedule.Metrics.StackResidentTempCount);
        Assert.Equal(2, schedule.MaxStackDepth);
        Assert.Equal(2, schedule.CarriedStackDepthBeforeInstructions[6]);
    }

    [Fact]
    public void Build_TypedNumericMode_DoesNotClaimCallResultsOrCrossCall()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body);
        var right = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Left",
            System.Array.Empty<TempVariable>(),
            left));
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Right",
            System.Array.Empty<TempVariable>(),
            right));
        body.Instructions.Add(new LIRAddNumber(left, right, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedNumeric));

        Assert.Equal(TempResidency.MaterializedLocal, schedule.TempResidencies[left.Index]);
        Assert.Equal(TempResidency.MaterializedLocal, schedule.TempResidencies[right.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[result.Index]);
        Assert.Equal(1, schedule.Metrics.StackResidentTempCount);
    }

    [Fact]
    public void Build_TypedNumericMode_DoesNotClaimValueAcrossSequencePoint()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body);
        var right = AddTemp(body);
        var result = AddTemp(body);
        var one = AddTemp(body);
        var final = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, left));
        body.Instructions.Add(new LIRLoadParameter(2, right));
        body.Instructions.Add(new LIRMulNumber(left, right, result));
        body.Instructions.Add(new LIRSequencePoint(
            SourceSpan.Hidden("scheduler.js")));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddNumber(result, one, final));
        body.Instructions.Add(new LIRReturn(final));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedNumeric));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[result.Index]);
        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[final.Index]);
    }

    [Fact]
    public void Build_TypedComparisonsMode_ClaimsNumericTreeAndBranchComparison()
    {
        var body = new MethodBodyIR();
        var a = AddTemp(body);
        var two = AddTemp(body);
        var product = AddTemp(body);
        var b = AddTemp(body);
        var one = AddTemp(body);
        var sum = AddTemp(body);
        var comparison = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, a));
        body.Instructions.Add(new LIRConstNumber(2, two));
        body.Instructions.Add(new LIRMulNumber(a, two, product));
        body.Instructions.Add(new LIRLoadParameter(2, b));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddNumber(b, one, sum));
        body.Instructions.Add(new LIRCompareNumberLessThan(product, sum, comparison));
        body.Instructions.Add(new LIRBranchIfFalse(comparison, 1));
        body.Instructions.Add(new LIRLabel(1));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedComparisons));

        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[product.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[sum.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[comparison.Index]);
        Assert.Equal(3, schedule.Metrics.StackResidentTempCount);
        Assert.Equal(2, schedule.MaxStackDepth);
        Assert.Equal(1, schedule.CarriedStackDepthBeforeInstructions[7]);
    }

    [Fact]
    public void Build_TypedComparisonsMode_DoesNotClaimCallOperands()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body);
        var right = AddTemp(body);
        var comparison = AddTemp(body);
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Left", System.Array.Empty<TempVariable>(), left));
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Right", System.Array.Empty<TempVariable>(), right));
        body.Instructions.Add(new LIRCompareNumberLessThan(left, right, comparison));
        body.Instructions.Add(new LIRReturn(comparison));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedComparisons));

        Assert.Equal(TempResidency.MaterializedLocal, schedule.TempResidencies[left.Index]);
        Assert.Equal(TempResidency.MaterializedLocal, schedule.TempResidencies[right.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[comparison.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_ClaimsLengthAndNumericConversion()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var length = AddTemp(body);
        var converted = AddTemp(body);
        var one = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, receiver));
        body.Instructions.Add(new LIRGetStringLength(receiver, length));
        body.Instructions.Add(new LIRConvertToNumber(length, converted));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddNumber(converted, one, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[length.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[converted.Index]);
        Assert.Equal(TempResidency.StackResident, schedule.TempResidencies[result.Index]);
        Assert.Equal(3, schedule.Metrics.StackResidentTempCount);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotClaimGenericGetter()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var index = AddTemp(body);
        var value = AddTemp(body);
        var one = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, receiver));
        body.Instructions.Add(new LIRConstString("x", index));
        body.Instructions.Add(new LIRGetItem(receiver, index, value));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddDynamic(value, one, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(TempResidency.MaterializedLocal, schedule.TempResidencies[value.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryValueIntoElidedConsumer()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body);
        var converted = AddTemp(body);
        var one = AddTemp(body);
        var unusedResult = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, source));
        body.Instructions.Add(new LIRConvertToNumber(source, converted));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRSubNumber(converted, one, unusedResult));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[converted.Index]);
        Assert.False(schedule.OwnedTemps[converted.Index]);
        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[unusedResult.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_CarriesScopeReadIntoReceiverConsumer()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadScopeFieldByName(
            "GlobalScope",
            "console",
            receiver));
        body.Instructions.Add(new LIRCallMember0(
            receiver,
            "log",
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[receiver.Index]);
        Assert.True(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_CarriesScopeReadIntoObjectCoercion()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadScopeFieldByName(
            "GlobalScope",
            "performance",
            receiver));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "ObjectRuntime",
            "RequireObjectCoercible",
            new[] { receiver },
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[receiver.Index]);
        Assert.True(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryScopeReadIntoOtherIntrinsic()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadScopeFieldByName(
            "GlobalScope",
            "performance",
            receiver));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "ObjectRuntime",
            "Other",
            new[] { receiver },
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryScopeReadAsLaterOperand()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var argument = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, receiver));
        body.Instructions.Add(new LIRLoadScopeFieldByName(
            "GlobalScope",
            "value",
            argument));
        body.Instructions.Add(new LIRCallMember1(
            receiver,
            "use",
            argument,
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[argument.Index]);
        Assert.False(schedule.OwnedTemps[argument.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_CarriesStableArrayLoadIntoGenericGetItemReceiver()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[receiver.Index]);
        Assert.True(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryMultiUseStableArrayLoad()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var property = AddTemp(body);
        var first = AddTemp(body);
        var second = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, first));
        body.Instructions.Add(new LIRGetItem(receiver, property, second));
        body.Instructions.Add(new LIRReturn(second));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryVariableSlotStableArrayLoad()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.TempVariableSlots.AddRange(
            Enumerable.Repeat(-1, body.Temps.Count));
        body.TempVariableSlots[receiver.Index] = 0;
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryStableArrayLoadAcrossScopeReplacement()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(
            new LIRCreateLeafScopeInstance(new ScopeId("block")));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryStableArrayLoadAcrossSequencePoint()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(new LIRSequencePoint(
            SourceSpan.Hidden("stable-load.js")));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryStableArrayLoadAcrossControlFlow()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(new LIRBranch(1));
        body.Instructions.Add(new LIRLabel(1));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryStableArrayLoadAcrossEhEntry()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(body);
        var index = AddTemp(body);
        var receiver = AddTemp(body);
        var exception = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, array));
        body.Instructions.Add(new LIRLoadParameter(2, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, receiver));
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(new LIRStoreException(exception));
        body.Instructions.Add(new LIRConstString("value", property));
        body.Instructions.Add(new LIRGetItem(receiver, property, result));
        body.Instructions.Add(new LIRReturn(result));
        body.ExceptionRegions.Add(new ExceptionRegionInfo(
            Jroc.IR.ExceptionRegionKind.Catch,
            TryStartLabelId: 1,
            TryEndLabelId: 2,
            HandlerStartLabelId: 10,
            HandlerEndLabelId: 11,
            CatchType: typeof(Exception)));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[receiver.Index]);
        Assert.False(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_ConversionsMode_DoesNotCarryStableArrayLoadAsGetItemIndex()
    {
        var body = new MethodBodyIR();
        var target = AddTemp(body);
        var array = AddTemp(body);
        var index = AddTemp(body);
        var property = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, target));
        body.Instructions.Add(new LIRLoadParameter(2, array));
        body.Instructions.Add(new LIRLoadParameter(3, index));
        body.Instructions.Add(new LIRGetJsArrayElement(array, index, property));
        body.Instructions.Add(new LIRGetItem(target, property, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.ConversionsAndStableLoads));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[property.Index]);
        Assert.False(schedule.OwnedTemps[property.Index]);
    }

    [Fact]
    public void Build_CallResultsMode_CarriesCallResultIntoReceiverConsumer()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var receiver = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, value));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "ObjectRuntime",
            "RequireObjectCoercible",
            new[] { value },
            receiver));
        body.Instructions.Add(new LIRCallMember0(
            receiver,
            "toString",
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[receiver.Index]);
        Assert.True(schedule.OwnedTemps[receiver.Index]);
    }

    [Fact]
    public void Build_LiteralMode_MovesConstructionBeforePureProducerSuffix()
    {
        var body = new MethodBodyIR();
        var a = AddTemp(body);
        var two = AddTemp(body);
        var left = AddTemp(body);
        var b = AddTemp(body);
        var three = AddTemp(body);
        var right = AddTemp(body);
        var array = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, a));
        body.Instructions.Add(new LIRConstNumber(2, two));
        body.Instructions.Add(new LIRMulNumber(a, two, left));
        body.Instructions.Add(new LIRLoadParameter(2, b));
        body.Instructions.Add(new LIRConstNumber(3, three));
        body.Instructions.Add(new LIRAddNumber(b, three, right));
        body.Instructions.Add(new LIRNewJsArray(
            new[] { left, right },
            array));
        body.Instructions.Add(new LIRReturn(array));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.LiteralAndArguments));

        Assert.Equal(6, schedule.Operations[0].StartLirIndex);
        Assert.Equal(
            TempResidency.ScheduledInline,
            schedule.TempResidencies[left.Index]);
        Assert.Equal(
            TempResidency.ScheduledInline,
            schedule.TempResidencies[right.Index]);
        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[array.Index]);
        Assert.Equal(1, schedule.MaxStackDepth);
        Assert.Equal(1, schedule.CarriedStackDepthBeforeInstructions[7]);
    }

    [Fact]
    public void Build_LiteralMode_DoesNotMoveConstructionAcrossEffectfulElement()
    {
        var body = new MethodBodyIR();
        var effectful = AddTemp(body);
        var one = AddTemp(body);
        var value = AddTemp(body);
        var array = AddTemp(body);
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Effect",
            System.Array.Empty<TempVariable>(),
            effectful));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddNumber(effectful, one, value));
        body.Instructions.Add(new LIRNewJsArray(
            new[] { value },
            array));
        body.Instructions.Add(new LIRReturn(array));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.LiteralAndArguments));

        Assert.Equal(
            Enumerable.Range(0, body.Instructions.Count),
            schedule.Operations.Select(operation => operation.StartLirIndex));
        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[array.Index]);
    }

    [Fact]
    public void Build_CallResultsMode_CarriesIntrinsicCallsInSourceOrder()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var floor = AddTemp(body);
        var sqrt = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, value));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { value },
            floor));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "sqrt",
            new[] { value },
            sqrt));
        body.Instructions.Add(new LIRAddNumber(floor, sqrt, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[floor.Index]);
        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[sqrt.Index]);
        Assert.Equal(2, schedule.CarriedStackDepthBeforeInstructions[3]);
        Assert.Equal(2, schedule.MaxStackDepth);
    }

    [Fact]
    public void Build_CallResultsMode_InlinesCallsIntoParamsArrayInOrder()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var floor = AddTemp(body);
        var sqrt = AddTemp(body);
        var maximum = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, value));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { value },
            floor));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "sqrt",
            new[] { value },
            sqrt));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "max",
            new[] { floor, sqrt },
            maximum));
        body.Instructions.Add(new LIRReturn(maximum));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            new[] { 0, 3, 1, 2, 4 },
            schedule.Operations.Select(operation => operation.StartLirIndex));
        Assert.Equal(
            TempResidency.ScheduledInline,
            schedule.TempResidencies[floor.Index]);
        Assert.Equal(
            TempResidency.ScheduledInline,
            schedule.TempResidencies[sqrt.Index]);
        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[maximum.Index]);
        Assert.Equal(1, schedule.MaxStackDepth);
    }

    [Fact]
    public void Build_CallResultsMode_DoesNotClaimMultiUseCall()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var call = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, value));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { value },
            call));
        body.Instructions.Add(new LIRAddNumber(call, call, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[call.Index]);
        Assert.False(schedule.OwnedTemps[call.Index]);
    }

    [Fact]
    public void Build_CallResultsMode_DoesNotCarryAcrossSequencePoint()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var call = AddTemp(body);
        var one = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, value));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { value },
            call));
        body.Instructions.Add(new LIRSequencePoint(
            SourceSpan.Hidden("calls.js")));
        body.Instructions.Add(new LIRConstNumber(1, one));
        body.Instructions.Add(new LIRAddNumber(call, one, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[call.Index]);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Build_CallResultsMode_DoesNotClaimAsyncOrGeneratorCalls(
        bool isAsync,
        bool isGenerator)
    {
        var body = new MethodBodyIR
        {
            IsAsync = isAsync,
            IsGenerator = isGenerator
        };
        var value = AddTemp(body);
        var call = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, value));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { value },
            call));
        body.Instructions.Add(new LIRReturn(call));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[call.Index]);
    }

    [Fact]
    public void Build_CallResultsMode_ClaimsImmediateTypedFluentCalls()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var first = AddTemp(body);
        var second = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, receiver));
        body.Instructions.Add(new LIRCallTypedMember(
            receiver,
            default,
            default,
            HasScopesParameter: false,
            typeof(object),
            MaxParamCount: 0,
            System.Array.Empty<Type?>(),
            System.Array.Empty<TempVariable>(),
            first));
        body.Instructions.Add(new LIRCallTypedMember(
            first,
            default,
            default,
            HasScopesParameter: false,
            typeof(object),
            MaxParamCount: 0,
            System.Array.Empty<Type?>(),
            System.Array.Empty<TempVariable>(),
            second));
        body.Instructions.Add(new LIRReturn(second));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.CallResults));

        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[first.Index]);
        Assert.Equal(
            TempResidency.StackResident,
            schedule.TempResidencies[second.Index]);
    }

    [Fact]
    public void Build_GeneralRegionsMode_SchedulesNestedConstructionAndCalls()
    {
        var body = new MethodBodyIR();
        var a = AddTemp(body);
        var two = AddTemp(body);
        var doubled = AddTemp(body);
        var b = AddTemp(body);
        var three = AddTemp(body);
        var added = AddTemp(body);
        var array = AddTemp(body);
        var aForFloor = AddTemp(body);
        var floor = AddTemp(body);
        var bForSqrt = AddTemp(body);
        var sqrt = AddTemp(body);
        var sum = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, a));
        body.Instructions.Add(new LIRConstNumber(2, two));
        body.Instructions.Add(new LIRMulNumber(a, two, doubled));
        body.Instructions.Add(new LIRLoadParameter(2, b));
        body.Instructions.Add(new LIRConstNumber(3, three));
        body.Instructions.Add(new LIRAddNumber(b, three, added));
        body.Instructions.Add(new LIRNewJsArray(
            new[] { doubled, added },
            array));
        body.Instructions.Add(new LIRLoadParameter(1, aForFloor));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { aForFloor },
            floor));
        body.Instructions.Add(new LIRLoadParameter(2, bForSqrt));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "sqrt",
            new[] { bForSqrt },
            sqrt));
        body.Instructions.Add(new LIRAddNumber(floor, sqrt, sum));
        body.Instructions.Add(new LIRNewJsObject(
            new[]
            {
                new ObjectProperty("x", array),
                new ObjectProperty("y", sum)
            },
            result));
        body.Instructions.Add(new LIRReturn(result));

        var first = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));
        var second = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(
            first.Operations,
            second.Operations);
        Assert.True(
            System.Array.FindIndex(
                first.Operations,
                operation => operation.StartLirIndex == 12)
            < System.Array.FindIndex(
                first.Operations,
                operation => operation.StartLirIndex == 0));
        Assert.Equal(
            TempResidency.ScheduledInline,
            first.TempResidencies[array.Index]);
        Assert.Equal(
            TempResidency.ScheduledInline,
            first.TempResidencies[floor.Index]);
        Assert.Equal(
            TempResidency.ScheduledInline,
            first.TempResidencies[sqrt.Index]);
        Assert.Equal(
            TempResidency.StackResident,
            first.TempResidencies[result.Index]);
        Assert.Equal(1, first.Metrics.CandidateRegionCount);
        Assert.Equal(1, first.Metrics.AcceptedRegionCount);
        Assert.Equal(0, first.Metrics.RejectedRegionCount);
        Assert.True(first.Metrics.EliminatedSpillCount >= 5);
        Assert.Equal(0, first.Metrics.ValidationFallbackCount);
    }

    [Fact]
    public void Build_GeneralRegionsMode_RejectsEffectOrderReversal()
    {
        var body = new MethodBodyIR();
        var firstReceiver = AddTemp(body);
        var first = AddTemp(body);
        var secondReceiver = AddTemp(body);
        var second = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, firstReceiver));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "floor",
            new[] { firstReceiver },
            first));
        body.Instructions.Add(new LIRLoadParameter(1, secondReceiver));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Math",
            "sqrt",
            new[] { secondReceiver },
            second));
        body.Instructions.Add(new LIRNewJsArray(
            new[] { second, first },
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[result.Index]);
        Assert.Equal(1, schedule.Metrics.CandidateRegionCount);
        Assert.Equal(1, schedule.Metrics.AcceptedRegionCount);
        Assert.Equal(0, schedule.Metrics.RejectedRegionCount);
        Assert.Equal(1, schedule.Metrics.RejectedEffectOrderCount);
    }

    [Fact]
    public void Build_GeneralRegionsMode_PreservesAtomicFusionOperations()
    {
        var body = new MethodBodyIR();
        var result = AddTemp(body);
        body.Instructions.Add(new LIRNewIntrinsicObject(
            "Int32Array",
            System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Example",
            "buffer",
            IsPrivateField: false,
            result));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Contains(
            schedule.Operations,
            operation => operation is
            {
                StartLirIndex: 0,
                InstructionCount: 2,
                Disposition: InstructionDisposition.FusedIntoEmissionUnit
            });
    }

    [Fact]
    public void Build_GeneralRegionsMode_MaterializesConstructionForImplicitReceiverStore()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body);
        var array = AddTemp(body);
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new LIRNewJsArray(
            new[] { value },
            array));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Example",
            "items",
            IsPrivateField: false,
            array));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[array.Index]);
        Assert.False(schedule.OwnedTemps[array.Index]);
    }

    [Fact]
    public void Build_GeneralRegionsMode_InlinesAdjacentCallArgumentsArray()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var argument = AddTemp(body);
        var arguments = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, receiver));
        body.Instructions.Add(new LIRLoadParameter(2, argument));
        body.Instructions.Add(new LIRBuildArray(
            new[] { argument },
            arguments));
        body.Instructions.Add(new LIRCallMember(
            receiver,
            "invoke",
            arguments,
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(
            TempResidency.ScheduledInline,
            schedule.TempResidencies[arguments.Index]);
        Assert.True(schedule.OwnedTemps[arguments.Index]);
    }

    [Fact]
    public void Build_GeneralRegionsMode_DoesNotMoveArgumentsAcrossInstruction()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body);
        var argument = AddTemp(body);
        var arguments = AddTemp(body);
        var intervening = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, receiver));
        body.Instructions.Add(new LIRLoadParameter(2, argument));
        body.Instructions.Add(new LIRBuildArray(
            new[] { argument },
            arguments));
        body.Instructions.Add(new LIRConstNumber(1, intervening));
        body.Instructions.Add(new LIRCallMember(
            receiver,
            "invoke",
            arguments,
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[arguments.Index]);
        Assert.False(schedule.OwnedTemps[arguments.Index]);
    }

    [Fact]
    public void Build_GeneralRegionsMode_InlinesSingleUseScopesArray()
    {
        var body = new MethodBodyIR();
        var callee = AddTemp(body);
        var scopes = AddTemp(body);
        var result = AddTemp(body);
        body.Instructions.Add(new LIRLoadParameter(1, callee));
        body.Instructions.Add(new LIRBuildScopesArray(
            System.Array.Empty<ScopeSlotSource>(),
            scopes));
        body.Instructions.Add(new LIRCallFunctionValue0(
            callee,
            scopes,
            result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(
            TempResidency.ScheduledInline,
            schedule.TempResidencies[scopes.Index]);
        Assert.True(schedule.OwnedTemps[scopes.Index]);
    }

    [Fact]
    public void Build_GeneralRegionsMode_DeepRejectedTreesFailClosedIteratively()
    {
        const int depth = 2_000;
        var body = new MethodBodyIR();
        var current = AddTemp(body);
        body.Instructions.Add(new LIRCallRuntimeServicesStatic(
            "Effect",
            System.Array.Empty<TempVariable>(),
            current));
        for (var index = 0; index < depth; index++)
        {
            var next = AddTemp(body);
            body.Instructions.Add(new LIRNewJsArray(
                new[] { current },
                next));
            current = next;
        }
        body.Instructions.Add(new LIRReturn(current));

        var schedule = LIRStackScheduler.Build(
            body,
            new LIRStackSchedulerOptions(
                LIRStackSchedulerMode.GeneralRegions));

        Assert.Equal(LIRStackSchedulerMode.GeneralRegions, schedule.Mode);
        Assert.Equal(0, schedule.Metrics.ValidationFallbackCount);
        Assert.True(schedule.Metrics.RejectedDependencyCount > 0);
        Assert.Equal(
            TempResidency.MaterializedLocal,
            schedule.TempResidencies[current.Index]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Compiler_IdentityMode_ProducesByteIdenticalArtifactToDisabledMode(bool emitPdb)
    {
        const string source = """
            "use strict";
            function numeric(a, b) { return a * 2 + b; }
            function control(a) { if (a) return 1; return 2; }
            function call(a) { return Math.floor(a); }
            function eh(a) {
              try { return a + 1; }
              catch (e) { return 0; }
              finally { Math.floor(a); }
            }
            function* gen(a) { yield a + 1; return a + 2; }
            async function af(a) { return (await a) + 1; }
            class Bar { constructor(value) { this.value = value; } }
            class Foo {
              constructor() {
                this.bar = new Bar(5);
                this.buffer = new Int32Array(3);
              }
            }
            class Getter { get value() { return 7; } }
            new Foo();
            Math.floor(1.5);
            new Getter().value;
            +true;
            console.log(numeric(4, 3), control(0), call(3.5));
            """;

        var disabled = Compile(source, LIRStackSchedulerMode.Disabled, emitPdb);
        var identity = Compile(source, LIRStackSchedulerMode.Identity, emitPdb);

        AssertEquivalentMethodBodies(disabled.PeBytes, identity.PeBytes);
        AssertEquivalentPortablePdb(disabled.PdbBytes, identity.PdbBytes);
    }

    [Fact]
    public void Compiler_ConversionsMode_EliminatesIntermediateSpills()
    {
        const string source = """
            "use strict";
            function mulAdd(factor) { return factor * 2 + 1; }
            function addChain(a, b, c, d) { return a + b + c + d; }
            function tree(a, b, c, d) { return a * b + c * d; }
            function subChain(a, b, c) { return a - b - c; }
            function divMod(a, b, c) { return a / b % c; }
            function expAdd(a, b, c) { return a ** b + c; }
            function less(a, b) { return a < b; }
            function branch(a, b) { if (a * 2 < b + 1) return a; return b; }
            function concat() { return "value=" + "6"; }
            function stringLength(a) { return a.length + 1; }
            function arrayLength(a) { return a.length + 1; }
            function arrayElement(a) { return a[0]; }
            console.log(mulAdd(4), addChain(1, 2, 3, 4), tree(2, 3, 4, 5),
              subChain(10, 3, 2), divMod(20, 3, 2), expAdd(2, 3, 1),
              less(1, 2), branch(2, 10), concat(), stringLength("abc"),
              arrayLength([1, 2, 3]), arrayElement([4]));
            """;

        var artifact = Compile(
            source,
            LIRStackSchedulerMode.ConversionsAndStableLoads,
            emitPdb: false);
        var path = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "TypedNumericScheduler",
            "typed-numeric.dll");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, artifact.PeBytes);
        var il = AssemblyToText.ConvertToText(path);

        AssertMethodHasNoIntermediateSpills(
            il,
            "mulAdd",
            "ldarg.1",
            "ldc.r8 2",
            "mul",
            "ldc.r8 1",
            "add");
        AssertMethodHasNoIntermediateSpills(
            il,
            "addChain",
            "ldarg.1",
            "ldarg.2",
            "add",
            "ldarg.3",
            "add",
            "ldarg.s d",
            "add");
        AssertMethodHasNoIntermediateSpills(
            il,
            "tree",
            "ldarg.1",
            "ldarg.2",
            "mul",
            "ldarg.3",
            "ldarg.s d",
            "mul",
            "add");
        AssertMethodHasNoIntermediateSpills(il, "subChain", "sub");
        AssertMethodHasNoIntermediateSpills(il, "divMod", "div", "rem");
        AssertMethodHasNoIntermediateSpills(
            il,
            "expAdd",
            "System.Math::Pow",
            "add");
        AssertMethodHasNoIntermediateSpills(
            il,
            "less",
            "ldarg.1",
            "ldarg.2",
            "clt",
            "box");
        AssertMethodHasNoIntermediateSpills(
            il,
            "branch",
            "ldarg.1",
            "ldc.r8 2",
            "mul",
            "ldarg.2",
            "ldc.r8 1",
            "add",
            "clt",
            "brfalse");
        AssertMethodHasNoIntermediateSpills(
            il,
            "concat",
            "ldstr \"value=\"",
            "ldstr \"6\"",
            "String::Concat",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "stringLength",
            "String::get_Length",
            "conv.r8",
            "add",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "arrayLength",
            "JavaScriptRuntime.Array::get_length",
            "add",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "arrayElement",
            "JavaScriptRuntime.Array::get_Item",
            "ret");
    }

    [Fact]
    public void Compiler_LiteralMode_EliminatesPositionalSpills()
    {
        const string source = """
            "use strict";
            function args(a, b) { return Math.max(a * 2, b + 3); }
            function arrayValue(a, b) { return [a * 2, b + 3]; }
            function objectValue(a, b) { return { x: a * 2, y: b + 3 }; }
            function deepArray(a, b, c, d, e, f, g, h, i) {
              return [a + (b + (c + (d + (e + (f + (g + (h + i)))))))];
            }
            console.log(args(2, 4), arrayValue(2, 4), objectValue(2, 4),
              deepArray(1, 2, 3, 4, 5, 6, 7, 8, 9));
            """;

        var artifact = Compile(
            source,
            LIRStackSchedulerMode.LiteralAndArguments,
            emitPdb: false);
        var path = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "LiteralScheduler",
            "literal-scheduler.dll");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, artifact.PeBytes);
        var il = AssemblyToText.ConvertToText(path);

        AssertMethodHasOrderedFragmentsAndLocalCount(
            il,
            "args",
            1,
            "newarr",
            "ldarg.1",
            "mul",
            "stelem.ref",
            "ldarg.2",
            "add",
            "stelem.ref",
            "JavaScriptRuntime.Math::max");
        AssertMethodHasNoIntermediateSpills(
            il,
            "arrayValue",
            "JavaScriptRuntime.Array::.ctor",
            "ldarg.1",
            "mul",
            "JavaScriptRuntime.Array::AddNumber",
            "ldarg.2",
            "add",
            "JavaScriptRuntime.Array::AddNumber",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "objectValue",
            "CreateObjectLiteral",
            "ldstr \"x\"",
            "ldarg.2",
            "mul",
            "SetNumber",
            "ldstr \"y\"",
            "ldarg.3",
            "add",
            "SetNumber",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "deepArray",
            ".maxstack 11",
            "JavaScriptRuntime.Array::.ctor",
            "add",
            "JavaScriptRuntime.Array::AddNumber",
            "ret");
    }

    [Fact]
    public void Compiler_CallResultsMode_EliminatesSingleUseCallSpills()
    {
        const string source = """
            "use strict";
            function math(a) { return Math.floor(a) + Math.sqrt(a); }
            function nestedCall(a) { return Math.abs(Math.floor(a)); }
            function compareCall(a, b) { return Math.floor(a) < Math.ceil(b); }
            function argumentCall(a) { return Math.max(Math.floor(a), Math.sqrt(a)); }
            console.log(math(9), nestedCall(-2.7), compareCall(1.2, 1.1), argumentCall(9));
            """;

        var artifact = Compile(
            source,
            LIRStackSchedulerMode.CallResults,
            emitPdb: false);
        var path = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "CallResultScheduler",
            "call-results.dll");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, artifact.PeBytes);
        var il = AssemblyToText.ConvertToText(path);

        AssertMethodHasNoIntermediateSpills(
            il,
            "math",
            "JavaScriptRuntime.Math::floor",
            "JavaScriptRuntime.Math::sqrt",
            "add",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "nestedCall",
            "JavaScriptRuntime.Math::floor",
            "JavaScriptRuntime.Math::abs",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "compareCall",
            "JavaScriptRuntime.Math::floor",
            "JavaScriptRuntime.Math::ceil",
            "clt",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "argumentCall",
            "newarr",
            "JavaScriptRuntime.Math::floor",
            "stelem.ref",
            "JavaScriptRuntime.Math::sqrt",
            "stelem.ref",
            "JavaScriptRuntime.Math::max",
            "ret");
    }

    [Fact]
    public void Compiler_ImmutableBinaryLeftSkipsSnapshotButMutableLeftPreservesIt()
    {
        const string source = """
            "use strict";
            function immutable(value, other) {
              const left = value;
              return left - other;
            }
            function mutable(value) {
              let left = value;
              return left - (left = 1);
            }
            console.log(immutable(5, 2), mutable(5));
            """;

        var artifact = Compile(
            source,
            LIRStackSchedulerMode.GeneralRegions,
            emitPdb: false);
        var path = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "ImmutableBinaryLeft",
            "immutable-binary-left.dll");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, artifact.PeBytes);
        var il = AssemblyToText.ConvertToText(path);

        AssertMethodHasOrderedFragmentsAndLocalCount(
            il,
            "immutable",
            1,
            "ldarg.1",
            "stloc",
            "ldloc",
            "ldarg.2",
            "sub",
            "ret");

        var mutableStart = il.IndexOf(
            "beforefieldinit mutable",
            StringComparison.Ordinal);
        Assert.True(mutableStart >= 0);
        var mutableEnd = il.IndexOf(
            "end of class mutable",
            mutableStart,
            StringComparison.Ordinal);
        Assert.True(mutableEnd > mutableStart);
        var mutableMethod = il[mutableStart..mutableEnd];
        Assert.True(CountOccurrences(mutableMethod, "stloc") > 1);
        Assert.True(CountOccurrences(mutableMethod, "ldloc") > 1);
    }

    [Fact]
    public void Compiler_GeneralRegionsMode_EliminatesNestedResidualSpills()
    {
        const string source = """
            "use strict";
            function mixed(a, b, c, d) {
              return Math.max((a + 1) * (b - 2), (c + 3) / (d + 4));
            }
            function nestedValue(a, b) {
              return { x: [a * 2, b + 3], y: Math.floor(a) + Math.sqrt(b) };
            }
            console.log(mixed(2, 4, 6, 5), nestedValue(2, 4));
            """;

        var artifact = Compile(
            source,
            LIRStackSchedulerMode.GeneralRegions,
            emitPdb: false);
        var path = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "GeneralRegionScheduler",
            "general-regions.dll");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, artifact.PeBytes);
        var il = AssemblyToText.ConvertToText(path);

        AssertMethodHasNoIntermediateSpills(
            il,
            "mixed",
            "newarr",
            "mul",
            "stelem.ref",
            "div",
            "stelem.ref",
            "JavaScriptRuntime.Math::max",
            "ret");
        AssertMethodHasNoIntermediateSpills(
            il,
            "nestedValue",
            "CreateObjectLiteral",
            "ldstr \"x\"",
            "JavaScriptRuntime.Array::.ctor",
            "mul",
            "JavaScriptRuntime.Array::AddNumber",
            "add",
            "JavaScriptRuntime.Array::AddNumber",
            "SetObject",
            "ldstr \"y\"",
            "JavaScriptRuntime.Math::floor",
            "JavaScriptRuntime.Math::sqrt",
            "add",
            "SetNumber",
            "ret");
    }

    private static TempVariable AddTemp(MethodBodyIR body)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        return temp;
    }

    private static JrocCompiledAssemblyArtifact Compile(
        string source,
        LIRStackSchedulerMode mode,
        bool emitPdb)
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "LIRStackSchedulerIdentity");
        var entryPath = Path.Combine(root, "identity.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(entryPath, source);

        var options = new CompilerOptions
        {
            OutputDirectory = root,
            EmitPdb = emitPdb,
            LIRStackSchedulerMode = mode
        };

        var logger = new TestLogger();
        using var services = CompilerServices.BuildServiceProvider(options, fileSystem, logger);
        var compiler = services.GetRequiredService<Compiler>();
        return compiler.CompileToArtifact(entryPath)
            ?? throw new InvalidOperationException(
                $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
    }

    private sealed record UnknownInstruction : LIRInstruction;

    private static void AssertEquivalentMethodBodies(byte[] expectedPe, byte[] actualPe)
    {
        using var expectedStream = new MemoryStream(expectedPe, writable: false);
        using var actualStream = new MemoryStream(actualPe, writable: false);
        using var expectedReader = new PEReader(expectedStream);
        using var actualReader = new PEReader(actualStream);
        var expectedMetadata = expectedReader.GetMetadataReader();
        var actualMetadata = actualReader.GetMetadataReader();

        Assert.Equal(
            expectedMetadata.MethodDefinitions.Count,
            actualMetadata.MethodDefinitions.Count);

        foreach (var expectedHandle in expectedMetadata.MethodDefinitions)
        {
            var actualHandle = MetadataTokens.MethodDefinitionHandle(
                MetadataTokens.GetRowNumber(expectedHandle));
            var expectedDefinition = expectedMetadata.GetMethodDefinition(expectedHandle);
            var actualDefinition = actualMetadata.GetMethodDefinition(actualHandle);

            Assert.Equal(
                expectedMetadata.GetString(expectedDefinition.Name),
                actualMetadata.GetString(actualDefinition.Name));
            Assert.Equal(
                expectedDefinition.RelativeVirtualAddress == 0,
                actualDefinition.RelativeVirtualAddress == 0);

            if (expectedDefinition.RelativeVirtualAddress == 0)
            {
                continue;
            }

            var expectedBody = expectedReader.GetMethodBody(expectedDefinition.RelativeVirtualAddress);
            var actualBody = actualReader.GetMethodBody(actualDefinition.RelativeVirtualAddress);

            Assert.Equal(expectedBody.GetILBytes(), actualBody.GetILBytes());
            Assert.Equal(expectedBody.MaxStack, actualBody.MaxStack);
            AssertEquivalentLocalSignature(
                expectedMetadata,
                expectedBody.LocalSignature,
                actualMetadata,
                actualBody.LocalSignature);
            Assert.Equal(
                expectedBody.LocalVariablesInitialized,
                actualBody.LocalVariablesInitialized);
            Assert.Equal(
                expectedBody.ExceptionRegions.ToArray(),
                actualBody.ExceptionRegions.ToArray());
        }
    }

    private static void AssertMethodHasNoIntermediateSpills(
        string il,
        string className,
        params string[] orderedFragments)
        => AssertMethodHasOrderedFragmentsAndLocalCount(
            il,
            className,
            0,
            orderedFragments);

    private static void AssertMethodHasOrderedFragmentsAndLocalCount(
        string il,
        string className,
        int expectedStoreCount,
        params string[] orderedFragments)
    {
        var classStart = il.IndexOf(
            $"beforefieldinit {className}",
            StringComparison.Ordinal);
        Assert.True(classStart >= 0, $"Could not find generated class '{className}'.");
        var classEnd = il.IndexOf(
            $"end of class {className}",
            classStart,
            StringComparison.Ordinal);
        Assert.True(classEnd > classStart);
        var method = il[classStart..classEnd];

        var searchIndex = 0;
        foreach (var fragment in orderedFragments)
        {
            var fragmentIndex = method.IndexOf(
                fragment,
                searchIndex,
                StringComparison.Ordinal);
            Assert.True(
                fragmentIndex >= 0,
                $"Could not find '{fragment}' in order for '{className}'.");
            searchIndex = fragmentIndex + fragment.Length;
        }

        Assert.Equal(expectedStoreCount, CountOccurrences(method, "stloc"));
        Assert.Equal(expectedStoreCount, CountOccurrences(method, "ldloc"));
    }

    private static int CountOccurrences(string value, string fragment)
    {
        var count = 0;
        var index = 0;
        while ((index = value.IndexOf(
                   fragment,
                   index,
                   StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += fragment.Length;
        }
        return count;
    }

    private static void AssertEquivalentLocalSignature(
        MetadataReader expectedMetadata,
        StandaloneSignatureHandle expectedHandle,
        MetadataReader actualMetadata,
        StandaloneSignatureHandle actualHandle)
    {
        Assert.Equal(expectedHandle.IsNil, actualHandle.IsNil);
        if (expectedHandle.IsNil || actualHandle.IsNil)
        {
            return;
        }

        var expectedSignature = expectedMetadata.GetStandaloneSignature(expectedHandle);
        var actualSignature = actualMetadata.GetStandaloneSignature(actualHandle);
        Assert.Equal(
            expectedMetadata.GetBlobBytes(expectedSignature.Signature),
            actualMetadata.GetBlobBytes(actualSignature.Signature));
    }

    private static void AssertEquivalentPortablePdb(byte[]? expectedPdb, byte[]? actualPdb)
    {
        Assert.Equal(expectedPdb is null, actualPdb is null);
        if (expectedPdb is null || actualPdb is null)
        {
            return;
        }

        using var expectedStream = new MemoryStream(expectedPdb, writable: false);
        using var actualStream = new MemoryStream(actualPdb, writable: false);
        using var expectedProvider = MetadataReaderProvider.FromPortablePdbStream(expectedStream);
        using var actualProvider = MetadataReaderProvider.FromPortablePdbStream(actualStream);
        var expectedReader = expectedProvider.GetMetadataReader();
        var actualReader = actualProvider.GetMetadataReader();

        Assert.Equal(
            expectedReader.MethodDebugInformation.Count,
            actualReader.MethodDebugInformation.Count);

        foreach (var expectedHandle in expectedReader.MethodDebugInformation)
        {
            var actualHandle = MetadataTokens.MethodDebugInformationHandle(
                MetadataTokens.GetRowNumber(expectedHandle));
            var expectedInfo = expectedReader.GetMethodDebugInformation(expectedHandle);
            var actualInfo = actualReader.GetMethodDebugInformation(actualHandle);

            Assert.Equal(
                MetadataTokens.GetToken(expectedInfo.Document),
                MetadataTokens.GetToken(actualInfo.Document));
            Assert.Equal(
                expectedInfo.SequencePointsBlob.IsNil
                    ? System.Array.Empty<byte>()
                    : expectedReader.GetBlobBytes(expectedInfo.SequencePointsBlob),
                actualInfo.SequencePointsBlob.IsNil
                    ? System.Array.Empty<byte>()
                    : actualReader.GetBlobBytes(actualInfo.SequencePointsBlob));
        }

        Assert.Equal(
            GetDocuments(expectedReader),
            GetDocuments(actualReader));
        Assert.Equal(
            GetLocalScopes(expectedReader),
            GetLocalScopes(actualReader));
    }

    private static string[] GetDocuments(MetadataReader reader)
        => reader.Documents
            .Select(handle =>
            {
                var document = reader.GetDocument(handle);
                var hash = document.Hash.IsNil
                    ? string.Empty
                    : Convert.ToHexString(reader.GetBlobBytes(document.Hash));
                return string.Join(
                    "|",
                    reader.GetString(document.Name),
                    reader.GetGuid(document.HashAlgorithm),
                    hash,
                    reader.GetGuid(document.Language));
            })
            .ToArray();

    private static string[] GetLocalScopes(MetadataReader reader)
        => reader.LocalScopes
            .Select(handle =>
            {
                var scope = reader.GetLocalScope(handle);
                var locals = scope.GetLocalVariables()
                    .Select(localHandle =>
                    {
                        var local = reader.GetLocalVariable(localHandle);
                        return string.Join(
                            ":",
                            local.Index,
                            (int)local.Attributes,
                            reader.GetString(local.Name));
                    });

                return string.Join(
                    "|",
                    MetadataTokens.GetToken(scope.Method),
                    scope.StartOffset,
                    scope.Length,
                    string.Join(",", locals));
            })
            .ToArray();
}
