using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.JSON.parse;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.parse") { }

    [Fact(DisplayName = "15.12.1.1-0-1", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_1()
        => ExecutionTestFromFile("15.12.1.1-0-1");

    [Fact(DisplayName = "15.12.1.1-0-2", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_2()
        => ExecutionTestFromFile("15.12.1.1-0-2");

    [Fact(DisplayName = "15.12.1.1-0-3", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_3()
        => ExecutionTestFromFile("15.12.1.1-0-3");

    [Fact(DisplayName = "15.12.1.1-0-4", Skip = "Global SyntaxError constructor exposure is incomplete.")]
    public Task _15_12_1_1_0_4()
        => ExecutionTestFromFile("15.12.1.1-0-4");

    [Fact(DisplayName = "15.12.1.1-0-5", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_5()
        => ExecutionTestFromFile("15.12.1.1-0-5");

    [Fact(DisplayName = "15.12.1.1-0-6", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_6()
        => ExecutionTestFromFile("15.12.1.1-0-6");

    [Fact(DisplayName = "15.12.1.1-0-8", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_12_1_1_0_8()
        => ExecutionTestFromFile("15.12.1.1-0-8");

    [Fact(DisplayName = "15.12.1.1-0-9")]
    public Task _15_12_1_1_0_9()
        => ExecutionTestFromFile("15.12.1.1-0-9");

    [Fact(DisplayName = "15.12.1.1-g1-1", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_12_1_1_g1_1()
        => ExecutionTestFromFile("15.12.1.1-g1-1");

    [Fact(DisplayName = "15.12.1.1-g1-2", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_12_1_1_g1_2()
        => ExecutionTestFromFile("15.12.1.1-g1-2");

    [Fact(DisplayName = "15.12.1.1-g1-3", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_12_1_1_g1_3()
        => ExecutionTestFromFile("15.12.1.1-g1-3");

    [Fact(DisplayName = "15.12.1.1-g1-4", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_12_1_1_g1_4()
        => ExecutionTestFromFile("15.12.1.1-g1-4");

    [Fact(DisplayName = "15.12.1.1-g2-1")]
    public Task _15_12_1_1_g2_1()
        => ExecutionTestFromFile("15.12.1.1-g2-1");

    [Fact(DisplayName = "15.12.1.1-g2-2", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_12_1_1_g2_2()
        => ExecutionTestFromFile("15.12.1.1-g2-2");

    [Fact(DisplayName = "15.12.1.1-g2-3", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_12_1_1_g2_3()
        => ExecutionTestFromFile("15.12.1.1-g2-3");

    [Fact(DisplayName = "15.12.1.1-g2-4", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_12_1_1_g2_4()
        => ExecutionTestFromFile("15.12.1.1-g2-4");
}
