namespace Js2IL.Tests.Promise;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("Promise")
    {       
    }

    [Fact]
    public Task Promise_Handler_Resolved()
    {
        return ExecutionTest(nameof(Promise_Handler_Resolved));
    }
}