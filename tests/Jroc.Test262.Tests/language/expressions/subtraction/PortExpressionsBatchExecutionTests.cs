using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.subtraction;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.subtraction") { }

    [Fact(DisplayName = "S11.6.2_A2.1_T1")]
    public Task S11_6_2_A2_1_T1()
        => ExecutionTest("S11.6.2_A2.1_T1");

    [Fact(DisplayName = "S11.6.2_A2.1_T2")]
    public Task S11_6_2_A2_1_T2()
        => ExecutionTest("S11.6.2_A2.1_T2");

    [Fact(DisplayName = "S11.6.2_A2.1_T3")]
    public Task S11_6_2_A2_1_T3()
        => ExecutionTest("S11.6.2_A2.1_T3");

    [Fact(DisplayName = "S11.6.2_A2.3_T1")]
    public Task S11_6_2_A2_3_T1()
        => ExecutionTest("S11.6.2_A2.3_T1");
}
