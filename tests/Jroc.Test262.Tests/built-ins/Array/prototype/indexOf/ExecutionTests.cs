using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.indexOf;


public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.indexOf") { }

    [Fact(DisplayName = "15.4.4.14-4-1")]
    public Task _15_4_4_14_4_1()
        => ExecutionTestFromFile("15.4.4.14-4-1");

    [Fact(DisplayName = "15.4.4.14-9-8")]
    public Task _15_4_4_14_9_8()
        => ExecutionTestFromFile("15.4.4.14-9-8");

    [Fact(DisplayName = "15.4.4.14-2-1")]
    public Task _15_4_4_14_2_1()
        => ExecutionTestFromFile("15.4.4.14-2-1");
}
