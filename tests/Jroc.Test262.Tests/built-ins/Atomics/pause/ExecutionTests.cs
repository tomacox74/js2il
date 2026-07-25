using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.pause;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.pause") { }

    [Fact(DisplayName = "non-integral-iterationnumber-throws")]
    public Task non_integral_iterationnumber_throws()
        => ExecutionTest("non-integral-iterationnumber-throws");

}
