using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.every;

public class ThisArgExecutionTests : InMemoryExecutionTestsBase
{
    public ThisArgExecutionTests() : base("built_ins.Array.prototype.every") { }

    [Fact(DisplayName = "15.4.4.16-5-10.js")]
    public Task _15_4_4_16_5_10() => ExecutionTestFromFile("15.4.4.16-5-10");

    [Fact(DisplayName = "15.4.4.16-5-11.js")]
    public Task _15_4_4_16_5_11() => ExecutionTestFromFile("15.4.4.16-5-11");

    [Fact(DisplayName = "15.4.4.16-5-12.js")]
    public Task _15_4_4_16_5_12() => ExecutionTestFromFile("15.4.4.16-5-12");

    [Fact(DisplayName = "15.4.4.16-5-13.js")]
    public Task _15_4_4_16_5_13() => ExecutionTestFromFile("15.4.4.16-5-13");

    [Fact(DisplayName = "15.4.4.16-5-14.js")]
    public Task _15_4_4_16_5_14() => ExecutionTestFromFile("15.4.4.16-5-14");
}
