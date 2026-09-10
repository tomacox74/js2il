using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number;

public class NextBatch100Followup6ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100Followup6ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "return-abrupt-tonumber-value-symbol")]
    public Task return_abrupt_tonumber_value_symbol() => ExecutionTestFromFile("return-abrupt-tonumber-value-symbol");

    [Fact(DisplayName = "string-binary-literal-invalid")]
    public Task string_binary_literal_invalid() => ExecutionTestFromFile("string-binary-literal-invalid");

    [Fact(DisplayName = "string-hex-literal-invalid")]
    public Task string_hex_literal_invalid() => ExecutionTestFromFile("string-hex-literal-invalid");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bd-nsl-bd")]
    public Task string_numeric_separator_literal_bil_bd_nsl_bd() => ExecutionTestFromFile("string-numeric-separator-literal-bil-bd-nsl-bd");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bd-nsl-bds")]
    public Task string_numeric_separator_literal_bil_bd_nsl_bds() => ExecutionTestFromFile("string-numeric-separator-literal-bil-bd-nsl-bds");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bds-nsl-bd")]
    public Task string_numeric_separator_literal_bil_bds_nsl_bd() => ExecutionTestFromFile("string-numeric-separator-literal-bil-bds-nsl-bd");

    [Fact(DisplayName = "string-numeric-separator-literal-bil-bds-nsl-bds")]
    public Task string_numeric_separator_literal_bil_bds_nsl_bds() => ExecutionTestFromFile("string-numeric-separator-literal-bil-bds-nsl-bds");

    [Fact(DisplayName = "string-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dd-nsl-dd")]
    public Task string_numeric_separator_literal_dd_dot_dd_ep_sign_minus_dd_nsl_dd() => ExecutionTestFromFile("string-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dd-nsl-dd");

}
