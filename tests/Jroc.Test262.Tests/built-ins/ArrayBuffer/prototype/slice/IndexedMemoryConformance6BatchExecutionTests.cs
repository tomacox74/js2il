using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.slice;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "species-returns-immutable-arraybuffer.js")]
    public Task species_returns_immutable_arraybuffer() => ExecutionTestFromFile("species-returns-immutable-arraybuffer");
}
