using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.RegExp") { }

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-g")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_g()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-g");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-non-display-1")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_non_display_1()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-non-display-1");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-non-display-2")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_non_display_2()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-non-display-2");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-non-flag")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_non_flag()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-non-flag");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-u")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_u()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-u");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-y")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_y()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-y");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-zwj")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_zwj()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-zwj");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-zwnbsp")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_zwnbsp()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-zwnbsp");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-zwnj")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_zwnj()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-zwnj");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-arbitrary")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_arbitrary()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-arbitrary");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-combining-i")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_combining_i()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-combining-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-combining-m")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_combining_m()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-combining-m");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-combining-s")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_combining_s()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-combining-s");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-d")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_d()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-d");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-g")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_g()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-g");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-non-display-1")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_non_display_1()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-non-display-1");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-non-display-2")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_non_display_2()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-non-display-2");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-non-flag")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_non_flag()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-non-flag");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-u")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_u()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-u");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-y")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_y()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-y");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-zwj")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_zwj()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-zwj");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-zwnbsp")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_zwnbsp()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-zwnbsp");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-zwnj")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_zwnj()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-other-code-point-zwnj");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-i")]
    public Task syntax_err_arithmetic_modifiers_reverse_should_not_unicode_case_fold_i()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-s")]
    public Task syntax_err_arithmetic_modifiers_reverse_should_not_unicode_case_fold_s()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-s");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-should-not-unicode-case-fold-i")]
    public Task syntax_err_arithmetic_modifiers_should_not_unicode_case_fold_i()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-should-not-unicode-case-fold-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-should-not-unicode-case-fold-s")]
    public Task syntax_err_arithmetic_modifiers_should_not_unicode_case_fold_s()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-should-not-unicode-case-fold-s");

}
