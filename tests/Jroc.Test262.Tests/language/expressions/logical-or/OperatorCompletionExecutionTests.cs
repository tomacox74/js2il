using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.logical_or;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.logical_or") { }

    [Fact(DisplayName = "S11.11.2_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_11_2_A1() => ExecutionTest("S11.11.2_A1");
}
