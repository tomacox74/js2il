using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array") { }

    [Fact(DisplayName = "length.js")]
    public Task test_length() => ExecutionTest("length");
}
