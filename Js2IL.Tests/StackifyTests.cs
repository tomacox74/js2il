using Js2IL.IL;
using Js2IL.IR;

namespace Js2IL.Tests;

public class StackifyTests
{
    #region Analyze Tests

    [Fact]
    public void Analyze_EmptyMethodBody_ReturnsEmptyResult()
    {
        // Arrange
        var methodBody = new MethodBodyIR();

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.Empty(result.CanStackify);
    }

    [Fact]
    public void Analyze_SingleUseConstantImmediatelyConsumed_IsStackable()
    {
        // Arrange
        var methodBody = new MethodBodyIR();
        var temp0 = new TempVariable(0);
        var temp1 = new TempVariable(1);
        var temp2 = new TempVariable(2);

        methodBody.Temps.Add(temp0);
        methodBody.Temps.Add(temp1);
        methodBody.Temps.Add(temp2);

        // const 5 -> t0
        // const 3 -> t1
        // add t0, t1 -> t2
        // When evaluating add: load t0, load t1, add
        // t1 is defined right before use, so could be stackable
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));
        methodBody.Instructions.Add(new LIRConstNumber(3.0, temp1));
        methodBody.Instructions.Add(new LIRAddNumber(temp0, temp1, temp2));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.Equal(3, result.CanStackify.Length);
        // temp1 is defined at index 1, used at index 2 as second operand (consumed from top of stack)
        Assert.True(result.IsStackable(temp1), "temp1 should be stackable (immediately used)");
    }

    [Fact]
    public void Analyze_TempUsedMultipleTimes_NotStackable()
    {
        // Arrange
        var methodBody = new MethodBodyIR();
        var temp0 = new TempVariable(0);
        var temp1 = new TempVariable(1);
        var temp2 = new TempVariable(2);

        methodBody.Temps.Add(temp0);
        methodBody.Temps.Add(temp1);
        methodBody.Temps.Add(temp2);

        // const 5 -> t0
        // add t0, t0 -> t1  (t0 used twice)
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));
        methodBody.Instructions.Add(new LIRAddNumber(temp0, temp0, temp1));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.False(result.IsStackable(temp0), "temp0 should not be stackable (used twice)");
    }

    [Fact]
    public void Analyze_TempWithControlFlowBetweenDefAndUse_NotStackable()
    {
        // Arrange
        var methodBody = new MethodBodyIR();
        var temp0 = new TempVariable(0);
        var temp1 = new TempVariable(1);
        var temp2 = new TempVariable(2);
        var temp3 = new TempVariable(3);

        methodBody.Temps.Add(temp0);
        methodBody.Temps.Add(temp1);
        methodBody.Temps.Add(temp2);
        methodBody.Temps.Add(temp3);

        // const 5 -> t0
        // label L1
        // const 3 -> t1
        // add t0, t1 -> t2
        // There's a label between t0 def and t0 use - not stackable
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));
        methodBody.Instructions.Add(new LIRLabel(1));
        methodBody.Instructions.Add(new LIRConstNumber(3.0, temp1));
        methodBody.Instructions.Add(new LIRAddNumber(temp0, temp1, temp2));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.False(result.IsStackable(temp0), "temp0 should not be stackable (control flow between def and use)");
    }

    [Fact]
    public void Analyze_UnusedTemp_NotStackable()
    {
        // Arrange
        var methodBody = new MethodBodyIR();
        var temp0 = new TempVariable(0);

        methodBody.Temps.Add(temp0);

        // const 5 -> t0, but never used
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.False(result.IsStackable(temp0), "Unused temp should not be stackable");
    }

    [Fact]
    public void Analyze_ReturnImmediatelyAfterConst_IsStackable()
    {
        // Arrange
        var methodBody = new MethodBodyIR();
        var temp0 = new TempVariable(0);

        methodBody.Temps.Add(temp0);

        // const 42 -> t0
        // return t0
        methodBody.Instructions.Add(new LIRConstNumber(42.0, temp0));
        methodBody.Instructions.Add(new LIRReturn(temp0));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.True(result.IsStackable(temp0), "temp0 should be stackable (return immediately after const)");
    }

    #endregion

    #region IsStackable Tests

    [Fact]
    public void IsStackable_InvalidTempIndex_ReturnsFalse()
    {
        // Arrange
        var result = new StackifyResult(new[] { true, false });

        // Act & Assert
        Assert.False(result.IsStackable(new TempVariable(-1)));
        Assert.False(result.IsStackable(new TempVariable(5)));
    }

    [Fact]
    public void IsStackable_ValidTempIndex_ReturnsCorrectValue()
    {
        // Arrange
        var result = new StackifyResult(new[] { true, false, true });

        // Act & Assert
        Assert.True(result.IsStackable(new TempVariable(0)));
        Assert.False(result.IsStackable(new TempVariable(1)));
        Assert.True(result.IsStackable(new TempVariable(2)));
    }

    #endregion
}
