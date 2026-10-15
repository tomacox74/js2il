using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toString;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.toString") { }

    [Fact(DisplayName = "non-callable-join-string-tag.js")]
    public Task test_non_callable_join_string_tag() => ExecutionTest("non-callable-join-string-tag");
}
