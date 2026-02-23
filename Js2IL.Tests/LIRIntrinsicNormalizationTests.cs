using Js2IL.IR;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests;

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
    public void Normalize_Rewrites_AritySpecificRequireCalls_To_LIRCallRequire(int arity)
    {
        var body = new MethodBodyIR();
        var requireValue = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.CommonJS.RequireDelegate)));
        var scopes = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        var a0 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a1 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var a2 = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRBuildScopesArray(System.Array.Empty<ScopeSlotSource>(), scopes));
        body.Instructions.Add(arity switch
        {
            0 => new LIRCallFunctionValue0(requireValue, scopes, result),
            1 => new LIRCallFunctionValue1(requireValue, scopes, a0, result),
            2 => new LIRCallFunctionValue2(requireValue, scopes, a0, a1, result),
            3 => new LIRCallFunctionValue3(requireValue, scopes, a0, a1, a2, result),
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
        var requireValue = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.CommonJS.RequireDelegate)));
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
}
