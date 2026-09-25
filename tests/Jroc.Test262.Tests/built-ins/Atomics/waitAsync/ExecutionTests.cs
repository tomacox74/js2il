using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.waitAsync;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.waitAsync") { }

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTest("bad-range");

    [Fact(DisplayName = "descriptor.js")]
    public Task ported_descriptor() => ExecutionTest("descriptor");

    [Fact(DisplayName = "false-for-timeout.js")]
    public Task ported_false_for_timeout() => ExecutionTest("false-for-timeout");

    [Fact(DisplayName = "implicit-infinity-for-timeout.js")]
    public Task ported_implicit_infinity_for_timeout() => ExecutionTest("implicit-infinity-for-timeout");

    [Fact(DisplayName = "is-function.js")]
    public Task ported_is_function() => ExecutionTest("is-function");

    [Fact(DisplayName = "length.js")]
    public Task ported_length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTest("name");

    [Fact(DisplayName = "negative-index-throws.js")]
    public Task ported_negative_index_throws() => ExecutionTest("negative-index-throws");

    [Fact(DisplayName = "negative-timeout.js")]
    public Task ported_negative_timeout() => ExecutionTest("negative-timeout");

    [Fact(DisplayName = "non-int32-typedarray-throws.js")]
    public Task ported_non_int32_typedarray_throws() => ExecutionTest("non-int32-typedarray-throws");

    [Fact(DisplayName = "non-shared-bufferdata-throws.js")]
    public Task ported_non_shared_bufferdata_throws() => ExecutionTest("non-shared-bufferdata-throws");

    [Fact(DisplayName = "not-a-typedarray-throws.js")]
    public Task ported_not_a_typedarray_throws() => ExecutionTest("not-a-typedarray-throws");

    [Fact(DisplayName = "not-an-object-throws.js")]
    public Task ported_not_an_object_throws() => ExecutionTest("not-an-object-throws");

    [Fact(DisplayName = "null-bufferdata-throws.js")]
    public Task ported_null_bufferdata_throws() => ExecutionTest("null-bufferdata-throws");

    [Fact(DisplayName = "null-for-timeout.js")]
    public Task ported_null_for_timeout() => ExecutionTest("null-for-timeout");

    [Fact(DisplayName = "object-for-timeout.js")]
    public Task ported_object_for_timeout() => ExecutionTest("object-for-timeout");

    [Fact(DisplayName = "out-of-range-index-throws.js")]
    public Task ported_out_of_range_index_throws() => ExecutionTest("out-of-range-index-throws");

    [Fact(DisplayName = "poisoned-object-for-timeout-throws.js")]
    public Task ported_poisoned_object_for_timeout_throws() => ExecutionTest("poisoned-object-for-timeout-throws");

    [Fact(DisplayName = "retrieve-length-before-index-coercion.js")]
    public Task ported_retrieve_length_before_index_coercion() => ExecutionTest("retrieve-length-before-index-coercion");

    [Fact(DisplayName = "returns-result-object-value-is-promise-resolves-to-ok.js")]
    public Task ported_returns_result_object_value_is_promise_resolves_to_ok() => ExecutionTest("returns-result-object-value-is-promise-resolves-to-ok");

    [Fact(DisplayName = "returns-result-object-value-is-string-not-equal.js")]
    public Task ported_returns_result_object_value_is_string_not_equal() => ExecutionTest("returns-result-object-value-is-string-not-equal");

    [Fact(DisplayName = "returns-result-object-value-is-string-timed-out.js")]
    public Task ported_returns_result_object_value_is_string_timed_out() => ExecutionTest("returns-result-object-value-is-string-timed-out");

    [Fact(DisplayName = "symbol-for-index-throws.js")]
    public Task ported_symbol_for_index_throws() => ExecutionTest("symbol-for-index-throws");

    [Fact(DisplayName = "symbol-for-timeout-throws.js")]
    public Task ported_symbol_for_timeout_throws() => ExecutionTest("symbol-for-timeout-throws");

    [Fact(DisplayName = "symbol-for-value-throws.js")]
    public Task ported_symbol_for_value_throws() => ExecutionTest("symbol-for-value-throws");

    [Fact(DisplayName = "undefined-for-timeout.js")]
    public Task ported_undefined_for_timeout() => ExecutionTest("undefined-for-timeout");

    [Fact(DisplayName = "validate-arraytype-before-index-coercion.js")]
    public Task ported_validate_arraytype_before_index_coercion() => ExecutionTest("validate-arraytype-before-index-coercion");

    [Fact(DisplayName = "validate-arraytype-before-timeout-coercion.js")]
    public Task ported_validate_arraytype_before_timeout_coercion() => ExecutionTest("validate-arraytype-before-timeout-coercion");

    [Fact(DisplayName = "validate-arraytype-before-value-coercion.js")]
    public Task ported_validate_arraytype_before_value_coercion() => ExecutionTest("validate-arraytype-before-value-coercion");

    [Fact(DisplayName = "value-not-equal.js")]
    public Task ported_value_not_equal() => ExecutionTest("value-not-equal");

}
