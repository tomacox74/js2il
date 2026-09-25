using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.OwnPropertyKeys.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : InMemoryExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.OwnPropertyKeys.BigInt") { }

    [Fact(DisplayName = "not-enumerable-keys.js")]
    public Task not_enumerable_keys() => ExecutionTestFromFile("not-enumerable-keys");
}
