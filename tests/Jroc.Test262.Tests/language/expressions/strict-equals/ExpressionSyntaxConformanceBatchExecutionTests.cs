using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.strict_equals;

public class ExpressionSyntaxConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchExecutionTests() : base("language/expressions/strict-equals", "language.expressions.strict_equals") { }

    [Fact(DisplayName = "S11.9.4_A2.1_T1.js")]
    public Task S11_9_4_A2_1_T1()
        => ExecutionTest("S11.9.4_A2.1_T1");

    [Fact(DisplayName = "S11.9.4_A2.1_T2.js")]
    public Task S11_9_4_A2_1_T2()
        => ExecutionTest("S11.9.4_A2.1_T2");

    [Fact(DisplayName = "S11.9.4_A2.1_T3.js")]
    public Task S11_9_4_A2_1_T3()
        => ExecutionTest("S11.9.4_A2.1_T3");

    [Fact(DisplayName = "S11.9.4_A2.4_T1.js")]
    public Task S11_9_4_A2_4_T1()
        => ExecutionTest("S11.9.4_A2.4_T1");

    [Fact(DisplayName = "S11.9.4_A2.4_T3.js")]
    public Task S11_9_4_A2_4_T3()
        => ExecutionTest("S11.9.4_A2.4_T3");

    [Fact(DisplayName = "S11.9.4_A2.4_T4.js")]
    public Task S11_9_4_A2_4_T4()
        => ExecutionTest("S11.9.4_A2.4_T4");

    [Fact(DisplayName = "S11.9.4_A3.js")]
    public Task S11_9_4_A3()
        => ExecutionTest("S11.9.4_A3");

    [Fact(DisplayName = "S11.9.4_A4.1_T1.js")]
    public Task S11_9_4_A4_1_T1()
        => ExecutionTest("S11.9.4_A4.1_T1");

    [Fact(DisplayName = "S11.9.4_A4.1_T2.js")]
    public Task S11_9_4_A4_1_T2()
        => ExecutionTest("S11.9.4_A4.1_T2");

    [Fact(DisplayName = "S11.9.4_A4.3.js")]
    public Task S11_9_4_A4_3()
        => ExecutionTest("S11.9.4_A4.3");

    [Fact(DisplayName = "S11.9.4_A5.js")]
    public Task S11_9_4_A5()
        => ExecutionTest("S11.9.4_A5");

    [Fact(DisplayName = "S11.9.4_A7.js")]
    public Task S11_9_4_A7()
        => ExecutionTest("S11.9.4_A7");

    [Fact(DisplayName = "S11.9.4_A8_T1.js")]
    public Task S11_9_4_A8_T1()
        => ExecutionTest("S11.9.4_A8_T1");

    [Fact(DisplayName = "S11.9.4_A8_T2.js")]
    public Task S11_9_4_A8_T2()
        => ExecutionTest("S11.9.4_A8_T2");

    [Fact(DisplayName = "S11.9.4_A8_T3.js")]
    public Task S11_9_4_A8_T3()
        => ExecutionTest("S11.9.4_A8_T3");

}
