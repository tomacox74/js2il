using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.toBase64;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Uint8Array.prototype.toBase64") { }

    [Fact(DisplayName = "receiver-not-uint8array.js")]
    public Task receiver_not_uint8array() => ExecutionTestFromFile("receiver-not-uint8array");
}
