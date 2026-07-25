using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.@return;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.return") { }

    [Fact(DisplayName = "tco")]
    public Task tco()
        => ExecutionTest("tco");

}
