using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.fill;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.fill") { }

    [Fact(DisplayName = "typed-array-resize.js")]
    public Task test_typed_array_resize() => ExecutionTest("typed-array-resize");
}
