using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Object.create;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.create") { }

    [Fact(DisplayName = "15.2.3.5-0-1")]
    public Task _15_2_3_5_0_1()
        => ExecutionTestFromFile("15.2.3.5-0-1");
    [Fact(DisplayName = "15.2.3.5-1-1")]
    public Task _15_2_3_5_1_1()
        => ExecutionTestFromFile("15.2.3.5-1-1");
    [Fact(DisplayName = "15.2.3.5-1-2")]
    public Task _15_2_3_5_1_2()
        => ExecutionTestFromFile("15.2.3.5-1-2");
    [Fact(DisplayName = "15.2.3.5-2-1")]
    public Task _15_2_3_5_2_1()
        => ExecutionTestFromFile("15.2.3.5-2-1");
    [Fact(DisplayName = "15.2.3.5-3-1")]
    public Task _15_2_3_5_3_1()
        => ExecutionTestFromFile("15.2.3.5-3-1");
    [Fact(DisplayName = "15.2.3.5-4-1")]
    public Task _15_2_3_5_4_1()
        => ExecutionTestFromFile("15.2.3.5-4-1");
}
