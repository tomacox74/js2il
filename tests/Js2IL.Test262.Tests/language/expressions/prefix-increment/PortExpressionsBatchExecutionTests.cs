using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.prefix_increment;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.prefix_increment") { }

    [Fact(DisplayName = "S11.4.4_A3_T1")]
    public Task S11_4_4_A3_T1()
        => ExecutionTest("S11.4.4_A3_T1");
}
