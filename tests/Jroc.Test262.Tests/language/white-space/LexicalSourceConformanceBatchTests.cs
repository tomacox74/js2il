using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.white_space;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/white-space", "language.white_space") { }

    [Fact(DisplayName = "S7.2_A2.1_T2.js")]
    public Task S7_2_A2_1_T2()
        => ExecutionTest("S7.2_A2.1_T2");

    [Fact(DisplayName = "S7.2_A2.2_T2.js")]
    public Task S7_2_A2_2_T2()
        => ExecutionTest("S7.2_A2.2_T2");

    [Fact(DisplayName = "S7.2_A2.3_T2.js")]
    public Task S7_2_A2_3_T2()
        => ExecutionTest("S7.2_A2.3_T2");

    [Fact(DisplayName = "S7.2_A2.4_T2.js")]
    public Task S7_2_A2_4_T2()
        => ExecutionTest("S7.2_A2.4_T2");

    [Fact(DisplayName = "S7.2_A2.5_T2.js")]
    public Task S7_2_A2_5_T2()
        => ExecutionTest("S7.2_A2.5_T2");

    [Fact(DisplayName = "S7.2_A3.1_T2.js")]
    public Task S7_2_A3_1_T2()
        => ExecutionTest("S7.2_A3.1_T2");

    [Fact(DisplayName = "S7.2_A3.2_T2.js")]
    public Task S7_2_A3_2_T2()
        => ExecutionTest("S7.2_A3.2_T2");

    [Fact(DisplayName = "S7.2_A3.3_T2.js")]
    public Task S7_2_A3_3_T2()
        => ExecutionTest("S7.2_A3.3_T2");

    [Fact(DisplayName = "S7.2_A3.4_T2.js")]
    public Task S7_2_A3_4_T2()
        => ExecutionTest("S7.2_A3.4_T2");

    [Fact(DisplayName = "S7.2_A3.5_T2.js")]
    public Task S7_2_A3_5_T2()
        => ExecutionTest("S7.2_A3.5_T2");

    [Fact(DisplayName = "S7.2_A4.1_T2.js")]
    public Task S7_2_A4_1_T2()
        => ExecutionTest("S7.2_A4.1_T2");

    [Fact(DisplayName = "S7.2_A4.2_T2.js")]
    public Task S7_2_A4_2_T2()
        => ExecutionTest("S7.2_A4.2_T2");

    [Fact(DisplayName = "S7.2_A4.3_T2.js")]
    public Task S7_2_A4_3_T2()
        => ExecutionTest("S7.2_A4.3_T2");

    [Fact(DisplayName = "S7.2_A4.4_T2.js")]
    public Task S7_2_A4_4_T2()
        => ExecutionTest("S7.2_A4.4_T2");

    [Fact(DisplayName = "S7.2_A4.5_T2.js")]
    public Task S7_2_A4_5_T2()
        => ExecutionTest("S7.2_A4.5_T2");

    [Fact(DisplayName = "S7.2_A5_T1.js")]
    public Task S7_2_A5_T1()
        => CompilationFailureTest("S7.2_A5_T1");

    [Fact(DisplayName = "S7.2_A5_T2.js")]
    public Task S7_2_A5_T2()
        => CompilationFailureTest("S7.2_A5_T2");

    [Fact(DisplayName = "S7.2_A5_T3.js")]
    public Task S7_2_A5_T3()
        => CompilationFailureTest("S7.2_A5_T3");

    [Fact(DisplayName = "S7.2_A5_T4.js")]
    public Task S7_2_A5_T4()
        => CompilationFailureTest("S7.2_A5_T4");

    [Fact(DisplayName = "S7.2_A5_T5.js")]
    public Task S7_2_A5_T5()
        => CompilationFailureTest("S7.2_A5_T5");

    [Fact(DisplayName = "after-regular-expression-literal-carriage-return.js")]
    public Task after_regular_expression_literal_carriage_return()
        => ExecutionTest("after-regular-expression-literal-carriage-return");

    [Fact(DisplayName = "after-regular-expression-literal-em-quad.js")]
    public Task after_regular_expression_literal_em_quad()
        => ExecutionTest("after-regular-expression-literal-em-quad");

    [Fact(DisplayName = "after-regular-expression-literal-em-space.js")]
    public Task after_regular_expression_literal_em_space()
        => ExecutionTest("after-regular-expression-literal-em-space");

    [Fact(DisplayName = "after-regular-expression-literal-en-quad.js")]
    public Task after_regular_expression_literal_en_quad()
        => ExecutionTest("after-regular-expression-literal-en-quad");

    [Fact(DisplayName = "after-regular-expression-literal-en-space.js")]
    public Task after_regular_expression_literal_en_space()
        => ExecutionTest("after-regular-expression-literal-en-space");

    [Fact(DisplayName = "after-regular-expression-literal-figure-space.js")]
    public Task after_regular_expression_literal_figure_space()
        => ExecutionTest("after-regular-expression-literal-figure-space");

    [Fact(DisplayName = "after-regular-expression-literal-form-feed.js")]
    public Task after_regular_expression_literal_form_feed()
        => ExecutionTest("after-regular-expression-literal-form-feed");

    [Fact(DisplayName = "after-regular-expression-literal-four-per-em-space.js")]
    public Task after_regular_expression_literal_four_per_em_space()
        => ExecutionTest("after-regular-expression-literal-four-per-em-space");

    [Fact(DisplayName = "after-regular-expression-literal-hair-space.js")]
    public Task after_regular_expression_literal_hair_space()
        => ExecutionTest("after-regular-expression-literal-hair-space");

    [Fact(DisplayName = "after-regular-expression-literal-ideographic-space.js")]
    public Task after_regular_expression_literal_ideographic_space()
        => ExecutionTest("after-regular-expression-literal-ideographic-space");

    [Fact(DisplayName = "after-regular-expression-literal-line-feed.js")]
    public Task after_regular_expression_literal_line_feed()
        => ExecutionTest("after-regular-expression-literal-line-feed");

    [Fact(DisplayName = "after-regular-expression-literal-line-separator.js")]
    public Task after_regular_expression_literal_line_separator()
        => ExecutionTest("after-regular-expression-literal-line-separator");

    [Fact(DisplayName = "after-regular-expression-literal-medium-mathematical-space.js")]
    public Task after_regular_expression_literal_medium_mathematical_space()
        => ExecutionTest("after-regular-expression-literal-medium-mathematical-space");

    [Fact(DisplayName = "after-regular-expression-literal-nbsp.js")]
    public Task after_regular_expression_literal_nbsp()
        => ExecutionTest("after-regular-expression-literal-nbsp");

    [Fact(DisplayName = "after-regular-expression-literal-nnbsp.js")]
    public Task after_regular_expression_literal_nnbsp()
        => ExecutionTest("after-regular-expression-literal-nnbsp");

    [Fact(DisplayName = "after-regular-expression-literal-ogham-space.js")]
    public Task after_regular_expression_literal_ogham_space()
        => ExecutionTest("after-regular-expression-literal-ogham-space");

    [Fact(DisplayName = "after-regular-expression-literal-paragraph-separator.js")]
    public Task after_regular_expression_literal_paragraph_separator()
        => ExecutionTest("after-regular-expression-literal-paragraph-separator");

    [Fact(DisplayName = "after-regular-expression-literal-punctuation-space.js")]
    public Task after_regular_expression_literal_punctuation_space()
        => ExecutionTest("after-regular-expression-literal-punctuation-space");

    [Fact(DisplayName = "after-regular-expression-literal-six-per-em-space.js")]
    public Task after_regular_expression_literal_six_per_em_space()
        => ExecutionTest("after-regular-expression-literal-six-per-em-space");

    [Fact(DisplayName = "after-regular-expression-literal-space.js")]
    public Task after_regular_expression_literal_space()
        => ExecutionTest("after-regular-expression-literal-space");

    [Fact(DisplayName = "after-regular-expression-literal-tab.js")]
    public Task after_regular_expression_literal_tab()
        => ExecutionTest("after-regular-expression-literal-tab");

    [Fact(DisplayName = "after-regular-expression-literal-thin-space.js")]
    public Task after_regular_expression_literal_thin_space()
        => ExecutionTest("after-regular-expression-literal-thin-space");

    [Fact(DisplayName = "after-regular-expression-literal-three-per-em-space.js")]
    public Task after_regular_expression_literal_three_per_em_space()
        => ExecutionTest("after-regular-expression-literal-three-per-em-space");

    [Fact(DisplayName = "after-regular-expression-literal-vertical-tab.js")]
    public Task after_regular_expression_literal_vertical_tab()
        => ExecutionTest("after-regular-expression-literal-vertical-tab");

    [Fact(DisplayName = "after-regular-expression-literal-zwnbsp.js")]
    public Task after_regular_expression_literal_zwnbsp()
        => ExecutionTest("after-regular-expression-literal-zwnbsp");

    [Fact(DisplayName = "between-form-feed.js")]
    public Task between_form_feed()
        => ExecutionTest("between-form-feed");

    [Fact(DisplayName = "between-horizontal-tab.js")]
    public Task between_horizontal_tab()
        => ExecutionTest("between-horizontal-tab");

    [Fact(DisplayName = "between-nbsp.js")]
    public Task between_nbsp()
        => ExecutionTest("between-nbsp");

    [Fact(DisplayName = "between-space.js")]
    public Task between_space()
        => ExecutionTest("between-space");

    [Fact(DisplayName = "between-vertical-tab.js")]
    public Task between_vertical_tab()
        => ExecutionTest("between-vertical-tab");

    [Fact(DisplayName = "mongolian-vowel-separator.js")]
    public Task mongolian_vowel_separator()
        => CompilationFailureTest("mongolian-vowel-separator");
}
