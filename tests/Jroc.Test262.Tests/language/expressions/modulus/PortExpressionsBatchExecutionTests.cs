using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.modulus;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.modulus") { }

    [Fact(DisplayName = "S11.5.3_A2.1_T1")]
    public Task S11_5_3_A2_1_T1()
        => ExecutionTest("S11.5.3_A2.1_T1");

    [Fact(DisplayName = "S11.5.3_A2.1_T2")]
    public Task S11_5_3_A2_1_T2()
        => ExecutionTest("S11.5.3_A2.1_T2");

    [Fact(DisplayName = "S11.5.3_A2.1_T3")]
    public Task S11_5_3_A2_1_T3()
        => ExecutionTest("S11.5.3_A2.1_T3");

    [Fact(DisplayName = "S11.5.3_A2.3_T1")]
    public Task S11_5_3_A2_3_T1()
        => ExecutionTest("S11.5.3_A2.3_T1");
}
