namespace Jroc.Test262.Tests.language.expressions.greater_than;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.greater_than") { }

    [Fact(DisplayName = "S11.8.2_A1.js", Skip = "eval is not supported.")]
    public Task S11_8_2_A1()
        => ExecutionTest("S11.8.2_A1");
}
