namespace Js2IL.Tests.Promise;

public class GeneratorTests : GeneratorTestsBase
{
    public GeneratorTests() : base("Promise")
    {
    }

    [Fact]
    public Task Promise_Executor_Resolved()
    {
        return GenerateTest(nameof(Promise_Executor_Resolved));
    }

    [Fact]
    public Task Promise_Executor_Rejected()
    {
        return GenerateTest(nameof(Promise_Executor_Resolved));
    }

    [Fact]
    public Task Promise_Resolve_Then()
    {
        return GenerateTest(nameof(Promise_Resolve_Then));
    }

    [Fact]
    public Task Promise_Reject_Then()
    {
        return GenerateTest(nameof(Promise_Reject_Then));
    }

    [Fact]
    public Task Promise_Resolve_ThenFinally()
    {
        return GenerateTest(nameof(Promise_Resolve_ThenFinally));
    }

    [Fact]
    public Task Promise_Reject_FinallyCatch()
    {
        return GenerateTest(nameof(Promise_Reject_FinallyCatch));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThen()
    {
        return GenerateTest(nameof(Promise_Resolve_FinallyThen));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThrows()
    {
        return GenerateTest(nameof(Promise_Resolve_FinallyThrows));
    }

    [Fact]
    public Task Promise_Then_ReturnsResolvedPromise()
    {
        return GenerateTest(nameof(Promise_Then_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Then_ReturnsRejectedPromise()
    {
        return GenerateTest(nameof(Promise_Then_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Catch_ReturnsResolvedPromise()
    {
        return GenerateTest(nameof(Promise_Catch_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Catch_ReturnsRejectedPromise()
    {
        return GenerateTest(nameof(Promise_Catch_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Finally_ReturnsResolvedPromise()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Finally_ReturnsRejectedPromise()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Scheduling_StarvationTest()
    {
        return GenerateTest(nameof(Promise_Scheduling_StarvationTest));
    }
}