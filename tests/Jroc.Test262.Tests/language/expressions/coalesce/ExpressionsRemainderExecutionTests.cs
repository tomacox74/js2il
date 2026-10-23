namespace Jroc.Test262.Tests.language.expressions.coalesce;

public sealed class ExpressionsRemainderExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsRemainderExecutionTests() : base("language.expressions.coalesce") { }

    [Fact(DisplayName = "tco-pos-null.js")]
    public Task tco_pos_null()
        => ExecutionTest("tco-pos-null");

    [Fact(DisplayName = "tco-pos-undefined.js")]
    public Task tco_pos_undefined()
        => ExecutionTest("tco-pos-undefined");
}
