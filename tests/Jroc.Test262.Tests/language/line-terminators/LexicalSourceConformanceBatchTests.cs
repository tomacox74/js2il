using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.line_terminators;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/line-terminators", "language.line_terminators") { }

    [Fact(DisplayName = "7.3-15.js")]
    public Task _7_3_15()
        => ExecutionTest("7.3-15");

    [Fact(DisplayName = "7.3-5.js")]
    public Task _7_3_5()
        => ExecutionTest("7.3-5");

    [Fact(DisplayName = "7.3-6.js")]
    public Task _7_3_6()
        => ExecutionTest("7.3-6");

    [Fact(DisplayName = "S7.3_A2.1_T2.js")]
    public Task S7_3_A2_1_T2()
        => CompilationFailureTest("S7.3_A2.1_T2");

    [Fact(DisplayName = "S7.3_A2.2_T2.js")]
    public Task S7_3_A2_2_T2()
        => CompilationFailureTest("S7.3_A2.2_T2");

    [Fact(DisplayName = "S7.3_A3.2_T1.js")]
    public Task S7_3_A3_2_T1()
        => CompilationFailureTest("S7.3_A3.2_T1");

    [Fact(DisplayName = "S7.3_A6_T1.js")]
    public Task S7_3_A6_T1()
        => CompilationFailureTest("S7.3_A6_T1");

    [Fact(DisplayName = "S7.3_A6_T2.js")]
    public Task S7_3_A6_T2()
        => CompilationFailureTest("S7.3_A6_T2");

    [Fact(DisplayName = "S7.3_A6_T3.js")]
    public Task S7_3_A6_T3()
        => CompilationFailureTest("S7.3_A6_T3");

    [Fact(DisplayName = "S7.3_A6_T4.js")]
    public Task S7_3_A6_T4()
        => CompilationFailureTest("S7.3_A6_T4");

    [Fact(DisplayName = "between-tokens-cr.js")]
    public Task between_tokens_cr()
        => ExecutionTest("between-tokens-cr");

    [Fact(DisplayName = "between-tokens-lf.js")]
    public Task between_tokens_lf()
        => ExecutionTest("between-tokens-lf");

    [Fact(DisplayName = "between-tokens-ls.js")]
    public Task between_tokens_ls()
        => ExecutionTest("between-tokens-ls");

    [Fact(DisplayName = "between-tokens-ps.js")]
    public Task between_tokens_ps()
        => ExecutionTest("between-tokens-ps");

    [Fact(DisplayName = "comment-multi-cr.js")]
    public Task comment_multi_cr()
        => ExecutionTest("comment-multi-cr");

    [Fact(DisplayName = "invalid-comment-single-cr.js")]
    public Task invalid_comment_single_cr()
        => CompilationFailureTest("invalid-comment-single-cr");

    [Fact(DisplayName = "invalid-comment-single-lf.js")]
    public Task invalid_comment_single_lf()
        => CompilationFailureTest("invalid-comment-single-lf");

    [Fact(DisplayName = "invalid-comment-single-ls.js")]
    public Task invalid_comment_single_ls()
        => CompilationFailureTest("invalid-comment-single-ls");

    [Fact(DisplayName = "invalid-comment-single-ps.js")]
    public Task invalid_comment_single_ps()
        => CompilationFailureTest("invalid-comment-single-ps");

    [Fact(DisplayName = "invalid-regexp-cr.js")]
    public Task invalid_regexp_cr()
        => CompilationFailureTest("invalid-regexp-cr");

    [Fact(DisplayName = "invalid-regexp-lf.js")]
    public Task invalid_regexp_lf()
        => CompilationFailureTest("invalid-regexp-lf");

    [Fact(DisplayName = "invalid-regexp-ls.js")]
    public Task invalid_regexp_ls()
        => CompilationFailureTest("invalid-regexp-ls");

    [Fact(DisplayName = "invalid-regexp-ps.js")]
    public Task invalid_regexp_ps()
        => CompilationFailureTest("invalid-regexp-ps");

    [Fact(DisplayName = "invalid-string-cr.js")]
    public Task invalid_string_cr()
        => CompilationFailureTest("invalid-string-cr");

    [Fact(DisplayName = "invalid-string-lf.js")]
    public Task invalid_string_lf()
        => CompilationFailureTest("invalid-string-lf");
}
