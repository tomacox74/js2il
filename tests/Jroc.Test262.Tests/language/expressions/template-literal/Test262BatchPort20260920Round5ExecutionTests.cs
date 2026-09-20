using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.template_literal;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.template_literal") { }

    [Fact(DisplayName = "literal-expr-abrupt")]
    public Task literal_expr_abrupt()
        => ExecutionTest("literal-expr-abrupt");

    [Fact(DisplayName = "literal-expr-member-expr")]
    public Task literal_expr_member_expr()
        => ExecutionTest("literal-expr-member-expr");

    [Fact(DisplayName = "literal-expr-obj")]
    public Task literal_expr_obj()
        => ExecutionTest("literal-expr-obj");

    [Fact(DisplayName = "literal-expr-primitive")]
    public Task literal_expr_primitive()
        => ExecutionTest("literal-expr-primitive");

    [Fact(DisplayName = "literal-expr-template")]
    public Task literal_expr_template()
        => ExecutionTest("literal-expr-template");

    [Fact(DisplayName = "literal-expr-tostr-error")]
    public Task literal_expr_tostr_error()
        => ExecutionTest("literal-expr-tostr-error");

    [Fact(DisplayName = "middle-list-many-expr-abrupt")]
    public Task middle_list_many_expr_abrupt()
        => ExecutionTest("middle-list-many-expr-abrupt");

    [Fact(DisplayName = "middle-list-many-expr-function")]
    public Task middle_list_many_expr_function()
        => ExecutionTest("middle-list-many-expr-function");

    [Fact(DisplayName = "middle-list-many-expr-member-expr")]
    public Task middle_list_many_expr_member_expr()
        => ExecutionTest("middle-list-many-expr-member-expr");

    [Fact(DisplayName = "middle-list-many-expr-method")]
    public Task middle_list_many_expr_method()
        => ExecutionTest("middle-list-many-expr-method");

    [Fact(DisplayName = "middle-list-many-expr-obj")]
    public Task middle_list_many_expr_obj()
        => ExecutionTest("middle-list-many-expr-obj");

    [Fact(DisplayName = "middle-list-many-expr-primitive")]
    public Task middle_list_many_expr_primitive()
        => ExecutionTest("middle-list-many-expr-primitive");

    [Fact(DisplayName = "middle-list-many-expr-template")]
    public Task middle_list_many_expr_template()
        => ExecutionTest("middle-list-many-expr-template");

    [Fact(DisplayName = "middle-list-many-expr-tostr-error")]
    public Task middle_list_many_expr_tostr_error()
        => ExecutionTest("middle-list-many-expr-tostr-error");

    [Fact(DisplayName = "middle-list-one-expr-abrupt")]
    public Task middle_list_one_expr_abrupt()
        => ExecutionTest("middle-list-one-expr-abrupt");

    [Fact(DisplayName = "middle-list-one-expr-function")]
    public Task middle_list_one_expr_function()
        => ExecutionTest("middle-list-one-expr-function");

    [Fact(DisplayName = "middle-list-one-expr-member-expr")]
    public Task middle_list_one_expr_member_expr()
        => ExecutionTest("middle-list-one-expr-member-expr");

    [Fact(DisplayName = "middle-list-one-expr-method")]
    public Task middle_list_one_expr_method()
        => ExecutionTest("middle-list-one-expr-method");

    [Fact(DisplayName = "middle-list-one-expr-obj")]
    public Task middle_list_one_expr_obj()
        => ExecutionTest("middle-list-one-expr-obj");

    [Fact(DisplayName = "middle-list-one-expr-primitive")]
    public Task middle_list_one_expr_primitive()
        => ExecutionTest("middle-list-one-expr-primitive");

    [Fact(DisplayName = "middle-list-one-expr-template")]
    public Task middle_list_one_expr_template()
        => ExecutionTest("middle-list-one-expr-template");

    [Fact(DisplayName = "middle-list-one-expr-tostr-error")]
    public Task middle_list_one_expr_tostr_error()
        => ExecutionTest("middle-list-one-expr-tostr-error");

    [Fact(DisplayName = "mongolian-vowel-separator")]
    public Task mongolian_vowel_separator()
        => ExecutionTest("mongolian-vowel-separator");

    [Fact(DisplayName = "no-sub")]
    public Task no_sub()
        => ExecutionTest("no-sub");

    [Fact(DisplayName = "tv-character-escape-sequence")]
    public Task tv_character_escape_sequence()
        => ExecutionTest("tv-character-escape-sequence");

    [Fact(DisplayName = "tv-hex-escape-sequence")]
    public Task tv_hex_escape_sequence()
        => ExecutionTest("tv-hex-escape-sequence");

    [Fact(DisplayName = "tv-line-continuation")]
    public Task tv_line_continuation()
        => ExecutionTest("tv-line-continuation");

    [Fact(DisplayName = "tv-line-terminator-sequence")]
    public Task tv_line_terminator_sequence()
        => ExecutionTest("tv-line-terminator-sequence");

    [Fact(DisplayName = "tv-no-substitution")]
    public Task tv_no_substitution()
        => ExecutionTest("tv-no-substitution");

    [Fact(DisplayName = "tv-null-character-escape-sequence")]
    public Task tv_null_character_escape_sequence()
        => ExecutionTest("tv-null-character-escape-sequence");

    [Fact(DisplayName = "tv-template-character")]
    public Task tv_template_character()
        => ExecutionTest("tv-template-character");

    [Fact(DisplayName = "tv-template-characters")]
    public Task tv_template_characters()
        => ExecutionTest("tv-template-characters");

    [Fact(DisplayName = "tv-template-head")]
    public Task tv_template_head()
        => ExecutionTest("tv-template-head");

    [Fact(DisplayName = "tv-template-middle")]
    public Task tv_template_middle()
        => ExecutionTest("tv-template-middle");

    [Fact(DisplayName = "tv-template-tail")]
    public Task tv_template_tail()
        => ExecutionTest("tv-template-tail");

    [Fact(DisplayName = "tv-utf16-escape-sequence")]
    public Task tv_utf16_escape_sequence()
        => ExecutionTest("tv-utf16-escape-sequence");

    [Fact(DisplayName = "tv-zwnbsp")]
    public Task tv_zwnbsp()
        => ExecutionTest("tv-zwnbsp");

}
