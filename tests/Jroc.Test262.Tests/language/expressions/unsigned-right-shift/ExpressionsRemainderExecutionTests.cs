namespace Jroc.Test262.Tests.language.expressions.unsigned_right_shift;

public sealed class ExpressionsRemainderExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsRemainderExecutionTests() : base("language.expressions.unsigned_right_shift") { }

    [Fact(DisplayName = "S11.7.3_A1.js", Skip = "eval is not supported.")]
    public Task S11_7_3_A1()
        => ExecutionTest("S11.7.3_A1");
}
