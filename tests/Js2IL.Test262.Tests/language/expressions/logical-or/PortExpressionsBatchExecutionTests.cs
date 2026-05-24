using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.logical_or;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.logical_or") { }

    [Fact(DisplayName = "tco-right")]
    public Task tco_right()
        => ExecutionTest("tco-right");
}
