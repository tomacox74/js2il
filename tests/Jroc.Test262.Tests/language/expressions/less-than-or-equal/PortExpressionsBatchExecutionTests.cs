using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.less_than_or_equal;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.less_than_or_equal") { }

    [Fact(DisplayName = "11.8.3-1")]
    public Task _11_8_3_1()
        => ExecutionTest("11.8.3-1");

    [Fact(DisplayName = "11.8.3-2")]
    public Task _11_8_3_2()
        => ExecutionTest("11.8.3-2");

    [Fact(DisplayName = "S11.8.3_A2.1_T1")]
    public Task S11_8_3_A2_1_T1()
        => ExecutionTest("S11.8.3_A2.1_T1");
}
