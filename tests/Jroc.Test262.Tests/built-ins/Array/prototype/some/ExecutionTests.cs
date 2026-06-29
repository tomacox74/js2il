using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.some;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.some") { }

    [Fact(DisplayName = "15.4.4.17-1-10")]
    public Task _15_4_4_17_1_10()
        => ExecutionTestFromFile("15.4.4.17-1-10");

    [Fact(DisplayName = "15.4.4.17-1-11")]
    public Task _15_4_4_17_1_11()
        => ExecutionTestFromFile("15.4.4.17-1-11");

    [Fact(DisplayName = "15.4.4.17-1-12")]
    public Task _15_4_4_17_1_12()
        => ExecutionTestFromFile("15.4.4.17-1-12");

    [Fact(DisplayName = "15.4.4.17-1-13")]
    public Task _15_4_4_17_1_13()
        => ExecutionTestFromFile("15.4.4.17-1-13");

    [Fact(DisplayName = "15.4.4.17-1-14")]
    public Task _15_4_4_17_1_14()
        => ExecutionTestFromFile("15.4.4.17-1-14");

    [Fact(DisplayName = "15.4.4.17-1-15")]
    public Task _15_4_4_17_1_15()
        => ExecutionTestFromFile("15.4.4.17-1-15");

    [Fact(DisplayName = "15.4.4.17-1-3")]
    public Task _15_4_4_17_1_3()
        => ExecutionTestFromFile("15.4.4.17-1-3");

    [Fact(DisplayName = "15.4.4.17-1-4")]
    public Task _15_4_4_17_1_4()
        => ExecutionTestFromFile("15.4.4.17-1-4");

    [Fact(DisplayName = "15.4.4.17-1-5")]
    public Task _15_4_4_17_1_5()
        => ExecutionTestFromFile("15.4.4.17-1-5");

    [Fact(DisplayName = "15.4.4.17-1-6")]
    public Task _15_4_4_17_1_6()
        => ExecutionTestFromFile("15.4.4.17-1-6");
}
