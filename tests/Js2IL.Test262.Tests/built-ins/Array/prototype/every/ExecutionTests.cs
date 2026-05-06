using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.every;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.every") { }

    [Fact(DisplayName = "15.4.4.16-0-1", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_0_1()
        => ExecutionTest("15.4.4.16-0-1");

    [Fact(DisplayName = "15.4.4.16-1-1", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_1_1()
        => ExecutionTest("15.4.4.16-1-1");

    [Fact(DisplayName = "15.4.4.16-1-10", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_1_10()
        => ExecutionTest("15.4.4.16-1-10");

    [Fact(DisplayName = "15.4.4.16-1-11", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_1_11()
        => ExecutionTest("15.4.4.16-1-11");
}
