using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.grouping;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.grouping") { }

    [Fact(DisplayName = "S11.1.6_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_1_6_A1() => ExecutionTest("S11.1.6_A1");

    [Fact(DisplayName = "S11.1.6_A3_T3.js")]
    public Task test_S11_1_6_A3_T3() => ExecutionTest("S11.1.6_A3_T3");

    [Fact(DisplayName = "S11.1.6_A3_T6.js")]
    public Task test_S11_1_6_A3_T6() => ExecutionTest("S11.1.6_A3_T6");

    [Fact(DisplayName = "S11.1.6_A3_T7.js")]
    public Task test_S11_1_6_A3_T7() => ExecutionTest("S11.1.6_A3_T7");
}
