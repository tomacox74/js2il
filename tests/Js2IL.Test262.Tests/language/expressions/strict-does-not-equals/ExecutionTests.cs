using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.strict_does_not_equals;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.strict-does-not-equals") { }

    [Fact(DisplayName = "bigint-and-boolean")]
    public Task bigint_and_boolean()
        => ExecutionTest("bigint-and-boolean");

    [Fact(DisplayName = "bigint-and-number-extremes")]
    public Task bigint_and_number_extremes()
        => ExecutionTest("bigint-and-number-extremes");

    [Fact(DisplayName = "bigint-and-string")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");
}
