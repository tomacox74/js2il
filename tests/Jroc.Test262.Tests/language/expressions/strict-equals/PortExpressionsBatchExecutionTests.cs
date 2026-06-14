using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.strict_equals;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.strict_equals") { }

    [Fact(DisplayName = "S11.9.4_A2.4_T2")]
    public Task S11_9_4_A2_4_T2()
        => ExecutionTest("S11.9.4_A2.4_T2");

    [Fact(DisplayName = "S11.9.4_A4.2")]
    public Task S11_9_4_A4_2()
        => ExecutionTest("S11.9.4_A4.2");

    [Fact(DisplayName = "S11.9.4_A6.2")]
    public Task S11_9_4_A6_2()
        => ExecutionTest("S11.9.4_A6.2");

    [Fact(DisplayName = "S11.9.4_A8_T4")]
    public Task S11_9_4_A8_T4()
        => ExecutionTest("S11.9.4_A8_T4");

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
