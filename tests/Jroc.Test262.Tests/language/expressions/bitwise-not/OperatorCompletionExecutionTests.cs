using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.bitwise_not;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.bitwise_not") { }

    [Fact(DisplayName = "S11.4.8_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_4_8_A1() => ExecutionTest("S11.4.8_A1");

    [Fact(DisplayName = "S11.4.8_A2.2_T1.js")]
    public Task test_S11_4_8_A2_2_T1() => ExecutionTest("S11.4.8_A2.2_T1");

    [Fact(DisplayName = "S11.4.8_A3_T4.js")]
    public Task test_S11_4_8_A3_T4() => ExecutionTest("S11.4.8_A3_T4");

    [Fact(DisplayName = "S11.4.8_A3_T5.js")]
    public Task test_S11_4_8_A3_T5() => ExecutionTest("S11.4.8_A3_T5");

    [Fact(DisplayName = "S9.5_A2.1_T2.js")]
    public Task test_S9_5_A2_1_T2() => ExecutionTest("S9.5_A2.1_T2");

    [Fact(DisplayName = "S9.5_A2.2_T2.js")]
    public Task test_S9_5_A2_2_T2() => ExecutionTest("S9.5_A2.2_T2");

    [Fact(DisplayName = "S9.5_A2.3_T2.js")]
    public Task test_S9_5_A2_3_T2() => ExecutionTest("S9.5_A2.3_T2");

    [Fact(DisplayName = "S9.5_A3.1_T4.js")]
    public Task test_S9_5_A3_1_T4() => ExecutionTest("S9.5_A3.1_T4");

    [Fact(DisplayName = "S9.5_A3.2_T2.js")]
    public Task test_S9_5_A3_2_T2() => ExecutionTest("S9.5_A3.2_T2");
}
