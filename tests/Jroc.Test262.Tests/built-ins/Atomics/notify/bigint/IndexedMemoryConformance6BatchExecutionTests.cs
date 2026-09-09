using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify.bigint;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Atomics.notify.bigint") { }

    [Fact(DisplayName = "non-bigint64-typedarray-throws.js")]
    public Task non_bigint64_typedarray_throws() => ExecutionTestFromFile("non-bigint64-typedarray-throws");

    [Fact(DisplayName = "non-shared-bufferdata-non-shared-int-views-throws.js")]
    public Task non_shared_bufferdata_non_shared_int_views_throws() => ExecutionTestFromFile("non-shared-bufferdata-non-shared-int-views-throws");
}
