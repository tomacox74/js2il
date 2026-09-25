using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.notify") { }

    [Fact(DisplayName = "non-views")]
    public Task non_views()
        => ExecutionTest("non-views");

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTest("bad-range");

    [Fact(DisplayName = "count-boundary-cases.js")]
    public Task ported_count_boundary_cases() => ExecutionTest("count-boundary-cases");

    [Fact(DisplayName = "count-from-nans.js")]
    public Task ported_count_from_nans() => ExecutionTest("count-from-nans");

    [Fact(DisplayName = "count-tointeger-throws-then-wake-throws.js")]
    public Task ported_count_tointeger_throws_then_wake_throws() => ExecutionTest("count-tointeger-throws-then-wake-throws");

    [Fact(DisplayName = "descriptor.js")]
    public Task ported_descriptor() => ExecutionTest("descriptor");

    [Fact(DisplayName = "length.js")]
    public Task ported_length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTest("name");

    [Fact(DisplayName = "negative-index-throws.js")]
    public Task ported_negative_index_throws() => ExecutionTest("negative-index-throws");

    [Fact(DisplayName = "non-shared-bufferdata-count-evaluation-throws.js")]
    public Task ported_non_shared_bufferdata_count_evaluation_throws() => ExecutionTest("non-shared-bufferdata-count-evaluation-throws");

    [Fact(DisplayName = "non-shared-bufferdata-index-evaluation-throws.js")]
    public Task ported_non_shared_bufferdata_index_evaluation_throws() => ExecutionTest("non-shared-bufferdata-index-evaluation-throws");

    [Fact(DisplayName = "non-shared-bufferdata-returns-0.js")]
    public Task ported_non_shared_bufferdata_returns_0() => ExecutionTest("non-shared-bufferdata-returns-0");

    [Fact(DisplayName = "out-of-range-index-throws.js")]
    public Task ported_out_of_range_index_throws() => ExecutionTest("out-of-range-index-throws");

    [Fact(DisplayName = "retrieve-length-before-index-coercion-non-shared-detached.js")]
    public Task ported_retrieve_length_before_index_coercion_non_shared_detached() => ExecutionTest("retrieve-length-before-index-coercion-non-shared-detached");

    [Fact(DisplayName = "retrieve-length-before-index-coercion-non-shared-resize-to-zero.js")]
    public Task ported_retrieve_length_before_index_coercion_non_shared_resize_to_zero() => ExecutionTest("retrieve-length-before-index-coercion-non-shared-resize-to-zero");

    [Fact(DisplayName = "retrieve-length-before-index-coercion-non-shared.js")]
    public Task ported_retrieve_length_before_index_coercion_non_shared() => ExecutionTest("retrieve-length-before-index-coercion-non-shared");

    [Fact(DisplayName = "retrieve-length-before-index-coercion.js")]
    public Task ported_retrieve_length_before_index_coercion() => ExecutionTest("retrieve-length-before-index-coercion");

    [Fact(DisplayName = "symbol-for-index-throws.js")]
    public Task ported_symbol_for_index_throws() => ExecutionTest("symbol-for-index-throws");
}
