using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.filter;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.filter") { }

    [Fact(DisplayName = "15.4.4.20-1-1", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_1()
        => ExecutionTestFromFile("15.4.4.20-1-1");

    [Fact(DisplayName = "15.4.4.20-1-10", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_10()
        => ExecutionTestFromFile("15.4.4.20-1-10");

    [Fact(DisplayName = "15.4.4.20-1-11", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_11()
        => ExecutionTestFromFile("15.4.4.20-1-11");

    [Fact(DisplayName = "15.4.4.20-1-12", Skip = "Array.prototype.filter null/undefined receiver handling is incomplete.")]
    public Task _15_4_4_20_1_12()
        => ExecutionTestFromFile("15.4.4.20-1-12");

    [Fact(DisplayName = "15.4.4.20-1-13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_20_1_13()
        => ExecutionTestFromFile("15.4.4.20-1-13");

    [Fact(DisplayName = "15.4.4.20-1-14", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_20_1_14()
        => ExecutionTestFromFile("15.4.4.20-1-14");

    [Fact(DisplayName = "15.4.4.20-1-15", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_20_1_15()
        => ExecutionTestFromFile("15.4.4.20-1-15");

    [Fact(DisplayName = "15.4.4.20-1-2", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_4_4_20_1_2()
        => ExecutionTestFromFile("15.4.4.20-1-2");

    [Fact(DisplayName = "15.4.4.20-1-3", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_20_1_3()
        => ExecutionTestFromFile("15.4.4.20-1-3");

    [Fact(DisplayName = "15.4.4.20-1-4", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_20_1_4()
        => ExecutionTestFromFile("15.4.4.20-1-4");

    [Fact(DisplayName = "15.4.4.20-1-5", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_20_1_5()
        => ExecutionTestFromFile("15.4.4.20-1-5");

    [Fact(DisplayName = "15.4.4.20-1-6", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_4_4_20_1_6()
        => ExecutionTestFromFile("15.4.4.20-1-6");

    [Fact(DisplayName = "15.4.4.20-1-7", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_20_1_7()
        => ExecutionTestFromFile("15.4.4.20-1-7");

    [Fact(DisplayName = "15.4.4.20-1-8", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_20_1_8()
        => ExecutionTestFromFile("15.4.4.20-1-8");

    [Fact(DisplayName = "15.4.4.20-1-9", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task _15_4_4_20_1_9()
        => ExecutionTestFromFile("15.4.4.20-1-9");

    [Fact(DisplayName = "15.4.4.20-10-1")]
    public Task _15_4_4_20_10_1()
        => ExecutionTestFromFile("15.4.4.20-10-1");
}
