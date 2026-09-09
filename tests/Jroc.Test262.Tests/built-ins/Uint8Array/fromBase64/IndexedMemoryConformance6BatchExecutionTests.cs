using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.fromBase64;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Uint8Array.fromBase64") { }

    [Fact(DisplayName = "results.js")]
    public Task results() => ExecutionTestFromFile("results");
}
