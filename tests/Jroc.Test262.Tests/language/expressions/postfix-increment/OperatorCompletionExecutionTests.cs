using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.postfix_increment;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.postfix_increment") { }

    [Fact(DisplayName = "11.3.1-2-1gs.js")]
    public Task test_11_3_1_2_1gs() => CompilationFailureTest("11.3.1-2-1gs", string.Empty);

    [Fact(DisplayName = "arguments.js")]
    public Task test_arguments() => CompilationFailureTest("arguments", string.Empty);

    [Fact(DisplayName = "eval.js")]
    public Task test_eval() => CompilationFailureTest("eval", string.Empty);

    [Fact(DisplayName = "line-terminator-carriage-return.js")]
    public Task test_line_terminator_carriage_return() => CompilationFailureTest("line-terminator-carriage-return", string.Empty);

    [Fact(DisplayName = "line-terminator-line-feed.js")]
    public Task test_line_terminator_line_feed() => CompilationFailureTest("line-terminator-line-feed", string.Empty);

    [Fact(DisplayName = "line-terminator-line-separator.js")]
    public Task test_line_terminator_line_separator() => CompilationFailureTest("line-terminator-line-separator", string.Empty);

    [Fact(DisplayName = "line-terminator-paragraph-separator.js")]
    public Task test_line_terminator_paragraph_separator() => CompilationFailureTest("line-terminator-paragraph-separator", string.Empty);

    [Fact(DisplayName = "S11.3.1_A6_T1.js")]
    public Task test_S11_3_1_A6_T1() => ExecutionTest("S11.3.1_A6_T1");

    [Fact(DisplayName = "S11.3.1_A6_T2.js")]
    public Task test_S11_3_1_A6_T2() => ExecutionTest("S11.3.1_A6_T2");

    [Fact(DisplayName = "target-cover-newtarget.js")]
    public Task test_target_cover_newtarget() => CompilationFailureTest("target-cover-newtarget", string.Empty);

    [Fact(DisplayName = "target-cover-yieldexpr.js")]
    public Task test_target_cover_yieldexpr() => CompilationFailureTest("target-cover-yieldexpr", string.Empty);

    [Fact(DisplayName = "target-newtarget.js")]
    public Task test_target_newtarget() => CompilationFailureTest("target-newtarget", string.Empty);

    [Fact(DisplayName = "this.js")]
    public Task test_this() => CompilationFailureTest("this", string.Empty);
}
