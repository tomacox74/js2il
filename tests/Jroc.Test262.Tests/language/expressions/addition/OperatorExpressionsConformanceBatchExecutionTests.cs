using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.addition;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/addition", "language.expressions.addition") { }

    [Fact(DisplayName = "S11.6.1_A2.1_T1.js")]
    public Task S11_6_1_A2_1_T1()
        => ExecutionTest("S11.6.1_A2.1_T1");

    [Fact(DisplayName = "S11.6.1_A2.1_T2.js")]
    public Task S11_6_1_A2_1_T2()
        => ExecutionTest("S11.6.1_A2.1_T2");

    [Fact(DisplayName = "S11.6.1_A2.2_T1.js")]
    public Task S11_6_1_A2_2_T1()
        => ExecutionTest("S11.6.1_A2.2_T1");

    [Fact(DisplayName = "S11.6.1_A2.2_T2.js")]
    public Task S11_6_1_A2_2_T2()
        => ExecutionTest("S11.6.1_A2.2_T2");

    [Fact(DisplayName = "S11.6.1_A2.2_T3.js")]
    public Task S11_6_1_A2_2_T3()
        => ExecutionTest("S11.6.1_A2.2_T3");

    [Fact(DisplayName = "S11.6.1_A2.4_T1.js")]
    public Task S11_6_1_A2_4_T1()
        => ExecutionTest("S11.6.1_A2.4_T1");

    [Fact(DisplayName = "S11.6.1_A2.4_T3.js")]
    public Task S11_6_1_A2_4_T3()
        => ExecutionTest("S11.6.1_A2.4_T3");

    [Fact(DisplayName = "S11.6.1_A2.4_T4.js")]
    public Task S11_6_1_A2_4_T4()
        => ExecutionTest("S11.6.1_A2.4_T4");

    [Fact(DisplayName = "S11.6.1_A3.1_T1.1.js")]
    public Task S11_6_1_A3_1_T1_1()
        => ExecutionTest("S11.6.1_A3.1_T1.1");

    [Fact(DisplayName = "S11.6.1_A3.1_T1.2.js")]
    public Task S11_6_1_A3_1_T1_2()
        => ExecutionTest("S11.6.1_A3.1_T1.2");

    [Fact(DisplayName = "S11.6.1_A3.1_T1.3.js")]
    public Task S11_6_1_A3_1_T1_3()
        => ExecutionTest("S11.6.1_A3.1_T1.3");

    [Fact(DisplayName = "S11.6.1_A3.1_T2.2.js")]
    public Task S11_6_1_A3_1_T2_2()
        => ExecutionTest("S11.6.1_A3.1_T2.2");

    [Fact(DisplayName = "S11.6.1_A3.1_T2.3.js")]
    public Task S11_6_1_A3_1_T2_3()
        => ExecutionTest("S11.6.1_A3.1_T2.3");

    [Fact(DisplayName = "S11.6.1_A3.1_T2.4.js")]
    public Task S11_6_1_A3_1_T2_4()
        => ExecutionTest("S11.6.1_A3.1_T2.4");

    [Fact(DisplayName = "S11.6.1_A3.1_T2.5.js")]
    public Task S11_6_1_A3_1_T2_5()
        => ExecutionTest("S11.6.1_A3.1_T2.5");

    [Fact(DisplayName = "S11.6.1_A3.2_T1.1.js")]
    public Task S11_6_1_A3_2_T1_1()
        => ExecutionTest("S11.6.1_A3.2_T1.1");

    [Fact(DisplayName = "S11.6.1_A3.2_T1.2.js")]
    public Task S11_6_1_A3_2_T1_2()
        => ExecutionTest("S11.6.1_A3.2_T1.2");

    [Fact(DisplayName = "S11.6.1_A3.2_T2.1.js")]
    public Task S11_6_1_A3_2_T2_1()
        => ExecutionTest("S11.6.1_A3.2_T2.1");

    [Fact(DisplayName = "S11.6.1_A3.2_T2.3.js")]
    public Task S11_6_1_A3_2_T2_3()
        => ExecutionTest("S11.6.1_A3.2_T2.3");

    [Fact(DisplayName = "S11.6.1_A3.2_T2.4.js")]
    public Task S11_6_1_A3_2_T2_4()
        => ExecutionTest("S11.6.1_A3.2_T2.4");

    [Fact(DisplayName = "S11.6.1_A4_T1.js")]
    public Task S11_6_1_A4_T1()
        => ExecutionTest("S11.6.1_A4_T1");

    [Fact(DisplayName = "S11.6.1_A4_T2.js")]
    public Task S11_6_1_A4_T2()
        => ExecutionTest("S11.6.1_A4_T2");

    [Fact(DisplayName = "S11.6.1_A4_T3.js")]
    public Task S11_6_1_A4_T3()
        => ExecutionTest("S11.6.1_A4_T3");

    [Fact(DisplayName = "S11.6.1_A4_T4.js")]
    public Task S11_6_1_A4_T4()
        => ExecutionTest("S11.6.1_A4_T4");

    [Fact(DisplayName = "S11.6.1_A4_T5.js")]
    public Task S11_6_1_A4_T5()
        => ExecutionTest("S11.6.1_A4_T5");

    [Fact(DisplayName = "S11.6.1_A4_T6.js")]
    public Task S11_6_1_A4_T6()
        => ExecutionTest("S11.6.1_A4_T6");

    [Fact(DisplayName = "S11.6.1_A4_T7.js")]
    public Task S11_6_1_A4_T7()
        => ExecutionTest("S11.6.1_A4_T7");

    [Fact(DisplayName = "S11.6.1_A4_T8.js")]
    public Task S11_6_1_A4_T8()
        => ExecutionTest("S11.6.1_A4_T8");

    [Fact(DisplayName = "S11.6.1_A4_T9.js")]
    public Task S11_6_1_A4_T9()
        => ExecutionTest("S11.6.1_A4_T9");

    [Fact(DisplayName = "bigint-errors.js")]
    public Task bigint_errors()
        => ExecutionTest("bigint-errors");

    [Fact(DisplayName = "bigint-wrapped-values.js")]
    public Task bigint_wrapped_values()
        => ExecutionTest("bigint-wrapped-values");

    [Fact(DisplayName = "coerce-symbol-to-prim-err.js")]
    public Task coerce_symbol_to_prim_err()
        => ExecutionTest("coerce-symbol-to-prim-err");

    [Fact(DisplayName = "coerce-symbol-to-prim-return-prim.js")]
    public Task coerce_symbol_to_prim_return_prim()
        => ExecutionTest("coerce-symbol-to-prim-return-prim");

    [Fact(DisplayName = "get-symbol-to-prim-err.js")]
    public Task get_symbol_to_prim_err()
        => ExecutionTest("get-symbol-to-prim-err");

    [Fact(DisplayName = "order-of-evaluation.js")]
    public Task order_of_evaluation()
        => ExecutionTest("order-of-evaluation");

}
