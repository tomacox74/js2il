using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.logical_and;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.logical_and") { }

    [Fact(DisplayName = "S11.11.1_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_11_1_A1() => ExecutionTest("S11.11.1_A1");
}
