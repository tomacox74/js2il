using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.JSON;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON") { }

    [Fact(DisplayName = "15.12-0-1", Skip = "Known JS2IL defect")]
    public Task _15_12_0_1()
        => ExecutionTest("15.12-0-1");

    [Fact(DisplayName = "15.12-0-4", Skip = "Known JS2IL defect")]
    public Task _15_12_0_4()
        => ExecutionTest("15.12-0-4");
}
