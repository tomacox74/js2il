using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.modulus;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/modulus", "language.expressions.modulus") { }

    [Fact(DisplayName = "S11.5.3_A2.2_T1.js")]
    public Task S11_5_3_A2_2_T1()
        => ExecutionTest("S11.5.3_A2.2_T1");

    [Fact(DisplayName = "S11.5.3_A2.4_T1.js")]
    public Task S11_5_3_A2_4_T1()
        => ExecutionTest("S11.5.3_A2.4_T1");

    [Fact(DisplayName = "S11.5.3_A2.4_T2.js")]
    public Task S11_5_3_A2_4_T2()
        => ExecutionTest("S11.5.3_A2.4_T2");

    [Fact(DisplayName = "S11.5.3_A2.4_T3.js")]
    public Task S11_5_3_A2_4_T3()
        => ExecutionTest("S11.5.3_A2.4_T3");

    [Fact(DisplayName = "S11.5.3_A2.4_T4.js")]
    public Task S11_5_3_A2_4_T4()
        => ExecutionTest("S11.5.3_A2.4_T4");

    [Fact(DisplayName = "S11.5.3_A3_T1.1.js")]
    public Task S11_5_3_A3_T1_1()
        => ExecutionTest("S11.5.3_A3_T1.1");

    [Fact(DisplayName = "S11.5.3_A3_T1.2.js")]
    public Task S11_5_3_A3_T1_2()
        => ExecutionTest("S11.5.3_A3_T1.2");

    [Fact(DisplayName = "S11.5.3_A3_T1.3.js")]
    public Task S11_5_3_A3_T1_3()
        => ExecutionTest("S11.5.3_A3_T1.3");

    [Fact(DisplayName = "S11.5.3_A3_T1.4.js")]
    public Task S11_5_3_A3_T1_4()
        => ExecutionTest("S11.5.3_A3_T1.4");

    [Fact(DisplayName = "S11.5.3_A3_T1.5.js")]
    public Task S11_5_3_A3_T1_5()
        => ExecutionTest("S11.5.3_A3_T1.5");

    [Fact(DisplayName = "S11.5.3_A3_T2.1.js")]
    public Task S11_5_3_A3_T2_1()
        => ExecutionTest("S11.5.3_A3_T2.1");

    [Fact(DisplayName = "S11.5.3_A3_T2.2.js")]
    public Task S11_5_3_A3_T2_2()
        => ExecutionTest("S11.5.3_A3_T2.2");

    [Fact(DisplayName = "S11.5.3_A3_T2.3.js")]
    public Task S11_5_3_A3_T2_3()
        => ExecutionTest("S11.5.3_A3_T2.3");

    [Fact(DisplayName = "S11.5.3_A3_T2.4.js")]
    public Task S11_5_3_A3_T2_4()
        => ExecutionTest("S11.5.3_A3_T2.4");

    [Fact(DisplayName = "S11.5.3_A3_T2.5.js")]
    public Task S11_5_3_A3_T2_5()
        => ExecutionTest("S11.5.3_A3_T2.5");

    [Fact(DisplayName = "S11.5.3_A3_T2.6.js")]
    public Task S11_5_3_A3_T2_6()
        => ExecutionTest("S11.5.3_A3_T2.6");

    [Fact(DisplayName = "S11.5.3_A3_T2.7.js")]
    public Task S11_5_3_A3_T2_7()
        => ExecutionTest("S11.5.3_A3_T2.7");

    [Fact(DisplayName = "S11.5.3_A3_T2.8.js")]
    public Task S11_5_3_A3_T2_8()
        => ExecutionTest("S11.5.3_A3_T2.8");

    [Fact(DisplayName = "S11.5.3_A3_T2.9.js")]
    public Task S11_5_3_A3_T2_9()
        => ExecutionTest("S11.5.3_A3_T2.9");

    [Fact(DisplayName = "S11.5.3_A4_T1.1.js")]
    public Task S11_5_3_A4_T1_1()
        => ExecutionTest("S11.5.3_A4_T1.1");

    [Fact(DisplayName = "S11.5.3_A4_T1.2.js")]
    public Task S11_5_3_A4_T1_2()
        => ExecutionTest("S11.5.3_A4_T1.2");

    [Fact(DisplayName = "S11.5.3_A4_T2.js")]
    public Task S11_5_3_A4_T2()
        => ExecutionTest("S11.5.3_A4_T2");

    [Fact(DisplayName = "S11.5.3_A4_T3.js")]
    public Task S11_5_3_A4_T3()
        => ExecutionTest("S11.5.3_A4_T3");

    [Fact(DisplayName = "S11.5.3_A4_T4.js")]
    public Task S11_5_3_A4_T4()
        => ExecutionTest("S11.5.3_A4_T4");

    [Fact(DisplayName = "S11.5.3_A4_T5.js")]
    public Task S11_5_3_A4_T5()
        => ExecutionTest("S11.5.3_A4_T5");

    [Fact(DisplayName = "S11.5.3_A4_T6.js")]
    public Task S11_5_3_A4_T6()
        => ExecutionTest("S11.5.3_A4_T6");

    [Fact(DisplayName = "S11.5.3_A4_T7.js")]
    public Task S11_5_3_A4_T7()
        => ExecutionTest("S11.5.3_A4_T7");

    [Fact(DisplayName = "bigint-and-number.js")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-arithmetic.js")]
    public Task bigint_arithmetic()
        => ExecutionTest("bigint-arithmetic");

    [Fact(DisplayName = "bigint-errors.js")]
    public Task bigint_errors()
        => ExecutionTest("bigint-errors");

    [Fact(DisplayName = "bigint-modulo-zero.js")]
    public Task bigint_modulo_zero()
        => ExecutionTest("bigint-modulo-zero");

    [Fact(DisplayName = "bigint-toprimitive.js")]
    public Task bigint_toprimitive()
        => ExecutionTest("bigint-toprimitive");

    [Fact(DisplayName = "bigint-wrapped-values.js")]
    public Task bigint_wrapped_values()
        => ExecutionTest("bigint-wrapped-values");

    [Fact(DisplayName = "line-terminator.js")]
    public Task line_terminator()
        => ExecutionTest("line-terminator");

    [Fact(DisplayName = "order-of-evaluation.js")]
    public Task order_of_evaluation()
        => ExecutionTest("order-of-evaluation");

}
