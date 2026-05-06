using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.filter;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.filter") { }

    [Fact(DisplayName = "15.4.4.20-1-1", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_1()
        => ExecutionTest("15.4.4.20-1-1");

    [Fact(DisplayName = "15.4.4.20-1-10", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_10()
        => ExecutionTest("15.4.4.20-1-10");

    [Fact(DisplayName = "15.4.4.20-1-11", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_11()
        => ExecutionTest("15.4.4.20-1-11");

    [Fact(DisplayName = "15.4.4.20-1-12", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_12()
        => ExecutionTest("15.4.4.20-1-12");
}
