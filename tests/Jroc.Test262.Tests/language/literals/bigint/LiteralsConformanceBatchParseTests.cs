using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.bigint;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/bigint", "language.literals.bigint") { }

    [Fact(DisplayName = "binary-invalid-digit.js")]
    public Task binary_invalid_digit()
        => CompilationFailureTest("binary-invalid-digit");

    [Fact(DisplayName = "exponent-part.js")]
    public Task exponent_part()
        => CompilationFailureTest("exponent-part");

    [Fact(DisplayName = "hexadecimal-invalid-digit.js")]
    public Task hexadecimal_invalid_digit()
        => CompilationFailureTest("hexadecimal-invalid-digit");

    [Fact(DisplayName = "legacy-octal-like-invalid-00n.js")]
    public Task legacy_octal_like_invalid_00n()
        => CompilationFailureTest("legacy-octal-like-invalid-00n");

    [Fact(DisplayName = "legacy-octal-like-invalid-01n.js")]
    public Task legacy_octal_like_invalid_01n()
        => CompilationFailureTest("legacy-octal-like-invalid-01n");

    [Fact(DisplayName = "legacy-octal-like-invalid-07n.js")]
    public Task legacy_octal_like_invalid_07n()
        => CompilationFailureTest("legacy-octal-like-invalid-07n");

    [Fact(DisplayName = "mv-is-not-integer-dil-dot-dds.js")]
    public Task mv_is_not_integer_dil_dot_dds()
        => CompilationFailureTest("mv-is-not-integer-dil-dot-dds");

    [Fact(DisplayName = "mv-is-not-integer-dot-dds.js")]
    public Task mv_is_not_integer_dot_dds()
        => CompilationFailureTest("mv-is-not-integer-dot-dds");

    [Fact(DisplayName = "non-octal-like-invalid-0008n.js")]
    public Task non_octal_like_invalid_0008n()
        => CompilationFailureTest("non-octal-like-invalid-0008n");

    [Fact(DisplayName = "non-octal-like-invalid-012348n.js")]
    public Task non_octal_like_invalid_012348n()
        => CompilationFailureTest("non-octal-like-invalid-012348n");

    [Fact(DisplayName = "non-octal-like-invalid-08n.js")]
    public Task non_octal_like_invalid_08n()
        => CompilationFailureTest("non-octal-like-invalid-08n");

    [Fact(DisplayName = "non-octal-like-invalid-09n.js")]
    public Task non_octal_like_invalid_09n()
        => CompilationFailureTest("non-octal-like-invalid-09n");

    [Fact(DisplayName = "octal-invalid-digit.js")]
    public Task octal_invalid_digit()
        => CompilationFailureTest("octal-invalid-digit");

}
