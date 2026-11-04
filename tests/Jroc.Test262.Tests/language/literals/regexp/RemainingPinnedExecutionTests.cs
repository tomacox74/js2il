namespace Jroc.Test262.Tests.language.literals.regexp;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.literals.regexp") { }

    [Fact(DisplayName = "7.8.5-1.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task _7_8_5_1()
        => ExecutionTest("7.8.5-1");

    [Fact(DisplayName = "S7.8.5_A1.1_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_1_T2()
        => ExecutionTest("S7.8.5_A1.1_T2");

    [Fact(DisplayName = "S7.8.5_A1.3_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_3_T2()
        => ExecutionTest("S7.8.5_A1.3_T2");

    [Fact(DisplayName = "S7.8.5_A1.3_T4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_3_T4()
        => ExecutionTest("S7.8.5_A1.3_T4");

    [Fact(DisplayName = "S7.8.5_A1.3_T5.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_3_T5()
        => ExecutionTest("S7.8.5_A1.3_T5");

    [Fact(DisplayName = "S7.8.5_A1.3_T6.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_3_T6()
        => ExecutionTest("S7.8.5_A1.3_T6");

    [Fact(DisplayName = "S7.8.5_A1.4_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_4_T2()
        => ExecutionTest("S7.8.5_A1.4_T2");

    [Fact(DisplayName = "S7.8.5_A1.5_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_5_T2()
        => ExecutionTest("S7.8.5_A1.5_T2");

    [Fact(DisplayName = "S7.8.5_A1.5_T4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_5_T4()
        => ExecutionTest("S7.8.5_A1.5_T4");

    [Fact(DisplayName = "S7.8.5_A1.5_T5.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_5_T5()
        => ExecutionTest("S7.8.5_A1.5_T5");

    [Fact(DisplayName = "S7.8.5_A1.5_T6.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A1_5_T6()
        => ExecutionTest("S7.8.5_A1.5_T6");

    [Fact(DisplayName = "S7.8.5_A2.1_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_1_T2()
        => ExecutionTest("S7.8.5_A2.1_T2");

    [Fact(DisplayName = "S7.8.5_A2.3_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_3_T2()
        => ExecutionTest("S7.8.5_A2.3_T2");

    [Fact(DisplayName = "S7.8.5_A2.3_T4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_3_T4()
        => ExecutionTest("S7.8.5_A2.3_T4");

    [Fact(DisplayName = "S7.8.5_A2.3_T5.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_3_T5()
        => ExecutionTest("S7.8.5_A2.3_T5");

    [Fact(DisplayName = "S7.8.5_A2.3_T6.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_3_T6()
        => ExecutionTest("S7.8.5_A2.3_T6");

    [Fact(DisplayName = "S7.8.5_A2.4_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_4_T2()
        => ExecutionTest("S7.8.5_A2.4_T2");

    [Fact(DisplayName = "S7.8.5_A2.5_T2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_5_T2()
        => ExecutionTest("S7.8.5_A2.5_T2");

    [Fact(DisplayName = "S7.8.5_A2.5_T4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_5_T4()
        => ExecutionTest("S7.8.5_A2.5_T4");

    [Fact(DisplayName = "S7.8.5_A2.5_T5.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_5_T5()
        => ExecutionTest("S7.8.5_A2.5_T5");

    [Fact(DisplayName = "S7.8.5_A2.5_T6.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S7_8_5_A2_5_T6()
        => ExecutionTest("S7.8.5_A2.5_T6");

    [Fact(DisplayName = "mongolian-vowel-separator-eval.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task mongolian_vowel_separator_eval()
        => ExecutionTest("mongolian-vowel-separator-eval");

    [Fact(DisplayName = "u-astral.js")]
    public Task u_astral()
        => ExecutionTest("u-astral");

    [Fact(DisplayName = "u-case-mapping.js")]
    public Task u_case_mapping()
        => ExecutionTest("u-case-mapping");

    [Fact(DisplayName = "u-surrogate-pairs-atom-char-class.js")]
    public Task u_surrogate_pairs_atom_char_class()
        => ExecutionTest("u-surrogate-pairs-atom-char-class");

    [Fact(DisplayName = "u-surrogate-pairs-atom-escape-char-class.js")]
    public Task u_surrogate_pairs_atom_escape_char_class()
        => ExecutionTest("u-surrogate-pairs-atom-escape-char-class");

    [Fact(DisplayName = "u-surrogate-pairs-atom-escape-decimal.js")]
    public Task u_surrogate_pairs_atom_escape_decimal()
        => ExecutionTest("u-surrogate-pairs-atom-escape-decimal");

    [Fact(DisplayName = "u-surrogate-pairs.js")]
    public Task u_surrogate_pairs()
        => ExecutionTest("u-surrogate-pairs");

    [Fact(DisplayName = "u-unicode-esc.js")]
    public Task u_unicode_esc()
        => ExecutionTest("u-unicode-esc");

}
