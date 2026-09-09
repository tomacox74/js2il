using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.comments;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/comments", "language.comments") { }

    [Fact(DisplayName = "S7.4_A1_T1.js")]
    public Task S7_4_A1_T1()
        => ExecutionTest("S7.4_A1_T1");

    [Fact(DisplayName = "S7.4_A1_T2.js")]
    public Task S7_4_A1_T2()
        => ExecutionTest("S7.4_A1_T2");

    [Fact(DisplayName = "S7.4_A2_T1.js")]
    public Task S7_4_A2_T1()
        => ExecutionTest("S7.4_A2_T1");

    [Fact(DisplayName = "S7.4_A2_T2.js")]
    public Task S7_4_A2_T2()
        => CompilationFailureTest("S7.4_A2_T2");

    [Fact(DisplayName = "S7.4_A3.js")]
    public Task S7_4_A3()
        => CompilationFailureTest("S7.4_A3");

    [Fact(DisplayName = "S7.4_A4_T1.js")]
    public Task S7_4_A4_T1()
        => CompilationFailureTest("S7.4_A4_T1");

    [Fact(DisplayName = "S7.4_A4_T2.js")]
    public Task S7_4_A4_T2()
        => ExecutionTest("S7.4_A4_T2");

    [Fact(DisplayName = "S7.4_A4_T3.js")]
    public Task S7_4_A4_T3()
        => ExecutionTest("S7.4_A4_T3");

    [Fact(DisplayName = "S7.4_A4_T4.js")]
    public Task S7_4_A4_T4()
        => CompilationFailureTest("S7.4_A4_T4");

    [Fact(DisplayName = "S7.4_A4_T5.js")]
    public Task S7_4_A4_T5()
        => ExecutionTest("S7.4_A4_T5");

    [Fact(DisplayName = "S7.4_A4_T6.js")]
    public Task S7_4_A4_T6()
        => ExecutionTest("S7.4_A4_T6");

    [Fact(DisplayName = "S7.4_A4_T7.js")]
    public Task S7_4_A4_T7()
        => ExecutionTest("S7.4_A4_T7");

    [Fact(DisplayName = "hashbang/function-body.js")]
    public Task hashbang_function_body()
        => CompilationFailureTest("hashbang/function-body");

    [Fact(DisplayName = "hashbang/statement-block.js")]
    public Task hashbang_statement_block()
        => CompilationFailureTest("hashbang/statement-block");

    [Fact(DisplayName = "mongolian-vowel-separator-multi.js")]
    public Task mongolian_vowel_separator_multi()
        => ExecutionTest("mongolian-vowel-separator-multi");

    [Fact(DisplayName = "mongolian-vowel-separator-single.js")]
    public Task mongolian_vowel_separator_single()
        => ExecutionTest("mongolian-vowel-separator-single");

    [Fact(DisplayName = "multi-line-asi-carriage-return.js")]
    public Task multi_line_asi_carriage_return()
        => ExecutionTest("multi-line-asi-carriage-return");

    [Fact(DisplayName = "multi-line-asi-line-feed.js")]
    public Task multi_line_asi_line_feed()
        => ExecutionTest("multi-line-asi-line-feed");

    [Fact(DisplayName = "multi-line-asi-line-separator.js")]
    public Task multi_line_asi_line_separator()
        => ExecutionTest("multi-line-asi-line-separator");

    [Fact(DisplayName = "multi-line-asi-paragraph-separator.js")]
    public Task multi_line_asi_paragraph_separator()
        => ExecutionTest("multi-line-asi-paragraph-separator");

    [Fact(DisplayName = "multi-line-html-close-extra.js")]
    public Task multi_line_html_close_extra()
        => CompilationFailureTest("multi-line-html-close-extra");

    [Fact(DisplayName = "single-line-html-close-without-lt.js")]
    public Task single_line_html_close_without_lt()
        => CompilationFailureTest("single-line-html-close-without-lt");
}
