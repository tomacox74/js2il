using Acornima;
using Jroc.IR;
using Jroc.Services;
using Jroc.SymbolTables;
using Xunit;

namespace Jroc.Tests;

public sealed class LIRIntrinsicNormalizationTests
{
    private static TempVariable AddTemp(MethodBodyIR body, ValueStorage storage)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(-1);
        return temp;
    }

    [Fact]
    public void Normalize_Rewrites_GetItem_To_Int32ArrayElementGet_WhenReceiverProvenAndIndexIsDouble()
    {
        var classRegistry = new ClassRegistry();
        classRegistry.RegisterFieldClrType("Classes.TestClass", "arr", typeof(JavaScriptRuntime.Int32Array));

        var body = new MethodBodyIR();
        var receiver = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Int32Array)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRLoadUserClassInstanceField("Classes.TestClass", "arr", IsPrivateField: false, receiver));
        body.Instructions.Add(new LIRGetItem(receiver, index, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry);

        var rewritten = Assert.IsType<LIRGetInt32ArrayElement>(body.Instructions[1]);
        Assert.Equal(receiver, rewritten.Receiver);
        Assert.Equal(index, rewritten.Index);
        Assert.Equal(result, rewritten.Result);

        Assert.Equal(ValueStorageKind.UnboxedValue, body.TempStorages[result.Index].Kind);
        Assert.Equal(typeof(double), body.TempStorages[result.Index].ClrType);
    }

    [Fact]
    public void Normalize_DoesNotRewrite_GetItem_WhenIndexNotUnboxedDouble()
    {
        var classRegistry = new ClassRegistry();
        classRegistry.RegisterFieldClrType("Classes.TestClass", "arr", typeof(JavaScriptRuntime.Int32Array));

        var body = new MethodBodyIR();
        var receiver = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Int32Array)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRLoadUserClassInstanceField("Classes.TestClass", "arr", IsPrivateField: false, receiver));
        body.Instructions.Add(new LIRGetItem(receiver, index, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry);

        Assert.IsType<LIRGetItem>(body.Instructions[1]);
    }

    [Fact]
    public void Normalize_Rewrites_ProvenStringMemberCall_ToGuardedIntrinsic()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var argument = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(
            new LIRCallMember1(receiver, "charAt", argument, result));

        LIRIntrinsicNormalization.Normalize(body, new ClassRegistry());

        var guarded = Assert.IsType<LIRCallGuardedStringIntrinsic>(
            body.Instructions[0]);
        Assert.True(guarded.ReceiverIsProvenString);
        Assert.Equal("charAt", guarded.MemberName);
        Assert.Equal(nameof(JavaScriptRuntime.String.CharAt), guarded.IntrinsicMethodName);
        Assert.Equal(
            [typeof(string), typeof(object)],
            guarded.IntrinsicParameterTypes);
        Assert.Equal(typeof(string), guarded.IntrinsicReturnClrType);
        Assert.Equal(LIRGuardedStringFallbackResultConversion.None, guarded.FallbackResultConversion);
        Assert.Equal(ValueStorageKind.Reference, body.TempStorages[result.Index].Kind);
        Assert.Equal(typeof(object), body.TempStorages[result.Index].ClrType);
    }

    [Fact]
    public void Normalize_Rewrites_UncertainStringMemberCall_WithReceiverTypeTest()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(new LIRCallMember0(receiver, "trim", result));

        LIRIntrinsicNormalization.Normalize(body, new ClassRegistry());

        var guarded = Assert.IsType<LIRCallGuardedStringIntrinsic>(
            body.Instructions[0]);
        Assert.False(guarded.ReceiverIsProvenString);
        Assert.Equal("trim", guarded.MemberName);
        Assert.Empty(guarded.Arguments);
    }

    [Fact]
    public void Normalize_DoesNotRewrite_StringCandidateFieldMemberCall_WithKnownNonStringType()
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript("var value;", "candidate.js");
        var scope = new Jroc.SymbolTables.Scope(
            "GlobalScope",
            ScopeKind.Global,
            parent: null,
            program);
        var binding = new BindingInfo("value", BindingKind.Var, scope, program)
        {
            ClrType = typeof(JavaScriptRuntime.String)
        };
        binding.ReceiverCandidateClrTypes.Add(typeof(string));

        var body = new MethodBodyIR();
        var scopeInstance = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var receiver = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(new LIRLoadScopeField(
            scopeInstance,
            binding,
            new FieldId("GlobalScope", "value"),
            new ScopeId("GlobalScope"),
            receiver));
        body.Instructions.Add(new LIRCallMember0(receiver, "trim", result));

        LIRIntrinsicNormalization.Normalize(body, new ClassRegistry());

        Assert.IsType<LIRCallMember0>(body.Instructions[1]);
    }

    [Fact]
    public void Normalize_DoesNotAddStringGuard_ForKnownNonStringReceiver()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(new LIRCallMember0(receiver, "trim", result));

        LIRIntrinsicNormalization.Normalize(body, new ClassRegistry());

        Assert.IsType<LIRCallMember0>(body.Instructions[0]);
    }

    [Fact]
    public void Normalize_KeepsProvenArrayMemberCallDirect()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(JavaScriptRuntime.Array)));
        var argument = AddTemp(
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
            new LIRCallMember1(
                receiver,
                "push",
                argument,
                result));

        LIRIntrinsicNormalization.Normalize(
            body,
            new ClassRegistry());

        var direct =
            Assert.IsType<LIRCallInstanceMethod>(
                body.Instructions[0]);
        Assert.Equal(
            typeof(JavaScriptRuntime.Array),
            direct.ReceiverClrType);
        Assert.Equal(
            typeof(double),
            body.TempStorages[result.Index].ClrType);
    }

    [Fact]
    public void ReceiverSpecialization_ConsumesArrayFlowCandidate()
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
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.Array),
            "Construct",
            [],
            receiver));
        body.Instructions.Add(
            new LIRCallMember0(receiver, "pop", result));
        body.ReceiverTypeFlowFacts =
            ReceiverTypeFlowAnalysis.Analyze(body);

        LIRReceiverSpecialization.Normalize(body);

        var guarded =
            Assert.IsType<LIRCallGuardedIntrinsicMember>(
                body.Instructions[1]);
        Assert.Equal(
            typeof(JavaScriptRuntime.Array),
            guarded.ReceiverClrType);
        Assert.True(guarded.ReceiverIsProvenType);
    }

    [Fact]
    public void ReceiverSpecialization_ConsumesTypedArrayFlowCandidate()
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
                ValueStorageKind.Reference,
                typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.ReceiverTempTypeSummaries[receiver.Index] =
            ReceiverTypeSummary.ForCandidate(
                typeof(JavaScriptRuntime.Int32Array));
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Int32Array",
            "Construct",
            [],
            receiver));
        body.Instructions.Add(
            new LIRCallMember1(
                receiver,
                "includes",
                argument,
                result));
        body.ReceiverTypeFlowFacts =
            ReceiverTypeFlowAnalysis.Analyze(body);

        LIRReceiverSpecialization.Normalize(body);

        var guarded =
            Assert.IsType<LIRCallGuardedIntrinsicMember>(
                body.Instructions[1]);
        Assert.Equal(
            typeof(JavaScriptRuntime.Int32Array),
            guarded.ReceiverClrType);
        Assert.Equal(
            JavaScriptRuntime.IntrinsicPrototypeFamily.TypedArray,
            guarded.PrototypeFamily);
        Assert.True(guarded.ReceiverIsProvenType);
    }

    [Fact]
    public void ReceiverSpecialization_KeepsTypeGuardForUncertainArrayCandidate()
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
                ValueStorageKind.Reference,
                typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(
                ValueStorageKind.Reference,
                typeof(object)));
        body.ReceiverTempTypeSummaries[receiver.Index] =
            new ReceiverTypeSummary(
                includesUnknown: true,
                includesNonCandidate: true,
                [typeof(JavaScriptRuntime.Array)]);
        body.Instructions.Add(new LIRCallIntrinsicStatic(
            "Unknown",
            "Call",
            [],
            receiver));
        body.Instructions.Add(
            new LIRCallMember1(
                receiver,
                "push",
                argument,
                result));
        body.ReceiverTypeFlowFacts =
            ReceiverTypeFlowAnalysis.Analyze(body);

        LIRReceiverSpecialization.Normalize(body);

        var guarded =
            Assert.IsType<LIRCallGuardedIntrinsicMember>(
                body.Instructions[1]);
        Assert.Equal(
            typeof(JavaScriptRuntime.Array),
            guarded.ReceiverClrType);
        Assert.False(guarded.ReceiverIsProvenType);
    }

    [Fact]
    public void Normalize_Fuses_CharCodeAtNumberConversion_IntoGuardedIntrinsic()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var index = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var callResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var numberResult = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.Instructions.Add(
            new LIRCallMember1(receiver, "charCodeAt", index, callResult));
        body.Instructions.Add(
            new LIRConvertToNumber(callResult, numberResult));

        LIRIntrinsicNormalization.Normalize(body, new ClassRegistry());

        var guarded = Assert.IsType<LIRCallGuardedStringIntrinsic>(
            Assert.Single(body.Instructions));
        Assert.False(guarded.ReceiverIsProvenString);
        Assert.Equal(
            LIRGuardedStringFallbackResultConversion.ToNumber,
            guarded.FallbackResultConversion);
        Assert.Equal(typeof(double), guarded.IntrinsicReturnClrType);
        Assert.Equal(ValueStorageKind.UnboxedValue, body.TempStorages[numberResult.Index].Kind);
        Assert.Equal(typeof(double), body.TempStorages[numberResult.Index].ClrType);
    }

    [Fact]
    public void Normalize_Rewrites_SetItem_To_Int32ArrayElementSet_WhenReceiverProvenViaCopyAndOperandsAreDouble()
    {
        var classRegistry = new ClassRegistry();
        classRegistry.RegisterFieldClrType("Classes.TestClass", "arr", typeof(JavaScriptRuntime.Int32Array));

        var body = new MethodBodyIR();
        var receiver0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Int32Array)));
        var receiver1 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Int32Array)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRLoadUserClassInstanceField("Classes.TestClass", "arr", IsPrivateField: false, receiver0));
        body.Instructions.Add(new LIRCopyTemp(receiver0, receiver1));
        body.Instructions.Add(new LIRSetItem(receiver1, index, value, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry);

        var rewritten = Assert.IsType<LIRSetInt32ArrayElement>(body.Instructions[2]);
        Assert.Equal(receiver1, rewritten.Receiver);
        Assert.Equal(index, rewritten.Index);
        Assert.Equal(value, rewritten.Value);
        Assert.Equal(result, rewritten.Result);

        Assert.Equal(ValueStorageKind.UnboxedValue, body.TempStorages[result.Index].Kind);
        Assert.Equal(typeof(double), body.TempStorages[result.Index].ClrType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Normalize_Rewrites_AritySpecificRequireCalls_To_LIRCallRequire(int arity)
    {
        var body = new MethodBodyIR();
        var requireValue = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Modules.CommonJS.RequireDelegate)));
        var scopes = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var a0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a1 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a2 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a3 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a4 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRBuildScopesArray(System.Array.Empty<ScopeSlotSource>(), scopes));
        body.Instructions.Add(arity switch
        {
            0 => new LIRCallFunctionValue0(requireValue, scopes, result),
            1 => new LIRCallFunctionValue1(requireValue, scopes, a0, result),
            2 => new LIRCallFunctionValue2(requireValue, scopes, a0, a1, result),
            3 => new LIRCallFunctionValue3(requireValue, scopes, a0, a1, a2, result),
            4 => new LIRCallFunctionValue4(requireValue, scopes, a0, a1, a2, a3, result),
            5 => new LIRCallFunctionValue5(requireValue, scopes, a0, a1, a2, a3, a4, result),
            _ => throw new ArgumentOutOfRangeException(nameof(arity))
        });

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        Assert.DoesNotContain(body.Instructions, static ins => ins is LIRBuildScopesArray);
        var requireCall = Assert.IsType<LIRCallRequire>(body.Instructions[body.Instructions.Count - 1]);
        Assert.Equal(requireValue, requireCall.RequireValue);
        Assert.Equal(result, requireCall.Result);

        if (arity == 0)
        {
            Assert.Equal(2, body.Instructions.Count);
            var undefined = Assert.IsType<LIRConstUndefined>(body.Instructions[0]);
            Assert.Equal(undefined.Result, requireCall.ModuleId);
        }
        else
        {
            Assert.Single(body.Instructions);
            Assert.Equal(a0, requireCall.ModuleId);
        }
    }

    [Fact]
    public void Normalize_Rewrites_ArrayBasedRequireCalls_To_LIRCallRequire()
    {
        var body = new MethodBodyIR();
        var requireValue = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Modules.CommonJS.RequireDelegate)));
        var scopes = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var a0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a1 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var argsArray = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRBuildScopesArray(System.Array.Empty<ScopeSlotSource>(), scopes));
        body.Instructions.Add(new LIRBuildArray(new[] { a0, a1 }, argsArray));
        body.Instructions.Add(new LIRCallFunctionValue(requireValue, scopes, argsArray, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        Assert.Single(body.Instructions);
        var requireCall = Assert.IsType<LIRCallRequire>(body.Instructions[0]);
        Assert.Equal(requireValue, requireCall.RequireValue);
        Assert.Equal(a0, requireCall.ModuleId);
        Assert.Equal(result, requireCall.Result);
    }

    [Fact]
    public void Normalize_DoesNotRewrite_AritySpecificCall_WhenCalleeIsNotRequireDelegate()
    {
        var body = new MethodBodyIR();
        var callee = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var scopes = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var a0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRBuildScopesArray(System.Array.Empty<ScopeSlotSource>(), scopes));
        body.Instructions.Add(new LIRCallFunctionValue1(callee, scopes, a0, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        Assert.IsType<LIRBuildScopesArray>(body.Instructions[0]);
        Assert.IsType<LIRCallFunctionValue1>(body.Instructions[1]);
    }

    [Fact]
    public void Normalize_RemovesUnusedScopeProducers()
    {
        var body = new MethodBodyIR();
        var loadedScopes = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var builtScopes = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        body.Instructions.Add(new LIRLoadScopesArgument(loadedScopes));
        body.Instructions.Add(new LIRBuildScopesArray(
            System.Array.Empty<ScopeSlotSource>(),
            builtScopes));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        Assert.Single(body.Instructions);
        Assert.IsType<LIRReturnUndefinedImmediate>(body.Instructions[0]);
    }

    [Fact]
    public void Normalize_PreservesUsedScopeProducer()
    {
        var body = new MethodBodyIR();
        var scopes = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var callee = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(
            body,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.Instructions.Add(new LIRBuildScopesArray(
            System.Array.Empty<ScopeSlotSource>(),
            scopes));
        body.Instructions.Add(new LIRCallFunctionValue0(
            callee,
            scopes,
            result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        Assert.IsType<LIRBuildScopesArray>(body.Instructions[0]);
        Assert.IsType<LIRCallFunctionValue0>(body.Instructions[1]);
    }

    [Fact]
    public void Normalize_Rewrites_CallIntrinsic_ToCallInstanceMethod_WhenArgsFromSmallBuildArray()
    {
        var body = new MethodBodyIR();
        var consoleObj = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var argsArray = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRBuildArray(new[] { a0 }, argsArray));
        body.Instructions.Add(new LIRCallIntrinsic(consoleObj, "log", argsArray, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        // LIRBuildArray should be removed since it's only used by the call.
        Assert.Single(body.Instructions);
        var instanceCall = Assert.IsType<LIRCallInstanceMethod>(body.Instructions[0]);
        Assert.Equal(consoleObj, instanceCall.Receiver);
        Assert.Equal(typeof(JavaScriptRuntime.Console), instanceCall.ReceiverClrType);
        Assert.Equal("log", instanceCall.MethodName);
        Assert.Equal(result, instanceCall.Result);
        Assert.Single(instanceCall.Arguments);
        Assert.Equal(a0, instanceCall.Arguments[0]);
    }

    [Fact]
    public void Normalize_DoesNotRewrite_CallIntrinsic_WhenArgsArrayHasMoreThanThreeElements()
    {
        var body = new MethodBodyIR();
        var consoleObj = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a1 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a2 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a3 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var argsArray = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRBuildArray(new[] { a0, a1, a2, a3 }, argsArray));
        body.Instructions.Add(new LIRCallIntrinsic(consoleObj, "log", argsArray, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry: null);

        // Should remain unchanged (4 args exceeds the arity-expansion limit of 3).
        Assert.Equal(2, body.Instructions.Count);
        Assert.IsType<LIRBuildArray>(body.Instructions[0]);
        Assert.IsType<LIRCallIntrinsic>(body.Instructions[1]);
    }

    [Fact]
    public void Normalize_Fuses_GetItem_And_ConvertToNumber_Into_GetItemAsNumber_WhenResultUsedOnlyByConvert()
    {
        var classRegistry = new ClassRegistry();
        var body = new MethodBodyIR();

        // receiver: unknown type (object), index: unboxed double
        var receiver = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var getItemResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var numResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRGetItem(receiver, index, getItemResult));
        body.Instructions.Add(new LIRConvertToNumber(getItemResult, numResult));

        LIRIntrinsicNormalization.Normalize(body, classRegistry);

        // Should be fused into a single GetItemAsNumber
        Assert.Single(body.Instructions);
        var fused = Assert.IsType<LIRGetItemAsNumber>(body.Instructions[0]);
        Assert.Equal(receiver, fused.Object);
        Assert.Equal(index, fused.Index);
        Assert.Equal(numResult, fused.Result);

        // numResult storage should be unboxed double
        Assert.Equal(ValueStorageKind.UnboxedValue, body.TempStorages[numResult.Index].Kind);
        Assert.Equal(typeof(double), body.TempStorages[numResult.Index].ClrType);
    }

    [Fact]
    public void Normalize_DoesNotFuse_GetItem_ConvertToNumber_WhenResultUsedElsewhere()
    {
        var classRegistry = new ClassRegistry();
        var body = new MethodBodyIR();

        var receiver = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var getItemResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var numResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var otherResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRGetItem(receiver, index, getItemResult));
        body.Instructions.Add(new LIRConvertToNumber(getItemResult, numResult));
        // getItemResult is also used by another instruction (e.g., boxing it back)
        body.Instructions.Add(new LIRConvertToObject(getItemResult, typeof(object), otherResult));

        LIRIntrinsicNormalization.Normalize(body, classRegistry);

        // Should NOT fuse since getItemResult is used by two instructions
        Assert.Equal(3, body.Instructions.Count);
        Assert.IsType<LIRGetItem>(body.Instructions[0]);
        Assert.IsType<LIRConvertToNumber>(body.Instructions[1]);
    }

    [Fact]
    public void Normalize_DoesNotFuse_GetItem_ConvertToNumber_WhenInstructionIntervenes()
    {
        var classRegistry = new ClassRegistry();
        var body = new MethodBodyIR();

        var receiver = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var getItemResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var numResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var otherResult = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRGetItem(receiver, index, getItemResult));
        body.Instructions.Add(new LIRConvertToObject(receiver, typeof(object), otherResult));
        body.Instructions.Add(new LIRConvertToNumber(getItemResult, numResult));

        LIRIntrinsicNormalization.Normalize(body, classRegistry);

        Assert.Equal(3, body.Instructions.Count);
        Assert.IsType<LIRGetItem>(body.Instructions[0]);
        Assert.IsType<LIRConvertToObject>(body.Instructions[1]);
        Assert.IsType<LIRConvertToNumber>(body.Instructions[2]);
    }

    [Fact]
    public void Normalize_Rewrites_GetItemAsNumber_To_StringSpecificInstruction_WhenIndexIsString()
    {
        var body = new MethodBodyIR();
        var receiver = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var index = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRGetItemAsNumber(receiver, index, result));

        LIRIntrinsicNormalization.Normalize(body, classRegistry: new ClassRegistry());

        var rewritten = Assert.IsType<LIRGetItemAsNumberString>(body.Instructions[0]);
        Assert.Equal(receiver, rewritten.Object);
        Assert.Equal(index, rewritten.Index);
        Assert.Equal(result, rewritten.Result);
    }
}
