using Xunit;

namespace Jroc.Test262.Tests.language.comments;

public sealed class RemainingCommentsExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public RemainingCommentsExecutionTests() : base("language.comments")
    {
    }

    [Fact(DisplayName = "language/comments/S7.4_A5.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_S7_4_A5() => ExecutionTest("S7.4_A5");

    [Fact(DisplayName = "language/comments/S7.4_A6.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_S7_4_A6() => ExecutionTest("S7.4_A6");

    [Fact(DisplayName = "language/comments/mongolian-vowel-separator-single-eval.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_mongolian_vowel_separator_single_eval() => ExecutionTest("mongolian-vowel-separator-single-eval");
}
