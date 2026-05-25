using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.equals;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.equals") { }

    [Fact(DisplayName = "S11.9.1_A2.4_T2")]
    public Task S11_9_1_A2_4_T2()
        => ExecutionTest("S11.9.1_A2.4_T2");

    [Fact(DisplayName = "S11.9.1_A6.2_T1")]
    public Task S11_9_1_A6_2_T1()
        => ExecutionTest("S11.9.1_A6.2_T1");

    [Fact(DisplayName = "S11.9.1_A4.2")]
    public Task S11_9_1_A4_2()
        => ExecutionTest("S11.9.1_A4.2");

    [Fact(DisplayName = "S11.9.1_A5.1")]
    public Task S11_9_1_A5_1()
        => ExecutionTest("S11.9.1_A5.1");

    [Fact(DisplayName = "bigint-and-bigint")]
    public Task bigint_and_bigint()
        => ExecutionTest("bigint-and-bigint");

    [Fact(DisplayName = "bigint-and-number")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-and-object")]
    public Task bigint_and_object()
        => ExecutionTest("bigint-and-object");

    [Fact(DisplayName = "bigint-and-string")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");

    [Fact(DisplayName = "coerce-symbol-to-prim-return-prim")]
    public Task coerce_symbol_to_prim_return_prim()
        => ExecutionTest("coerce-symbol-to-prim-return-prim");

    [Fact(DisplayName = "symbol-abstract-equality-comparison")]
    public Task symbol_abstract_equality_comparison()
        => ExecutionTest("symbol-abstract-equality-comparison");

    [Fact(DisplayName = "to-prim-hint")]
    public Task to_prim_hint()
        => ExecutionTest("to-prim-hint");
}
