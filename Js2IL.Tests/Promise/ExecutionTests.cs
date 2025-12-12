namespace Js2IL.Tests.Promise;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("Promise")
    {       
    }

    [Fact]
    public Task Promise_Executor_Resolved()
    {
        return ExecutionTest(nameof(Promise_Executor_Resolved));
    }

    [Fact]
    public Task Promise_Executor_Rejected()
    {
        return ExecutionTest(nameof(Promise_Executor_Rejected));
    }

    [Fact]
    public Task Promise_Resolve_Then()
    {
        return ExecutionTest(nameof(Promise_Resolve_Then));
    }

    [Fact]
    public Task Promise_Reject_Then()
    {
        return ExecutionTest(nameof(Promise_Reject_Then));
    }
}