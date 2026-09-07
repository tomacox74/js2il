using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.bigint.numeric_separators;

public class LiteralsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LiteralsConformanceBatchExecutionTests() : base("language.literals.bigint.numeric-separators") { }

    [Fact(DisplayName = "numeric-separator-literal-hil-hd-nsl-hd.js")]
    public Task numeric_separator_literal_hil_hd_nsl_hd()
        => ExecutionTest("numeric-separator-literal-hil-hd-nsl-hd");

    [Fact(DisplayName = "numeric-separator-literal-hil-hd-nsl-hds.js")]
    public Task numeric_separator_literal_hil_hd_nsl_hds()
        => ExecutionTest("numeric-separator-literal-hil-hd-nsl-hds");

    [Fact(DisplayName = "numeric-separator-literal-hil-hds-nsl-hd.js")]
    public Task numeric_separator_literal_hil_hds_nsl_hd()
        => ExecutionTest("numeric-separator-literal-hil-hds-nsl-hd");

    [Fact(DisplayName = "numeric-separator-literal-hil-hds-nsl-hds.js")]
    public Task numeric_separator_literal_hil_hds_nsl_hds()
        => ExecutionTest("numeric-separator-literal-hil-hds-nsl-hds");

    [Fact(DisplayName = "numeric-separator-literal-hil-od-nsl-od-one-of.js")]
    public Task numeric_separator_literal_hil_od_nsl_od_one_of()
        => ExecutionTest("numeric-separator-literal-hil-od-nsl-od-one-of");

    [Fact(DisplayName = "numeric-separator-literal-nzd-nsl-dd-one-of.js")]
    public Task numeric_separator_literal_nzd_nsl_dd_one_of()
        => ExecutionTest("numeric-separator-literal-nzd-nsl-dd-one-of");

    [Fact(DisplayName = "numeric-separator-literal-nzd-nsl-dd.js")]
    public Task numeric_separator_literal_nzd_nsl_dd()
        => ExecutionTest("numeric-separator-literal-nzd-nsl-dd");

    [Fact(DisplayName = "numeric-separator-literal-nzd-nsl-dds.js")]
    public Task numeric_separator_literal_nzd_nsl_dds()
        => ExecutionTest("numeric-separator-literal-nzd-nsl-dds");

    [Fact(DisplayName = "numeric-separator-literal-oil-od-nsl-od-one-of.js")]
    public Task numeric_separator_literal_oil_od_nsl_od_one_of()
        => ExecutionTest("numeric-separator-literal-oil-od-nsl-od-one-of");

    [Fact(DisplayName = "numeric-separator-literal-oil-od-nsl-od.js")]
    public Task numeric_separator_literal_oil_od_nsl_od()
        => ExecutionTest("numeric-separator-literal-oil-od-nsl-od");

    [Fact(DisplayName = "numeric-separator-literal-oil-od-nsl-ods.js")]
    public Task numeric_separator_literal_oil_od_nsl_ods()
        => ExecutionTest("numeric-separator-literal-oil-od-nsl-ods");

    [Fact(DisplayName = "numeric-separator-literal-oil-ods-nsl-od.js")]
    public Task numeric_separator_literal_oil_ods_nsl_od()
        => ExecutionTest("numeric-separator-literal-oil-ods-nsl-od");

    [Fact(DisplayName = "numeric-separator-literal-oil-ods-nsl-ods.js")]
    public Task numeric_separator_literal_oil_ods_nsl_ods()
        => ExecutionTest("numeric-separator-literal-oil-ods-nsl-ods");

    [Fact(DisplayName = "numeric-separator-literal-sign-minus-dds-nsl-dd.js")]
    public Task numeric_separator_literal_sign_minus_dds_nsl_dd()
        => ExecutionTest("numeric-separator-literal-sign-minus-dds-nsl-dd");

}
