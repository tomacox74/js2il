using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.setFromHex;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Uint8Array.prototype.setFromHex") { }

    [Fact(DisplayName = "string-coercion.js")]
    public Task string_coercion() => ExecutionTestFromFile("string-coercion");
}
