using Jroc.IR;
using Xunit;

namespace Jroc.Tests;

public sealed class LIRIntrinsicGuardHoistingTests
{
    [Fact]
    public void Normalize_HoistsOneStringEpochCheckToLoopPreheader()
    {
        var body = CreateStringLoop(guardedCallCount: 1);

        LIRIntrinsicGuardHoisting.Normalize(body);

        var capture = Assert.IsType<
            LIRCaptureIntrinsicPrototypeAssumption>(
            body.Instructions[2]);
        Assert.Equal(
            JavaScriptRuntime.IntrinsicPrototypeFamily.String,
            capture.PrototypeFamily);
        Assert.Contains(
            capture.Result.Index,
            body.PinnedTempIndices);
        var guarded = Assert.Single(
            body.Instructions
                .OfType<LIRCallGuardedStringIntrinsic>());
        Assert.Equal(capture.Result, guarded.PrototypeAssumption);
    }

    [Fact]
    public void Normalize_SharesOneAssumptionAcrossLoopCalls()
    {
        var body = CreateStringLoop(guardedCallCount: 2);

        LIRIntrinsicGuardHoisting.Normalize(body);

        var capture = Assert.Single(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
        var guardedCalls = body.Instructions
            .OfType<LIRCallGuardedStringIntrinsic>()
            .ToArray();
        Assert.Equal(2, guardedCalls.Length);
        Assert.All(
            guardedCalls,
            guarded => Assert.Equal(
                capture.Result,
                guarded.PrototypeAssumption));
    }

    [Fact]
    public void Normalize_RejectsLoopContainingUnknownCall()
    {
        var body = CreateStringLoop(guardedCallCount: 1);
        var unknownResult = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.Instructions.Insert(
            body.Instructions.Count - 1,
            new LIRCallMember0(
                body.Temps[0],
                "unknown",
                unknownResult));
        body.LoopNestingFacts = null;

        body.LoopNestingFacts =
            LIRLoopNestingAnalysis.Analyze(body);
        LIRIntrinsicGuardHoisting.Normalize(body);

        Assert.Empty(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
        Assert.Null(
            Assert.Single(
                    body.Instructions
                        .OfType<LIRCallGuardedStringIntrinsic>())
                .PrototypeAssumption);
    }

    [Fact]
    public void Normalize_RejectsLoopContainingHeapMutation()
    {
        var body = CreateStringLoop(guardedCallCount: 1);
        var setResult = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.Instructions.Insert(
            body.Instructions.Count - 1,
            new LIRSetItem(
                body.Temps[0],
                body.Temps[1],
                body.Temps[0],
                setResult));
        body.LoopNestingFacts = null;

        body.LoopNestingFacts =
            LIRLoopNestingAnalysis.Analyze(body);
        LIRIntrinsicGuardHoisting.Normalize(body);

        Assert.Empty(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
    }

    [Fact]
    public void Normalize_RejectsAccessorCapableArrayRead()
    {
        var body = CreateStringLoop(guardedCallCount: 1);
        var array = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var readResult = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.Instructions.Insert(
            body.Instructions.Count - 1,
            new LIRGetJsArrayElement(
                array,
                body.Temps[1],
                readResult));
        body.LoopNestingFacts =
            LIRLoopNestingAnalysis.Analyze(body);

        LIRIntrinsicGuardHoisting.Normalize(body);

        Assert.Empty(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
    }

    [Fact]
    public void Normalize_RejectsMethodsWithExceptionRegions()
    {
        var body = CreateStringLoop(guardedCallCount: 1);
        body.ExceptionRegions.Add(
            new ExceptionRegionInfo(
                ExceptionRegionKind.Catch,
                TryStartLabelId: 10,
                TryEndLabelId: 11,
                HandlerStartLabelId: 12,
                HandlerEndLabelId: 13,
                typeof(Exception)));

        LIRIntrinsicGuardHoisting.Normalize(body);

        Assert.Empty(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
    }

    [Fact]
    public void Normalize_RejectsStringCallWithObjectArgument()
    {
        var body = CreateStringLoop(guardedCallCount: 1);
        body.TempStorages[1] = new ValueStorage(
            ValueStorageKind.Reference,
            typeof(object));
        body.Instructions[1] =
            new LIRLoadParameter(0, body.Temps[1]);
        body.LoopNestingFacts =
            LIRLoopNestingAnalysis.Analyze(body);

        LIRIntrinsicGuardHoisting.Normalize(body);

        Assert.Empty(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
    }

    [Fact]
    public void Normalize_DoesNotHoistArrayMemberCalls()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        var argument = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.UnboxedValue,
                typeof(double)));
        var result = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.Instructions.Add(
            new LIRLoadParameter(0, receiver));
        body.Instructions.Add(
            new LIRConstNumber(1, argument));
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(
            new LIRCallGuardedIntrinsicMember(
                receiver,
                typeof(JavaScriptRuntime.Array),
                JavaScriptRuntime.IntrinsicPrototypeFamily.Array,
                "push",
                ReceiverIsProvenType: false,
                [argument],
                result));
        body.Instructions.Add(new LIRBranch(10));

        LIRIntrinsicGuardHoisting.Normalize(body);

        Assert.Empty(
            body.Instructions
                .OfType<LIRCaptureIntrinsicPrototypeAssumption>());
    }

    private static MethodBodyIR CreateStringLoop(
        int guardedCallCount)
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(string)));
        var argument = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.UnboxedValue,
                typeof(double)));
        body.Instructions.Add(
            new LIRConstString("abc", receiver));
        body.Instructions.Add(
            new LIRConstNumber(0, argument));
        body.Instructions.Add(new LIRLabel(10));

        for (var index = 0; index < guardedCallCount; index++)
        {
            var result = AddTemp(
                body,
                new ValueStorage(
                    ValueStorageKind.Reference,
                    typeof(object)));
            body.Instructions.Add(
                new LIRCallGuardedStringIntrinsic(
                    receiver,
                    "charAt",
                    nameof(JavaScriptRuntime.String.CharAt),
                    [typeof(string), typeof(object)],
                    typeof(string),
                    ReceiverIsProvenString: true,
                    [argument],
                    result));
        }

        body.Instructions.Add(new LIRBranch(10));
        body.LoopNestingFacts =
            LIRLoopNestingAnalysis.Analyze(body);
        return body;
    }

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
}
