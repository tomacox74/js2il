using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.unary_plus;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.unary_plus") { }

    [Fact(DisplayName = "S11.4.6_A2.1_T2")]
    public Task S11_4_6_A2_1_T2()
        => ExecutionTest("S11.4.6_A2.1_T2");

    [Fact(DisplayName = "S9.3_A2_T2")]
    public Task S9_3_A2_T2()
        => ExecutionTest("S9.3_A2_T2");

    [Fact(DisplayName = "S11.4.6_A3_T4")]
    public Task S11_4_6_A3_T4()
        => ExecutionTest("S11.4.6_A3_T4");

    [Fact(DisplayName = "S9.3_A5_T2")]
    public Task S9_3_A5_T2()
        => ExecutionTest("S9.3_A5_T2");
}
