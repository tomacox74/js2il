namespace Jroc.Test262.Tests.language.expressions.comma;

public sealed class ExpressionsRemainderExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsRemainderExecutionTests() : base("language.expressions.comma") { }

    [Fact(DisplayName = "tco-final.js")]
    public Task tco_final()
        => ExecutionTest("tco-final");
}
