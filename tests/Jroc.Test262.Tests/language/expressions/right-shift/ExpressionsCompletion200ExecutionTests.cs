namespace Jroc.Test262.Tests.language.expressions.right_shift;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.right_shift") { }

    [Fact(DisplayName = "S11.7.2_A1.js", Skip = "eval is not supported.")]
    public Task S11_7_2_A1()
        => ExecutionTest("S11.7.2_A1");
}
