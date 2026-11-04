namespace Jroc.Test262.Tests.language.white_space;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.white_space") { }

    [Fact(DisplayName = "comment-multi-form-feed.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_multi_form_feed()
        => ExecutionTest("comment-multi-form-feed");

    [Fact(DisplayName = "comment-multi-horizontal-tab.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_multi_horizontal_tab()
        => ExecutionTest("comment-multi-horizontal-tab");

    [Fact(DisplayName = "comment-multi-nbsp.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_multi_nbsp()
        => ExecutionTest("comment-multi-nbsp");

    [Fact(DisplayName = "comment-multi-space.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_multi_space()
        => ExecutionTest("comment-multi-space");

    [Fact(DisplayName = "comment-multi-vertical-tab.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_multi_vertical_tab()
        => ExecutionTest("comment-multi-vertical-tab");

    [Fact(DisplayName = "comment-single-form-feed.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_single_form_feed()
        => ExecutionTest("comment-single-form-feed");

    [Fact(DisplayName = "comment-single-horizontal-tab.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_single_horizontal_tab()
        => ExecutionTest("comment-single-horizontal-tab");

    [Fact(DisplayName = "comment-single-nbsp.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_single_nbsp()
        => ExecutionTest("comment-single-nbsp");

    [Fact(DisplayName = "comment-single-space.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_single_space()
        => ExecutionTest("comment-single-space");

    [Fact(DisplayName = "comment-single-vertical-tab.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task comment_single_vertical_tab()
        => ExecutionTest("comment-single-vertical-tab");

    [Fact(DisplayName = "mongolian-vowel-separator-eval.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task mongolian_vowel_separator_eval()
        => ExecutionTest("mongolian-vowel-separator-eval");

    [Fact(DisplayName = "string-form-feed.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task string_form_feed()
        => ExecutionTest("string-form-feed");

    [Fact(DisplayName = "string-horizontal-tab.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task string_horizontal_tab()
        => ExecutionTest("string-horizontal-tab");

    [Fact(DisplayName = "string-nbsp.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task string_nbsp()
        => ExecutionTest("string-nbsp");

    [Fact(DisplayName = "string-space.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task string_space()
        => ExecutionTest("string-space");

    [Fact(DisplayName = "string-vertical-tab.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task string_vertical_tab()
        => ExecutionTest("string-vertical-tab");

}
