using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.charCodeAt;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.charCodeAt") { }

    [Fact(DisplayName = "S15.5.4.5_A1.1.js", Skip = "eval is not supported.")]
    public Task test_S15_5_4_5_A1_1() => ExecutionTest("S15.5.4.5_A1.1");
}
