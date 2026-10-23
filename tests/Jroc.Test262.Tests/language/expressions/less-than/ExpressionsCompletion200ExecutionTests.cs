namespace Jroc.Test262.Tests.language.expressions.less_than;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.less_than") { }

    [Fact(DisplayName = "S11.8.1_A1.js", Skip = "eval is not supported.")]
    public Task S11_8_1_A1()
        => ExecutionTest("S11.8.1_A1");
}
