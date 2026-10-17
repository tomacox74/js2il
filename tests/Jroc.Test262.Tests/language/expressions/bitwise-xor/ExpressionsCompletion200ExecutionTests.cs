namespace Jroc.Test262.Tests.language.expressions.bitwise_xor;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.bitwise_xor") { }

    [Fact(DisplayName = "S11.10.2_A1.js", Skip = "eval is not supported.")]
    public Task S11_10_2_A1()
        => ExecutionTest("S11.10.2_A1");
}
