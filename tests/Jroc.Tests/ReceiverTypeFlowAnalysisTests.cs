using Acornima;
using Jroc.IR;
using Jroc.Services;
using Jroc.SymbolTables;
using Xunit;

namespace Jroc.Tests;

public sealed class ReceiverTypeFlowAnalysisTests
{
    [Fact]
    public void Analyze_TracksStrongAssignmentsAtEachProgramPoint()
    {
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var variable = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var firstResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var arrayValue = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var secondResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRCopyTemp(stringValue, variable));
        body.Instructions.Add(new LIRCallMember0(variable, "trim", firstResult));
        body.Instructions.Add(new LIRNewJsArray([], arrayValue));
        body.Instructions.Add(new LIRCopyTemp(arrayValue, variable));
        body.Instructions.Add(new LIRCallMember0(variable, "join", secondResult));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(2, variable),
            typeof(string));
        AssertKnownCandidates(
            facts.GetTempBefore(5, variable),
            typeof(JavaScriptRuntime.Array));
    }

    [Fact]
    public void Analyze_UnionsBranchAssignmentsAtPhiLikeJoin()
    {
        var body = new MethodBodyIR();
        var condition = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var arrayValue = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var joined = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstBoolean(true, condition));
        body.Instructions.Add(new LIRBranchIfFalse(condition, 10));
        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRCopyTemp(stringValue, joined));
        body.Instructions.Add(new LIRBranch(20));
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(new LIRNewJsArray([], arrayValue));
        body.Instructions.Add(new LIRCopyTemp(arrayValue, joined));
        body.Instructions.Add(new LIRLabel(20));
        body.Instructions.Add(new LIRCallMember0(joined, "toString", result));

        var diagnostics = new ReceiverTypeFlowDiagnosticTrace();
        var facts = ReceiverTypeFlowAnalysis.Analyze(
            body,
            diagnostics);

        AssertKnownCandidates(
            facts.GetTempBefore(9, joined),
            typeof(string),
            typeof(JavaScriptRuntime.Array));
        Assert.Contains(
            diagnostics.Events,
            item =>
                item.Kind == ReceiverTypeFlowDiagnosticKind.Merge
                && item.Message
                    == "merge @8 temp:t3: "
                    + "b1=candidates=[System.String]; unknown=false; non-candidate=false, "
                    + "b2=candidates=[JavaScriptRuntime.Array]; unknown=false; non-candidate=false "
                    + "=> candidates=[JavaScriptRuntime.Array, System.String]; "
                    + "unknown=false; non-candidate=false");
        Assert.Contains(
            diagnostics.Events,
            item =>
                item.Kind == ReceiverTypeFlowDiagnosticKind.Retained
                && item.Message
                    == "retain @9 LIRCallMember0 receiver=t3: "
                    + "candidates=[JavaScriptRuntime.Array, System.String]; "
                    + "unknown=false; non-candidate=false");
    }

    [Fact]
    public void Analyze_PreservesNonCandidateUncertaintyAtBranchJoin()
    {
        var body = new MethodBodyIR();
        var condition = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var numberValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var joined = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstBoolean(true, condition));
        body.Instructions.Add(new LIRBranchIfFalse(condition, 10));
        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRCopyTemp(stringValue, joined));
        body.Instructions.Add(new LIRBranch(20));
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(new LIRConstNumber(1, numberValue));
        body.Instructions.Add(new LIRCopyTemp(numberValue, joined));
        body.Instructions.Add(new LIRLabel(20));
        body.Instructions.Add(new LIRCallMember0(joined, "toString", result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);
        var joinedValue = facts.GetTempBefore(9, joined);

        Assert.False(joinedValue.IncludesUnknown);
        Assert.True(joinedValue.IncludesNonCandidate);
        Assert.Equal(
            [typeof(string)],
            joinedValue.CandidateClrTypes);
    }

    [Fact]
    public void Analyze_ReachesFixedPointAcrossLoopBackEdge()
    {
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var arrayValue = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var loopValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)),
            variableSlot: 0);
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRCopyTemp(stringValue, loopValue));
        body.Instructions.Add(new LIRLabel(10));
        body.Instructions.Add(new LIRCallMember0(loopValue, "toString", result));
        body.Instructions.Add(new LIRNewJsArray([], arrayValue));
        body.Instructions.Add(new LIRCopyTemp(arrayValue, loopValue));
        body.Instructions.Add(new LIRBranch(10));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(3, loopValue),
            typeof(string),
            typeof(JavaScriptRuntime.Array));
    }

    [Fact]
    public void Analyze_TracksCapturedFieldAssignmentsAtEachRead()
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript("var value;", "flow.js");
        var scope = new Jroc.SymbolTables.Scope(
            "GlobalScope",
            ScopeKind.Global,
            parent: null,
            program);
        var binding = new BindingInfo(
            "value",
            BindingKind.Var,
            scope,
            program);

        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var arrayValue = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var firstRead = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var secondRead = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var scopeInstance = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var callResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var scopeId = new ScopeId("GlobalScope");
        var fieldId = new FieldId("GlobalScope", "value");
        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRStoreScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            stringValue));
        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            firstRead));
        body.Instructions.Add(new LIRCallMember0(
            firstRead,
            "toString",
            callResult));
        body.Instructions.Add(new LIRNewJsArray([], arrayValue));
        body.Instructions.Add(new LIRStoreScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            arrayValue));
        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            secondRead));
        body.Instructions.Add(new LIRCallMember0(
            secondRead,
            "toString",
            callResult));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempAfter(2, firstRead),
            typeof(string));
        AssertKnownCandidates(
            facts.GetTempAfter(6, secondRead),
            typeof(JavaScriptRuntime.Array));
    }

    [Fact]
    public void Analyze_InvalidatesCapturedFieldAcrossCall()
    {
        var (binding, scopeId, fieldId) = CreateCapturedBinding();
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var scopeInstance = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var callReceiver = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var callResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var readAfterCall = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRStoreScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            stringValue));
        body.Instructions.Add(new LIRCallMember0(
            callReceiver,
            "unknown",
            callResult));
        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            readAfterCall));
        body.Instructions.Add(new LIRCallMember0(
            readAfterCall,
            "toString",
            callResult));

        var diagnostics = new ReceiverTypeFlowDiagnosticTrace();
        var facts = ReceiverTypeFlowAnalysis.Analyze(
            body,
            diagnostics);
        var readValue = facts.GetTempAfter(3, readAfterCall);

        Assert.True(readValue.IncludesUnknown);
        Assert.DoesNotContain(
            typeof(string),
            readValue.CandidateClrTypes);
        var invalidation = Assert.Single(diagnostics.Events);
        Assert.Equal(
            ReceiverTypeFlowDiagnosticKind.Invalidation,
            invalidation.Kind);
        Assert.Equal(
            "invalidate @2 LIRCallMember0 reason=call: "
            + "binding:GlobalScope/value=candidates=[System.String]; "
            + "unknown=false; non-candidate=false",
            invalidation.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Analyze_InvalidatesCapturedFieldAcrossDynamicPropertyAccess(
        int accessKind)
    {
        var (binding, scopeId, fieldId) = CreateCapturedBinding();
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var scopeInstance = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var propertyReceiver = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var propertyName = AddTemp(
            body,
            accessKind == 2
                ? new ValueStorage(
                    ValueStorageKind.UnboxedValue,
                    typeof(double))
                : new ValueStorage(
                    ValueStorageKind.Reference,
                    typeof(string)));
        var propertyValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var readAfterAccess = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var callResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRStoreScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            stringValue));
        body.Instructions.Add(
            accessKind == 2
                ? new LIRConstNumber(0, propertyName)
                : new LIRConstString("property", propertyName));
        body.Instructions.Add(
            accessKind switch
            {
                0 => new LIRGetItem(
                    propertyReceiver,
                    propertyName,
                    propertyValue),
                1 => new LIRSetItem(
                    propertyReceiver,
                    propertyName,
                    stringValue,
                    propertyValue),
                _ => new LIRGetJsArrayElement(
                    propertyReceiver,
                    propertyName,
                    propertyValue)
            });
        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            readAfterAccess));
        body.Instructions.Add(new LIRCallMember0(
            readAfterAccess,
            "toString",
            callResult));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);
        var readValue = facts.GetTempAfter(4, readAfterAccess);

        Assert.True(readValue.IncludesUnknown);
        Assert.DoesNotContain(
            typeof(string),
            readValue.CandidateClrTypes);
    }

    [Fact]
    public void Analyze_InvalidatesAllMutableFactsAtUnsupportedBarrier()
    {
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var variable = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)),
            variableSlot: 0);
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRCopyTemp(stringValue, variable));
        body.Instructions.Add(new UnknownInstruction());
        body.Instructions.Add(new LIRCallMember0(variable, "trim", result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);
        var value = facts.GetTempBefore(3, variable);

        Assert.True(value.IncludesUnknown);
        Assert.DoesNotContain(typeof(string), value.CandidateClrTypes);
    }

    [Fact]
    public void Analyze_ContinuesAcrossAwaitResumeBoundary()
    {
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var awaited = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var awaitResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var callResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRAwait(
            awaited,
            AwaitId: 1,
            ResumeStateId: 1,
            ResumeLabelId: 100,
            awaitResult));
        body.Instructions.Add(new LIRCallMember0(
            stringValue,
            "trim",
            callResult));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(2, stringValue),
            typeof(string));
    }

    [Fact]
    public void Analyze_RecognizesSealedRuntimeReceiverTypes()
    {
        var body = new MethodBodyIR();
        var typedArray = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Uint8Array)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Uint8Array",
            "Construct",
            [],
            typedArray));
        body.Instructions.Add(
            new LIRCallMember0(typedArray, "join", result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(1, typedArray),
            typeof(JavaScriptRuntime.Uint8Array));
    }

    [Fact]
    public void Analyze_RecognizesArrayConstructorResults()
    {
        var body = new MethodBodyIR();
        var array = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Array",
            "Construct",
            [],
            array));
        body.Instructions.Add(new LIRCallMember0(array, "join", result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(1, array),
            typeof(JavaScriptRuntime.Array));
    }

    [Fact]
    public void Analyze_SeedsInterproceduralParameterFacts()
    {
        var body = new MethodBodyIR();
        var parameter = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.ReceiverParameterTypeSummaries[0] =
            new ReceiverTypeSummary(
                includesUnknown: false,
                includesNonCandidate: false,
                [typeof(string), typeof(JavaScriptRuntime.Array)]);

        body.Instructions.Add(new LIRLoadParameter(0, parameter));
        body.Instructions.Add(
            new LIRCallMember0(parameter, "toString", result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(1, parameter),
            typeof(string),
            typeof(JavaScriptRuntime.Array));
    }

    [Fact]
    public void Analyze_SeedsProvenCapturedClosureEntryFacts()
    {
        var (binding, scopeId, fieldId) = CreateCapturedBinding();
        var body = new MethodBodyIR();
        var scopeInstance = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var capturedValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.ReceiverCapturedEntryTypeSummaries[binding] =
            ReceiverTypeSummary.ForCandidate(typeof(string));

        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            capturedValue));
        body.Instructions.Add(new LIRCallMember0(
            capturedValue,
            "trim",
            result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);

        AssertKnownCandidates(
            facts.GetTempBefore(1, capturedValue),
            typeof(string));
    }

    [Fact]
    public void Analyze_PropagatesKnownCallableReturnSummary()
    {
        var body = new MethodBodyIR();
        var returnedValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.ReceiverTempTypeSummaries[returnedValue.Index] =
            new ReceiverTypeSummary(
                includesUnknown: true,
                includesNonCandidate: true,
                [typeof(string)]);

        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Unknown",
            "Call",
            [],
            returnedValue));
        body.Instructions.Add(new LIRCallMember0(
            returnedValue,
            "trim",
            result));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);
        var returnedFact = facts.GetTempBefore(1, returnedValue);

        Assert.True(returnedFact.IncludesUnknown);
        Assert.True(returnedFact.IncludesNonCandidate);
        Assert.Contains(
            typeof(string),
            returnedFact.CandidateClrTypes);
    }

    [Fact]
    public void Analyze_DropsMutableFactsAcrossFinallyLeave()
    {
        var (binding, scopeId, fieldId) = CreateCapturedBinding();
        var body = new MethodBodyIR();
        var stringValue = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var scopeInstance = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var readAfterFinally = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var callResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.ExceptionRegions.Add(new ExceptionRegionInfo(
            ExceptionRegionKind.Finally,
            TryStartLabelId: 1,
            TryEndLabelId: 2,
            HandlerStartLabelId: 3,
            HandlerEndLabelId: 4));
        body.Instructions.Add(new LIRLabel(1));
        body.Instructions.Add(new LIRConstString("value", stringValue));
        body.Instructions.Add(new LIRStoreScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            stringValue));
        body.Instructions.Add(new LIRLeave(5));
        body.Instructions.Add(new LIRLabel(2));
        body.Instructions.Add(new LIRLabel(3));
        body.Instructions.Add(new LIREndFinally());
        body.Instructions.Add(new LIRLabel(4));
        body.Instructions.Add(new LIRLabel(5));
        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            fieldId,
            scopeId,
            readAfterFinally));
        body.Instructions.Add(new LIRCallMember0(
            readAfterFinally,
            "toString",
            callResult));

        var facts = ReceiverTypeFlowAnalysis.Analyze(body);
        var readValue = facts.GetTempAfter(9, readAfterFinally);

        Assert.True(readValue.IncludesUnknown);
        Assert.DoesNotContain(
            typeof(string),
            readValue.CandidateClrTypes);
    }

    [Fact]
    public void RequiresAnalysis_OnlyForDynamicReceiverSites()
    {
        var noReceiverBody = new MethodBodyIR();
        var stringValue = AddTemp(
            noReceiverBody,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        noReceiverBody.Instructions.Add(
            new LIRConstString("value", stringValue));

        var receiverBody = new MethodBodyIR();
        var receiver = AddTemp(
            receiverBody,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            receiverBody,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        receiverBody.Instructions.Add(
            new LIRCallMember0(receiver, "trim", result));

        Assert.False(
            ReceiverTypeFlowAnalysis.RequiresAnalysis(noReceiverBody));
        Assert.True(
            ReceiverTypeFlowAnalysis.RequiresAnalysis(receiverBody));
    }

    [Fact]
    public void RequiresSpecializationAnalysis_SkipsCandidateFreeReceiver()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.Instructions.Add(
            new LIRCallMember0(receiver, "pop", result));

        Assert.False(
            ReceiverTypeFlowAnalysis
                .RequiresSpecializationAnalysis(body));
    }

    [Fact]
    public void RequiresSpecializationAnalysis_FollowsCandidateProducer()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.Instructions.Add(
            new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.Array),
                "Construct",
                [],
                receiver));
        body.Instructions.Add(
            new LIRCallMember0(receiver, "pop", result));

        Assert.True(
            ReceiverTypeFlowAnalysis
                .RequiresSpecializationAnalysis(body));
    }

    private static TempVariable AddTemp(
        MethodBodyIR body,
        ValueStorage storage,
        int variableSlot = -1)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(variableSlot);
        return temp;
    }

    private static (BindingInfo Binding, ScopeId Scope, FieldId Field)
        CreateCapturedBinding()
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript("var value;", "flow.js");
        var scope = new Jroc.SymbolTables.Scope(
            "GlobalScope",
            ScopeKind.Global,
            parent: null,
            program);
        return (
            new BindingInfo(
                "value",
                BindingKind.Var,
                scope,
                program),
            new ScopeId("GlobalScope"),
            new FieldId("GlobalScope", "value"));
    }

    private static void AssertKnownCandidates(
        ReceiverTypeFlowValue value,
        params Type[] expected)
    {
        Assert.False(value.IncludesUnknown);
        Assert.False(value.IncludesNonCandidate);
        Assert.Equal(
            expected.OrderBy(static type => type.FullName),
            value.CandidateClrTypes.OrderBy(static type => type.FullName));
    }

    private sealed record UnknownInstruction : LIRInstruction;
}
