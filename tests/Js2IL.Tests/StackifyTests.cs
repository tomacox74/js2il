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

    #region Bug Reproduction Tests

    /// <summary>
    /// Reproduces the bug found by the code reviewer where temps that are results of
    /// binary operations (like LIRAddDynamic) were incorrectly marked as stackable.
    /// 
    /// When the IL emitter encounters a stackable temp, it re-emits the defining instruction
    /// inline. For constants this is fine, but for operations like "Hello, " + name,
    /// re-emitting causes duplicate computation.
    /// 
    /// Example JavaScript: return "Hello, " + name + "!";
    /// LIR:
    ///   t0 = "Hello, "
    ///   t1 = ldarg name
    ///   t2 = t0 + t1         // "Hello, " + name
    ///   t3 = "!"
    ///   t4 = t2 + t3         // ("Hello, " + name) + "!"
    ///   return t4
    /// 
    /// Bug: t2 was marked stackable because LIRAddDynamic was in CanEmitInline.
    /// This caused the IL emitter to re-emit "Hello, " + name multiple times.
    /// </summary>
    [Fact]
    public void Analyze_BinaryOperationResult_NotStackable()
    {
        // Arrange - simulates: return "Hello, " + name + "!"
        var methodBody = new MethodBodyIR();
        var t0 = new TempVariable(0); // "Hello, "
        var t1 = new TempVariable(1); // name (parameter)
        var t2 = new TempVariable(2); // "Hello, " + name
        var t3 = new TempVariable(3); // "!"
        var t4 = new TempVariable(4); // ("Hello, " + name) + "!"

        methodBody.Temps.Add(t0);
        methodBody.Temps.Add(t1);
        methodBody.Temps.Add(t2);
        methodBody.Temps.Add(t3);
        methodBody.Temps.Add(t4);

        // t0 = "Hello, "
        methodBody.Instructions.Add(new LIRConstString("Hello, ", t0));
        // t1 = ldarg.1 (name parameter)
        methodBody.Instructions.Add(new LIRLoadParameter(1, t1));
        // t2 = t0 + t1 (dynamic add for string concatenation)
        methodBody.Instructions.Add(new LIRAddDynamic(t0, t1, t2));
        // t3 = "!"
        methodBody.Instructions.Add(new LIRConstString("!", t3));
        // t4 = t2 + t3
        methodBody.Instructions.Add(new LIRAddDynamic(t2, t3, t4));
        // return t4
        methodBody.Instructions.Add(new LIRReturn(t4));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        // t0 and t1 could be stackable (simple constants/parameters used once)
        // But t2 (result of binary op) should NOT be stackable!
        // If t2 is marked stackable, the IL emitter will re-emit the entire
        // "Hello, " + name computation when loading t2 for the second add.
        Assert.False(result.IsStackable(t2), 
            "t2 (result of LIRAddDynamic) should NOT be stackable - " +
            "re-emitting would cause duplicate computation of the binary operation");

        // t4 is also a binary op result, but it's only used by return which consumes it immediately,
        // so whether it's stackable or not doesn't cause duplicate computation.
        // However, for consistency, binary op results should not be stackable.
        Assert.False(result.IsStackable(t4),
            "t4 (result of LIRAddDynamic) should NOT be stackable for consistency");
    }

    /// <summary>
    /// Verifies that temps defined by multiplication operations are not stackable.
    /// </summary>
    [Fact]
    public void Analyze_MultiplyOperationResult_NotStackable()
    {
        // Arrange - simulates: return (a * b) * c
        var methodBody = new MethodBodyIR();
        var t0 = new TempVariable(0); // a
        var t1 = new TempVariable(1); // b
        var t2 = new TempVariable(2); // a * b
        var t3 = new TempVariable(3); // c
        var t4 = new TempVariable(4); // (a * b) * c

        methodBody.Temps.Add(t0);
        methodBody.Temps.Add(t1);
        methodBody.Temps.Add(t2);
        methodBody.Temps.Add(t3);
        methodBody.Temps.Add(t4);

        methodBody.Instructions.Add(new LIRLoadParameter(1, t0));
        methodBody.Instructions.Add(new LIRLoadParameter(2, t1));
        methodBody.Instructions.Add(new LIRMulDynamic(t0, t1, t2));
        methodBody.Instructions.Add(new LIRLoadParameter(3, t3));
        methodBody.Instructions.Add(new LIRMulDynamic(t2, t3, t4));
        methodBody.Instructions.Add(new LIRReturn(t4));

        // Act
        var result = Stackify.Analyze(methodBody);

        // Assert
        Assert.False(result.IsStackable(t2),
            "t2 (result of LIRMulDynamic) should NOT be stackable");
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
