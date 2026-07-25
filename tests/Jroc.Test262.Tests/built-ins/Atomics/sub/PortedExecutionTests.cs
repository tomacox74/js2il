using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.sub;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("built_ins.Atomics.sub") { }

    [Fact(DisplayName = "non-views")]
    public Task non_views()
        => ExecutionTest("non-views");

}
