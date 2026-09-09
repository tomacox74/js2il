using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray") { }

    [Fact(DisplayName = "prototype.js")]
    public Task prototype() => ExecutionTestFromFile("prototype");
}
