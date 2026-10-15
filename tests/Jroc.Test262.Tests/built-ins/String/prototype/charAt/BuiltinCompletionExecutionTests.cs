using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.charAt;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.charAt") { }

    [Fact(DisplayName = "S15.5.4.4_A1.1.js", Skip = "eval is not supported.")]
    public Task test_S15_5_4_4_A1_1() => ExecutionTest("S15.5.4.4_A1.1");
}
