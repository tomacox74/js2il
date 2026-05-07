using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.every;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.every") { }

    [Fact(DisplayName = "15.4.4.16-0-1", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_0_1()
        => ExecutionTestFromFile("15.4.4.16-0-1");

    [Fact(DisplayName = "15.4.4.16-1-1", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_1_1()
        => ExecutionTestFromFile("15.4.4.16-1-1");

    [Fact(DisplayName = "15.4.4.16-1-10", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_1_10()
        => ExecutionTestFromFile("15.4.4.16-1-10");

    [Fact(DisplayName = "15.4.4.16-1-11", Skip = "Array.prototype.every null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_16_1_11()
        => ExecutionTestFromFile("15.4.4.16-1-11");

    [Fact(DisplayName = "15.4.4.16-1-12", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_16_1_12()
        => ExecutionTestFromFile("15.4.4.16-1-12");

    [Fact(DisplayName = "15.4.4.16-1-13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_16_1_13()
        => ExecutionTestFromFile("15.4.4.16-1-13");

    [Fact(DisplayName = "15.4.4.16-1-14", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_16_1_14()
        => ExecutionTestFromFile("15.4.4.16-1-14");

    [Fact(DisplayName = "15.4.4.16-1-15", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_16_1_15()
        => ExecutionTestFromFile("15.4.4.16-1-15");

    [Fact(DisplayName = "15.4.4.16-1-2", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_16_1_2()
        => ExecutionTestFromFile("15.4.4.16-1-2");

    [Fact(DisplayName = "15.4.4.16-1-3", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_16_1_3()
        => ExecutionTestFromFile("15.4.4.16-1-3");

    [Fact(DisplayName = "15.4.4.16-1-4", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_16_1_4()
        => ExecutionTestFromFile("15.4.4.16-1-4");

    [Fact(DisplayName = "15.4.4.16-1-5", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_16_1_5()
        => ExecutionTestFromFile("15.4.4.16-1-5");

    [Fact(DisplayName = "15.4.4.16-1-6", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_16_1_6()
        => ExecutionTestFromFile("15.4.4.16-1-6");

    [Fact(DisplayName = "15.4.4.16-1-7", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_16_1_7()
        => ExecutionTestFromFile("15.4.4.16-1-7");

    [Fact(DisplayName = "15.4.4.16-1-8", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_16_1_8()
        => ExecutionTestFromFile("15.4.4.16-1-8");

    [Fact(DisplayName = "15.4.4.16-1-9", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_16_1_9()
        => ExecutionTestFromFile("15.4.4.16-1-9");
}
