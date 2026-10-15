using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toLocaleString;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.toLocaleString") { }

    [Fact(DisplayName = "user-provided-tolocalestring-grow.js")]
    public Task test_user_provided_tolocalestring_grow() => ExecutionTest("user-provided-tolocalestring-grow");

    [Fact(DisplayName = "user-provided-tolocalestring-shrink.js")]
    public Task test_user_provided_tolocalestring_shrink() => ExecutionTest("user-provided-tolocalestring-shrink");
}
