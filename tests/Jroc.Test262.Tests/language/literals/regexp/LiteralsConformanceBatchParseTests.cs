using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.regexp;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/regexp", "language.literals.regexp") { }

    [Fact(DisplayName = "S7.8.5_A1.2_T1.js")]
    public Task S7_8_5_A1_2_T1()
        => CompilationFailureTest("S7.8.5_A1.2_T1");

    [Fact(DisplayName = "S7.8.5_A1.2_T2.js")]
    public Task S7_8_5_A1_2_T2()
        => CompilationFailureTest("S7.8.5_A1.2_T2");

    [Fact(DisplayName = "S7.8.5_A1.2_T3.js")]
    public Task S7_8_5_A1_2_T3()
        => CompilationFailureTest("S7.8.5_A1.2_T3");

    [Fact(DisplayName = "S7.8.5_A1.2_T4.js")]
    public Task S7_8_5_A1_2_T4()
        => CompilationFailureTest("S7.8.5_A1.2_T4");

    [Fact(DisplayName = "S7.8.5_A1.3_T1.js")]
    public Task S7_8_5_A1_3_T1()
        => CompilationFailureTest("S7.8.5_A1.3_T1");

    [Fact(DisplayName = "S7.8.5_A1.3_T3.js")]
    public Task S7_8_5_A1_3_T3()
        => CompilationFailureTest("S7.8.5_A1.3_T3");

    [Fact(DisplayName = "S7.8.5_A1.5_T1.js")]
    public Task S7_8_5_A1_5_T1()
        => CompilationFailureTest("S7.8.5_A1.5_T1");

    [Fact(DisplayName = "S7.8.5_A1.5_T3.js")]
    public Task S7_8_5_A1_5_T3()
        => CompilationFailureTest("S7.8.5_A1.5_T3");

    [Fact(DisplayName = "S7.8.5_A2.2_T1.js")]
    public Task S7_8_5_A2_2_T1()
        => CompilationFailureTest("S7.8.5_A2.2_T1");

    [Fact(DisplayName = "S7.8.5_A2.2_T2.js")]
    public Task S7_8_5_A2_2_T2()
        => CompilationFailureTest("S7.8.5_A2.2_T2");

    [Fact(DisplayName = "S7.8.5_A2.3_T1.js")]
    public Task S7_8_5_A2_3_T1()
        => CompilationFailureTest("S7.8.5_A2.3_T1");

    [Fact(DisplayName = "S7.8.5_A2.3_T3.js")]
    public Task S7_8_5_A2_3_T3()
        => CompilationFailureTest("S7.8.5_A2.3_T3");

    [Fact(DisplayName = "S7.8.5_A2.5_T1.js")]
    public Task S7_8_5_A2_5_T1()
        => CompilationFailureTest("S7.8.5_A2.5_T1");

    [Fact(DisplayName = "S7.8.5_A2.5_T3.js")]
    public Task S7_8_5_A2_5_T3()
        => CompilationFailureTest("S7.8.5_A2.5_T3");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-add-remove-i.js")]
    public Task early_err_arithmetic_modifiers_add_remove_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-add-remove-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-add-remove-m.js")]
    public Task early_err_arithmetic_modifiers_add_remove_m()
        => CompilationFailureTest("early-err-arithmetic-modifiers-add-remove-m");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-add-remove-multi-duplicate.js")]
    public Task early_err_arithmetic_modifiers_add_remove_multi_duplicate()
        => CompilationFailureTest("early-err-arithmetic-modifiers-add-remove-multi-duplicate");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-add-remove-s-escape.js")]
    public Task early_err_arithmetic_modifiers_add_remove_s_escape()
        => CompilationFailureTest("early-err-arithmetic-modifiers-add-remove-s-escape");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-add-remove-s.js")]
    public Task early_err_arithmetic_modifiers_add_remove_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-add-remove-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-both-empty.js")]
    public Task early_err_arithmetic_modifiers_both_empty()
        => CompilationFailureTest("early-err-arithmetic-modifiers-both-empty");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-code-point-repeat-i-1.js")]
    public Task early_err_arithmetic_modifiers_code_point_repeat_i_1()
        => CompilationFailureTest("early-err-arithmetic-modifiers-code-point-repeat-i-1");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-code-point-repeat-i-2.js")]
    public Task early_err_arithmetic_modifiers_code_point_repeat_i_2()
        => CompilationFailureTest("early-err-arithmetic-modifiers-code-point-repeat-i-2");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-no-colon-1.js")]
    public Task early_err_arithmetic_modifiers_no_colon_1()
        => CompilationFailureTest("early-err-arithmetic-modifiers-no-colon-1");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-no-colon-2.js")]
    public Task early_err_arithmetic_modifiers_no_colon_2()
        => CompilationFailureTest("early-err-arithmetic-modifiers-no-colon-2");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-no-colon-3.js")]
    public Task early_err_arithmetic_modifiers_no_colon_3()
        => CompilationFailureTest("early-err-arithmetic-modifiers-no-colon-3");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-arbitrary.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_arbitrary()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-arbitrary");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-combining-i.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_combining_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-combining-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-combining-m.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_combining_m()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-combining-m");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-combining-s.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_combining_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-combining-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-d.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_d()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-d");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-g.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_g()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-g");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-non-display-1.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_non_display_1()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-non-display-1");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-non-display-2.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_non_display_2()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-non-display-2");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-non-flag.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_non_flag()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-non-flag");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-u.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_u()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-u");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-uppercase-I.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_uppercase_I()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-uppercase-I");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-y.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_y()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-y");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-zwj.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_zwj()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-zwj");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-zwnbsp.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_zwnbsp()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-zwnbsp");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-other-code-point-zwnj.js")]
    public Task early_err_arithmetic_modifiers_other_code_point_zwnj()
        => CompilationFailureTest("early-err-arithmetic-modifiers-other-code-point-zwnj");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-add-remove-i.js")]
    public Task early_err_arithmetic_modifiers_reverse_add_remove_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-add-remove-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-add-remove-m.js")]
    public Task early_err_arithmetic_modifiers_reverse_add_remove_m()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-add-remove-m");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-add-remove-multi-duplicate.js")]
    public Task early_err_arithmetic_modifiers_reverse_add_remove_multi_duplicate()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-add-remove-multi-duplicate");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-add-remove-s-escape.js")]
    public Task early_err_arithmetic_modifiers_reverse_add_remove_s_escape()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-add-remove-s-escape");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-add-remove-s.js")]
    public Task early_err_arithmetic_modifiers_reverse_add_remove_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-add-remove-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-code-point-repeat-i-1.js")]
    public Task early_err_arithmetic_modifiers_reverse_code_point_repeat_i_1()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-code-point-repeat-i-1");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-code-point-repeat-i-2.js")]
    public Task early_err_arithmetic_modifiers_reverse_code_point_repeat_i_2()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-code-point-repeat-i-2");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-arbitrary.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_arbitrary()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-arbitrary");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-combining-i.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_combining_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-combining-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-combining-m.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_combining_m()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-combining-m");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-combining-s.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_combining_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-combining-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-d.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_d()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-d");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-g.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_g()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-g");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-non-display-1.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_non_display_1()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-non-display-1");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-non-display-2.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_non_display_2()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-non-display-2");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-non-flag.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_non_flag()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-non-flag");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-u.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_u()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-u");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-uppercase-I.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_uppercase_I()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-uppercase-I");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-y.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_y()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-y");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-zwj.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_zwj()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-zwj");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-zwnbsp.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_zwnbsp()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-zwnbsp");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-other-code-point-zwnj.js")]
    public Task early_err_arithmetic_modifiers_reverse_other_code_point_zwnj()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-other-code-point-zwnj");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-should-not-case-fold-i.js")]
    public Task early_err_arithmetic_modifiers_reverse_should_not_case_fold_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-should-not-case-fold-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-should-not-case-fold-m.js")]
    public Task early_err_arithmetic_modifiers_reverse_should_not_case_fold_m()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-should-not-case-fold-m");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-should-not-case-fold-s.js")]
    public Task early_err_arithmetic_modifiers_reverse_should_not_case_fold_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-should-not-case-fold-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-i.js")]
    public Task early_err_arithmetic_modifiers_reverse_should_not_unicode_case_fold_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-s.js")]
    public Task early_err_arithmetic_modifiers_reverse_should_not_unicode_case_fold_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-reverse-should-not-unicode-case-fold-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-should-not-case-fold-i.js")]
    public Task early_err_arithmetic_modifiers_should_not_case_fold_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-should-not-case-fold-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-should-not-case-fold-m.js")]
    public Task early_err_arithmetic_modifiers_should_not_case_fold_m()
        => CompilationFailureTest("early-err-arithmetic-modifiers-should-not-case-fold-m");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-should-not-case-fold-s.js")]
    public Task early_err_arithmetic_modifiers_should_not_case_fold_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-should-not-case-fold-s");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-should-not-unicode-case-fold-i.js")]
    public Task early_err_arithmetic_modifiers_should_not_unicode_case_fold_i()
        => CompilationFailureTest("early-err-arithmetic-modifiers-should-not-unicode-case-fold-i");

    [Fact(DisplayName = "early-err-arithmetic-modifiers-should-not-unicode-case-fold-s.js")]
    public Task early_err_arithmetic_modifiers_should_not_unicode_case_fold_s()
        => CompilationFailureTest("early-err-arithmetic-modifiers-should-not-unicode-case-fold-s");

    [Fact(DisplayName = "early-err-bad-flag.js")]
    public Task early_err_bad_flag()
        => CompilationFailureTest("early-err-bad-flag");

    [Fact(DisplayName = "early-err-dup-flag.js")]
    public Task early_err_dup_flag()
        => CompilationFailureTest("early-err-dup-flag");

    [Fact(DisplayName = "early-err-flags-unicode-escape.js")]
    public Task early_err_flags_unicode_escape()
        => CompilationFailureTest("early-err-flags-unicode-escape");

    [Fact(DisplayName = "early-err-modifiers-code-point-repeat-i-1.js")]
    public Task early_err_modifiers_code_point_repeat_i_1()
        => CompilationFailureTest("early-err-modifiers-code-point-repeat-i-1");

    [Fact(DisplayName = "early-err-modifiers-code-point-repeat-i-2.js")]
    public Task early_err_modifiers_code_point_repeat_i_2()
        => CompilationFailureTest("early-err-modifiers-code-point-repeat-i-2");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-arbitrary.js")]
    public Task early_err_modifiers_other_code_point_arbitrary()
        => CompilationFailureTest("early-err-modifiers-other-code-point-arbitrary");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-combining-i.js")]
    public Task early_err_modifiers_other_code_point_combining_i()
        => CompilationFailureTest("early-err-modifiers-other-code-point-combining-i");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-combining-m.js")]
    public Task early_err_modifiers_other_code_point_combining_m()
        => CompilationFailureTest("early-err-modifiers-other-code-point-combining-m");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-combining-s.js")]
    public Task early_err_modifiers_other_code_point_combining_s()
        => CompilationFailureTest("early-err-modifiers-other-code-point-combining-s");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-d.js")]
    public Task early_err_modifiers_other_code_point_d()
        => CompilationFailureTest("early-err-modifiers-other-code-point-d");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-g.js")]
    public Task early_err_modifiers_other_code_point_g()
        => CompilationFailureTest("early-err-modifiers-other-code-point-g");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-non-display-1.js")]
    public Task early_err_modifiers_other_code_point_non_display_1()
        => CompilationFailureTest("early-err-modifiers-other-code-point-non-display-1");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-non-display-2.js")]
    public Task early_err_modifiers_other_code_point_non_display_2()
        => CompilationFailureTest("early-err-modifiers-other-code-point-non-display-2");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-non-flag.js")]
    public Task early_err_modifiers_other_code_point_non_flag()
        => CompilationFailureTest("early-err-modifiers-other-code-point-non-flag");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-u.js")]
    public Task early_err_modifiers_other_code_point_u()
        => CompilationFailureTest("early-err-modifiers-other-code-point-u");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-uppercase-I.js")]
    public Task early_err_modifiers_other_code_point_uppercase_I()
        => CompilationFailureTest("early-err-modifiers-other-code-point-uppercase-I");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-y.js")]
    public Task early_err_modifiers_other_code_point_y()
        => CompilationFailureTest("early-err-modifiers-other-code-point-y");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-zwj.js")]
    public Task early_err_modifiers_other_code_point_zwj()
        => CompilationFailureTest("early-err-modifiers-other-code-point-zwj");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-zwnbsp.js")]
    public Task early_err_modifiers_other_code_point_zwnbsp()
        => CompilationFailureTest("early-err-modifiers-other-code-point-zwnbsp");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-zwnj.js")]
    public Task early_err_modifiers_other_code_point_zwnj()
        => CompilationFailureTest("early-err-modifiers-other-code-point-zwnj");

    [Fact(DisplayName = "early-err-modifiers-should-not-case-fold-i.js")]
    public Task early_err_modifiers_should_not_case_fold_i()
        => CompilationFailureTest("early-err-modifiers-should-not-case-fold-i");

    [Fact(DisplayName = "early-err-modifiers-should-not-case-fold-m.js")]
    public Task early_err_modifiers_should_not_case_fold_m()
        => CompilationFailureTest("early-err-modifiers-should-not-case-fold-m");

    [Fact(DisplayName = "early-err-modifiers-should-not-case-fold-s.js")]
    public Task early_err_modifiers_should_not_case_fold_s()
        => CompilationFailureTest("early-err-modifiers-should-not-case-fold-s");

    [Fact(DisplayName = "early-err-modifiers-should-not-unicode-case-fold-i.js")]
    public Task early_err_modifiers_should_not_unicode_case_fold_i()
        => CompilationFailureTest("early-err-modifiers-should-not-unicode-case-fold-i");

    [Fact(DisplayName = "early-err-modifiers-should-not-unicode-case-fold-s.js")]
    public Task early_err_modifiers_should_not_unicode_case_fold_s()
        => CompilationFailureTest("early-err-modifiers-should-not-unicode-case-fold-s");

    [Fact(DisplayName = "early-err-modifiers-should-not-unicode-escape-i.js")]
    public Task early_err_modifiers_should_not_unicode_escape_i()
        => CompilationFailureTest("early-err-modifiers-should-not-unicode-escape-i");

    [Fact(DisplayName = "early-err-modifiers-should-not-unicode-escape-m.js")]
    public Task early_err_modifiers_should_not_unicode_escape_m()
        => CompilationFailureTest("early-err-modifiers-should-not-unicode-escape-m");

    [Fact(DisplayName = "early-err-modifiers-should-not-unicode-escape-s.js")]
    public Task early_err_modifiers_should_not_unicode_escape_s()
        => CompilationFailureTest("early-err-modifiers-should-not-unicode-escape-s");

    [Fact(DisplayName = "early-err-pattern.js")]
    public Task early_err_pattern()
        => CompilationFailureTest("early-err-pattern");

    [Fact(DisplayName = "invalid-braced-quantifier-exact.js")]
    public Task invalid_braced_quantifier_exact()
        => CompilationFailureTest("invalid-braced-quantifier-exact");

    [Fact(DisplayName = "invalid-braced-quantifier-lower.js")]
    public Task invalid_braced_quantifier_lower()
        => CompilationFailureTest("invalid-braced-quantifier-lower");

    [Fact(DisplayName = "invalid-braced-quantifier-range.js")]
    public Task invalid_braced_quantifier_range()
        => CompilationFailureTest("invalid-braced-quantifier-range");

    [Fact(DisplayName = "invalid-optional-lookbehind.js")]
    public Task invalid_optional_lookbehind()
        => CompilationFailureTest("invalid-optional-lookbehind");

    [Fact(DisplayName = "invalid-optional-negative-lookbehind.js")]
    public Task invalid_optional_negative_lookbehind()
        => CompilationFailureTest("invalid-optional-negative-lookbehind");

    [Fact(DisplayName = "invalid-range-lookbehind.js")]
    public Task invalid_range_lookbehind()
        => CompilationFailureTest("invalid-range-lookbehind");

    [Fact(DisplayName = "invalid-range-negative-lookbehind.js")]
    public Task invalid_range_negative_lookbehind()
        => CompilationFailureTest("invalid-range-negative-lookbehind");

    [Fact(DisplayName = "regexp-first-char-no-line-separator.js")]
    public Task regexp_first_char_no_line_separator()
        => CompilationFailureTest("regexp-first-char-no-line-separator");

    [Fact(DisplayName = "regexp-first-char-no-paragraph-separator.js")]
    public Task regexp_first_char_no_paragraph_separator()
        => CompilationFailureTest("regexp-first-char-no-paragraph-separator");

    [Fact(DisplayName = "regexp-source-char-no-line-separator.js")]
    public Task regexp_source_char_no_line_separator()
        => CompilationFailureTest("regexp-source-char-no-line-separator");

    [Fact(DisplayName = "regexp-source-char-no-paragraph-separator.js")]
    public Task regexp_source_char_no_paragraph_separator()
        => CompilationFailureTest("regexp-source-char-no-paragraph-separator");

    [Fact(DisplayName = "u-invalid-class-escape.js")]
    public Task u_invalid_class_escape()
        => CompilationFailureTest("u-invalid-class-escape");

    [Fact(DisplayName = "u-invalid-extended-pattern-char.js")]
    public Task u_invalid_extended_pattern_char()
        => CompilationFailureTest("u-invalid-extended-pattern-char");

    [Fact(DisplayName = "u-invalid-identity-escape.js")]
    public Task u_invalid_identity_escape()
        => CompilationFailureTest("u-invalid-identity-escape");

    [Fact(DisplayName = "u-invalid-legacy-octal-escape.js")]
    public Task u_invalid_legacy_octal_escape()
        => CompilationFailureTest("u-invalid-legacy-octal-escape");

    [Fact(DisplayName = "u-invalid-non-empty-class-ranges-no-dash-a.js")]
    public Task u_invalid_non_empty_class_ranges_no_dash_a()
        => CompilationFailureTest("u-invalid-non-empty-class-ranges-no-dash-a");

    [Fact(DisplayName = "u-invalid-non-empty-class-ranges-no-dash-ab.js")]
    public Task u_invalid_non_empty_class_ranges_no_dash_ab()
        => CompilationFailureTest("u-invalid-non-empty-class-ranges-no-dash-ab");

    [Fact(DisplayName = "u-invalid-non-empty-class-ranges-no-dash-b.js")]
    public Task u_invalid_non_empty_class_ranges_no_dash_b()
        => CompilationFailureTest("u-invalid-non-empty-class-ranges-no-dash-b");

    [Fact(DisplayName = "u-invalid-non-empty-class-ranges.js")]
    public Task u_invalid_non_empty_class_ranges()
        => CompilationFailureTest("u-invalid-non-empty-class-ranges");

    [Fact(DisplayName = "u-invalid-oob-decimal-escape.js")]
    public Task u_invalid_oob_decimal_escape()
        => CompilationFailureTest("u-invalid-oob-decimal-escape");

    [Fact(DisplayName = "u-invalid-optional-lookahead.js")]
    public Task u_invalid_optional_lookahead()
        => CompilationFailureTest("u-invalid-optional-lookahead");

    [Fact(DisplayName = "u-invalid-optional-lookbehind.js")]
    public Task u_invalid_optional_lookbehind()
        => CompilationFailureTest("u-invalid-optional-lookbehind");

    [Fact(DisplayName = "u-invalid-optional-negative-lookahead.js")]
    public Task u_invalid_optional_negative_lookahead()
        => CompilationFailureTest("u-invalid-optional-negative-lookahead");

    [Fact(DisplayName = "u-invalid-optional-negative-lookbehind.js")]
    public Task u_invalid_optional_negative_lookbehind()
        => CompilationFailureTest("u-invalid-optional-negative-lookbehind");

    [Fact(DisplayName = "u-invalid-range-lookahead.js")]
    public Task u_invalid_range_lookahead()
        => CompilationFailureTest("u-invalid-range-lookahead");

    [Fact(DisplayName = "u-invalid-range-lookbehind.js")]
    public Task u_invalid_range_lookbehind()
        => CompilationFailureTest("u-invalid-range-lookbehind");

    [Fact(DisplayName = "u-invalid-range-negative-lookahead.js")]
    public Task u_invalid_range_negative_lookahead()
        => CompilationFailureTest("u-invalid-range-negative-lookahead");

    [Fact(DisplayName = "u-invalid-range-negative-lookbehind.js")]
    public Task u_invalid_range_negative_lookbehind()
        => CompilationFailureTest("u-invalid-range-negative-lookbehind");

    [Fact(DisplayName = "u-unicode-esc-bounds.js")]
    public Task u_unicode_esc_bounds()
        => CompilationFailureTest("u-unicode-esc-bounds");

    [Fact(DisplayName = "u-unicode-esc-non-hex.js")]
    public Task u_unicode_esc_non_hex()
        => CompilationFailureTest("u-unicode-esc-non-hex");

    [Fact(DisplayName = "unicode-escape-nls-err.js")]
    public Task unicode_escape_nls_err()
        => CompilationFailureTest("unicode-escape-nls-err");

}
