using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.strict_equals;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.strict-equals") { }

    [Fact(DisplayName = "bigint-and-number")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-and-string")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");

    [Fact(DisplayName = "bigint-and-object")]
    public Task bigint_and_object()
        => ExecutionTest("bigint-and-object");

    [Fact(DisplayName = "bigint-and-incomparable-primitive")]
    public Task bigint_and_incomparable_primitive()
        => ExecutionTest("bigint-and-incomparable-primitive");
}
