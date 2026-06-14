using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.greater_than_or_equal;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.greater_than_or_equal") { }

    [Fact(DisplayName = "S11.8.4_A2.1_T1")]
    public Task S11_8_4_A2_1_T1()
        => ExecutionTest("S11.8.4_A2.1_T1");

    [Fact(DisplayName = "S11.8.4_A2.1_T2")]
    public Task S11_8_4_A2_1_T2()
        => ExecutionTest("S11.8.4_A2.1_T2");

    [Fact(DisplayName = "S11.8.4_A2.1_T3")]
    public Task S11_8_4_A2_1_T3()
        => ExecutionTest("S11.8.4_A2.1_T3");
}
