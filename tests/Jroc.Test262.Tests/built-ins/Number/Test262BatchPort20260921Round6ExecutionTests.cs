using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number;

public class Test262BatchPort20260921Round6ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260921Round6ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "return-abrupt-tonumber-value-symbol")]
    public Task return_abrupt_tonumber_value_symbol()
        => ExecutionTestFromFile("return-abrupt-tonumber-value-symbol");

    [Fact(DisplayName = "string-binary-literal-invalid")]
    public Task string_binary_literal_invalid()
        => ExecutionTestFromFile("string-binary-literal-invalid");

    [Fact(DisplayName = "string-hex-literal-invalid")]
    public Task string_hex_literal_invalid()
        => ExecutionTestFromFile("string-hex-literal-invalid");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bd-nsl-bd")]
    public Task string_numeric_separator_literal_bil_bd_nsl_bd()
        => ExecutionTestFromFile("string-numeric-separator-literal-bil-bd-nsl-bd");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bd-nsl-bds")]
    public Task string_numeric_separator_literal_bil_bd_nsl_bds()
        => ExecutionTestFromFile("string-numeric-separator-literal-bil-bd-nsl-bds");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bds-nsl-bd")]
    public Task string_numeric_separator_literal_bil_bds_nsl_bd()
        => ExecutionTestFromFile("string-numeric-separator-literal-bil-bds-nsl-bd");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bds-nsl-bds")]
    public Task string_numeric_separator_literal_bil_bds_nsl_bds()
        => ExecutionTestFromFile("string-numeric-separator-literal-bil-bds-nsl-bds");

    [Fact(DisplayName = "string-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dd-nsl-dd")]
    public Task string_numeric_separator_literal_dd_dot_dd_ep_sign_minus_dd_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dd-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dds-nsl-dd")]
    public Task string_numeric_separator_literal_dd_dot_dd_ep_sign_minus_dds_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dds-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dd-nsl-dd")]
    public Task string_numeric_separator_literal_dd_dot_dd_ep_sign_plus_dd_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dd-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dds-nsl-dd")]
    public Task string_numeric_separator_literal_dd_dot_dd_ep_sign_plus_dds_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dds-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-dd-nsl-dd-one-of")]
    public Task string_numeric_separator_literal_dd_nsl_dd_one_of()
        => ExecutionTestFromFile("string-numeric-separator-literal-dd-nsl-dd-one-of");

    [Fact(DisplayName = "string-numeric-separator-literal-dds-dot-dd-nsl-dd-ep-dd")]
    public Task string_numeric_separator_literal_dds_dot_dd_nsl_dd_ep_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-dds-dot-dd-nsl-dd-ep-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-dds-nsl-dd")]
    public Task string_numeric_separator_literal_dds_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-dds-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-dot-dd-nsl-dd-ep")]
    public Task string_numeric_separator_literal_dot_dd_nsl_dd_ep()
        => ExecutionTestFromFile("string-numeric-separator-literal-dot-dd-nsl-dd-ep");

    [Fact(DisplayName = "string-numeric-separator-literal-dot-dd-nsl-dds-ep")]
    public Task string_numeric_separator_literal_dot_dd_nsl_dds_ep()
        => ExecutionTestFromFile("string-numeric-separator-literal-dot-dd-nsl-dds-ep");

    [Fact(DisplayName = "string-numeric-separator-literal-dot-dds-nsl-dd-ep")]
    public Task string_numeric_separator_literal_dot_dds_nsl_dd_ep()
        => ExecutionTestFromFile("string-numeric-separator-literal-dot-dds-nsl-dd-ep");

    [Fact(DisplayName = "string-numeric-separator-literal-dot-dds-nsl-dds-ep")]
    public Task string_numeric_separator_literal_dot_dds_nsl_dds_ep()
        => ExecutionTestFromFile("string-numeric-separator-literal-dot-dds-nsl-dds-ep");

    [Fact(DisplayName = "string-numeric-separator-literal-hil-hd-nsl-hd")]
    public Task string_numeric_separator_literal_hil_hd_nsl_hd()
        => ExecutionTestFromFile("string-numeric-separator-literal-hil-hd-nsl-hd");

    [Fact(DisplayName = "string-numeric-separator-literal-hil-hd-nsl-hds")]
    public Task string_numeric_separator_literal_hil_hd_nsl_hds()
        => ExecutionTestFromFile("string-numeric-separator-literal-hil-hd-nsl-hds");

    [Fact(DisplayName = "string-numeric-separator-literal-hil-hds-nsl-hd")]
    public Task string_numeric_separator_literal_hil_hds_nsl_hd()
        => ExecutionTestFromFile("string-numeric-separator-literal-hil-hds-nsl-hd");

    [Fact(DisplayName = "string-numeric-separator-literal-hil-hds-nsl-hds")]
    public Task string_numeric_separator_literal_hil_hds_nsl_hds()
        => ExecutionTestFromFile("string-numeric-separator-literal-hil-hds-nsl-hds");

    [Fact(DisplayName = "string-numeric-separator-literal-hil-od-nsl-od-one-of")]
    public Task string_numeric_separator_literal_hil_od_nsl_od_one_of()
        => ExecutionTestFromFile("string-numeric-separator-literal-hil-od-nsl-od-one-of");

    [Fact(DisplayName = "string-numeric-separator-literal-nzd-nsl-dd-one-of")]
    public Task string_numeric_separator_literal_nzd_nsl_dd_one_of()
        => ExecutionTestFromFile("string-numeric-separator-literal-nzd-nsl-dd-one-of");

    [Fact(DisplayName = "string-numeric-separator-literal-nzd-nsl-dd")]
    public Task string_numeric_separator_literal_nzd_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-nzd-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-nzd-nsl-dds")]
    public Task string_numeric_separator_literal_nzd_nsl_dds()
        => ExecutionTestFromFile("string-numeric-separator-literal-nzd-nsl-dds");

    [Fact(DisplayName = "string-numeric-separator-literal-oil-od-nsl-od-one-of")]
    public Task string_numeric_separator_literal_oil_od_nsl_od_one_of()
        => ExecutionTestFromFile("string-numeric-separator-literal-oil-od-nsl-od-one-of");

    [Fact(DisplayName = "string-numeric-separator-literal-oil-od-nsl-od")]
    public Task string_numeric_separator_literal_oil_od_nsl_od()
        => ExecutionTestFromFile("string-numeric-separator-literal-oil-od-nsl-od");

    [Fact(DisplayName = "string-numeric-separator-literal-oil-od-nsl-ods")]
    public Task string_numeric_separator_literal_oil_od_nsl_ods()
        => ExecutionTestFromFile("string-numeric-separator-literal-oil-od-nsl-ods");

    [Fact(DisplayName = "string-numeric-separator-literal-oil-ods-nsl-od")]
    public Task string_numeric_separator_literal_oil_ods_nsl_od()
        => ExecutionTestFromFile("string-numeric-separator-literal-oil-ods-nsl-od");

    [Fact(DisplayName = "string-numeric-separator-literal-oil-ods-nsl-ods")]
    public Task string_numeric_separator_literal_oil_ods_nsl_ods()
        => ExecutionTestFromFile("string-numeric-separator-literal-oil-ods-nsl-ods");

    [Fact(DisplayName = "string-numeric-separator-literal-sign-minus-dds-nsl-dd")]
    public Task string_numeric_separator_literal_sign_minus_dds_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-sign-minus-dds-nsl-dd");

    [Fact(DisplayName = "string-numeric-separator-literal-sign-plus-dds-nsl-dd")]
    public Task string_numeric_separator_literal_sign_plus_dds_nsl_dd()
        => ExecutionTestFromFile("string-numeric-separator-literal-sign-plus-dds-nsl-dd");

    [Fact(DisplayName = "string-octal-literal-invald")]
    public Task string_octal_literal_invald()
        => ExecutionTestFromFile("string-octal-literal-invald");

}
