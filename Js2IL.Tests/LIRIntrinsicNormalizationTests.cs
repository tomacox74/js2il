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
}
