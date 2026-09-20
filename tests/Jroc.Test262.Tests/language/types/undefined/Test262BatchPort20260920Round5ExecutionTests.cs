using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.undefined;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.undefined") { }

    [Fact(DisplayName = "S8.1_A1_T1")]
    public Task S8_1_A1_T1()
        => ExecutionTest("S8.1_A1_T1");

    [Fact(DisplayName = "S8.1_A1_T2")]
    public Task S8_1_A1_T2()
        => ExecutionTest("S8.1_A1_T2");

    [Fact(DisplayName = "S8.1_A2_T1")]
    public Task S8_1_A2_T1()
        => ExecutionTest("S8.1_A2_T1");

    [Fact(DisplayName = "S8.1_A2_T2")]
    public Task S8_1_A2_T2()
        => ExecutionTest("S8.1_A2_T2");

    [Fact(DisplayName = "S8.1_A3_T1")]
    public Task S8_1_A3_T1()
        => ExecutionTest("S8.1_A3_T1");

    [Fact(DisplayName = "S8.1_A3_T2")]
    public Task S8_1_A3_T2()
        => ExecutionTest("S8.1_A3_T2");

    [Fact(DisplayName = "S8.1_A4")]
    public Task S8_1_A4()
        => ExecutionTest("S8.1_A4");

    [Fact(DisplayName = "S8.1_A5")]
    public Task S8_1_A5()
        => ExecutionTest("S8.1_A5");

}
