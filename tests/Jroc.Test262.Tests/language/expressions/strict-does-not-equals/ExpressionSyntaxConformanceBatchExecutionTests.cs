using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.strict_does_not_equals;

public class ExpressionSyntaxConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchExecutionTests() : base("language/expressions/strict-does-not-equals", "language.expressions.strict_does_not_equals") { }

    [Fact(DisplayName = "S11.9.5_A2.4_T4.js")]
    public Task S11_9_5_A2_4_T4()
        => ExecutionTest("S11.9.5_A2.4_T4");

    [Fact(DisplayName = "S11.9.5_A3.js")]
    public Task S11_9_5_A3()
        => ExecutionTest("S11.9.5_A3");

    [Fact(DisplayName = "S11.9.5_A4.1_T1.js")]
    public Task S11_9_5_A4_1_T1()
        => ExecutionTest("S11.9.5_A4.1_T1");

    [Fact(DisplayName = "S11.9.5_A4.1_T2.js")]
    public Task S11_9_5_A4_1_T2()
        => ExecutionTest("S11.9.5_A4.1_T2");

    [Fact(DisplayName = "S11.9.5_A4.2.js")]
    public Task S11_9_5_A4_2()
        => ExecutionTest("S11.9.5_A4.2");

    [Fact(DisplayName = "S11.9.5_A4.3.js")]
    public Task S11_9_5_A4_3()
        => ExecutionTest("S11.9.5_A4.3");

    [Fact(DisplayName = "S11.9.5_A5.js")]
    public Task S11_9_5_A5()
        => ExecutionTest("S11.9.5_A5");

    [Fact(DisplayName = "S11.9.5_A6.2.js")]
    public Task S11_9_5_A6_2()
        => ExecutionTest("S11.9.5_A6.2");

    [Fact(DisplayName = "S11.9.5_A7.js")]
    public Task S11_9_5_A7()
        => ExecutionTest("S11.9.5_A7");

    [Fact(DisplayName = "S11.9.5_A8_T1.js")]
    public Task S11_9_5_A8_T1()
        => ExecutionTest("S11.9.5_A8_T1");

    [Fact(DisplayName = "S11.9.5_A8_T2.js")]
    public Task S11_9_5_A8_T2()
        => ExecutionTest("S11.9.5_A8_T2");

    [Fact(DisplayName = "S11.9.5_A8_T3.js")]
    public Task S11_9_5_A8_T3()
        => ExecutionTest("S11.9.5_A8_T3");

    [Fact(DisplayName = "S11.9.5_A8_T4.js")]
    public Task S11_9_5_A8_T4()
        => ExecutionTest("S11.9.5_A8_T4");

    [Fact(DisplayName = "S11.9.5_A8_T5.js")]
    public Task S11_9_5_A8_T5()
        => ExecutionTest("S11.9.5_A8_T5");

    [Fact(DisplayName = "bigint-and-bigint.js")]
    public Task bigint_and_bigint()
        => ExecutionTest("bigint-and-bigint");

    [Fact(DisplayName = "bigint-and-incomparable-primitive.js")]
    public Task bigint_and_incomparable_primitive()
        => ExecutionTest("bigint-and-incomparable-primitive");

    [Fact(DisplayName = "bigint-and-non-finite.js")]
    public Task bigint_and_non_finite()
        => ExecutionTest("bigint-and-non-finite");

    [Fact(DisplayName = "bigint-and-number.js")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-and-object.js")]
    public Task bigint_and_object()
        => ExecutionTest("bigint-and-object");

}
