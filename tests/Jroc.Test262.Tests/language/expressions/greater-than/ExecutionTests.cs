using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.greater_than;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.greater_than") { }

    [Fact(DisplayName = "bigint-and-string")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");

    [Fact(DisplayName = "bigint-and-boolean")]
    public Task bigint_and_boolean()
        => ExecutionTest("bigint-and-boolean");

    [Fact(DisplayName = "bigint-and-incomparable-string")]
    public Task bigint_and_incomparable_string()
        => ExecutionTest("bigint-and-incomparable-string");
}
