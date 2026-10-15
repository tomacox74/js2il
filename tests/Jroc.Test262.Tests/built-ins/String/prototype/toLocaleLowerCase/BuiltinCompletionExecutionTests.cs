using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.toLocaleLowerCase;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.toLocaleLowerCase") { }

    [Fact(DisplayName = "S15.5.4.17_A1_T3.js", Skip = "eval is not supported.")]
    public Task test_S15_5_4_17_A1_T3() => ExecutionTest("S15.5.4.17_A1_T3");
}
