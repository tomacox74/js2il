using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.values.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.values.BigInt") { }

    [Fact(DisplayName = "iter-prototype.js")]
    public Task iter_prototype() => ExecutionTestFromFile("iter-prototype");

    [Fact(DisplayName = "return-itor.js")]
    public Task return_itor() => ExecutionTestFromFile("return-itor");
}
