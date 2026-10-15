using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.prefix_decrement;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.prefix_decrement") { }

    [Fact(DisplayName = "11.4.5-2-2gs.js")]
    public Task test_11_4_5_2_2gs() => CompilationFailureTest("11.4.5-2-2gs", string.Empty);

    [Fact(DisplayName = "arguments.js")]
    public Task test_arguments() => CompilationFailureTest("arguments", string.Empty);

    [Fact(DisplayName = "eval.js")]
    public Task test_eval() => CompilationFailureTest("eval", string.Empty);

    [Fact(DisplayName = "S11.4.5_A6_T1.js")]
    public Task test_S11_4_5_A6_T1() => ExecutionTest("S11.4.5_A6_T1");

    [Fact(DisplayName = "S11.4.5_A6_T2.js")]
    public Task test_S11_4_5_A6_T2() => ExecutionTest("S11.4.5_A6_T2");

    [Fact(DisplayName = "target-cover-newtarget.js")]
    public Task test_target_cover_newtarget() => CompilationFailureTest("target-cover-newtarget", string.Empty);

    [Fact(DisplayName = "target-cover-yieldexpr.js")]
    public Task test_target_cover_yieldexpr() => CompilationFailureTest("target-cover-yieldexpr", string.Empty);

    [Fact(DisplayName = "target-newtarget.js")]
    public Task test_target_newtarget() => CompilationFailureTest("target-newtarget", string.Empty);

    [Fact(DisplayName = "this.js")]
    public Task test_this() => CompilationFailureTest("this", string.Empty);
}
