using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.strict_equals;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.strict_equals") { }

    [Fact(DisplayName = "bigint-and-bigint")]
    public Task bigint_and_bigint()
        => ExecutionTest("bigint-and-bigint");

    [Fact(DisplayName = "bigint-and-number")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-and-number-extremes")]
    public Task bigint_and_number_extremes()
        => ExecutionTest("bigint-and-number-extremes");

    [Fact(DisplayName = "bigint-and-object")]
    public Task bigint_and_object()
        => ExecutionTest("bigint-and-object");

    [Fact(DisplayName = "bigint-and-string")]
    public Task bigint_and_string()
        => ExecutionTest("bigint-and-string");
}
