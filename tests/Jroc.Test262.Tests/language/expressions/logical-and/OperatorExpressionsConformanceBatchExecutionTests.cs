using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.logical_and;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/logical-and", "language.expressions.logical_and") { }

    [Fact(DisplayName = "S11.11.1_A2.1_T3.js")]
    public Task S11_11_1_A2_1_T3()
        => ExecutionTest("S11.11.1_A2.1_T3");

    [Fact(DisplayName = "S11.11.1_A2.1_T4.js")]
    public Task S11_11_1_A2_1_T4()
        => ExecutionTest("S11.11.1_A2.1_T4");

    [Fact(DisplayName = "S11.11.1_A2.4_T1.js")]
    public Task S11_11_1_A2_4_T1()
        => ExecutionTest("S11.11.1_A2.4_T1");

    [Fact(DisplayName = "S11.11.1_A2.4_T3.js")]
    public Task S11_11_1_A2_4_T3()
        => ExecutionTest("S11.11.1_A2.4_T3");

    [Fact(DisplayName = "S11.11.1_A3_T1.js")]
    public Task S11_11_1_A3_T1()
        => ExecutionTest("S11.11.1_A3_T1");

    [Fact(DisplayName = "S11.11.1_A3_T3.js")]
    public Task S11_11_1_A3_T3()
        => ExecutionTest("S11.11.1_A3_T3");

    [Fact(DisplayName = "S11.11.1_A3_T4.js")]
    public Task S11_11_1_A3_T4()
        => ExecutionTest("S11.11.1_A3_T4");

    [Fact(DisplayName = "S11.11.1_A4_T1.js")]
    public Task S11_11_1_A4_T1()
        => ExecutionTest("S11.11.1_A4_T1");

    [Fact(DisplayName = "S11.11.1_A4_T3.js")]
    public Task S11_11_1_A4_T3()
        => ExecutionTest("S11.11.1_A4_T3");

}
