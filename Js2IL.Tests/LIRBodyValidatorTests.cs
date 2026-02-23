using System;
using Xunit;
using Js2IL.IR;
using Js2IL.IL;

namespace Js2IL.Tests;

/// <summary>
/// Unit tests for <see cref="LIRBodyValidator"/>.
/// </summary>
public class LIRBodyValidatorTests
{
    // -----------------------------------------------------------------------
    // Slot materialization invariant
    // -----------------------------------------------------------------------

    [Fact]
    public void Validate_SlotMaterialization_PassesWhenAllSlotTaggedTempsAreDefined()
    {
        var body = new MethodBodyIR();

        // Create a temp and give it a variable slot — backed by a LIRCopyTemp.
        var sourceTemp = new TempVariable(0);
        body.Temps.Add(sourceTemp);
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.Add(-1); // sourceTemp has no slot

        var slotTaggedTemp = new TempVariable(1);
        body.Temps.Add(slotTaggedTemp);
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.Add(0); // tagged with variable slot 0
        body.VariableNames.Add("$x");
        body.VariableStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        body.Instructions.Add(new LIRConstNumber(1.0, sourceTemp));
        body.Instructions.Add(new LIRCopyTemp(sourceTemp, slotTaggedTemp));

        // Should not throw — slotTaggedTemp is defined by LIRCopyTemp.
        LIRBodyValidator.Validate(body);
    }

    [Fact]
    public void Validate_SlotMaterialization_ThrowsWhenSlotTaggedTempHasNoDefiningInstruction()
    {
        var body = new MethodBodyIR();

        // Create a temp, tag it with a slot, but add NO defining instruction.
        var phantom = new TempVariable(0);
        body.Temps.Add(phantom);
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
        body.TempVariableSlots.Add(0); // tagged with variable slot 0
        body.VariableNames.Add("$phantom");
        body.VariableStorages.Add(new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));

        // No instruction that defines 'phantom' is added.

        var ex = Assert.Throws<InvalidOperationException>(() => LIRBodyValidator.Validate(body));
        Assert.Contains("slot materialization invariant violated", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("temp 0", ex.Message);
        Assert.Contains("variable slot 0", ex.Message);
    }

    [Fact]
    public void Validate_SlotMaterialization_PassesWhenNoSlotsAreTagged()
    {
        var body = new MethodBodyIR();
        var t = new TempVariable(0);
        body.Temps.Add(t);
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.Add(-1); // not tagged
        body.Instructions.Add(new LIRConstNumber(42.0, t));

        // No slot-tagged temps, so no invariant to check.
        LIRBodyValidator.Validate(body);
    }

    // -----------------------------------------------------------------------
    // Numeric lowering invariant
    // -----------------------------------------------------------------------

    [Fact]
    public void Validate_NumericLowering_PassesWhenArithmeticOperandsAreUnboxedDoubles()
    {
        var body = new MethodBodyIR();

        var left = new TempVariable(0);
        var right = new TempVariable(1);
        var result = new TempVariable(2);

        body.Temps.AddRange(new[] { left, right, result });
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.AddRange(new[] { -1, -1, -1 });

        body.Instructions.Add(new LIRConstNumber(1.0, left));
        body.Instructions.Add(new LIRConstNumber(2.0, right));
        body.Instructions.Add(new LIRAddNumber(left, right, result));

        // Should not throw.
        LIRBodyValidator.Validate(body);
    }

    [Fact]
    public void Validate_NumericLowering_ThrowsWhenAddNumberOperandIsNotUnboxedDouble()
    {
        var body = new MethodBodyIR();

        var left = new TempVariable(0);
        var right = new TempVariable(1);
        var result = new TempVariable(2);

        body.Temps.AddRange(new[] { left, right, result });
        // left is a boxed object — not an unboxed double
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.AddRange(new[] { -1, -1, -1 });

        body.Instructions.Add(new LIRConstNumber(2.0, right));
        body.Instructions.Add(new LIRAddNumber(left, right, result));

        var ex = Assert.Throws<InvalidOperationException>(() => LIRBodyValidator.Validate(body));
        Assert.Contains("numeric lowering invariant violated", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("LIRAddNumber", ex.Message);
        Assert.Contains("Left", ex.Message);
    }

    [Fact]
    public void Validate_NumericLowering_ThrowsWhenSubNumberOperandIsUnknownStorage()
    {
        var body = new MethodBodyIR();

        var left = new TempVariable(0);
        var right = new TempVariable(1);
        var result = new TempVariable(2);

        body.Temps.AddRange(new[] { left, right, result });
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        // right has Unknown storage (e.g., a temp whose DefineTempStorage was never called)
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.Unknown));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.AddRange(new[] { -1, -1, -1 });

