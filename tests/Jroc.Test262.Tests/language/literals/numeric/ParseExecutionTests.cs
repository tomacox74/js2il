using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.literals.numeric;

public class ParseExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\literals\numeric", "language.literals.numeric") { }

    [Fact(DisplayName = "octal-invalid-truncated")]
    public Task octal_invalid_truncated()
        => CompilationFailureTest("octal-invalid-truncated");

    [Fact(DisplayName = "binary-invalid-digit")]
    public Task binary_invalid_digit()
        => CompilationFailureTest("binary-invalid-digit");

    [Fact(DisplayName = "binary-invalid-leading")]
    public Task binary_invalid_leading()
        => CompilationFailureTest("binary-invalid-leading");

    [Fact(DisplayName = "binary-invalid-truncated")]
    public Task binary_invalid_truncated()
        => CompilationFailureTest("binary-invalid-truncated");

    [Fact(DisplayName = "binary-invalid-unicode")]
    public Task binary_invalid_unicode()
        => CompilationFailureTest("binary-invalid-unicode");

    [Fact(DisplayName = "numeric-separators/numeric-separator-literal-dd-nsl-err")]
    public Task numeric_separators_numeric_separator_literal_dd_nsl_err()
        => CompilationFailureTest("numeric-separators/numeric-separator-literal-dd-nsl-err");

    [Fact(DisplayName = "numeric-separators/numeric-separator-literal-dds-nsl-err")]
    public Task numeric_separators_numeric_separator_literal_dds_nsl_err()
        => CompilationFailureTest("numeric-separators/numeric-separator-literal-dds-nsl-err");

    [Fact(DisplayName = "numeric-separators/numeric-separator-literal-dot-nsl-err")]
    public Task numeric_separators_numeric_separator_literal_dot_nsl_err()
        => CompilationFailureTest("numeric-separators/numeric-separator-literal-dot-nsl-err");

    [Fact(DisplayName = "numeric-separators/numeric-separator-literal-dot-nsl-ep-err")]
    public Task numeric_separators_numeric_separator_literal_dot_nsl_ep_err()
        => CompilationFailureTest("numeric-separators/numeric-separator-literal-dot-nsl-ep-err");

    [Fact(DisplayName = "numeric-separators/numeric-separator-literal-hil-nsl-hd-err")]
    public Task numeric_separators_numeric_separator_literal_hil_nsl_hd_err()
        => CompilationFailureTest("numeric-separators/numeric-separator-literal-hil-nsl-hd-err");

    [Fact(DisplayName = "numeric-separators/numeric-separator-literal-oil-nsl-od-err")]
    public Task numeric_separators_numeric_separator_literal_oil_nsl_od_err()
        => CompilationFailureTest("numeric-separators/numeric-separator-literal-oil-nsl-od-err");

    [Fact(DisplayName = "octal-invalid-digit")]
    public Task octal_invalid_digit()
        => CompilationFailureTest("octal-invalid-digit");

    [Fact(DisplayName = "octal-invalid-leading")]
    public Task octal_invalid_leading()
        => CompilationFailureTest("octal-invalid-leading");

    [Fact(DisplayName = "numeric-followed-by-ident")]
    public Task numeric_followed_by_ident()
        => CompilationFailureTest("numeric-followed-by-ident");

}
