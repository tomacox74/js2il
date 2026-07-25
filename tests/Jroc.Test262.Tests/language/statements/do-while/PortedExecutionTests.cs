using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.do_while;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.do_while") { }

    [Fact(DisplayName = "tco-body")]
    public Task tco_body()
        => ExecutionTest("tco-body");

}
