using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.does_not_equals;

public class PortExpressionOperatorExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionOperatorExecutionTests() : base("language.expressions.does_not_equals") { }

    [Fact(DisplayName = "S11.9.2_A2.1_T1")]
    public Task S11_9_2_A2_1_T1()
        => ExecutionTest("S11.9.2_A2.1_T1");

    [Fact(DisplayName = "S11.9.2_A2.1_T2")]
    public Task S11_9_2_A2_1_T2()
        => ExecutionTest("S11.9.2_A2.1_T2");

    [Fact(DisplayName = "S11.9.2_A2.1_T3")]
    public Task S11_9_2_A2_1_T3()
        => ExecutionTest("S11.9.2_A2.1_T3");

    [Fact(DisplayName = "S11.9.2_A2.4_T1")]
    public Task S11_9_2_A2_4_T1()
        => ExecutionTest("S11.9.2_A2.4_T1");

    [Fact(DisplayName = "S11.9.2_A2.4_T2")]
    public Task S11_9_2_A2_4_T2()
        => ExecutionTest("S11.9.2_A2.4_T2");
}