        body.Instructions.Add(new LIRConstNumber(1.0, left));
        body.Instructions.Add(new LIRSubNumber(left, right, result));

        var ex = Assert.Throws<InvalidOperationException>(() => LIRBodyValidator.Validate(body));
        Assert.Contains("numeric lowering invariant violated", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("LIRSubNumber", ex.Message);
        Assert.Contains("Right", ex.Message);
    }

    [Fact]
    public void Validate_NumericLowering_ThrowsWhenCompareNumberOperandIsBoxed()
    {
        var body = new MethodBodyIR();

        var left = new TempVariable(0);
        var right = new TempVariable(1);
        var result = new TempVariable(2);

        body.Temps.AddRange(new[] { left, right, result });
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        body.TempVariableSlots.AddRange(new[] { -1, -1, -1 });

        body.Instructions.Add(new LIRConstNumber(2.0, right));
        body.Instructions.Add(new LIRCompareNumberLessThan(left, right, result));

        var ex = Assert.Throws<InvalidOperationException>(() => LIRBodyValidator.Validate(body));
        Assert.Contains("numeric lowering invariant violated", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("LIRCompareNumberLessThan", ex.Message);
    }

    [Fact]
    public void Validate_NumericLowering_PassesForNegateNumberWithUnboxedDouble()
    {
        var body = new MethodBodyIR();

        var value = new TempVariable(0);
        var result = new TempVariable(1);

        body.Temps.AddRange(new[] { value, result });
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.AddRange(new[] { -1, -1 });

        body.Instructions.Add(new LIRConstNumber(5.0, value));
        body.Instructions.Add(new LIRNegateNumber(value, result));

        // Should not throw.
        LIRBodyValidator.Validate(body);
    }

    [Fact]
    public void Validate_NumericLowering_ThrowsForNegateNumberWithBoxedOperand()
    {
        var body = new MethodBodyIR();

        var value = new TempVariable(0);
        var result = new TempVariable(1);

        body.Temps.AddRange(new[] { value, result });
        // value is boxed — should have gone through EnsureNumber first
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        body.TempVariableSlots.AddRange(new[] { -1, -1 });

        body.Instructions.Add(new LIRNegateNumber(value, result));

        var ex = Assert.Throws<InvalidOperationException>(() => LIRBodyValidator.Validate(body));
        Assert.Contains("numeric lowering invariant violated", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("LIRNegateNumber", ex.Message);
        Assert.Contains("Value", ex.Message);
    }

    // -----------------------------------------------------------------------
    // Empty body / edge cases
    // -----------------------------------------------------------------------

    [Fact]
    public void Validate_EmptyBody_Passes()
    {
        var body = new MethodBodyIR();
        // Should not throw for an empty method body.
        LIRBodyValidator.Validate(body);
    }

    [Fact]
    public void Validate_SlotMaterialization_StoreExceptionCountsAsDefinition()
    {
        var body = new MethodBodyIR();

        // Simulate a catch handler: exTemp is defined by LIRStoreException.
        var exTemp = new TempVariable(0);
        body.Temps.Add(exTemp);
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.Reference, typeof(Exception)));
        body.TempVariableSlots.Add(0); // tagged with variable slot
        body.VariableNames.Add("$catch_ex");
        body.VariableStorages.Add(new ValueStorage(ValueStorageKind.Reference, typeof(Exception)));

        body.Instructions.Add(new LIRStoreException(exTemp));

        // Should not throw — LIRStoreException is recognised as a defining instruction.
        LIRBodyValidator.Validate(body);
    }

    [Fact]
    public void Validate_SlotMaterialization_UnwrapCatchExceptionCountsAsDefinition()
    {
        var body = new MethodBodyIR();

        var exTemp = new TempVariable(0);
        var jsValue = new TempVariable(1);

        body.Temps.AddRange(new[] { exTemp, jsValue });
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.Reference, typeof(Exception)));
        body.TempStorages.Add(new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        body.TempVariableSlots.AddRange(new[] { -1, 0 }); // jsValue tagged
        body.VariableNames.Add("$catch_value");
        body.VariableStorages.Add(new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        body.Instructions.Add(new LIRStoreException(exTemp));
        body.Instructions.Add(new LIRUnwrapCatchException(exTemp, jsValue));

        // Should not throw — LIRUnwrapCatchException is recognised as a defining instruction.
        LIRBodyValidator.Validate(body);
    }
}
