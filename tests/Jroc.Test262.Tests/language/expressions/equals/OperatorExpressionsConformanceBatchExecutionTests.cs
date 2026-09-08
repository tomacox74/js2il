using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.equals;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/equals", "language.expressions.equals") { }

    [Fact(DisplayName = "S11.9.1_A2.1_T1.js")]
    public Task S11_9_1_A2_1_T1()
        => ExecutionTest("S11.9.1_A2.1_T1");

    [Fact(DisplayName = "S11.9.1_A2.1_T2.js")]
    public Task S11_9_1_A2_1_T2()
        => ExecutionTest("S11.9.1_A2.1_T2");

    [Fact(DisplayName = "S11.9.1_A2.1_T3.js")]
    public Task S11_9_1_A2_1_T3()
        => ExecutionTest("S11.9.1_A2.1_T3");

    [Fact(DisplayName = "S11.9.1_A2.4_T1.js")]
    public Task S11_9_1_A2_4_T1()
        => ExecutionTest("S11.9.1_A2.4_T1");

    [Fact(DisplayName = "S11.9.1_A2.4_T3.js")]
    public Task S11_9_1_A2_4_T3()
        => ExecutionTest("S11.9.1_A2.4_T3");

    [Fact(DisplayName = "S11.9.1_A2.4_T4.js")]
    public Task S11_9_1_A2_4_T4()
        => ExecutionTest("S11.9.1_A2.4_T4");

    [Fact(DisplayName = "S11.9.1_A3.1.js")]
    public Task S11_9_1_A3_1()
        => ExecutionTest("S11.9.1_A3.1");

    [Fact(DisplayName = "S11.9.1_A3.2.js")]
    public Task S11_9_1_A3_2()
        => ExecutionTest("S11.9.1_A3.2");

    [Fact(DisplayName = "S11.9.1_A3.3.js")]
    public Task S11_9_1_A3_3()
        => ExecutionTest("S11.9.1_A3.3");

    [Fact(DisplayName = "S11.9.1_A4.1_T1.js")]
    public Task S11_9_1_A4_1_T1()
        => ExecutionTest("S11.9.1_A4.1_T1");

    [Fact(DisplayName = "S11.9.1_A4.1_T2.js")]
    public Task S11_9_1_A4_1_T2()
        => ExecutionTest("S11.9.1_A4.1_T2");

    [Fact(DisplayName = "S11.9.1_A4.3.js")]
    public Task S11_9_1_A4_3()
        => ExecutionTest("S11.9.1_A4.3");

    [Fact(DisplayName = "S11.9.1_A5.2.js")]
    public Task S11_9_1_A5_2()
        => ExecutionTest("S11.9.1_A5.2");

    [Fact(DisplayName = "S11.9.1_A5.3.js")]
    public Task S11_9_1_A5_3()
        => ExecutionTest("S11.9.1_A5.3");

    [Fact(DisplayName = "S11.9.1_A6.2_T2.js")]
    public Task S11_9_1_A6_2_T2()
        => ExecutionTest("S11.9.1_A6.2_T2");

    [Fact(DisplayName = "S11.9.1_A7.1.js")]
    public Task S11_9_1_A7_1()
        => ExecutionTest("S11.9.1_A7.1");

    [Fact(DisplayName = "S11.9.1_A7.2.js")]
    public Task S11_9_1_A7_2()
        => ExecutionTest("S11.9.1_A7.2");

    [Fact(DisplayName = "S11.9.1_A7.3.js")]
    public Task S11_9_1_A7_3()
        => ExecutionTest("S11.9.1_A7.3");

    [Fact(DisplayName = "S11.9.1_A7.4.js")]
    public Task S11_9_1_A7_4()
        => ExecutionTest("S11.9.1_A7.4");

    [Fact(DisplayName = "S11.9.1_A7.5.js")]
    public Task S11_9_1_A7_5()
        => ExecutionTest("S11.9.1_A7.5");

    [Fact(DisplayName = "S11.9.1_A7.6.js")]
    public Task S11_9_1_A7_6()
        => ExecutionTest("S11.9.1_A7.6");

    [Fact(DisplayName = "S11.9.1_A7.7.js")]
    public Task S11_9_1_A7_7()
        => ExecutionTest("S11.9.1_A7.7");

    [Fact(DisplayName = "S9.1_A1_T3.js")]
    public Task S9_1_A1_T3()
        => ExecutionTest("S9.1_A1_T3");

    [Fact(DisplayName = "bigint-and-number-extremes.js")]
    public Task bigint_and_number_extremes()
        => ExecutionTest("bigint-and-number-extremes");

    [Fact(DisplayName = "coerce-symbol-to-prim-err.js")]
    public Task coerce_symbol_to_prim_err()
        => ExecutionTest("coerce-symbol-to-prim-err");

    [Fact(DisplayName = "coerce-symbol-to-prim-invocation.js")]
    public Task coerce_symbol_to_prim_invocation()
        => ExecutionTest("coerce-symbol-to-prim-invocation");

    [Fact(DisplayName = "coerce-symbol-to-prim-return-obj.js")]
    public Task coerce_symbol_to_prim_return_obj()
        => ExecutionTest("coerce-symbol-to-prim-return-obj");

    [Fact(DisplayName = "get-symbol-to-prim-err.js")]
    public Task get_symbol_to_prim_err()
        => ExecutionTest("get-symbol-to-prim-err");

    [Fact(DisplayName = "symbol-strict-equality-comparison.js")]
    public Task symbol_strict_equality_comparison()
        => ExecutionTest("symbol-strict-equality-comparison");

}
