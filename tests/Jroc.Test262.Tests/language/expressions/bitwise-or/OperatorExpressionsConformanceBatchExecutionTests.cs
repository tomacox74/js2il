using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.bitwise_or;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/bitwise-or", "language.expressions.bitwise_or") { }

    [Fact(DisplayName = "S11.10.3_A2.2_T1.js")]
    public Task S11_10_3_A2_2_T1()
        => ExecutionTest("S11.10.3_A2.2_T1");

    [Fact(DisplayName = "S11.10.3_A2.3_T1.js")]
    public Task S11_10_3_A2_3_T1()
        => ExecutionTest("S11.10.3_A2.3_T1");

    [Fact(DisplayName = "S11.10.3_A2.4_T3.js")]
    public Task S11_10_3_A2_4_T3()
        => ExecutionTest("S11.10.3_A2.4_T3");

    [Fact(DisplayName = "S11.10.3_A3_T1.1.js")]
    public Task S11_10_3_A3_T1_1()
        => ExecutionTest("S11.10.3_A3_T1.1");

    [Fact(DisplayName = "S11.10.3_A3_T1.2.js")]
    public Task S11_10_3_A3_T1_2()
        => ExecutionTest("S11.10.3_A3_T1.2");

    [Fact(DisplayName = "S11.10.3_A3_T1.3.js")]
    public Task S11_10_3_A3_T1_3()
        => ExecutionTest("S11.10.3_A3_T1.3");

    [Fact(DisplayName = "S11.10.3_A3_T1.4.js")]
    public Task S11_10_3_A3_T1_4()
        => ExecutionTest("S11.10.3_A3_T1.4");

    [Fact(DisplayName = "S11.10.3_A3_T1.5.js")]
    public Task S11_10_3_A3_T1_5()
        => ExecutionTest("S11.10.3_A3_T1.5");

    [Fact(DisplayName = "S11.10.3_A3_T2.1.js")]
    public Task S11_10_3_A3_T2_1()
        => ExecutionTest("S11.10.3_A3_T2.1");

    [Fact(DisplayName = "S11.10.3_A3_T2.2.js")]
    public Task S11_10_3_A3_T2_2()
        => ExecutionTest("S11.10.3_A3_T2.2");

    [Fact(DisplayName = "S11.10.3_A3_T2.3.js")]
    public Task S11_10_3_A3_T2_3()
        => ExecutionTest("S11.10.3_A3_T2.3");

    [Fact(DisplayName = "S11.10.3_A3_T2.4.js")]
    public Task S11_10_3_A3_T2_4()
        => ExecutionTest("S11.10.3_A3_T2.4");

    [Fact(DisplayName = "S11.10.3_A3_T2.5.js")]
    public Task S11_10_3_A3_T2_5()
        => ExecutionTest("S11.10.3_A3_T2.5");

    [Fact(DisplayName = "S11.10.3_A3_T2.6.js")]
    public Task S11_10_3_A3_T2_6()
        => ExecutionTest("S11.10.3_A3_T2.6");

    [Fact(DisplayName = "S11.10.3_A3_T2.7.js")]
    public Task S11_10_3_A3_T2_7()
        => ExecutionTest("S11.10.3_A3_T2.7");

    [Fact(DisplayName = "S11.10.3_A3_T2.8.js")]
    public Task S11_10_3_A3_T2_8()
        => ExecutionTest("S11.10.3_A3_T2.8");

    [Fact(DisplayName = "S11.10.3_A3_T2.9.js")]
    public Task S11_10_3_A3_T2_9()
        => ExecutionTest("S11.10.3_A3_T2.9");

    [Fact(DisplayName = "bigint-and-number.js")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-errors.js")]
    public Task bigint_errors()
        => ExecutionTest("bigint-errors");

    [Fact(DisplayName = "bigint.js")]
    public Task bigint()
        => ExecutionTest("bigint");

    [Fact(DisplayName = "order-of-evaluation.js")]
    public Task order_of_evaluation()
        => ExecutionTest("order-of-evaluation");

}
