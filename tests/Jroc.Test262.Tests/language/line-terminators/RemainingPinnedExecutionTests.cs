namespace Jroc.Test262.Tests.language.line_terminators;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.line_terminators") { }

    [Fact(DisplayName = "S7.3_A5.4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A5_4()
        => ExecutionTest("S7.3_A5.4");

    [Fact(DisplayName = "S7.3_A7_T1.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T1()
        => ExecutionTest("S7.3_A7_T1");

    [Fact(DisplayName = "S7.3_A7_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T2()
        => ExecutionTest("S7.3_A7_T2");

    [Fact(DisplayName = "S7.3_A7_T3.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T3()
        => ExecutionTest("S7.3_A7_T3");

    [Fact(DisplayName = "S7.3_A7_T4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T4()
        => ExecutionTest("S7.3_A7_T4");

    [Fact(DisplayName = "S7.3_A7_T5.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T5()
        => ExecutionTest("S7.3_A7_T5");

    [Fact(DisplayName = "S7.3_A7_T6.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T6()
        => ExecutionTest("S7.3_A7_T6");

    [Fact(DisplayName = "S7.3_A7_T7.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T7()
        => ExecutionTest("S7.3_A7_T7");

    [Fact(DisplayName = "S7.3_A7_T8.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_3_A7_T8()
        => ExecutionTest("S7.3_A7_T8");

    [Fact(DisplayName = "comment-multi-lf.js")]
    public Task comment_multi_lf()
        => ExecutionTest("comment-multi-lf");

    [Fact(DisplayName = "comment-multi-ls.js")]
    public Task comment_multi_ls()
        => ExecutionTest("comment-multi-ls");

    [Fact(DisplayName = "comment-multi-ps.js")]
    public Task comment_multi_ps()
        => ExecutionTest("comment-multi-ps");

    [Fact(DisplayName = "comment-single-cr.js")]
    public Task comment_single_cr()
        => ExecutionTest("comment-single-cr");

    [Fact(DisplayName = "comment-single-lf.js")]
    public Task comment_single_lf()
        => ExecutionTest("comment-single-lf");

    [Fact(DisplayName = "comment-single-ls.js")]
    public Task comment_single_ls()
        => ExecutionTest("comment-single-ls");

    [Fact(DisplayName = "comment-single-ps.js")]
    public Task comment_single_ps()
        => ExecutionTest("comment-single-ps");

}
