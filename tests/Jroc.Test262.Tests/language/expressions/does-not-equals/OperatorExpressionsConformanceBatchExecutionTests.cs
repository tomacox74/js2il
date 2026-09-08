using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.does_not_equals;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/does-not-equals", "language.expressions.does_not_equals") { }

    [Fact(DisplayName = "S11.9.2_A2.4_T3.js")]
    public Task S11_9_2_A2_4_T3()
        => ExecutionTest("S11.9.2_A2.4_T3");

    [Fact(DisplayName = "S11.9.2_A2.4_T4.js")]
    public Task S11_9_2_A2_4_T4()
        => ExecutionTest("S11.9.2_A2.4_T4");

    [Fact(DisplayName = "S11.9.2_A3.1.js")]
    public Task S11_9_2_A3_1()
        => ExecutionTest("S11.9.2_A3.1");

    [Fact(DisplayName = "S11.9.2_A3.2.js")]
    public Task S11_9_2_A3_2()
        => ExecutionTest("S11.9.2_A3.2");

    [Fact(DisplayName = "S11.9.2_A3.3.js")]
    public Task S11_9_2_A3_3()
        => ExecutionTest("S11.9.2_A3.3");

    [Fact(DisplayName = "S11.9.2_A4.1_T1.js")]
    public Task S11_9_2_A4_1_T1()
        => ExecutionTest("S11.9.2_A4.1_T1");

    [Fact(DisplayName = "S11.9.2_A4.1_T2.js")]
    public Task S11_9_2_A4_1_T2()
        => ExecutionTest("S11.9.2_A4.1_T2");

    [Fact(DisplayName = "S11.9.2_A4.2.js")]
    public Task S11_9_2_A4_2()
        => ExecutionTest("S11.9.2_A4.2");

    [Fact(DisplayName = "S11.9.2_A4.3.js")]
    public Task S11_9_2_A4_3()
        => ExecutionTest("S11.9.2_A4.3");

    [Fact(DisplayName = "S11.9.2_A5.1.js")]
    public Task S11_9_2_A5_1()
        => ExecutionTest("S11.9.2_A5.1");

    [Fact(DisplayName = "S11.9.2_A5.2.js")]
    public Task S11_9_2_A5_2()
        => ExecutionTest("S11.9.2_A5.2");

    [Fact(DisplayName = "S11.9.2_A5.3.js")]
    public Task S11_9_2_A5_3()
        => ExecutionTest("S11.9.2_A5.3");

    [Fact(DisplayName = "S11.9.2_A6.2_T1.js")]
    public Task S11_9_2_A6_2_T1()
        => ExecutionTest("S11.9.2_A6.2_T1");

    [Fact(DisplayName = "S11.9.2_A6.2_T2.js")]
    public Task S11_9_2_A6_2_T2()
        => ExecutionTest("S11.9.2_A6.2_T2");

    [Fact(DisplayName = "S11.9.2_A7.1.js")]
    public Task S11_9_2_A7_1()
        => ExecutionTest("S11.9.2_A7.1");

    [Fact(DisplayName = "S11.9.2_A7.2.js")]
    public Task S11_9_2_A7_2()
        => ExecutionTest("S11.9.2_A7.2");

    [Fact(DisplayName = "S11.9.2_A7.3.js")]
    public Task S11_9_2_A7_3()
        => ExecutionTest("S11.9.2_A7.3");

    [Fact(DisplayName = "S11.9.2_A7.4.js")]
    public Task S11_9_2_A7_4()
        => ExecutionTest("S11.9.2_A7.4");

    [Fact(DisplayName = "S11.9.2_A7.5.js")]
    public Task S11_9_2_A7_5()
        => ExecutionTest("S11.9.2_A7.5");

    [Fact(DisplayName = "S11.9.2_A7.6.js")]
    public Task S11_9_2_A7_6()
        => ExecutionTest("S11.9.2_A7.6");

    [Fact(DisplayName = "S11.9.2_A7.7.js")]
    public Task S11_9_2_A7_7()
        => ExecutionTest("S11.9.2_A7.7");

    [Fact(DisplayName = "bigint-and-bigint.js")]
    public Task bigint_and_bigint()
        => ExecutionTest("bigint-and-bigint");

    [Fact(DisplayName = "bigint-and-boolean.js")]
    public Task bigint_and_boolean()
        => ExecutionTest("bigint-and-boolean");

    [Fact(DisplayName = "bigint-and-incomparable-primitive.js")]
    public Task bigint_and_incomparable_primitive()
        => ExecutionTest("bigint-and-incomparable-primitive");

    [Fact(DisplayName = "bigint-and-non-finite.js")]
    public Task bigint_and_non_finite()
        => ExecutionTest("bigint-and-non-finite");

    [Fact(DisplayName = "bigint-and-number-extremes.js")]
    public Task bigint_and_number_extremes()
        => ExecutionTest("bigint-and-number-extremes");

    [Fact(DisplayName = "bigint-and-number.js")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-and-object.js")]
    public Task bigint_and_object()
        => ExecutionTest("bigint-and-object");

    [Fact(DisplayName = "bigint-and-string.js")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");

}
