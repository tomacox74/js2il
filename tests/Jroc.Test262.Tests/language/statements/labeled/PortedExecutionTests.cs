using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.labeled;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.labeled") { }

    [Fact(DisplayName = "tco")]
    public Task tco()
        => ExecutionTest("tco");

}
