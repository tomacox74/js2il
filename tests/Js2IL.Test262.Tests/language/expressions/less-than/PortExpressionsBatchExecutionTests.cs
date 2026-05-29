using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.less_than;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.less_than") { }

    [Fact(DisplayName = "S11.8.1_A2.1_T1")]
    public Task S11_8_1_A2_1_T1()
        => ExecutionTest("S11.8.1_A2.1_T1");

    [Fact(DisplayName = "S11.8.1_A2.1_T2")]
    public Task S11_8_1_A2_1_T2()
        => ExecutionTest("S11.8.1_A2.1_T2");

    [Fact(DisplayName = "S11.8.1_A2.1_T3")]
    public Task S11_8_1_A2_1_T3()
        => ExecutionTest("S11.8.1_A2.1_T3");
}
