using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.void_;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.void") { }

    [Fact(DisplayName = "S11.4.2_A2_T1")]
    public Task S11_4_2_A2_T1()
        => ExecutionTest("S11.4.2_A2_T1");

    [Fact(DisplayName = "S11.4.2_A2_T2")]
    public Task S11_4_2_A2_T2()
        => ExecutionTest("S11.4.2_A2_T2");

    [Fact(DisplayName = "S11.4.2_A4_T1")]
    public Task S11_4_2_A4_T1()
        => ExecutionTest("S11.4.2_A4_T1");

    [Fact(DisplayName = "S11.4.2_A4_T2")]
    public Task S11_4_2_A4_T2()
        => ExecutionTest("S11.4.2_A4_T2");
}
