using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.postfix_decrement;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.postfix_decrement") { }

    [Fact(DisplayName = "S11.3.2_A3_T1")]
    public Task S11_3_2_A3_T1()
        => ExecutionTest("S11.3.2_A3_T1");

    [Fact(DisplayName = "S11.3.2_A2.1_T1")]
    public Task S11_3_2_A2_1_T1()
        => ExecutionTest("S11.3.2_A2.1_T1");
}
