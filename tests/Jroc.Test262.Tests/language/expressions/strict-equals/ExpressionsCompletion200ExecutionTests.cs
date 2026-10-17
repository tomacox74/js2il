namespace Jroc.Test262.Tests.language.expressions.strict_equals;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.strict_equals") { }

    [Fact(DisplayName = "S11.9.4_A1.js", Skip = "eval is not supported.")]
    public Task S11_9_4_A1()
        => ExecutionTest("S11.9.4_A1");

    [Fact(DisplayName = "S11.9.4_A6.1.js", Skip = "eval is not supported.")]
    public Task S11_9_4_A6_1()
        => ExecutionTest("S11.9.4_A6.1");

    [Fact(DisplayName = "S11.9.4_A8_T5.js")]
    public Task S11_9_4_A8_T5()
        => ExecutionTest("S11.9.4_A8_T5");

    [Fact(DisplayName = "bigint-and-boolean.js")]
    public Task bigint_and_boolean()
        => ExecutionTest("bigint-and-boolean");

    [Fact(DisplayName = "bigint-and-non-finite.js")]
    public Task bigint_and_non_finite()
        => ExecutionTest("bigint-and-non-finite");
}
