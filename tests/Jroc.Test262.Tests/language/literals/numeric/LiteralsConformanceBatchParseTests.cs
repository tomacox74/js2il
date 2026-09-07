using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.numeric;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/numeric", "language.literals.numeric") { }

    [Fact(DisplayName = "7.8.3-1gs.js")]
    public Task case_7_8_3_1gs()
        => CompilationFailureTest("7.8.3-1gs");

    [Fact(DisplayName = "7.8.3-2gs.js")]
    public Task case_7_8_3_2gs()
        => CompilationFailureTest("7.8.3-2gs");

    [Fact(DisplayName = "S7.8.3_A6.1_T1.js")]
    public Task S7_8_3_A6_1_T1()
        => CompilationFailureTest("S7.8.3_A6.1_T1");

    [Fact(DisplayName = "S7.8.3_A6.1_T2.js")]
    public Task S7_8_3_A6_1_T2()
        => CompilationFailureTest("S7.8.3_A6.1_T2");

    [Fact(DisplayName = "S7.8.3_A6.2_T1.js")]
    public Task S7_8_3_A6_2_T1()
        => CompilationFailureTest("S7.8.3_A6.2_T1");

    [Fact(DisplayName = "S7.8.3_A6.2_T2.js")]
    public Task S7_8_3_A6_2_T2()
        => CompilationFailureTest("S7.8.3_A6.2_T2");

    [Fact(DisplayName = "legacy-octal-integer-strict.js")]
    public Task legacy_octal_integer_strict()
        => CompilationFailureTest("legacy-octal-integer-strict");

    [Fact(DisplayName = "legacy-octal-integery-000-strict.js")]
    public Task legacy_octal_integery_000_strict()
        => CompilationFailureTest("legacy-octal-integery-000-strict");

    [Fact(DisplayName = "legacy-octal-integery-005-strict.js")]
    public Task legacy_octal_integery_005_strict()
        => CompilationFailureTest("legacy-octal-integery-005-strict");

    [Fact(DisplayName = "legacy-octal-integery-01-strict.js")]
    public Task legacy_octal_integery_01_strict()
        => CompilationFailureTest("legacy-octal-integery-01-strict");

    [Fact(DisplayName = "legacy-octal-integery-010-strict.js")]
    public Task legacy_octal_integery_010_strict()
        => CompilationFailureTest("legacy-octal-integery-010-strict");

    [Fact(DisplayName = "legacy-octal-integery-06-strict.js")]
    public Task legacy_octal_integery_06_strict()
        => CompilationFailureTest("legacy-octal-integery-06-strict");

    [Fact(DisplayName = "legacy-octal-integery-07-strict.js")]
    public Task legacy_octal_integery_07_strict()
        => CompilationFailureTest("legacy-octal-integery-07-strict");

    [Fact(DisplayName = "non-octal-decimal-integer-strict.js")]
    public Task non_octal_decimal_integer_strict()
        => CompilationFailureTest("non-octal-decimal-integer-strict");

    [Fact(DisplayName = "octal-invalid-unicode.js")]
    public Task octal_invalid_unicode()
        => CompilationFailureTest("octal-invalid-unicode");

}
