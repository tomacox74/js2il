using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.types.object_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.types.object_") { }

    [Fact(DisplayName = "S8.6_A2_T1")]
    public Task S8_6_A2_T1()
        => ExecutionTest("S8.6_A2_T1");

    [Fact(DisplayName = "S8.6_A3_T1")]
    public Task S8_6_A3_T1()
        => ExecutionTest("S8.6_A3_T1");
}
