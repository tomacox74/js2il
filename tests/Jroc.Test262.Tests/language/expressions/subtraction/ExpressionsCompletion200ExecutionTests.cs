namespace Jroc.Test262.Tests.language.expressions.subtraction;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.subtraction") { }

    [Fact(DisplayName = "S11.6.2_A1.js", Skip = "eval is not supported.")]
    public Task S11_6_2_A1()
        => ExecutionTest("S11.6.2_A1");
}
