using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.reduce;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.reduce") { }

    [Fact(DisplayName = "15.4.4.21-9-b-1.js")]
    public Task test_15_4_4_21_9_b_1() => ExecutionTest("15.4.4.21-9-b-1");

    [Fact(DisplayName = "15.4.4.21-9-b-11.js")]
    public Task test_15_4_4_21_9_b_11() => ExecutionTest("15.4.4.21-9-b-11");

    [Fact(DisplayName = "15.4.4.21-9-b-15.js")]
    public Task test_15_4_4_21_9_b_15() => ExecutionTest("15.4.4.21-9-b-15");

    [Fact(DisplayName = "15.4.4.21-9-b-28.js")]
    public Task test_15_4_4_21_9_b_28() => ExecutionTest("15.4.4.21-9-b-28");

    [Fact(DisplayName = "15.4.4.21-9-b-9.js")]
    public Task test_15_4_4_21_9_b_9() => ExecutionTest("15.4.4.21-9-b-9");
}
