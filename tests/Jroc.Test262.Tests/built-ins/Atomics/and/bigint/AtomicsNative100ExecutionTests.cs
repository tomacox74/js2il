using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.and.bigint;

public class AtomicsNative100ExecutionTests : InMemoryExecutionTestsBase
{
    public AtomicsNative100ExecutionTests() : base("built_ins.Atomics.and.bigint") { }

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTestFromFile("bad-range");

    [Fact(DisplayName = "good-views.js")]
    public Task ported_good_views() => ExecutionTestFromFile("good-views");

    [Fact(DisplayName = "non-shared-bufferdata.js")]
    public Task ported_non_shared_bufferdata() => ExecutionTestFromFile("non-shared-bufferdata");

}
