using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.wait.bigint;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.wait.bigint") { }

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTest("bad-range");

    [Fact(DisplayName = "false-for-timeout.js")]
    public Task ported_false_for_timeout() => ExecutionTest("false-for-timeout");

    [Fact(DisplayName = "negative-index-throws.js")]
    public Task ported_negative_index_throws() => ExecutionTest("negative-index-throws");

    [Fact(DisplayName = "negative-timeout.js")]
    public Task ported_negative_timeout() => ExecutionTest("negative-timeout");

    [Fact(DisplayName = "out-of-range-index-throws.js")]
    public Task ported_out_of_range_index_throws() => ExecutionTest("out-of-range-index-throws");
}
