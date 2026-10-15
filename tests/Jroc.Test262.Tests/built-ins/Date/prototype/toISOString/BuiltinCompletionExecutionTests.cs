using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toISOString;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Date.prototype.toISOString") { }

    [Fact(DisplayName = "15.9.5.43-0-10.js")]
    public Task test_15_9_5_43_0_10() => ExecutionTest("15.9.5.43-0-10");

    [Fact(DisplayName = "15.9.5.43-0-11.js")]
    public Task test_15_9_5_43_0_11() => ExecutionTest("15.9.5.43-0-11");

    [Fact(DisplayName = "15.9.5.43-0-12.js")]
    public Task test_15_9_5_43_0_12() => ExecutionTest("15.9.5.43-0-12");

    [Fact(DisplayName = "15.9.5.43-0-9.js")]
    public Task test_15_9_5_43_0_9() => ExecutionTest("15.9.5.43-0-9");
}
