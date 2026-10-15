using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.unary_plus;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.unary_plus") { }

    [Fact(DisplayName = "bigint-throws.js")]
    public Task test_bigint_throws() => ExecutionTest("bigint-throws");

    [Fact(DisplayName = "S11.4.6_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_4_6_A1() => ExecutionTest("S11.4.6_A1");

    [Fact(DisplayName = "S11.4.6_A2.2_T1.js")]
    public Task test_S11_4_6_A2_2_T1() => ExecutionTest("S11.4.6_A2.2_T1");

    [Fact(DisplayName = "S11.4.6_A3_T5.js")]
    public Task test_S11_4_6_A3_T5() => ExecutionTest("S11.4.6_A3_T5");

    [Fact(DisplayName = "S9.3_A1_T2.js", Skip = "eval is not supported.")]
    public Task test_S9_3_A1_T2() => ExecutionTest("S9.3_A1_T2");

    [Fact(DisplayName = "S9.3_A3_T2.js")]
    public Task test_S9_3_A3_T2() => ExecutionTest("S9.3_A3_T2");

    [Fact(DisplayName = "S9.3_A4.1_T2.js")]
    public Task test_S9_3_A4_1_T2() => ExecutionTest("S9.3_A4.1_T2");

    [Fact(DisplayName = "S9.3_A4.2_T2.js")]
    public Task test_S9_3_A4_2_T2() => ExecutionTest("S9.3_A4.2_T2");
}
