using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.multiplication;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.multiplication") { }

    [Fact(DisplayName = "S11.5.1_A2.1_T1")]
    public Task S11_5_1_A2_1_T1()
        => ExecutionTest("S11.5.1_A2.1_T1");

    [Fact(DisplayName = "S11.5.1_A2.1_T2")]
    public Task S11_5_1_A2_1_T2()
        => ExecutionTest("S11.5.1_A2.1_T2");

    [Fact(DisplayName = "S11.5.1_A2.1_T3")]
    public Task S11_5_1_A2_1_T3()
        => ExecutionTest("S11.5.1_A2.1_T3");

    [Fact(DisplayName = "S11.5.1_A2.3_T1")]
    public Task S11_5_1_A2_3_T1()
        => ExecutionTest("S11.5.1_A2.3_T1");
}
