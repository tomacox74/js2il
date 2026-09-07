using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.regexp;

public class LiteralsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LiteralsConformanceBatchExecutionTests() : base("language.literals.regexp") { }

    [Fact(DisplayName = "7.8.5-1gs.js")]
    public Task case_7_8_5_1gs()
        => ExecutionTest("7.8.5-1gs");

    [Fact(DisplayName = "7.8.5-2gs.js")]
    public Task case_7_8_5_2gs()
        => ExecutionTest("7.8.5-2gs");

    [Fact(DisplayName = "S7.8.5_A1.1_T1.js")]
    public Task S7_8_5_A1_1_T1()
        => ExecutionTest("S7.8.5_A1.1_T1");

    [Fact(DisplayName = "S7.8.5_A1.4_T1.js")]
    public Task S7_8_5_A1_4_T1()
        => ExecutionTest("S7.8.5_A1.4_T1");

    [Fact(DisplayName = "S7.8.5_A2.1_T1.js")]
    public Task S7_8_5_A2_1_T1()
        => ExecutionTest("S7.8.5_A2.1_T1");

    [Fact(DisplayName = "S7.8.5_A2.4_T1.js")]
    public Task S7_8_5_A2_4_T1()
        => ExecutionTest("S7.8.5_A2.4_T1");

    [Fact(DisplayName = "S7.8.5_A3.1_T1.js")]
    public Task S7_8_5_A3_1_T1()
        => ExecutionTest("S7.8.5_A3.1_T1");

    [Fact(DisplayName = "S7.8.5_A3.1_T2.js")]
    public Task S7_8_5_A3_1_T2()
        => ExecutionTest("S7.8.5_A3.1_T2");

    [Fact(DisplayName = "S7.8.5_A3.1_T3.js")]
    public Task S7_8_5_A3_1_T3()
        => ExecutionTest("S7.8.5_A3.1_T3");

    [Fact(DisplayName = "S7.8.5_A3.1_T4.js")]
    public Task S7_8_5_A3_1_T4()
        => ExecutionTest("S7.8.5_A3.1_T4");

    [Fact(DisplayName = "S7.8.5_A3.1_T5.js")]
    public Task S7_8_5_A3_1_T5()
        => ExecutionTest("S7.8.5_A3.1_T5");

    [Fact(DisplayName = "S7.8.5_A3.1_T6.js")]
    public Task S7_8_5_A3_1_T6()
        => ExecutionTest("S7.8.5_A3.1_T6");

    [Fact(DisplayName = "S7.8.5_A4.1.js")]
    public Task S7_8_5_A4_1()
        => ExecutionTest("S7.8.5_A4.1");

    [Fact(DisplayName = "S7.8.5_A4.2.js")]
    public Task S7_8_5_A4_2()
        => ExecutionTest("S7.8.5_A4.2");

    [Fact(DisplayName = "inequality.js")]
    public Task inequality()
        => ExecutionTest("inequality");

    [Fact(DisplayName = "lastIndex.js")]
    public Task lastIndex()
        => ExecutionTest("lastIndex");

    [Fact(DisplayName = "mongolian-vowel-separator.js")]
    public Task mongolian_vowel_separator()
        => ExecutionTest("mongolian-vowel-separator");

    [Fact(DisplayName = "u-astral-char-class-invert.js")]
    public Task u_astral_char_class_invert()
        => ExecutionTest("u-astral-char-class-invert");

    [Fact(DisplayName = "u-null-character-escape.js")]
    public Task u_null_character_escape()
        => ExecutionTest("u-null-character-escape");

    [Fact(DisplayName = "u-surrogate-pairs-atom-dot.js")]
    public Task u_surrogate_pairs_atom_dot()
        => ExecutionTest("u-surrogate-pairs-atom-dot");

    [Fact(DisplayName = "y-assertion-start.js")]
    public Task y_assertion_start()
        => ExecutionTest("y-assertion-start");

}
