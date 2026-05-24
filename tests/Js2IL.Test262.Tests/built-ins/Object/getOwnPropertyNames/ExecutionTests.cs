using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Object.getOwnPropertyNames;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.getOwnPropertyNames") { }

    [Fact(DisplayName = "15.2.3.4-0-1")]
    public Task _15_2_3_4_0_1()
        => ExecutionTestFromFile("15.2.3.4-0-1");
    [Fact(DisplayName = "15.2.3.4-2-1")]
    public Task _15_2_3_4_2_1()
        => ExecutionTestFromFile("15.2.3.4-2-1");
    [Fact(DisplayName = "15.2.3.4-3-1")]
    public Task _15_2_3_4_3_1()
        => ExecutionTestFromFile("15.2.3.4-3-1");
    [Fact(DisplayName = "15.2.3.4-0-2")]
    public Task _15_2_3_4_0_2()
        => ExecutionTestFromFile("15.2.3.4-0-2");
}
