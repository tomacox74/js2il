using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Math.acos;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.acos") { }

    [Fact(DisplayName = "S15.8.2.2_A1")]
    public Task S15_8_2_2_A1()
        => ExecutionTest("S15.8.2.2_A1");
}
