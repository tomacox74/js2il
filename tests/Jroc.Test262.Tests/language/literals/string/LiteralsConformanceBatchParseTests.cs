using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.@string;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/string", "language.literals.string") { }

    [Fact(DisplayName = "S7.8.4_A1.1_T2.js")]
    public Task S7_8_4_A1_1_T2()
        => CompilationFailureTest("S7.8.4_A1.1_T2");

    [Fact(DisplayName = "S7.8.4_A1.2_T2.js")]
    public Task S7_8_4_A1_2_T2()
        => CompilationFailureTest("S7.8.4_A1.2_T2");

    [Fact(DisplayName = "S7.8.4_A3.1_T1.js")]
    public Task S7_8_4_A3_1_T1()
        => CompilationFailureTest("S7.8.4_A3.1_T1");

    [Fact(DisplayName = "S7.8.4_A3.1_T2.js")]
    public Task S7_8_4_A3_1_T2()
        => CompilationFailureTest("S7.8.4_A3.1_T2");

    [Fact(DisplayName = "S7.8.4_A3.2_T1.js")]
    public Task S7_8_4_A3_2_T1()
        => CompilationFailureTest("S7.8.4_A3.2_T1");

    [Fact(DisplayName = "S7.8.4_A3.2_T2.js")]
    public Task S7_8_4_A3_2_T2()
        => CompilationFailureTest("S7.8.4_A3.2_T2");

    [Fact(DisplayName = "S7.8.4_A4.3_T1.js")]
    public Task S7_8_4_A4_3_T1()
        => CompilationFailureTest("S7.8.4_A4.3_T1");

    [Fact(DisplayName = "S7.8.4_A4.3_T2.js")]
    public Task S7_8_4_A4_3_T2()
        => CompilationFailureTest("S7.8.4_A4.3_T2");

    [Fact(DisplayName = "S7.8.4_A7.1_T4.js")]
    public Task S7_8_4_A7_1_T4()
        => CompilationFailureTest("S7.8.4_A7.1_T4");

    [Fact(DisplayName = "S7.8.4_A7.2_T1.js")]
    public Task S7_8_4_A7_2_T1()
        => CompilationFailureTest("S7.8.4_A7.2_T1");

    [Fact(DisplayName = "S7.8.4_A7.2_T2.js")]
    public Task S7_8_4_A7_2_T2()
        => CompilationFailureTest("S7.8.4_A7.2_T2");

    [Fact(DisplayName = "S7.8.4_A7.2_T3.js")]
    public Task S7_8_4_A7_2_T3()
        => CompilationFailureTest("S7.8.4_A7.2_T3");

    [Fact(DisplayName = "S7.8.4_A7.2_T4.js")]
    public Task S7_8_4_A7_2_T4()
        => CompilationFailureTest("S7.8.4_A7.2_T4");

    [Fact(DisplayName = "S7.8.4_A7.2_T5.js")]
    public Task S7_8_4_A7_2_T5()
        => CompilationFailureTest("S7.8.4_A7.2_T5");

    [Fact(DisplayName = "S7.8.4_A7.2_T6.js")]
    public Task S7_8_4_A7_2_T6()
        => CompilationFailureTest("S7.8.4_A7.2_T6");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-1-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_1_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-1-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-2-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_2_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-2-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-3-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_3_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-3-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-4-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_4_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-4-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-5-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_5_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-5-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-6-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_6_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-6-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-7-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_7_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-7-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-8-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_8_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-8-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-8-strict.js")]
    public Task legacy_non_octal_escape_sequence_8_strict()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-8-strict");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-9-strict-explicit-pragma.js")]
    public Task legacy_non_octal_escape_sequence_9_strict_explicit_pragma()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-9-strict-explicit-pragma");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-9-strict.js")]
    public Task legacy_non_octal_escape_sequence_9_strict()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-9-strict");

    [Fact(DisplayName = "legacy-non-octal-escape-sequence-strict.js")]
    public Task legacy_non_octal_escape_sequence_strict()
        => CompilationFailureTest("legacy-non-octal-escape-sequence-strict");

    [Fact(DisplayName = "legacy-octal-escape-sequence-prologue-strict.js")]
    public Task legacy_octal_escape_sequence_prologue_strict()
        => CompilationFailureTest("legacy-octal-escape-sequence-prologue-strict");

    [Fact(DisplayName = "legacy-octal-escape-sequence-strict.js")]
    public Task legacy_octal_escape_sequence_strict()
        => CompilationFailureTest("legacy-octal-escape-sequence-strict");

    [Fact(DisplayName = "unicode-escape-nls-err-double.js")]
    public Task unicode_escape_nls_err_double()
        => CompilationFailureTest("unicode-escape-nls-err-double");

    [Fact(DisplayName = "unicode-escape-nls-err-single.js")]
    public Task unicode_escape_nls_err_single()
        => CompilationFailureTest("unicode-escape-nls-err-single");

    [Fact(DisplayName = "unicode-escape-no-hex-err-double.js")]
    public Task unicode_escape_no_hex_err_double()
        => CompilationFailureTest("unicode-escape-no-hex-err-double");

    [Fact(DisplayName = "unicode-escape-no-hex-err-single.js")]
    public Task unicode_escape_no_hex_err_single()
        => CompilationFailureTest("unicode-escape-no-hex-err-single");

}
