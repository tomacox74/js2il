using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Object.getOwnPropertyDescriptor;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "15.2.3.3-0-1")]
    public Task _15_2_3_3_0_1()
        => ExecutionTestFromFile("15.2.3.3-0-1");

    [Fact(DisplayName = "15.2.3.3-1-1")]
    public Task _15_2_3_3_1_1()
        => ExecutionTestFromFile("15.2.3.3-1-1");

    [Fact(DisplayName = "15.2.3.3-1-2")]
    public Task _15_2_3_3_1_2()
        => ExecutionTestFromFile("15.2.3.3-1-2");

    [Fact(DisplayName = "15.2.3.3-1-3")]
    public Task _15_2_3_3_1_3()
        => ExecutionTestFromFile("15.2.3.3-1-3");

    [Fact(DisplayName = "15.2.3.3-1-4")]
    public Task _15_2_3_3_1_4()
        => ExecutionTestFromFile("15.2.3.3-1-4");

    [Fact(DisplayName = "15.2.3.3-1")]
    public Task _15_2_3_3_1()
        => ExecutionTestFromFile("15.2.3.3-1");

    [Fact(DisplayName = "15.2.3.3-2-1")]
    public Task _15_2_3_3_2_1()
        => ExecutionTestFromFile("15.2.3.3-2-1");

    [Fact(DisplayName = "15.2.3.3-2-10", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task _15_2_3_3_2_10()
        => ExecutionTestFromFile("15.2.3.3-2-10");

    [Fact(DisplayName = "15.2.3.3-2-11")]
    public Task _15_2_3_3_2_11()
        => ExecutionTestFromFile("15.2.3.3-2-11");

    [Fact(DisplayName = "15.2.3.3-2-12")]
    public Task _15_2_3_3_2_12()
        => ExecutionTestFromFile("15.2.3.3-2-12");

    [Fact(DisplayName = "15.2.3.3-2-13")]
    public Task _15_2_3_3_2_13()
        => ExecutionTestFromFile("15.2.3.3-2-13");

    [Fact(DisplayName = "15.2.3.3-2-14")]
    public Task _15_2_3_3_2_14()
        => ExecutionTestFromFile("15.2.3.3-2-14");

    [Fact(DisplayName = "15.2.3.3-2-15")]
    public Task _15_2_3_3_2_15()
        => ExecutionTestFromFile("15.2.3.3-2-15");

    [Fact(DisplayName = "15.2.3.3-2-16", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task _15_2_3_3_2_16()
        => ExecutionTestFromFile("15.2.3.3-2-16");

    [Fact(DisplayName = "15.2.3.3-2-17", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task _15_2_3_3_2_17()
        => ExecutionTestFromFile("15.2.3.3-2-17");

    [Fact(DisplayName = "15.2.3.3-2-18", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task _15_2_3_3_2_18()
        => ExecutionTestFromFile("15.2.3.3-2-18");
}
