namespace Jroc.Test262.Tests.language.expressions.equals;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.equals") { }

    [Fact(DisplayName = "S11.9.1_A1.js", Skip = "eval is not supported.")]
    public Task S11_9_1_A1()
        => ExecutionTest("S11.9.1_A1");

    [Fact(DisplayName = "S11.9.1_A6.1.js", Skip = "eval is not supported.")]
    public Task S11_9_1_A6_1()
        => ExecutionTest("S11.9.1_A6.1");
}
