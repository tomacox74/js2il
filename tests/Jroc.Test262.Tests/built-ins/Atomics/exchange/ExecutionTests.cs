using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.exchange;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.exchange") { }

    [Fact(DisplayName = "non-views")]
    public Task non_views()
        => ExecutionTest("non-views");

}
