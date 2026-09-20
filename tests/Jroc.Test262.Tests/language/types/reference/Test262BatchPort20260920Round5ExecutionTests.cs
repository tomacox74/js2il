using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.reference;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.reference") { }

    [Fact(DisplayName = "8.7.2-3-1-s")]
    public Task _8_7_2_3_1_s()
        => ExecutionTest("8.7.2-3-1-s");

    [Fact(DisplayName = "8.7.2-3-s")]
    public Task _8_7_2_3_s()
        => ExecutionTest("8.7.2-3-s");

    [Fact(DisplayName = "8.7.2-4-s")]
    public Task _8_7_2_4_s()
        => ExecutionTest("8.7.2-4-s");

    [Fact(DisplayName = "S8.7.1_A1")]
    public Task S8_7_1_A1()
        => ExecutionTest("S8.7.1_A1");

    [Fact(DisplayName = "S8.7.1_A2")]
    public Task S8_7_1_A2()
        => ExecutionTest("S8.7.1_A2");

    [Fact(DisplayName = "S8.7.2_A2")]
    public Task S8_7_2_A2()
        => ExecutionTest("S8.7.2_A2");

    [Fact(DisplayName = "S8.7.2_A3")]
    public Task S8_7_2_A3()
        => ExecutionTest("S8.7.2_A3");

    [Fact(DisplayName = "S8.7_A1")]
    public Task S8_7_A1()
        => ExecutionTest("S8.7_A1");

    [Fact(DisplayName = "S8.7_A2")]
    public Task S8_7_A2()
        => ExecutionTest("S8.7_A2");

    [Fact(DisplayName = "S8.7_A3")]
    public Task S8_7_A3()
        => ExecutionTest("S8.7_A3");

    [Fact(DisplayName = "S8.7_A4")]
    public Task S8_7_A4()
        => ExecutionTest("S8.7_A4");

    [Fact(DisplayName = "S8.7_A5_T1")]
    public Task S8_7_A5_T1()
        => ExecutionTest("S8.7_A5_T1");

    [Fact(DisplayName = "S8.7_A6")]
    public Task S8_7_A6()
        => ExecutionTest("S8.7_A6");

    [Fact(DisplayName = "S8.7_A7")]
    public Task S8_7_A7()
        => ExecutionTest("S8.7_A7");

}
