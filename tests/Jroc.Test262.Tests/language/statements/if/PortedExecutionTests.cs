using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.@if;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.if") { }

    [Fact(DisplayName = "tco-else-body")]
    public Task tco_else_body()
        => ExecutionTest("tco-else-body");

    [Fact(DisplayName = "tco-if-body")]
    public Task tco_if_body()
        => ExecutionTest("tco-if-body");

}
