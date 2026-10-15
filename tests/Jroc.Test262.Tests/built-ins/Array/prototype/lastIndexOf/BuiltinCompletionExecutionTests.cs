using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.lastIndexOf;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.lastIndexOf") { }

    [Fact(DisplayName = "coerced-position-grow.js")]
    public Task test_coerced_position_grow() => ExecutionTest("coerced-position-grow");

    [Fact(DisplayName = "coerced-position-shrink.js")]
    public Task test_coerced_position_shrink() => ExecutionTest("coerced-position-shrink");
}
