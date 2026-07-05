using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.now;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.now") { }

    [Fact(DisplayName = "15.9.4.4-0-4")]
    public Task _15_9_4_4_0_4()
        => ExecutionTestFromFile("15.9.4.4-0-4");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
