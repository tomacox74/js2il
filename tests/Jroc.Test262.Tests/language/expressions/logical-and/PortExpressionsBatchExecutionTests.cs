using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.logical_and;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.logical_and") { }

    [Fact(DisplayName = "S11.11.1_A2.4_T2")]
    public Task S11_11_1_A2_4_T2()
        => ExecutionTest("S11.11.1_A2.4_T2");

    [Fact(DisplayName = "S11.11.1_A3_T2")]
    public Task S11_11_1_A3_T2()
        => ExecutionTest("S11.11.1_A3_T2");

    [Fact(DisplayName = "S11.11.1_A4_T2")]
    public Task S11_11_1_A4_T2()
        => ExecutionTest("S11.11.1_A4_T2");

    [Fact(DisplayName = "S11.11.1_A4_T4")]
    public Task S11_11_1_A4_T4()
        => ExecutionTest("S11.11.1_A4_T4");

    [Fact(DisplayName = "tco-right")]
    public Task tco_right()
        => ExecutionTest("tco-right");

    [Fact(DisplayName = "S11.11.1_A2.1_T1")]
    public Task S11_11_1_A2_1_T1()
        => ExecutionTest("S11.11.1_A2.1_T1");

    [Fact(DisplayName = "S11.11.1_A2.1_T2")]
    public Task S11_11_1_A2_1_T2()
        => ExecutionTest("S11.11.1_A2.1_T2");
}
