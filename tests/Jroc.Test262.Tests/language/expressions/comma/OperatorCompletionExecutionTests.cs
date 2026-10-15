using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.comma;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.comma") { }

    [Fact(DisplayName = "S11.14_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_14_A1() => ExecutionTest("S11.14_A1");
}
