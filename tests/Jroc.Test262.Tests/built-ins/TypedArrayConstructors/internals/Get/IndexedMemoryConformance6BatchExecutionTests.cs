using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Get;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.Get") { }

    [Fact(DisplayName = "indexed-value-sab.js")]
    public Task indexed_value_sab() => ExecutionTestFromFile("indexed-value-sab");

    [Fact(DisplayName = "indexed-value.js")]
    public Task indexed_value() => ExecutionTestFromFile("indexed-value");

    [Fact(DisplayName = "key-is-not-canonical-index.js")]
    public Task key_is_not_canonical_index() => ExecutionTestFromFile("key-is-not-canonical-index");

    [Fact(DisplayName = "key-is-not-numeric-index-get-throws.js")]
    public Task key_is_not_numeric_index_get_throws() => ExecutionTestFromFile("key-is-not-numeric-index-get-throws");

    [Fact(DisplayName = "key-is-not-numeric-index.js")]
    public Task key_is_not_numeric_index() => ExecutionTestFromFile("key-is-not-numeric-index");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");
}
