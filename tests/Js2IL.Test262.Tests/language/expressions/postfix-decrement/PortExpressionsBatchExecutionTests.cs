using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.postfix_decrement;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.postfix_decrement") { }

    [Fact(DisplayName = "S11.3.2_A3_T1")]
    public Task S11_3_2_A3_T1()
        => ExecutionTest("S11.3.2_A3_T1");
}
