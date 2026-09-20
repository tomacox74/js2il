using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.@null;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.null") { }

    [Fact(DisplayName = "S8.2_A1_T1")]
    public Task S8_2_A1_T1()
        => ExecutionTest("S8.2_A1_T1");

    [Fact(DisplayName = "S8.2_A1_T2")]
    public Task S8_2_A1_T2()
        => ExecutionTest("S8.2_A1_T2");

    [Fact(DisplayName = "S8.2_A3")]
    public Task S8_2_A3()
        => ExecutionTest("S8.2_A3");

}
