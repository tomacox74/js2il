using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.unsigned_right_shift;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.unsigned_right_shift") { }

    [Fact(DisplayName = "S11.7.3_A4_T1")]
    public Task S11_7_3_A4_T1()
        => ExecutionTest("S11.7.3_A4_T1");

    [Fact(DisplayName = "S11.7.3_A4_T2")]
    public Task S11_7_3_A4_T2()
        => ExecutionTest("S11.7.3_A4_T2");

    [Fact(DisplayName = "S11.7.3_A4_T3")]
    public Task S11_7_3_A4_T3()
        => ExecutionTest("S11.7.3_A4_T3");

    [Fact(DisplayName = "S11.7.3_A4_T4")]
    public Task S11_7_3_A4_T4()
        => ExecutionTest("S11.7.3_A4_T4");

    [Fact(DisplayName = "S11.7.3_A5.1_T1")]
    public Task S11_7_3_A5_1_T1()
        => ExecutionTest("S11.7.3_A5.1_T1");

    [Fact(DisplayName = "S11.7.3_A5.2_T1")]
    public Task S11_7_3_A5_2_T1()
        => ExecutionTest("S11.7.3_A5.2_T1");

    [Fact(DisplayName = "S9.6_A1")]
    public Task S9_6_A1()
        => ExecutionTest("S9.6_A1");

    [Fact(DisplayName = "S9.6_A2.1")]
    public Task S9_6_A2_1()
        => ExecutionTest("S9.6_A2.1");

    [Fact(DisplayName = "S9.6_A2.2")]
    public Task S9_6_A2_2()
        => ExecutionTest("S9.6_A2.2");
}
