namespace Js2IL.Tests.Promise;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("Promise")
    {       
    }

    // Tests sorted alphabetically
    [Fact]
    public Task Promise_Catch_ReturnsRejectedPromise()
    {
        return ExecutionTest(nameof(Promise_Catch_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Catch_ReturnsResolvedPromise()
    {
        return ExecutionTest(nameof(Promise_Catch_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Executor_Rejected()
    {
        return ExecutionTest(nameof(Promise_Executor_Rejected));
    }

    [Fact]
    public Task Promise_Executor_Resolved()
    {
        return ExecutionTest(nameof(Promise_Executor_Resolved));
    }

    [Fact]
    public Task Promise_Finally_ReturnsRejectedPromise()
    {
        return ExecutionTest(nameof(Promise_Finally_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Finally_ReturnsResolvedPromise()
    {
        return ExecutionTest(nameof(Promise_Finally_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Reject_FinallyCatch()
    {
        return ExecutionTest(nameof(Promise_Reject_FinallyCatch));
    }

    [Fact]
    public Task Promise_Reject_Then()
    {
        return ExecutionTest(nameof(Promise_Reject_Then));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThen()
    {
        return ExecutionTest(nameof(Promise_Resolve_FinallyThen));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThrows()
    {
        return ExecutionTest(nameof(Promise_Resolve_FinallyThrows));
    }

    [Fact]
    public Task Promise_Resolve_Then()
    {
        return ExecutionTest(nameof(Promise_Resolve_Then));
    }

    [Fact]
    public Task Promise_Resolve_ThenFinally()
    {
        return ExecutionTest(nameof(Promise_Resolve_ThenFinally));
    }

    [Fact]
    public Task Promise_Scheduling_StarvationTest()
    {
        return ExecutionTest(nameof(Promise_Scheduling_StarvationTest));
    }

    [Fact]
    public Task Promise_Then_ReturnsRejectedPromise()
    {
        return ExecutionTest(nameof(Promise_Then_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Then_ReturnsResolvedPromise()
    {
        return ExecutionTest(nameof(Promise_Then_ReturnsResolvedPromise));
    }
}