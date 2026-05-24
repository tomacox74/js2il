using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.conditional;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.conditional") { }

    [Fact(DisplayName = "tco-cond")]
    public Task tco_cond()
        => ExecutionTest("tco-cond");

    [Fact(DisplayName = "tco-pos")]
    public Task tco_pos()
        => ExecutionTest("tco-pos");
}
