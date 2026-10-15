using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.void_;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.void") { }

    [Fact(DisplayName = "S11.4.2_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_4_2_A1() => ExecutionTest("S11.4.2_A1");

    [Fact(DisplayName = "S11.4.2_A4_T3.js")]
    public Task test_S11_4_2_A4_T3() => ExecutionTest("S11.4.2_A4_T3");

    [Fact(DisplayName = "S11.4.2_A4_T4.js")]
    public Task test_S11_4_2_A4_T4() => ExecutionTest("S11.4.2_A4_T4");

    [Fact(DisplayName = "S11.4.2_A4_T5.js")]
    public Task test_S11_4_2_A4_T5() => ExecutionTest("S11.4.2_A4_T5");

    [Fact(DisplayName = "S11.4.2_A4_T6.js")]
    public Task test_S11_4_2_A4_T6() => ExecutionTest("S11.4.2_A4_T6");
}
