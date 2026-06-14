using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.bitwise_or;

public class PortExpressionOperatorExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionOperatorExecutionTests() : base("language.expressions.bitwise_or") { }

    [Fact(DisplayName = "S11.10.3_A2.1_T1")]
    public Task S11_10_3_A2_1_T1()
        => ExecutionTest("S11.10.3_A2.1_T1");

    [Fact(DisplayName = "S11.10.3_A2.1_T2")]
    public Task S11_10_3_A2_1_T2()
        => ExecutionTest("S11.10.3_A2.1_T2");

    [Fact(DisplayName = "S11.10.3_A2.1_T3")]
    public Task S11_10_3_A2_1_T3()
        => ExecutionTest("S11.10.3_A2.1_T3");

    [Fact(DisplayName = "S11.10.3_A2.4_T1")]
    public Task S11_10_3_A2_4_T1()
        => ExecutionTest("S11.10.3_A2.4_T1");

    [Fact(DisplayName = "S11.10.3_A2.4_T2")]
    public Task S11_10_3_A2_4_T2()
        => ExecutionTest("S11.10.3_A2.4_T2");
}
