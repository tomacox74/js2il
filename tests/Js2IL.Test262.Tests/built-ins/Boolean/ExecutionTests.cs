using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Boolean;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Boolean") { }

    [Fact(DisplayName = "S15.6.1.1_A2", Skip = "Known JS2IL defect")]
    public Task S15_6_1_1_A2()
        => ExecutionTest("S15.6.1.1_A2");

    [Fact(DisplayName = "S15.6.2.1_A1", Skip = "Known JS2IL defect")]
    public Task S15_6_2_1_A1()
        => ExecutionTest("S15.6.2.1_A1");
}
