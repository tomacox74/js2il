namespace Jroc.Test262.Tests.built_ins.JSON.stringify;

public partial class ExecutionTests
{
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "replacer-array-wrong-type")]
    public Task replacer_array_wrong_type() => ExecutionTestFromFile("replacer-array-wrong-type");

    [Fact(DisplayName = "replacer-function-result-undefined")]
    public Task replacer_function_result_undefined() => ExecutionTestFromFile("replacer-function-result-undefined");

    [Fact(DisplayName = "replacer-function-tojson")]
    public Task replacer_function_tojson() => ExecutionTestFromFile("replacer-function-tojson");

    [Fact(DisplayName = "replacer-wrong-type")]
    public Task replacer_wrong_type() => ExecutionTestFromFile("replacer-wrong-type");

    [Fact(DisplayName = "space-number-float")]
    public Task space_number_float() => ExecutionTestFromFile("space-number-float");

    [Fact(DisplayName = "space-number")]
    public Task space_number() => ExecutionTestFromFile("space-number");

    [Fact(DisplayName = "space-number-range")]
    public Task space_number_range() => ExecutionTestFromFile("space-number-range");

    [Fact(DisplayName = "space-string-range")]
    public Task space_string_range() => ExecutionTestFromFile("space-string-range");

    [Fact(DisplayName = "space-wrong-type")]
    public Task space_wrong_type() => ExecutionTestFromFile("space-wrong-type");

    [Fact(DisplayName = "value-array-abrupt")]
    public Task value_array_abrupt() => ExecutionTestFromFile("value-array-abrupt");

    [Fact(DisplayName = "value-array-circular")]
    public Task value_array_circular() => ExecutionTestFromFile("value-array-circular");

    [Fact(DisplayName = "value-array-proxy")]
    public Task value_array_proxy() => ExecutionTestFromFile("value-array-proxy");

    [Fact(DisplayName = "value-array-proxy-revoked")]
    public Task value_array_proxy_revoked() => ExecutionTestFromFile("value-array-proxy-revoked");

    [Fact(DisplayName = "value-bigint-order")]
    public Task value_bigint_order() => ExecutionTestFromFile("value-bigint-order");

    [Fact(DisplayName = "value-bigint-replacer")]
    public Task value_bigint_replacer() => ExecutionTestFromFile("value-bigint-replacer");

    [Fact(DisplayName = "value-bigint-tojson-receiver")]
    public Task value_bigint_tojson_receiver() => ExecutionTestFromFile("value-bigint-tojson-receiver");

    [Fact(DisplayName = "value-boolean-object")]
    public Task value_boolean_object() => ExecutionTestFromFile("value-boolean-object");

    [Fact(DisplayName = "value-function")]
    public Task value_function() => ExecutionTestFromFile("value-function");

    [Fact(DisplayName = "value-number-negative-zero")]
    public Task value_number_negative_zero() => ExecutionTestFromFile("value-number-negative-zero");

    [Fact(DisplayName = "value-number-non-finite")]
    public Task value_number_non_finite() => ExecutionTestFromFile("value-number-non-finite");

    [Fact(DisplayName = "value-number-object")]
    public Task value_number_object() => ExecutionTestFromFile("value-number-object");

    [Fact(DisplayName = "value-object-abrupt")]
    public Task value_object_abrupt() => ExecutionTestFromFile("value-object-abrupt");

    [Fact(DisplayName = "value-object-circular")]
    public Task value_object_circular() => ExecutionTestFromFile("value-object-circular");

    [Fact(DisplayName = "value-object-proxy")]
    public Task value_object_proxy() => ExecutionTestFromFile("value-object-proxy");

    [Fact(DisplayName = "value-object-proxy-revoked")]
    public Task value_object_proxy_revoked() => ExecutionTestFromFile("value-object-proxy-revoked");

    [Fact(DisplayName = "value-primitive-top-level")]
    public Task value_primitive_top_level() => ExecutionTestFromFile("value-primitive-top-level");

    [Fact(DisplayName = "value-string-escape-ascii")]
    public Task value_string_escape_ascii() => ExecutionTestFromFile("value-string-escape-ascii");

    [Fact(DisplayName = "value-string-escape-unicode")]
    public Task value_string_escape_unicode() => ExecutionTestFromFile("value-string-escape-unicode");

    [Fact(DisplayName = "value-string-object")]
    public Task value_string_object() => ExecutionTestFromFile("value-string-object");

    [Fact(DisplayName = "value-symbol")]
    public Task value_symbol() => ExecutionTestFromFile("value-symbol");

    [Fact(DisplayName = "value-tojson-abrupt")]
    public Task value_tojson_abrupt() => ExecutionTestFromFile("value-tojson-abrupt");

    [Fact(DisplayName = "value-tojson-arguments")]
    public Task value_tojson_arguments() => ExecutionTestFromFile("value-tojson-arguments");

    [Fact(DisplayName = "value-tojson-array-circular")]
    public Task value_tojson_array_circular() => ExecutionTestFromFile("value-tojson-array-circular");

    [Fact(DisplayName = "value-tojson-not-function")]
    public Task value_tojson_not_function() => ExecutionTestFromFile("value-tojson-not-function");

    [Fact(DisplayName = "value-tojson-object-circular")]
    public Task value_tojson_object_circular() => ExecutionTestFromFile("value-tojson-object-circular");
}
