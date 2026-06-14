using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertyNames;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.getOwnPropertyNames") { }

    [Fact(DisplayName = "15.2.3.4-0-1")]
    public Task _15_2_3_4_0_1()
        => ExecutionTestFromFile("15.2.3.4-0-1");
    [Fact(DisplayName = "15.2.3.4-0-2")]
    public Task _15_2_3_4_0_2()
        => ExecutionTestFromFile("15.2.3.4-0-2");
    [Fact(DisplayName = "15.2.3.4-1-2")]
    public Task _15_2_3_4_1_2()
        => ExecutionTestFromFile("15.2.3.4-1-2");
    [Fact(DisplayName = "15.2.3.4-1-3")]
    public Task _15_2_3_4_1_3()
        => ExecutionTestFromFile("15.2.3.4-1-3");
    [Fact(DisplayName = "15.2.3.4-1-5")]
    public Task _15_2_3_4_1_5()
        => ExecutionTestFromFile("15.2.3.4-1-5");
    [Fact(DisplayName = "15.2.3.4-2-1")]
    public Task _15_2_3_4_2_1()
        => ExecutionTestFromFile("15.2.3.4-2-1");
    [Fact(DisplayName = "15.2.3.4-2-2")]
    public Task _15_2_3_4_2_2()
        => ExecutionTestFromFile("15.2.3.4-2-2");
    [Fact(DisplayName = "15.2.3.4-2-3")]
    public Task _15_2_3_4_2_3()
        => ExecutionTestFromFile("15.2.3.4-2-3");
    [Fact(DisplayName = "15.2.3.4-3-1")]
    public Task _15_2_3_4_3_1()
        => ExecutionTestFromFile("15.2.3.4-3-1");
}
