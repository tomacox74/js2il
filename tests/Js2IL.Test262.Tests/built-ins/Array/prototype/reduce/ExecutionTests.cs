using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.reduce;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.reduce") { }

[Fact(DisplayName = "15.4.4.21-1-1")]
    public Task _15_4_4_21_1_1()
        => ExecutionTest("15.4.4.21-1-1");

[Fact(DisplayName = "15.4.4.21-1-10", Skip = "Array.prototype.reduce edge-case receiver handling is incomplete.")]
    public Task _15_4_4_21_1_10()
        => ExecutionTest("15.4.4.21-1-10");

[Fact(DisplayName = "15.4.4.21-1-11", Skip = "Array.prototype.reduce edge-case receiver handling is incomplete.")]
    public Task _15_4_4_21_1_11()
        => ExecutionTest("15.4.4.21-1-11");

[Fact(DisplayName = "15.4.4.21-1-12")]
    public Task _15_4_4_21_1_12()
        => ExecutionTest("15.4.4.21-1-12");

[Fact(DisplayName = "15.4.4.21-1-13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_21_1_13()
        => ExecutionTest("15.4.4.21-1-13");

[Fact(DisplayName = "15.4.4.21-1-14", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_21_1_14()
        => ExecutionTest("15.4.4.21-1-14");

[Fact(DisplayName = "15.4.4.21-1-15", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_21_1_15()
        => ExecutionTest("15.4.4.21-1-15");

[Fact(DisplayName = "15.4.4.21-1-2", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_21_1_2()
        => ExecutionTest("15.4.4.21-1-2");
}
