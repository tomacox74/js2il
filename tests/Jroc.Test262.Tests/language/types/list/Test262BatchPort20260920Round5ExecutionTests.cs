using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.list;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.list") { }

    [Fact(DisplayName = "S8.8_A2_T1")]
    public Task S8_8_A2_T1()
        => ExecutionTest("S8.8_A2_T1");

    [Fact(DisplayName = "S8.8_A2_T2")]
    public Task S8_8_A2_T2()
        => ExecutionTest("S8.8_A2_T2");

    [Fact(DisplayName = "S8.8_A2_T3")]
    public Task S8_8_A2_T3()
        => ExecutionTest("S8.8_A2_T3");

}
