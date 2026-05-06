using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.map;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.map") { }

    [Fact(DisplayName = "15.4.4.19-1-1", Skip = "Array.prototype.map null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_19_1_1()
        => ExecutionTest("15.4.4.19-1-1");

    [Fact(DisplayName = "15.4.4.19-1-10", Skip = "Array.prototype.map null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_19_1_10()
        => ExecutionTest("15.4.4.19-1-10");

    [Fact(DisplayName = "15.4.4.19-1-11", Skip = "Array.prototype.map null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_19_1_11()
        => ExecutionTest("15.4.4.19-1-11");

    [Fact(DisplayName = "15.4.4.19-1-12", Skip = "Array.prototype.map null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_19_1_12()
        => ExecutionTest("15.4.4.19-1-12");
}
