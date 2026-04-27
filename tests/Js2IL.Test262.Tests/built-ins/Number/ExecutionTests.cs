using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Number;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "15.7.3-1", Skip = "Known JS2IL defect")]
    public Task _15_7_3_1()
        => ExecutionTest("15.7.3-1");

    [Fact(DisplayName = "15.7.3-2", Skip = "Known JS2IL defect")]
    public Task _15_7_3_2()
        => ExecutionTest("15.7.3-2");

    [Fact(DisplayName = "15.7.4-1", Skip = "Known JS2IL defect")]
    public Task _15_7_4_1()
        => ExecutionTest("15.7.4-1");
}
