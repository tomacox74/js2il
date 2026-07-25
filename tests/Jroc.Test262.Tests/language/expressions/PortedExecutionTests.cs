using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.expressions") { }

    [Fact(DisplayName = "tco-pos")]
    public Task tco_pos()
        => ExecutionTest("tco-pos");

}
