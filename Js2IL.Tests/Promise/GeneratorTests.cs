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
}