using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.block;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.block") { }

    [Fact(DisplayName = "tco-stmt-list")]
    public Task tco_stmt_list()
        => ExecutionTest("tco-stmt-list");

    [Fact(DisplayName = "tco-stmt")]
    public Task tco_stmt()
        => ExecutionTest("tco-stmt");

}
