using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.@string;

public class LiteralsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LiteralsConformanceBatchExecutionTests() : base("language.literals.string") { }

    [Fact(DisplayName = "S7.8.4_A2.1_T1.js")]
    public Task S7_8_4_A2_1_T1()
        => ExecutionTest("S7.8.4_A2.1_T1");

    [Fact(DisplayName = "S7.8.4_A2.1_T2.js")]
    public Task S7_8_4_A2_1_T2()
        => ExecutionTest("S7.8.4_A2.1_T2");

    [Fact(DisplayName = "S7.8.4_A2.2_T1.js")]
    public Task S7_8_4_A2_2_T1()
        => ExecutionTest("S7.8.4_A2.2_T1");

    [Fact(DisplayName = "S7.8.4_A2.2_T2.js")]
    public Task S7_8_4_A2_2_T2()
        => ExecutionTest("S7.8.4_A2.2_T2");

    [Fact(DisplayName = "S7.8.4_A2.3_T1.js")]
    public Task S7_8_4_A2_3_T1()
        => ExecutionTest("S7.8.4_A2.3_T1");

    [Fact(DisplayName = "S7.8.4_A4.1_T1.js")]
    public Task S7_8_4_A4_1_T1()
        => ExecutionTest("S7.8.4_A4.1_T1");

    [Fact(DisplayName = "S7.8.4_A4.1_T2.js")]
    public Task S7_8_4_A4_1_T2()
        => ExecutionTest("S7.8.4_A4.1_T2");

    [Fact(DisplayName = "S7.8.4_A4.2_T1.js")]
    public Task S7_8_4_A4_2_T1()
        => ExecutionTest("S7.8.4_A4.2_T1");

    [Fact(DisplayName = "S7.8.4_A4.2_T2.js")]
    public Task S7_8_4_A4_2_T2()
        => ExecutionTest("S7.8.4_A4.2_T2");

    [Fact(DisplayName = "S7.8.4_A4.2_T3.js")]
    public Task S7_8_4_A4_2_T3()
        => ExecutionTest("S7.8.4_A4.2_T3");

    [Fact(DisplayName = "S7.8.4_A4.2_T4.js")]
    public Task S7_8_4_A4_2_T4()
        => ExecutionTest("S7.8.4_A4.2_T4");

    [Fact(DisplayName = "S7.8.4_A4.2_T5.js")]
    public Task S7_8_4_A4_2_T5()
        => ExecutionTest("S7.8.4_A4.2_T5");

    [Fact(DisplayName = "S7.8.4_A4.2_T6.js")]
    public Task S7_8_4_A4_2_T6()
        => ExecutionTest("S7.8.4_A4.2_T6");

    [Fact(DisplayName = "S7.8.4_A4.2_T7.js")]
    public Task S7_8_4_A4_2_T7()
        => ExecutionTest("S7.8.4_A4.2_T7");

    [Fact(DisplayName = "S7.8.4_A4.2_T8.js")]
    public Task S7_8_4_A4_2_T8()
        => ExecutionTest("S7.8.4_A4.2_T8");

    [Fact(DisplayName = "S7.8.4_A4.3_T7.js")]
    public Task S7_8_4_A4_3_T7()
        => ExecutionTest("S7.8.4_A4.3_T7");

    [Fact(DisplayName = "S7.8.4_A5.1_T1.js")]
    public Task S7_8_4_A5_1_T1()
        => ExecutionTest("S7.8.4_A5.1_T1");

    [Fact(DisplayName = "S7.8.4_A5.1_T2.js")]
    public Task S7_8_4_A5_1_T2()
        => ExecutionTest("S7.8.4_A5.1_T2");

    [Fact(DisplayName = "S7.8.4_A5.1_T3.js")]
    public Task S7_8_4_A5_1_T3()
        => ExecutionTest("S7.8.4_A5.1_T3");

    [Fact(DisplayName = "S7.8.4_A6.1_T1.js")]
    public Task S7_8_4_A6_1_T1()
        => ExecutionTest("S7.8.4_A6.1_T1");

    [Fact(DisplayName = "S7.8.4_A6.1_T2.js")]
    public Task S7_8_4_A6_1_T2()
        => ExecutionTest("S7.8.4_A6.1_T2");

    [Fact(DisplayName = "S7.8.4_A6.1_T3.js")]
    public Task S7_8_4_A6_1_T3()
        => ExecutionTest("S7.8.4_A6.1_T3");

    [Fact(DisplayName = "S7.8.4_A6.3_T1.js")]
    public Task S7_8_4_A6_3_T1()
        => ExecutionTest("S7.8.4_A6.3_T1");

    [Fact(DisplayName = "S7.8.4_A7.1_T1.js")]
    public Task S7_8_4_A7_1_T1()
        => ExecutionTest("S7.8.4_A7.1_T1");

    [Fact(DisplayName = "S7.8.4_A7.1_T2.js")]
    public Task S7_8_4_A7_1_T2()
        => ExecutionTest("S7.8.4_A7.1_T2");

    [Fact(DisplayName = "S7.8.4_A7.1_T3.js")]
    public Task S7_8_4_A7_1_T3()
        => ExecutionTest("S7.8.4_A7.1_T3");

    [Fact(DisplayName = "S7.8.4_A7.3_T1.js")]
    public Task S7_8_4_A7_3_T1()
        => ExecutionTest("S7.8.4_A7.3_T1");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-8-non-strict.js")]
    public Task legacy_non_octal_escape_sequence_8_non_strict()
        => ExecutionTest("legacy-non-octal-escape-sequence-8-non-strict");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-9-non-strict.js")]
    public Task legacy_non_octal_escape_sequence_9_non_strict()
        => ExecutionTest("legacy-non-octal-escape-sequence-9-non-strict");

    [Fact(DisplayName = "line-continuation-double.js")]
    public Task line_continuation_double()
        => ExecutionTest("line-continuation-double");

    [Fact(DisplayName = "line-continuation-single.js")]
    public Task line_continuation_single()
        => ExecutionTest("line-continuation-single");

    [Fact(DisplayName = "line-separator.js")]
    public Task line_separator()
        => ExecutionTest("line-separator");

    [Fact(DisplayName = "mongolian-vowel-separator.js")]
    public Task mongolian_vowel_separator()
        => ExecutionTest("mongolian-vowel-separator");

    [Fact(DisplayName = "paragraph-separator.js")]
    public Task paragraph_separator()
        => ExecutionTest("paragraph-separator");

}
