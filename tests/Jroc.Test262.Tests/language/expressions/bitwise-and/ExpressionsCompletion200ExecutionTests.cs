namespace Jroc.Test262.Tests.language.expressions.bitwise_and;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.bitwise_and") { }

    [Fact(DisplayName = "S11.10.1_A1.js", Skip = "eval is not supported.")]
    public Task S11_10_1_A1()
        => ExecutionTest("S11.10.1_A1");
}
