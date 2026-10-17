namespace Jroc.Test262.Tests.language.expressions.multiplication;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.multiplication") { }

    [Fact(DisplayName = "S11.5.1_A1.js", Skip = "eval is not supported.")]
    public Task S11_5_1_A1()
        => ExecutionTest("S11.5.1_A1");
}
