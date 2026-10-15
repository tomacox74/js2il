using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.valueOf;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.valueOf") { }

    [Fact(DisplayName = "non-generic-realm.js")]
    public Task test_non_generic_realm() => ExecutionTest("non-generic-realm");

    [Fact(DisplayName = "non-generic.js")]
    public Task test_non_generic() => ExecutionTest("non-generic");
}
