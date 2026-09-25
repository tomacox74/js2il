using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.waitAsync.bigint;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.waitAsync.bigint") { }

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTest("bad-range");

    [Fact(DisplayName = "false-for-timeout.js")]
    public Task ported_false_for_timeout() => ExecutionTest("false-for-timeout");

    [Fact(DisplayName = "negative-index-throws.js")]
    public Task ported_negative_index_throws() => ExecutionTest("negative-index-throws");

    [Fact(DisplayName = "negative-timeout.js")]
    public Task ported_negative_timeout() => ExecutionTest("negative-timeout");

    [Fact(DisplayName = "non-bigint64-typedarray-throws.js")]
    public Task ported_non_bigint64_typedarray_throws() => ExecutionTest("non-bigint64-typedarray-throws");

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

    [Fact(DisplayName = "symbol-for-index-throws.js")]
    public Task ported_symbol_for_index_throws() => ExecutionTest("symbol-for-index-throws");

    [Fact(DisplayName = "symbol-for-timeout-throws.js")]
    public Task ported_symbol_for_timeout_throws() => ExecutionTest("symbol-for-timeout-throws");

    [Fact(DisplayName = "symbol-for-value-throws.js")]
    public Task ported_symbol_for_value_throws() => ExecutionTest("symbol-for-value-throws");

    [Fact(DisplayName = "undefined-for-timeout.js")]
    public Task ported_undefined_for_timeout() => ExecutionTest("undefined-for-timeout");

    [Fact(DisplayName = "value-not-equal.js")]
    public Task ported_value_not_equal() => ExecutionTest("value-not-equal");

}
