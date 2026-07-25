using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.wait;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("built_ins.Atomics.wait") { }

    [Fact(DisplayName = "bad-range")]
    public Task bad_range()
        => ExecutionTest("bad-range");

    [Fact(DisplayName = "negative-index-throws")]
    public Task negative_index_throws()
        => ExecutionTest("negative-index-throws");

    [Fact(DisplayName = "non-shared-bufferdata-throws")]
    public Task non_shared_bufferdata_throws()
        => ExecutionTest("non-shared-bufferdata-throws");

    [Fact(DisplayName = "not-a-typedarray-throws")]
    public Task not_a_typedarray_throws()
        => ExecutionTest("not-a-typedarray-throws");

    [Fact(DisplayName = "not-an-object-throws")]
    public Task not_an_object_throws()
        => ExecutionTest("not-an-object-throws");

    [Fact(DisplayName = "out-of-range-index-throws")]
    public Task out_of_range_index_throws()
        => ExecutionTest("out-of-range-index-throws");

    [Fact(DisplayName = "symbol-for-index-throws")]
    public Task symbol_for_index_throws()
        => ExecutionTest("symbol-for-index-throws");

    [Fact(DisplayName = "symbol-for-timeout-throws")]
    public Task symbol_for_timeout_throws()
        => ExecutionTest("symbol-for-timeout-throws");

}
