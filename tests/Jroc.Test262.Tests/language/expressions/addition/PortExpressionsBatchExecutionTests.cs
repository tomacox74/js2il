using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.addition;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.addition") { }

    [Fact(DisplayName = "S11.6.1_A2.1_T3")]
    public Task S11_6_1_A2_1_T3()
        => ExecutionTest("S11.6.1_A2.1_T3");

    [Fact(DisplayName = "S11.6.1_A2.4_T2")]
    public Task S11_6_1_A2_4_T2()
        => ExecutionTest("S11.6.1_A2.4_T2");

    [Fact(DisplayName = "S11.6.1_A2.3_T1")]
    public Task S11_6_1_A2_3_T1()
        => ExecutionTest("S11.6.1_A2.3_T1");

    [Fact(DisplayName = "S11.6.1_A3.1_T2.1")]
    public Task S11_6_1_A3_1_T2_1()
        => ExecutionTest("S11.6.1_A3.1_T2.1");

    [Fact(DisplayName = "S11.6.1_A3.2_T2.2")]
    public Task S11_6_1_A3_2_T2_2()
        => ExecutionTest("S11.6.1_A3.2_T2.2");

    [Fact(DisplayName = "bigint-and-number")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-arithmetic")]
    public Task bigint_arithmetic()
        => ExecutionTest("bigint-arithmetic");

    [Fact(DisplayName = "symbol-to-string")]
    public Task symbol_to_string()
        => ExecutionTest("symbol-to-string");
}
