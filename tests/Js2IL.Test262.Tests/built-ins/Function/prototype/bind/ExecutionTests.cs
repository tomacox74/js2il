using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Function.prototype.bind;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype.bind") { }

    [Fact(DisplayName = "15.3.4.5-0-1")]
    public Task _15_3_4_5_0_1()
        => ExecutionTestFromFile("15.3.4.5-0-1");

    [Fact(DisplayName = "15.3.4.5-10-1")]
    public Task _15_3_4_5_10_1()
        => ExecutionTestFromFile("15.3.4.5-10-1");

    [Fact(DisplayName = "15.3.4.5-11-1")]
    public Task _15_3_4_5_11_1()
        => ExecutionTestFromFile("15.3.4.5-11-1");

    [Fact(DisplayName = "15.3.4.5-16-1")]
    public Task _15_3_4_5_16_1()
        => ExecutionTestFromFile("15.3.4.5-16-1");

    [Fact(DisplayName = "15.3.4.5-16-2")]
    public Task _15_3_4_5_16_2()
        => ExecutionTestFromFile("15.3.4.5-16-2");

    [Fact(DisplayName = "15.3.4.5-2-1")]
    public Task _15_3_4_5_2_1()
        => ExecutionTestFromFile("15.3.4.5-2-1");

    [Fact(DisplayName = "15.3.4.5-2-10")]
    public Task _15_3_4_5_2_10()
        => ExecutionTestFromFile("15.3.4.5-2-10");

    [Fact(DisplayName = "15.3.4.5-2-11")]
    public Task _15_3_4_5_2_11()
        => ExecutionTestFromFile("15.3.4.5-2-11");

    [Fact(DisplayName = "15.3.4.5-2-12", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_3_4_5_2_12()
        => ExecutionTestFromFile("15.3.4.5-2-12");

    [Fact(DisplayName = "15.3.4.5-2-13", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_3_4_5_2_13()
        => ExecutionTestFromFile("15.3.4.5-2-13");

    [Fact(DisplayName = "15.3.4.5-2-14", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_3_4_5_2_14()
        => ExecutionTestFromFile("15.3.4.5-2-14");

    [Fact(DisplayName = "15.3.4.5-2-15", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _15_3_4_5_2_15()
        => ExecutionTestFromFile("15.3.4.5-2-15");

    [Fact(DisplayName = "15.3.4.5-2-16", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task _15_3_4_5_2_16()
        => ExecutionTestFromFile("15.3.4.5-2-16");

    [Fact(DisplayName = "15.3.4.5-2-2")]
    public Task _15_3_4_5_2_2()
        => ExecutionTestFromFile("15.3.4.5-2-2");

    [Fact(DisplayName = "15.3.4.5-2-3")]
    public Task _15_3_4_5_2_3()
        => ExecutionTestFromFile("15.3.4.5-2-3");

    [Fact(DisplayName = "15.3.4.5-2-4")]
    public Task _15_3_4_5_2_4()
        => ExecutionTestFromFile("15.3.4.5-2-4");
}
