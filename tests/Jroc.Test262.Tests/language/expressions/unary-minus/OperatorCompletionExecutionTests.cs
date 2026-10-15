using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.unary_minus;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.unary_minus") { }

    [Fact(DisplayName = "bigint.js")]
    public Task test_bigint() => ExecutionTest("bigint");

    [Fact(DisplayName = "S11.4.7_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_4_7_A1() => ExecutionTest("S11.4.7_A1");

    [Fact(DisplayName = "S11.4.7_A2.1_T2.js")]
    public Task test_S11_4_7_A2_1_T2() => ExecutionTest("S11.4.7_A2.1_T2");

    [Fact(DisplayName = "S11.4.7_A2.2_T1.js")]
    public Task test_S11_4_7_A2_2_T1() => ExecutionTest("S11.4.7_A2.2_T1");

    [Fact(DisplayName = "S11.4.7_A3_T4.js")]
    public Task test_S11_4_7_A3_T4() => ExecutionTest("S11.4.7_A3_T4");

    [Fact(DisplayName = "S11.4.7_A3_T5.js")]
    public Task test_S11_4_7_A3_T5() => ExecutionTest("S11.4.7_A3_T5");

    [Fact(DisplayName = "S11.4.7_A4.1.js")]
    public Task test_S11_4_7_A4_1() => ExecutionTest("S11.4.7_A4.1");

    [Fact(DisplayName = "S11.4.7_A4.2.js")]
    public Task test_S11_4_7_A4_2() => ExecutionTest("S11.4.7_A4.2");
}
