namespace Jroc.Test262.Tests.language.expressions.modulus;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.modulus") { }

    [Fact(DisplayName = "S11.5.3_A1.js", Skip = "eval is not supported.")]
    public Task S11_5_3_A1()
        => ExecutionTest("S11.5.3_A1");
}
