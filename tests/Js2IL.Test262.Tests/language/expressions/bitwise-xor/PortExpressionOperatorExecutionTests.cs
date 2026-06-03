using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.bitwise_xor;

public class PortExpressionOperatorExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionOperatorExecutionTests() : base("language.expressions.bitwise_xor") { }

    [Fact(DisplayName = "S11.10.2_A2.1_T1")]
    public Task S11_10_2_A2_1_T1()
        => ExecutionTest("S11.10.2_A2.1_T1");

    [Fact(DisplayName = "S11.10.2_A2.1_T2")]
    public Task S11_10_2_A2_1_T2()
        => ExecutionTest("S11.10.2_A2.1_T2");

    [Fact(DisplayName = "S11.10.2_A2.1_T3")]
    public Task S11_10_2_A2_1_T3()
        => ExecutionTest("S11.10.2_A2.1_T3");

    [Fact(DisplayName = "S11.10.2_A2.4_T1")]
    public Task S11_10_2_A2_4_T1()
        => ExecutionTest("S11.10.2_A2.4_T1");

    [Fact(DisplayName = "S11.10.2_A2.4_T2")]
    public Task S11_10_2_A2_4_T2()
        => ExecutionTest("S11.10.2_A2.4_T2");
}
