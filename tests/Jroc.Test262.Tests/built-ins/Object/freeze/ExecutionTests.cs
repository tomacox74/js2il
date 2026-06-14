using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.freeze;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.freeze") { }

    [Fact(DisplayName = "15.2.3.9-0-1")]
    public Task _15_2_3_9_0_1()
        => ExecutionTestFromFile("15.2.3.9-0-1");
    [Fact(DisplayName = "15.2.3.9-2-a-1")]
    public Task _15_2_3_9_2_a_1()
        => ExecutionTestFromFile("15.2.3.9-2-a-1");
    [Fact(DisplayName = "15.2.3.9-2-c-1")]
    public Task _15_2_3_9_2_c_1()
        => ExecutionTestFromFile("15.2.3.9-2-c-1");
}
