using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.some;

public class ThisArgExecutionTests : InMemoryExecutionTestsBase
{
    public ThisArgExecutionTests() : base("built_ins.Array.prototype.some") { }

    [Fact(DisplayName = "15.4.4.17-5-10.js")]
    public Task _15_4_4_17_5_10() => ExecutionTestFromFile("15.4.4.17-5-10");

    [Fact(DisplayName = "15.4.4.17-5-11.js")]
    public Task _15_4_4_17_5_11() => ExecutionTestFromFile("15.4.4.17-5-11");

    [Fact(DisplayName = "15.4.4.17-5-12.js")]
    public Task _15_4_4_17_5_12() => ExecutionTestFromFile("15.4.4.17-5-12");

    [Fact(DisplayName = "15.4.4.17-5-13.js")]
    public Task _15_4_4_17_5_13() => ExecutionTestFromFile("15.4.4.17-5-13");

    [Fact(DisplayName = "15.4.4.17-5-14.js")]
    public Task _15_4_4_17_5_14() => ExecutionTestFromFile("15.4.4.17-5-14");
}
