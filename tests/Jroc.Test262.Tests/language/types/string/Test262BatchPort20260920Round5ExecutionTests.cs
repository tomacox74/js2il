using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.@string;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.string") { }

    [Fact(DisplayName = "S8.4_A1")]
    public Task S8_4_A1()
        => ExecutionTest("S8.4_A1");

    [Fact(DisplayName = "S8.4_A10")]
    public Task S8_4_A10()
        => ExecutionTest("S8.4_A10");

    [Fact(DisplayName = "S8.4_A11")]
    public Task S8_4_A11()
        => ExecutionTest("S8.4_A11");

    [Fact(DisplayName = "S8.4_A12")]
    public Task S8_4_A12()
        => ExecutionTest("S8.4_A12");

    [Fact(DisplayName = "S8.4_A2")]
    public Task S8_4_A2()
        => ExecutionTest("S8.4_A2");

    [Fact(DisplayName = "S8.4_A3")]
    public Task S8_4_A3()
        => ExecutionTest("S8.4_A3");

    [Fact(DisplayName = "S8.4_A4")]
    public Task S8_4_A4()
        => ExecutionTest("S8.4_A4");

    [Fact(DisplayName = "S8.4_A5")]
    public Task S8_4_A5()
        => ExecutionTest("S8.4_A5");

    [Fact(DisplayName = "S8.4_A6.1")]
    public Task S8_4_A6_1()
        => ExecutionTest("S8.4_A6.1");

    [Fact(DisplayName = "S8.4_A6.2")]
    public Task S8_4_A6_2()
        => ExecutionTest("S8.4_A6.2");

    [Fact(DisplayName = "S8.4_A8")]
    public Task S8_4_A8()
        => ExecutionTest("S8.4_A8");

    [Fact(DisplayName = "S8.4_A9_T1")]
    public Task S8_4_A9_T1()
        => ExecutionTest("S8.4_A9_T1");

    [Fact(DisplayName = "S8.4_A9_T2")]
    public Task S8_4_A9_T2()
        => ExecutionTest("S8.4_A9_T2");

    [Fact(DisplayName = "S8.4_A9_T3")]
    public Task S8_4_A9_T3()
        => ExecutionTest("S8.4_A9_T3");

}
