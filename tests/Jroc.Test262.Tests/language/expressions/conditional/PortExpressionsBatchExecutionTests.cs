using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.conditional;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.conditional") { }

    [Fact(DisplayName = "S11.12_A2.1_T2")]
    public Task S11_12_A2_1_T2()
        => ExecutionTest("S11.12_A2.1_T2");

    [Fact(DisplayName = "S11.12_A2.1_T5")]
    public Task S11_12_A2_1_T5()
        => ExecutionTest("S11.12_A2.1_T5");

    [Fact(DisplayName = "S11.12_A3_T2")]
    public Task S11_12_A3_T2()
        => ExecutionTest("S11.12_A3_T2");

    [Fact(DisplayName = "S11.12_A4_T2")]
    public Task S11_12_A4_T2()
        => ExecutionTest("S11.12_A4_T2");

    [Fact(DisplayName = "tco-cond")]
    public Task tco_cond()
        => ExecutionTest("tco-cond");

    [Fact(DisplayName = "tco-pos")]
    public Task tco_pos()
        => ExecutionTest("tco-pos");

    [Fact(DisplayName = "S11.12_A2.1_T1")]
    public Task S11_12_A2_1_T1()
        => ExecutionTest("S11.12_A2.1_T1");

    [Fact(DisplayName = "S11.12_A2.1_T3")]
    public Task S11_12_A2_1_T3()
        => ExecutionTest("S11.12_A2.1_T3");

    [Fact(DisplayName = "S11.12_A2.1_T4")]
    public Task S11_12_A2_1_T4()
        => ExecutionTest("S11.12_A2.1_T4");
}
