using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify.bigint;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics.notify.bigint") { }

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTest("bad-range");

    [Fact(DisplayName = "non-shared-bufferdata-count-evaluation-throws.js")]
    public Task ported_non_shared_bufferdata_count_evaluation_throws() => ExecutionTest("non-shared-bufferdata-count-evaluation-throws");

    [Fact(DisplayName = "non-shared-bufferdata-index-evaluation-throws.js")]
    public Task ported_non_shared_bufferdata_index_evaluation_throws() => ExecutionTest("non-shared-bufferdata-index-evaluation-throws");

    [Fact(DisplayName = "non-shared-bufferdata-returns-0.js")]
    public Task ported_non_shared_bufferdata_returns_0() => ExecutionTest("non-shared-bufferdata-returns-0");
}
