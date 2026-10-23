namespace Jroc.Test262.Tests.built_ins.RegExp;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp") { }

    [Fact(DisplayName = "S15.10.2.11_A1_T5.js")]
    public Task S15_10_2_11_A1_T5()
        => ExecutionTest("S15.10.2.11_A1_T5");

    [Fact(DisplayName = "S15.10.2.11_A1_T7.js")]
    public Task S15_10_2_11_A1_T7()
        => ExecutionTest("S15.10.2.11_A1_T7");

    [Fact(DisplayName = "S15.10.2.8_A2_T1.js")]
    public Task S15_10_2_8_A2_T1()
        => ExecutionTest("S15.10.2.8_A2_T1");

    [Fact(DisplayName = "S15.10.4.1_A5_T3.js", Skip = "eval is not supported.")]
    public Task S15_10_4_1_A5_T3()
        => ExecutionTest("S15.10.4.1_A5_T3");

    [Fact(DisplayName = "S15.10.4.1_A5_T6.js")]
    public Task S15_10_4_1_A5_T6()
        => ExecutionTest("S15.10.4.1_A5_T6");

    [Fact(DisplayName = "S15.10.4.1_A8_T11.js", Skip = "eval is not supported.")]
    public Task S15_10_4_1_A8_T11()
        => ExecutionTest("S15.10.4.1_A8_T11");

    [Fact(DisplayName = "S15.10.5_A1.js")]
    public Task S15_10_5_A1()
        => ExecutionTest("S15.10.5_A1");

    [Fact(DisplayName = "call_with_regexp_match_falsy.js")]
    public Task call_with_regexp_match_falsy()
        => ExecutionTest("call_with_regexp_match_falsy");

    [Fact(DisplayName = "character-class-escape-non-whitespace.js")]
    public Task character_class_escape_non_whitespace()
        => ExecutionTest("character-class-escape-non-whitespace");

    [Fact(DisplayName = "duplicate-named-capturing-groups-syntax.js")]
    public Task duplicate_named_capturing_groups_syntax()
        => ExecutionTest("duplicate-named-capturing-groups-syntax");

    [Fact(DisplayName = "early-err-modifiers-code-point-repeat-i-1.js")]
    public Task early_err_modifiers_code_point_repeat_i_1()
        => ExecutionTest("early-err-modifiers-code-point-repeat-i-1");

    [Fact(DisplayName = "early-err-modifiers-code-point-repeat-i-2.js")]
    public Task early_err_modifiers_code_point_repeat_i_2()
        => ExecutionTest("early-err-modifiers-code-point-repeat-i-2");

    [Fact(DisplayName = "early-err-modifiers-other-code-point-uppercase-I.js")]
    public Task early_err_modifiers_other_code_point_uppercase_I()
        => ExecutionTest("early-err-modifiers-other-code-point-uppercase-I");

    [Fact(DisplayName = "early-err-modifiers-should-not-case-fold-i.js")]
    public Task early_err_modifiers_should_not_case_fold_i()
        => ExecutionTest("early-err-modifiers-should-not-case-fold-i");

    [Fact(DisplayName = "early-err-modifiers-should-not-case-fold-m.js")]
    public Task early_err_modifiers_should_not_case_fold_m()
        => ExecutionTest("early-err-modifiers-should-not-case-fold-m");

    [Fact(DisplayName = "early-err-modifiers-should-not-case-fold-s.js")]
    public Task early_err_modifiers_should_not_case_fold_s()
        => ExecutionTest("early-err-modifiers-should-not-case-fold-s");

    [Fact(DisplayName = "from-regexp-like-flag-override.js")]
    public Task from_regexp_like_flag_override()
        => ExecutionTest("from-regexp-like-flag-override");

    [Fact(DisplayName = "from-regexp-like-get-flags-err.js")]
    public Task from_regexp_like_get_flags_err()
        => ExecutionTest("from-regexp-like-get-flags-err");

    [Fact(DisplayName = "from-regexp-like-get-source-err.js")]
    public Task from_regexp_like_get_source_err()
        => ExecutionTest("from-regexp-like-get-source-err");

    [Fact(DisplayName = "from-regexp-like.js")]
    public Task from_regexp_like()
        => ExecutionTest("from-regexp-like");

    [Fact(DisplayName = "lookahead-quantifier-match-groups.js")]
    public Task lookahead_quantifier_match_groups()
        => ExecutionTest("lookahead-quantifier-match-groups");

    [Fact(DisplayName = "nullable-quantifier.js")]
    public Task nullable_quantifier()
        => ExecutionTest("nullable-quantifier");

    [Fact(DisplayName = "quantifier-integer-limit.js")]
    public Task quantifier_integer_limit()
        => ExecutionTest("quantifier-integer-limit");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-add-remove-i.js")]
    public Task syntax_err_arithmetic_modifiers_add_remove_i()
        => ExecutionTest("syntax-err-arithmetic-modifiers-add-remove-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-add-remove-m.js")]
    public Task syntax_err_arithmetic_modifiers_add_remove_m()
        => ExecutionTest("syntax-err-arithmetic-modifiers-add-remove-m");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-add-remove-multi-duplicate.js")]
    public Task syntax_err_arithmetic_modifiers_add_remove_multi_duplicate()
        => ExecutionTest("syntax-err-arithmetic-modifiers-add-remove-multi-duplicate");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-add-remove-s-escape.js")]
    public Task syntax_err_arithmetic_modifiers_add_remove_s_escape()
        => ExecutionTest("syntax-err-arithmetic-modifiers-add-remove-s-escape");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-add-remove-s.js")]
    public Task syntax_err_arithmetic_modifiers_add_remove_s()
        => ExecutionTest("syntax-err-arithmetic-modifiers-add-remove-s");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-both-empty.js")]
    public Task syntax_err_arithmetic_modifiers_both_empty()
        => ExecutionTest("syntax-err-arithmetic-modifiers-both-empty");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-code-point-repeat-i-1.js")]
    public Task syntax_err_arithmetic_modifiers_code_point_repeat_i_1()
        => ExecutionTest("syntax-err-arithmetic-modifiers-code-point-repeat-i-1");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-code-point-repeat-i-2.js")]
    public Task syntax_err_arithmetic_modifiers_code_point_repeat_i_2()
        => ExecutionTest("syntax-err-arithmetic-modifiers-code-point-repeat-i-2");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-uppercase-I.js")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_uppercase_I()
        => ExecutionTest("syntax-err-arithmetic-modifiers-other-code-point-uppercase-I");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-add-remove-i.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_add_remove_i()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-add-remove-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-add-remove-m.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_add_remove_m()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-add-remove-m");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-add-remove-multi-duplicate.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_add_remove_multi_duplicate()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-add-remove-multi-duplicate");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-add-remove-s-escape.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_add_remove_s_escape()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-add-remove-s-escape");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-add-remove-s.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_add_remove_s()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-add-remove-s");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-code-point-repeat-i-1.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_code_point_repeat_i_1()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-code-point-repeat-i-1");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-code-point-repeat-i-2.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_code_point_repeat_i_2()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-code-point-repeat-i-2");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-other-code-point-uppercase-I.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_other_code_point_uppercase_I()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-other-code-point-uppercase-I");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-should-not-case-fold-i.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_should_not_case_fold_i()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-should-not-case-fold-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-should-not-case-fold-m.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_should_not_case_fold_m()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-should-not-case-fold-m");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-reverse-should-not-case-fold-s.js")]
    public Task syntax_err_arithmetic_modifiers_reverse_should_not_case_fold_s()
        => ExecutionTest("syntax-err-arithmetic-modifiers-reverse-should-not-case-fold-s");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-should-not-case-fold-i.js")]
    public Task syntax_err_arithmetic_modifiers_should_not_case_fold_i()
        => ExecutionTest("syntax-err-arithmetic-modifiers-should-not-case-fold-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-should-not-case-fold-m.js")]
    public Task syntax_err_arithmetic_modifiers_should_not_case_fold_m()
        => ExecutionTest("syntax-err-arithmetic-modifiers-should-not-case-fold-m");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-should-not-case-fold-s.js")]
    public Task syntax_err_arithmetic_modifiers_should_not_case_fold_s()
        => ExecutionTest("syntax-err-arithmetic-modifiers-should-not-case-fold-s");

    [Fact(DisplayName = "unicode_full_case_folding.js")]
    public Task unicode_full_case_folding()
        => ExecutionTest("unicode_full_case_folding");

    [Fact(DisplayName = "unicode_restricted_brackets.js")]
    public Task unicode_restricted_brackets()
        => ExecutionTest("unicode_restricted_brackets");

    [Fact(DisplayName = "unicode_restricted_character_class_escape.js")]
    public Task unicode_restricted_character_class_escape()
        => ExecutionTest("unicode_restricted_character_class_escape");

    [Fact(DisplayName = "unicode_restricted_identity_escape.js")]
    public Task unicode_restricted_identity_escape()
        => ExecutionTest("unicode_restricted_identity_escape");

    [Fact(DisplayName = "unicode_restricted_identity_escape_alpha.js")]
    public Task unicode_restricted_identity_escape_alpha()
        => ExecutionTest("unicode_restricted_identity_escape_alpha");

    [Fact(DisplayName = "unicode_restricted_identity_escape_c.js")]
    public Task unicode_restricted_identity_escape_c()
        => ExecutionTest("unicode_restricted_identity_escape_c");

    [Fact(DisplayName = "unicode_restricted_incomplete_quantifier.js")]
    public Task unicode_restricted_incomplete_quantifier()
        => ExecutionTest("unicode_restricted_incomplete_quantifier");

    [Fact(DisplayName = "unicode_restricted_octal_escape.js")]
    public Task unicode_restricted_octal_escape()
        => ExecutionTest("unicode_restricted_octal_escape");

    [Fact(DisplayName = "unicode_restricted_quantifiable_assertion.js")]
    public Task unicode_restricted_quantifiable_assertion()
        => ExecutionTest("unicode_restricted_quantifiable_assertion");
}
