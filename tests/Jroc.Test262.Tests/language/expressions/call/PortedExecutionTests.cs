using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.call;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.expressions.call") { }

    [Fact(DisplayName = "tco-member-args")]
    public Task tco_member_args()
        => ExecutionTest("tco-member-args");

}
