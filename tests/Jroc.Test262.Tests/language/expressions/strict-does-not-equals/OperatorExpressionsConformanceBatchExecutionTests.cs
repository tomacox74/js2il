using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.strict_does_not_equals;

public class OperatorExpressionsConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public OperatorExpressionsConformanceBatchExecutionTests() : base("language/expressions/strict-does-not-equals", "language.expressions.strict_does_not_equals") { }

    [Fact(DisplayName = "S11.9.5_A2.4_T3.js")]
    public Task S11_9_5_A2_4_T3()
        => ExecutionTest("S11.9.5_A2.4_T3");

}
