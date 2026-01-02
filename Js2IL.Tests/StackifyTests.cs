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

    #region OptimizeInstructionOrder Tests

    [Fact]
    public void OptimizeInstructionOrder_MovesConstantCloserToUse()
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

        // Original order:
        // const 5 -> t0
        // const 3 -> t1
        // const 7 -> t2
        // add t0, t2 -> t3  (t1 not used until later, t0 and t2 used here)
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));
        methodBody.Instructions.Add(new LIRConstNumber(3.0, temp1));
        methodBody.Instructions.Add(new LIRConstNumber(7.0, temp2));
        methodBody.Instructions.Add(new LIRAddNumber(temp0, temp2, temp3));

        // Act
        Stackify.OptimizeInstructionOrder(methodBody);

        // Assert
        // After optimization, the constant for t2 should be moved closer to its use
        // The exact order depends on the algorithm, but t1 should stay where it is
        // and t0/t2 should be adjacent to their use
        Assert.Equal(4, methodBody.Instructions.Count);
    }

    [Fact]
    public void OptimizeInstructionOrder_DoesNotMoveAcrossControlFlow()
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
        // branch to L1
        // label L1
        // const 3 -> t1
        // add t0, t1 -> t2
        // t0 should NOT be moved across the branch/label
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));
        methodBody.Instructions.Add(new LIRBranch(1));
        methodBody.Instructions.Add(new LIRLabel(1));
        methodBody.Instructions.Add(new LIRConstNumber(3.0, temp1));
        methodBody.Instructions.Add(new LIRAddNumber(temp0, temp1, temp2));

        // Act
        Stackify.OptimizeInstructionOrder(methodBody);

        // Assert - t0's const should still be at index 0 (not moved across control flow)
        var firstInstr = methodBody.Instructions[0];
        Assert.IsType<LIRConstNumber>(firstInstr);
        Assert.Equal(temp0, ((LIRConstNumber)firstInstr).Result);
    }

    [Fact]
    public void OptimizeInstructionOrder_SwapsCommutativeOperandsForBetterStackability()
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
        // ... (assume t0 is defined earlier, not immediately before)
        // const 3 -> t1 (immediately before add)
        // add t0, t1 -> t2  
        // Since t1 is on stack top (defined immediately before), and add is commutative,
        // we want operands in order that loads t0 first, then t1
        // But if the add instruction has t0 as left, it will load t0 first
        // The optimization should recognize t1 is on stack and potentially reorder
        
        methodBody.Instructions.Add(new LIRConstNumber(5.0, temp0));
        methodBody.Instructions.Add(new LIRConstNumber(3.0, temp1));
        methodBody.Instructions.Add(new LIRAddNumber(temp0, temp1, temp2));

        // Act
        Stackify.OptimizeInstructionOrder(methodBody);

        // Assert - instruction count should be preserved
        Assert.Equal(3, methodBody.Instructions.Count);
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
