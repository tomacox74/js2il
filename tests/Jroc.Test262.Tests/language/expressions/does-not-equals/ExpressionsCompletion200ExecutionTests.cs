namespace Jroc.Test262.Tests.language.expressions.does_not_equals;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.does_not_equals") { }

    [Fact(DisplayName = "S11.9.2_A1.js", Skip = "eval is not supported.")]
    public Task S11_9_2_A1()
        => ExecutionTest("S11.9.2_A1");

    [Fact(DisplayName = "S11.9.2_A6.1.js", Skip = "eval is not supported.")]
    public Task S11_9_2_A6_1()
        => ExecutionTest("S11.9.2_A6.1");
}
