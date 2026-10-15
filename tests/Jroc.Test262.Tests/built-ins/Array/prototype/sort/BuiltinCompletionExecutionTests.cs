using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.sort;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.sort") { }

    [Fact(DisplayName = "comparefn-grow.js")]
    public Task test_comparefn_grow() => ExecutionTest("comparefn-grow");

    [Fact(DisplayName = "comparefn-shrink.js")]
    public Task test_comparefn_shrink() => ExecutionTest("comparefn-shrink");
}
