using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.logical_and;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.logical_and") { }

    [Fact(DisplayName = "tco-right", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task tco_right()
        => ExecutionTest("tco-right");
}
