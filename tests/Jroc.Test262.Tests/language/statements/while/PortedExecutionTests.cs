using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.@while;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.while") { }

    [Fact(DisplayName = "tco-body")]
    public Task tco_body()
        => ExecutionTest("tco-body");

}
