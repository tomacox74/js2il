using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.@object;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.object") { }

    [Fact(DisplayName = "S8.6.1_A1")]
    public Task S8_6_1_A1()
        => ExecutionTest("S8.6.1_A1");

    [Fact(DisplayName = "S8.6.1_A2")]
    public Task S8_6_1_A2()
        => ExecutionTest("S8.6.1_A2");

    [Fact(DisplayName = "S8.6.1_A3")]
    public Task S8_6_1_A3()
        => ExecutionTest("S8.6.1_A3");

    [Fact(DisplayName = "S8.6.2_A1")]
    public Task S8_6_2_A1()
        => ExecutionTest("S8.6.2_A1");

    [Fact(DisplayName = "S8.6.2_A2")]
    public Task S8_6_2_A2()
        => ExecutionTest("S8.6.2_A2");

    [Fact(DisplayName = "S8.6.2_A3")]
    public Task S8_6_2_A3()
        => ExecutionTest("S8.6.2_A3");

    [Fact(DisplayName = "S8.6.2_A4")]
    public Task S8_6_2_A4()
        => ExecutionTest("S8.6.2_A4");

    [Fact(DisplayName = "S8.6.2_A5_T1")]
    public Task S8_6_2_A5_T1()
        => ExecutionTest("S8.6.2_A5_T1");

    [Fact(DisplayName = "S8.6.2_A5_T2")]
    public Task S8_6_2_A5_T2()
        => ExecutionTest("S8.6.2_A5_T2");

    [Fact(DisplayName = "S8.6.2_A5_T4")]
    public Task S8_6_2_A5_T4()
        => ExecutionTest("S8.6.2_A5_T4");

    [Fact(DisplayName = "S8.6.2_A6")]
    public Task S8_6_2_A6()
        => ExecutionTest("S8.6.2_A6");

    [Fact(DisplayName = "S8.6.2_A8")]
    public Task S8_6_2_A8()
        => ExecutionTest("S8.6.2_A8");

    [Fact(DisplayName = "S8.6_A2_T2")]
    public Task S8_6_A2_T2()
        => ExecutionTest("S8.6_A2_T2");

    [Fact(DisplayName = "S8.6_A3_T2")]
    public Task S8_6_A3_T2()
        => ExecutionTest("S8.6_A3_T2");

    [Fact(DisplayName = "S8.6_A4_T1")]
    public Task S8_6_A4_T1()
        => ExecutionTest("S8.6_A4_T1");

}
