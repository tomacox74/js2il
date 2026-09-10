using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.lastIndexOf;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.lastIndexOf") { }

    [Fact(DisplayName = "15.4.4.15-2-8")]
    public Task _15_4_4_15_2_8() => ExecutionTestFromFile("15.4.4.15-2-8");

    [Fact(DisplayName = "15.4.4.15-5-1")]
    public Task _15_4_4_15_5_1() => ExecutionTestFromFile("15.4.4.15-5-1");

    [Fact(DisplayName = "15.4.4.15-5-12")]
    public Task _15_4_4_15_5_12() => ExecutionTestFromFile("15.4.4.15-5-12");

    [Fact(DisplayName = "15.4.4.15-5-14")]
    public Task _15_4_4_15_5_14() => ExecutionTestFromFile("15.4.4.15-5-14");

    [Fact(DisplayName = "15.4.4.15-5-16")]
    public Task _15_4_4_15_5_16() => ExecutionTestFromFile("15.4.4.15-5-16");

    [Fact(DisplayName = "15.4.4.15-5-19")]
    public Task _15_4_4_15_5_19() => ExecutionTestFromFile("15.4.4.15-5-19");

    [Fact(DisplayName = "15.4.4.15-5-24")]
    public Task _15_4_4_15_5_24() => ExecutionTestFromFile("15.4.4.15-5-24");

    [Fact(DisplayName = "15.4.4.15-5-4")]
    public Task _15_4_4_15_5_4() => ExecutionTestFromFile("15.4.4.15-5-4");

    [Fact(DisplayName = "15.4.4.15-5-5")]
    public Task _15_4_4_15_5_5() => ExecutionTestFromFile("15.4.4.15-5-5");

    [Fact(DisplayName = "15.4.4.15-8-9")]
    public Task _15_4_4_15_8_9() => ExecutionTestFromFile("15.4.4.15-8-9");

    [Fact(DisplayName = "15.4.4.15-8-a-17")]
    public Task _15_4_4_15_8_a_17() => ExecutionTestFromFile("15.4.4.15-8-a-17");

    [Fact(DisplayName = "15.4.4.15-8-a-18")]
    public Task _15_4_4_15_8_a_18() => ExecutionTestFromFile("15.4.4.15-8-a-18");

}
