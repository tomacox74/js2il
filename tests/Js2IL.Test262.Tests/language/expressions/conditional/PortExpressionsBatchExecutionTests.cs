using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.conditional;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.conditional") { }

    [Fact(DisplayName = "tco-cond", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task tco_cond()
        => ExecutionTest("tco-cond");

    [Fact(DisplayName = "tco-pos", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task tco_pos()
        => ExecutionTest("tco-pos");
}
