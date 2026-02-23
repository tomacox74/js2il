using Js2IL.IR;
using Xunit;

namespace Js2IL.Tests;

public sealed class LIRCoercionCSETests
{
    private static TempVariable AddTemp(MethodBodyIR body, ValueStorage storage)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(-1);
        return temp;
    }

    // -----------------------------------------------------------------------
    // ToNumber CSE
    // -----------------------------------------------------------------------

    [Fact]
    public void Optimize_CsesDuplicateToNumber_WhenSourceIsDouble()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConvertToNumber(source, result1));
        body.Instructions.Add(new LIRConvertToNumber(source, result2));

        LIRCoercionCSE.Optimize(body);

        // First occurrence is kept.
        var first = Assert.IsType<LIRConvertToNumber>(body.Instructions[0]);
        Assert.Equal(source, first.Source);
        Assert.Equal(result1, first.Result);

        // Second occurrence becomes a copy from the first result.
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[1]);
        Assert.Equal(result1, copy.Source);
        Assert.Equal(result2, copy.Destination);
    }

    [Fact]
    public void Optimize_CsesDuplicateToNumber_WhenSourceIsBool()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConvertToNumber(source, result1));
        body.Instructions.Add(new LIRConvertToNumber(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRConvertToNumber>(body.Instructions[0]);
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[1]);
        Assert.Equal(result1, copy.Source);
        Assert.Equal(result2, copy.Destination);
    }

    [Fact]
    public void Optimize_DoesNotCse_ToNumber_WhenSourceIsObject()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConvertToNumber(source, result1));
        body.Instructions.Add(new LIRConvertToNumber(source, result2));

        LIRCoercionCSE.Optimize(body);

        // Both should remain as LIRConvertToNumber (valueOf side-effects possible).
        Assert.IsType<LIRConvertToNumber>(body.Instructions[0]);
        Assert.IsType<LIRConvertToNumber>(body.Instructions[1]);
    }

    [Fact]
    public void Optimize_DoesNotCse_ToNumber_AcrossLabel()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConvertToNumber(source, result1));
        body.Instructions.Add(new LIRLabel(42));        // basic block boundary
        body.Instructions.Add(new LIRConvertToNumber(source, result2));

        LIRCoercionCSE.Optimize(body);

        // The label resets the available-set, so result2 is NOT replaced.
        Assert.IsType<LIRConvertToNumber>(body.Instructions[0]);
        Assert.IsType<LIRLabel>(body.Instructions[1]);
        Assert.IsType<LIRConvertToNumber>(body.Instructions[2]);
    }

    // -----------------------------------------------------------------------
    // ToBoolean CSE
    // -----------------------------------------------------------------------

    [Fact]
    public void Optimize_CsesDuplicateToBoolean_WhenSourceIsBool()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRConvertToBoolean(source, result1));
        body.Instructions.Add(new LIRConvertToBoolean(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRConvertToBoolean>(body.Instructions[0]);
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[1]);
        Assert.Equal(result1, copy.Source);
        Assert.Equal(result2, copy.Destination);
    }

    [Fact]
    public void Optimize_CsesDuplicateToBoolean_WhenSourceIsDouble()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRConvertToBoolean(source, result1));
        body.Instructions.Add(new LIRConvertToBoolean(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRConvertToBoolean>(body.Instructions[0]);
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[1]);
        Assert.Equal(result1, copy.Source);
        Assert.Equal(result2, copy.Destination);
    }

    [Fact]
    public void Optimize_DoesNotCse_ToBoolean_WhenSourceIsObject()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRConvertToBoolean(source, result1));
        body.Instructions.Add(new LIRConvertToBoolean(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRConvertToBoolean>(body.Instructions[0]);
        Assert.IsType<LIRConvertToBoolean>(body.Instructions[1]);
    }

    // -----------------------------------------------------------------------
    // IsTruthy CSE
    // -----------------------------------------------------------------------

    [Fact]
    public void Optimize_CsesDuplicateIsTruthy_WhenSourceIsDouble()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(source, result1));
        body.Instructions.Add(new LIRCallIsTruthy(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRCallIsTruthy>(body.Instructions[0]);
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[1]);
        Assert.Equal(result1, copy.Source);
        Assert.Equal(result2, copy.Destination);
    }

    [Fact]
    public void Optimize_CsesDuplicateIsTruthy_WhenSourceIsBool()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(source, result1));
        body.Instructions.Add(new LIRCallIsTruthy(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRCallIsTruthy>(body.Instructions[0]);
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[1]);
        Assert.Equal(result1, copy.Source);
        Assert.Equal(result2, copy.Destination);
    }

    [Fact]
    public void Optimize_DoesNotCse_IsTruthy_WhenSourceIsObject()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        body.Instructions.Add(new LIRCallIsTruthy(source, result1));
        body.Instructions.Add(new LIRCallIsTruthy(source, result2));

        LIRCoercionCSE.Optimize(body);

        Assert.IsType<LIRCallIsTruthy>(body.Instructions[0]);
        Assert.IsType<LIRCallIsTruthy>(body.Instructions[1]);
    }

    // -----------------------------------------------------------------------
    // Result storage propagation
    // -----------------------------------------------------------------------

    [Fact]
    public void Optimize_PropagatesStorageType_ToCopiedResult()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConvertToNumber(source, result1));
        body.Instructions.Add(new LIRConvertToNumber(source, result2));

        LIRCoercionCSE.Optimize(body);

        // After CSE, result2's storage should match result1's storage.
        var storage2 = body.TempStorages[result2.Index];
        Assert.Equal(ValueStorageKind.UnboxedValue, storage2.Kind);
        Assert.Equal(typeof(double), storage2.ClrType);
    }

    // -----------------------------------------------------------------------
    // Mixed kinds do not interfere
    // -----------------------------------------------------------------------

    [Fact]
    public void Optimize_DoesNotCrossKinds_ToNumberAndToBoolean()
    {
        var body = new MethodBodyIR();
        var source = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var numResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var boolResult = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var numResult2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        // ToNumber, ToBoolean, then ToNumber again on same source.
        body.Instructions.Add(new LIRConvertToNumber(source, numResult));
        body.Instructions.Add(new LIRConvertToBoolean(source, boolResult));
        body.Instructions.Add(new LIRConvertToNumber(source, numResult2));

        LIRCoercionCSE.Optimize(body);

        // First ToNumber is kept.
        Assert.IsType<LIRConvertToNumber>(body.Instructions[0]);
        // ToBoolean is kept (different kind from the first ToNumber).
        Assert.IsType<LIRConvertToBoolean>(body.Instructions[1]);
        // Second ToNumber is CSE'd to a copy.
        var copy = Assert.IsType<LIRCopyTemp>(body.Instructions[2]);
        Assert.Equal(numResult, copy.Source);
        Assert.Equal(numResult2, copy.Destination);
    }

    // -----------------------------------------------------------------------
    // Different source temps are not confused
    // -----------------------------------------------------------------------

    [Fact]
    public void Optimize_DoesNotCse_WhenSourceTempsAreDifferent()
    {
        var body = new MethodBodyIR();
        var source1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var source2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result1 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var result2 = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConvertToNumber(source1, result1));
        body.Instructions.Add(new LIRConvertToNumber(source2, result2));

        LIRCoercionCSE.Optimize(body);

        // Different sources: both conversions are kept.
        Assert.IsType<LIRConvertToNumber>(body.Instructions[0]);
        Assert.IsType<LIRConvertToNumber>(body.Instructions[1]);
    }
}
