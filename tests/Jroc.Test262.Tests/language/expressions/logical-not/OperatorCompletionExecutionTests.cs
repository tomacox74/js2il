using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.logical_not;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.logical_not") { }

    [Fact(DisplayName = "bigint.js")]
    public Task test_bigint() => ExecutionTest("bigint");

    [Fact(DisplayName = "S11.4.9_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_4_9_A1() => ExecutionTest("S11.4.9_A1");

    [Fact(DisplayName = "S11.4.9_A2.2_T1.js")]
    public Task test_S11_4_9_A2_2_T1() => ExecutionTest("S11.4.9_A2.2_T1");

    [Fact(DisplayName = "S11.4.9_A3_T4.js")]
    public Task test_S11_4_9_A3_T4() => ExecutionTest("S11.4.9_A3_T4");

    [Fact(DisplayName = "S11.4.9_A3_T5.js")]
    public Task test_S11_4_9_A3_T5() => ExecutionTest("S11.4.9_A3_T5");

    [Fact(DisplayName = "S9.2_A1_T2.js", Skip = "eval is not supported.")]
    public Task test_S9_2_A1_T2() => ExecutionTest("S9.2_A1_T2");

    [Fact(DisplayName = "S9.2_A2_T2.js")]
    public Task test_S9_2_A2_T2() => ExecutionTest("S9.2_A2_T2");

    [Fact(DisplayName = "S9.2_A3_T2.js")]
    public Task test_S9_2_A3_T2() => ExecutionTest("S9.2_A3_T2");

    [Fact(DisplayName = "S9.2_A4_T2.js")]
    public Task test_S9_2_A4_T2() => ExecutionTest("S9.2_A4_T2");

    [Fact(DisplayName = "S9.2_A4_T4.js")]
    public Task test_S9_2_A4_T4() => ExecutionTest("S9.2_A4_T4");

    [Fact(DisplayName = "S9.2_A5_T2.js")]
    public Task test_S9_2_A5_T2() => ExecutionTest("S9.2_A5_T2");

    [Fact(DisplayName = "S9.2_A5_T4.js")]
    public Task test_S9_2_A5_T4() => ExecutionTest("S9.2_A5_T4");

    [Fact(DisplayName = "S9.2_A6_T2.js")]
    public Task test_S9_2_A6_T2() => ExecutionTest("S9.2_A6_T2");

    [Fact(DisplayName = "symbol-logical-not-evaluation.js")]
    public Task test_symbol_logical_not_evaluation() => ExecutionTest("symbol-logical-not-evaluation");
}
