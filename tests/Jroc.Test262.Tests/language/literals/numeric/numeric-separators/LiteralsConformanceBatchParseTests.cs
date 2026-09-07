using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.numeric.numeric_separators;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/numeric/numeric-separators", "language.literals.numeric.numeric-separators") { }

    [Fact(DisplayName = "numeric-separator-literal-bil-bd-nsl-bd-err.js")]
    public Task numeric_separator_literal_bil_bd_nsl_bd_err()
        => CompilationFailureTest("numeric-separator-literal-bil-bd-nsl-bd-err");

    [Fact(DisplayName = "numeric-separator-literal-bil-nsl-bd-dunder-err.js")]
    public Task numeric_separator_literal_bil_nsl_bd_dunder_err()
        => CompilationFailureTest("numeric-separator-literal-bil-nsl-bd-dunder-err");

    [Fact(DisplayName = "numeric-separator-literal-bil-nsl-bd-err.js")]
    public Task numeric_separator_literal_bil_nsl_bd_err()
        => CompilationFailureTest("numeric-separator-literal-bil-nsl-bd-err");

    [Fact(DisplayName = "numeric-separator-literal-dd-nsl-dds-dunder-err.js")]
    public Task numeric_separator_literal_dd_nsl_dds_dunder_err()
        => CompilationFailureTest("numeric-separator-literal-dd-nsl-dds-dunder-err");

    [Fact(DisplayName = "numeric-separator-literal-dd-nsl-err.js")]
    public Task numeric_separator_literal_dd_nsl_err()
        => CompilationFailureTest("numeric-separator-literal-dd-nsl-err");

    [Fact(DisplayName = "numeric-separator-literal-dds-nsl-dds-dunder-err.js")]
    public Task numeric_separator_literal_dds_nsl_dds_dunder_err()
        => CompilationFailureTest("numeric-separator-literal-dds-nsl-dds-dunder-err");

    [Fact(DisplayName = "numeric-separator-literal-dds-nsl-err.js")]
    public Task numeric_separator_literal_dds_nsl_err()
        => CompilationFailureTest("numeric-separator-literal-dds-nsl-err");

    [Fact(DisplayName = "numeric-separator-literal-dil-dot-dds-nsl-ep-dd-err.js")]
    public Task numeric_separator_literal_dil_dot_dds_nsl_ep_dd_err()
        => CompilationFailureTest("numeric-separator-literal-dil-dot-dds-nsl-ep-dd-err");

    [Fact(DisplayName = "numeric-separator-literal-dil-dot-nsl-dd-err.js")]
    public Task numeric_separator_literal_dil_dot_nsl_dd_err()
        => CompilationFailureTest("numeric-separator-literal-dil-dot-nsl-dd-err");

    [Fact(DisplayName = "numeric-separator-literal-dil-dot-nsl-ep-err.js")]
    public Task numeric_separator_literal_dil_dot_nsl_ep_err()
        => CompilationFailureTest("numeric-separator-literal-dil-dot-nsl-ep-err");

    [Fact(DisplayName = "numeric-separator-literal-dil-dot-nsl-err.js")]
    public Task numeric_separator_literal_dil_dot_nsl_err()
        => CompilationFailureTest("numeric-separator-literal-dil-dot-nsl-err");

    [Fact(DisplayName = "numeric-separator-literal-dot-dds-nsl-ep-err.js")]
    public Task numeric_separator_literal_dot_dds_nsl_ep_err()
        => CompilationFailureTest("numeric-separator-literal-dot-dds-nsl-ep-err");

    [Fact(DisplayName = "numeric-separator-literal-dot-nsl-ep-err.js")]
    public Task numeric_separator_literal_dot_nsl_ep_err()
        => CompilationFailureTest("numeric-separator-literal-dot-nsl-ep-err");

    [Fact(DisplayName = "numeric-separator-literal-dot-nsl-err.js")]
    public Task numeric_separator_literal_dot_nsl_err()
        => CompilationFailureTest("numeric-separator-literal-dot-nsl-err");

    [Fact(DisplayName = "numeric-separator-literal-hil-hd-nsl-hd-err.js")]
    public Task numeric_separator_literal_hil_hd_nsl_hd_err()
        => CompilationFailureTest("numeric-separator-literal-hil-hd-nsl-hd-err");

    [Fact(DisplayName = "numeric-separator-literal-hil-nsl-hd-dunder-err.js")]
    public Task numeric_separator_literal_hil_nsl_hd_dunder_err()
        => CompilationFailureTest("numeric-separator-literal-hil-nsl-hd-dunder-err");

    [Fact(DisplayName = "numeric-separator-literal-hil-nsl-hd-err.js")]
    public Task numeric_separator_literal_hil_nsl_hd_err()
        => CompilationFailureTest("numeric-separator-literal-hil-nsl-hd-err");

    [Fact(DisplayName = "numeric-separator-literal-lol-00-err.js")]
    public Task numeric_separator_literal_lol_00_err()
        => CompilationFailureTest("numeric-separator-literal-lol-00-err");

    [Fact(DisplayName = "numeric-separator-literal-lol-01-err.js")]
    public Task numeric_separator_literal_lol_01_err()
        => CompilationFailureTest("numeric-separator-literal-lol-01-err");

    [Fact(DisplayName = "numeric-separator-literal-lol-07-err.js")]
    public Task numeric_separator_literal_lol_07_err()
        => CompilationFailureTest("numeric-separator-literal-lol-07-err");

    [Fact(DisplayName = "numeric-separator-literal-lol-0_0-err.js")]
    public Task numeric_separator_literal_lol_0_0_err()
        => CompilationFailureTest("numeric-separator-literal-lol-0_0-err");

    [Fact(DisplayName = "numeric-separator-literal-lol-0_1-err.js")]
    public Task numeric_separator_literal_lol_0_1_err()
        => CompilationFailureTest("numeric-separator-literal-lol-0_1-err");

    [Fact(DisplayName = "numeric-separator-literal-lol-0_7-err.js")]
    public Task numeric_separator_literal_lol_0_7_err()
        => CompilationFailureTest("numeric-separator-literal-lol-0_7-err");

    [Fact(DisplayName = "numeric-separator-literal-nonoctal-08-err.js")]
    public Task numeric_separator_literal_nonoctal_08_err()
        => CompilationFailureTest("numeric-separator-literal-nonoctal-08-err");

    [Fact(DisplayName = "numeric-separator-literal-nonoctal-09-err.js")]
    public Task numeric_separator_literal_nonoctal_09_err()
        => CompilationFailureTest("numeric-separator-literal-nonoctal-09-err");

    [Fact(DisplayName = "numeric-separator-literal-nonoctal-0_8-err.js")]
    public Task numeric_separator_literal_nonoctal_0_8_err()
        => CompilationFailureTest("numeric-separator-literal-nonoctal-0_8-err");

    [Fact(DisplayName = "numeric-separator-literal-nonoctal-0_9-err.js")]
    public Task numeric_separator_literal_nonoctal_0_9_err()
        => CompilationFailureTest("numeric-separator-literal-nonoctal-0_9-err");

    [Fact(DisplayName = "numeric-separator-literal-nzd-nsl-dds-dunder-err.js")]
    public Task numeric_separator_literal_nzd_nsl_dds_dunder_err()
        => CompilationFailureTest("numeric-separator-literal-nzd-nsl-dds-dunder-err");

    [Fact(DisplayName = "numeric-separator-literal-nzd-nsl-dds-leading-zero-err.js")]
    public Task numeric_separator_literal_nzd_nsl_dds_leading_zero_err()
        => CompilationFailureTest("numeric-separator-literal-nzd-nsl-dds-leading-zero-err");

    [Fact(DisplayName = "numeric-separator-literal-oil-nsl-od-dunder-err.js")]
    public Task numeric_separator_literal_oil_nsl_od_dunder_err()
        => CompilationFailureTest("numeric-separator-literal-oil-nsl-od-dunder-err");

    [Fact(DisplayName = "numeric-separator-literal-oil-nsl-od-err.js")]
    public Task numeric_separator_literal_oil_nsl_od_err()
        => CompilationFailureTest("numeric-separator-literal-oil-nsl-od-err");

    [Fact(DisplayName = "numeric-separator-literal-oil-od-nsl-od-err.js")]
    public Task numeric_separator_literal_oil_od_nsl_od_err()
        => CompilationFailureTest("numeric-separator-literal-oil-od-nsl-od-err");

    [Fact(DisplayName = "numeric-separator-literal-unicode-err.js")]
    public Task numeric_separator_literal_unicode_err()
        => CompilationFailureTest("numeric-separator-literal-unicode-err");

}
