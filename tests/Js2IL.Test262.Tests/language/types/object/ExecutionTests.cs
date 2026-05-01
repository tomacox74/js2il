using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.types.object_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.types.object_") { }

    [Fact(DisplayName = "S8.6_A2_T1")]
    public Task S8_6_A2_T1()
        => ExecutionTest("S8.6_A2_T1");

    [Fact(DisplayName = "S8.6_A3_T1")]
    public Task S8_6_A3_T1()
        => ExecutionTest("S8.6_A3_T1");

    [Fact(DisplayName = "S8.6.2_A7")]
    public Task S8_6_2_A7()
        => ExecutionTest("S8.6.2_A7");
}
