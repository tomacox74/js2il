using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.object_arg;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.object-arg") { }

    [Fact(DisplayName = "conversion-operation.js")]
    public Task conversion_operation() => ExecutionTestFromFile("conversion-operation");
}
