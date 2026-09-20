using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.types.boolean;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.types.boolean") { }

    [Fact(DisplayName = "S8.3_A1_T1")]
    public Task S8_3_A1_T1()
        => ExecutionTest("S8.3_A1_T1");

    [Fact(DisplayName = "S8.3_A1_T2")]
    public Task S8_3_A1_T2()
        => ExecutionTest("S8.3_A1_T2");

    [Fact(DisplayName = "S8.3_A3")]
    public Task S8_3_A3()
        => ExecutionTest("S8.3_A3");

}
