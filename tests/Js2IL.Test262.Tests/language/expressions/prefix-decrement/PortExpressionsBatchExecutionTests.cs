using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.prefix_decrement;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.prefix_decrement") { }

    [Fact(DisplayName = "S11.4.5_A3_T1")]
    public Task S11_4_5_A3_T1()
        => ExecutionTest("S11.4.5_A3_T1");

    [Fact(DisplayName = "S11.4.5_A2.1_T1")]
    public Task S11_4_5_A2_1_T1()
        => ExecutionTest("S11.4.5_A2.1_T1");
}
