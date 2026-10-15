using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.this_;

public class ConstructionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructionCompletionExecutionTests() : base("language.expressions.this") { }

    [Fact(DisplayName = "11.1.1-1.js")]
    public Task test_11_1_1_1() => ExecutionTest("11.1.1-1");

    [Fact(DisplayName = "S11.1.1_A1.js")]
    public Task test_S11_1_1_A1() => CompilationFailureTest("S11.1.1_A1", string.Empty);

    [Fact(DisplayName = "S11.1.1_A3.1.js", Skip = "eval is not supported.")]
    public Task test_S11_1_1_A3_1() => ExecutionTest("S11.1.1_A3.1");

    [Fact(DisplayName = "S11.1.1_A3.2.js", Skip = "eval is not supported.")]
    public Task test_S11_1_1_A3_2() => ExecutionTest("S11.1.1_A3.2");

    [Fact(DisplayName = "S11.1.1_A4.1.js", Skip = "eval is not supported.")]
    public Task test_S11_1_1_A4_1() => ExecutionTest("S11.1.1_A4.1");

    [Fact(DisplayName = "S11.1.1_A4.2.js", Skip = "eval is not supported.")]
    public Task test_S11_1_1_A4_2() => ExecutionTest("S11.1.1_A4.2");
}
