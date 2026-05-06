using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.JSON.parse;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.parse") { }

[Fact(DisplayName = "15.12.1.1-0-1", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_1()
        => ExecutionTest("15.12.1.1-0-1");

[Fact(DisplayName = "15.12.1.1-0-2", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_2()
        => ExecutionTest("15.12.1.1-0-2");

[Fact(DisplayName = "15.12.1.1-0-3", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_3()
        => ExecutionTest("15.12.1.1-0-3");

[Fact(DisplayName = "15.12.1.1-0-4", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_4()
        => ExecutionTest("15.12.1.1-0-4");

[Fact(DisplayName = "15.12.1.1-0-5", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_5()
        => ExecutionTest("15.12.1.1-0-5");

[Fact(DisplayName = "15.12.1.1-0-6", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_6()
        => ExecutionTest("15.12.1.1-0-6");

[Fact(DisplayName = "15.12.1.1-0-8", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_8()
        => ExecutionTest("15.12.1.1-0-8");

[Fact(DisplayName = "15.12.1.1-0-9")]
    public Task _15_12_1_1_0_9()
        => ExecutionTest("15.12.1.1-0-9");

[Fact(DisplayName = "15.12.1.1-0-1", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_1()
        => ExecutionTest("15.12.1.1-0-1");

[Fact(DisplayName = "15.12.1.1-0-2", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_2()
        => ExecutionTest("15.12.1.1-0-2");

[Fact(DisplayName = "15.12.1.1-0-3", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_3()
        => ExecutionTest("15.12.1.1-0-3");

[Fact(DisplayName = "15.12.1.1-0-4", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_4()
        => ExecutionTest("15.12.1.1-0-4");

[Fact(DisplayName = "15.12.1.1-0-5", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_5()
        => ExecutionTest("15.12.1.1-0-5");

[Fact(DisplayName = "15.12.1.1-0-6", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_6()
        => ExecutionTest("15.12.1.1-0-6");

[Fact(DisplayName = "15.12.1.1-0-8", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_8()
        => ExecutionTest("15.12.1.1-0-8");

[Fact(DisplayName = "15.12.1.1-0-9")]
    public Task _15_12_1_1_0_9()
        => ExecutionTest("15.12.1.1-0-9");
}
