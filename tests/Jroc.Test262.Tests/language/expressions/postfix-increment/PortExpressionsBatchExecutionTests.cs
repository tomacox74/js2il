using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.postfix_increment;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.postfix_increment") { }

    [Fact(DisplayName = "S11.3.1_A3_T1")]
    public Task S11_3_1_A3_T1()
        => ExecutionTest("S11.3.1_A3_T1");

    [Fact(DisplayName = "S11.3.1_A2.1_T1")]
    public Task S11_3_1_A2_1_T1()
        => ExecutionTest("S11.3.1_A2.1_T1");
}
