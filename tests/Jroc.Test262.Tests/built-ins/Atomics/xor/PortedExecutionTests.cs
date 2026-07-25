using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.xor;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("built_ins.Atomics.xor") { }

    [Fact(DisplayName = "non-views")]
    public Task non_views()
        => ExecutionTest("non-views");

}
