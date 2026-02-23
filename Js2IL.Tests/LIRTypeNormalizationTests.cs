using Js2IL.IR;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests;

public sealed class LIRTypeNormalizationTests
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
    public void Normalize_Rewrites_CallIsTruthy_ToCallIsTruthyDouble_WhenValueIsUnboxedDouble()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(value, result));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        var rewritten = Assert.IsType<LIRCallIsTruthyDouble>(body.Instructions[0]);
        Assert.Equal(value, rewritten.Value);
        Assert.Equal(result, rewritten.Result);
    }

    [Fact]
    public void Normalize_Rewrites_CallIsTruthy_ToCallIsTruthyBool_WhenValueIsUnboxedBool()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(value, result));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        var rewritten = Assert.IsType<LIRCallIsTruthyBool>(body.Instructions[0]);
        Assert.Equal(value, rewritten.Value);
        Assert.Equal(result, rewritten.Result);
    }

    [Fact]
    public void Normalize_DoesNotRewrite_CallIsTruthy_WhenValueIsObjectTyped()
    {
        var body = new MethodBodyIR();
        var value = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(value, result));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        // Should remain as LIRCallIsTruthy (object overload).
        Assert.IsType<LIRCallIsTruthy>(body.Instructions[0]);
    }

    [Fact]
    public void Normalize_SpecializesMultiple_CallIsTruthy_InSameMethod()
    {
        var body = new MethodBodyIR();
        var doubleVal = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var boolVal = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var objVal = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var res0 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var res1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var res2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(doubleVal, res0));
        body.Instructions.Add(new LIRCallIsTruthy(boolVal, res1));
        body.Instructions.Add(new LIRCallIsTruthy(objVal, res2));

        LIRTypeNormalization.Normalize(body, classRegistry: null);

        Assert.IsType<LIRCallIsTruthyDouble>(body.Instructions[0]);
        Assert.IsType<LIRCallIsTruthyBool>(body.Instructions[1]);
        Assert.IsType<LIRCallIsTruthy>(body.Instructions[2]);
    }
}
