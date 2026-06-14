using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.create;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.create") { }

    [Fact(DisplayName = "15.2.3.5-0-1")]
    public Task _15_2_3_5_0_1()
        => ExecutionTestFromFile("15.2.3.5-0-1");
    [Fact(DisplayName = "15.2.3.5-1")]
    public Task _15_2_3_5_1()
        => ExecutionTestFromFile("15.2.3.5-1");
    [Fact(DisplayName = "15.2.3.5-1-1")]
    public Task _15_2_3_5_1_1()
        => ExecutionTestFromFile("15.2.3.5-1-1");
    [Fact(DisplayName = "15.2.3.5-1-2")]
    public Task _15_2_3_5_1_2()
        => ExecutionTestFromFile("15.2.3.5-1-2");
    [Fact(DisplayName = "15.2.3.5-1-3")]
    public Task _15_2_3_5_1_3()
        => ExecutionTestFromFile("15.2.3.5-1-3");
    [Fact(DisplayName = "15.2.3.5-1-4")]
    public Task _15_2_3_5_1_4()
        => ExecutionTestFromFile("15.2.3.5-1-4");
    [Fact(DisplayName = "15.2.3.5-2-1")]
    public Task _15_2_3_5_2_1()
        => ExecutionTestFromFile("15.2.3.5-2-1");
    [Fact(DisplayName = "15.2.3.5-2-2")]
    public Task _15_2_3_5_2_2()
        => ExecutionTestFromFile("15.2.3.5-2-2");
    [Fact(DisplayName = "15.2.3.5-3-1")]
    public Task _15_2_3_5_3_1()
        => ExecutionTestFromFile("15.2.3.5-3-1");
    [Fact(DisplayName = "15.2.3.5-4-1")]
    public Task _15_2_3_5_4_1()
        => ExecutionTestFromFile("15.2.3.5-4-1");
    [Fact(DisplayName = "15.2.3.5-4-16")]
    public Task _15_2_3_5_4_16()
        => ExecutionTestFromFile("15.2.3.5-4-16");
    [Fact(DisplayName = "15.2.3.5-4-17")]
    public Task _15_2_3_5_4_17()
        => ExecutionTestFromFile("15.2.3.5-4-17");
    [Fact(DisplayName = "15.2.3.5-4-18")]
    public Task _15_2_3_5_4_18()
        => ExecutionTestFromFile("15.2.3.5-4-18");
    [Fact(DisplayName = "15.2.3.5-4-19")]
    public Task _15_2_3_5_4_19()
        => ExecutionTestFromFile("15.2.3.5-4-19");
    [Fact(DisplayName = "15.2.3.5-4-20")]
    public Task _15_2_3_5_4_20()
        => ExecutionTestFromFile("15.2.3.5-4-20");
}
