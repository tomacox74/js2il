using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.wait.bigint;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Atomics.wait.bigint") { }

    [Fact(DisplayName = "non-bigint64-typedarray-throws.js")]
    public Task non_bigint64_typedarray_throws() => ExecutionTestFromFile("non-bigint64-typedarray-throws");

    [Fact(DisplayName = "non-shared-bufferdata-throws.js")]
    public Task non_shared_bufferdata_throws() => ExecutionTestFromFile("non-shared-bufferdata-throws");
}
