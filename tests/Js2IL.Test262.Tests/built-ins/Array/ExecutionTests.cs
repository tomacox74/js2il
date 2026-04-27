using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array") { }

    [Fact(DisplayName = "15.4.5-1", Skip = "Known JS2IL defect")]
    public Task _15_4_5_1()
        => ExecutionTest("15.4.5-1");

    [Fact(DisplayName = "15.4.5.1-5-1", Skip = "Known JS2IL defect")]
    public Task _15_4_5_1_5_1()
        => ExecutionTest("15.4.5.1-5-1");

    [Fact(DisplayName = "15.4.5.1-5-2", Skip = "Known JS2IL defect")]
    public Task _15_4_5_1_5_2()
        => ExecutionTest("15.4.5.1-5-2");

    [Fact(DisplayName = "constructor", Skip = "Known JS2IL defect")]
    public Task constructor()
        => ExecutionTest("constructor");
}
