namespace Jroc.Test262.Tests.language.literals.@string;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.literals.string") { }

    [Fact(DisplayName = "line-separator-eval.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task line_separator_eval()
        => ExecutionTest("line-separator-eval");

    [Fact(DisplayName = "mongolian-vowel-separator-eval.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task mongolian_vowel_separator_eval()
        => ExecutionTest("mongolian-vowel-separator-eval");

    [Fact(DisplayName = "paragraph-separator-eval.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task paragraph_separator_eval()
        => ExecutionTest("paragraph-separator-eval");

}
