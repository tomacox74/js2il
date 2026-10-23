namespace Jroc.Test262.Tests.language.expressions.strict_does_not_equals;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.strict_does_not_equals") { }

    [Fact(DisplayName = "S11.9.5_A1.js", Skip = "eval is not supported.")]
    public Task S11_9_5_A1()
        => ExecutionTest("S11.9.5_A1");

    [Fact(DisplayName = "S11.9.5_A6.1.js", Skip = "eval is not supported.")]
    public Task S11_9_5_A6_1()
        => ExecutionTest("S11.9.5_A6.1");
}
