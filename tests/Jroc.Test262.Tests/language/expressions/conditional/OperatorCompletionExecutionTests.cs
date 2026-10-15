using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.conditional;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.conditional") { }

    [Fact(DisplayName = "S11.12_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_12_A1() => ExecutionTest("S11.12_A1");

    [Fact(DisplayName = "S11.12_A2.1_T6.js")]
    public Task test_S11_12_A2_1_T6() => ExecutionTest("S11.12_A2.1_T6");

    [Fact(DisplayName = "S11.12_A3_T1.js")]
    public Task test_S11_12_A3_T1() => ExecutionTest("S11.12_A3_T1");

    [Fact(DisplayName = "S11.12_A3_T3.js")]
    public Task test_S11_12_A3_T3() => ExecutionTest("S11.12_A3_T3");

    [Fact(DisplayName = "S11.12_A3_T4.js")]
    public Task test_S11_12_A3_T4() => ExecutionTest("S11.12_A3_T4");

    [Fact(DisplayName = "S11.12_A4_T1.js")]
    public Task test_S11_12_A4_T1() => ExecutionTest("S11.12_A4_T1");

    [Fact(DisplayName = "S11.12_A4_T3.js")]
    public Task test_S11_12_A4_T3() => ExecutionTest("S11.12_A4_T3");

    [Fact(DisplayName = "S11.12_A4_T4.js")]
    public Task test_S11_12_A4_T4() => ExecutionTest("S11.12_A4_T4");
}
