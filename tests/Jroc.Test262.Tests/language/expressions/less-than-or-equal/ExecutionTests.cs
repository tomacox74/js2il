using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.less_than_or_equal;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.less_than_or_equal") { }

    [Fact(DisplayName = "bigint-and-string")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");

    [Fact(DisplayName = "bigint-and-incomparable-string")]
    public Task bigint_and_incomparable_string()
        => ExecutionTest("bigint-and-incomparable-string");
}
