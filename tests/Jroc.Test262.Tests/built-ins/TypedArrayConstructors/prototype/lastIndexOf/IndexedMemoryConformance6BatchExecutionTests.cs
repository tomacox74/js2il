using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.prototype.lastIndexOf;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.prototype.lastIndexOf") { }

    [Fact(DisplayName = "bigint-inherited.js")]
    public Task bigint_inherited() => ExecutionTestFromFile("bigint-inherited");

    [Fact(DisplayName = "inherited.js")]
    public Task inherited() => ExecutionTestFromFile("inherited");
}
