using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Get.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.Get.BigInt") { }

    [Fact(DisplayName = "key-is-not-numeric-index-get-throws.js")]
    public Task key_is_not_numeric_index_get_throws() => ExecutionTestFromFile("key-is-not-numeric-index-get-throws");

    [Fact(DisplayName = "key-is-not-numeric-index.js")]
    public Task key_is_not_numeric_index() => ExecutionTestFromFile("key-is-not-numeric-index");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");
}
