using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reverse.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.reverse.BigInt") { }

    [Fact(DisplayName = "reverts.js")]
    public Task reverts() => ExecutionTestFromFile("reverts");
}
