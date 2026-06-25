using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.splice;


public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.splice") { }

    [Fact(DisplayName = "15.4.4.12-9-a-1")]
    public Task _15_4_4_12_9_a_1()
        => ExecutionTestFromFile("15.4.4.12-9-a-1");
}
