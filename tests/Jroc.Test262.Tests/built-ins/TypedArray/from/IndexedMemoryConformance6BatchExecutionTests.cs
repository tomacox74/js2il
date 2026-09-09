using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.@from;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.from") { }

    [Fact(DisplayName = "iterated-array-changed-by-tonumber.js")]
    public Task iterated_array_changed_by_tonumber() => ExecutionTestFromFile("iterated-array-changed-by-tonumber");
}
