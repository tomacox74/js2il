using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.HasProperty;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.HasProperty") { }

    [Fact(DisplayName = "indexed-value.js")]
    public Task indexed_value() => ExecutionTestFromFile("indexed-value");

    [Fact(DisplayName = "key-is-not-canonical-index.js")]
    public Task key_is_not_canonical_index() => ExecutionTestFromFile("key-is-not-canonical-index");

    [Fact(DisplayName = "key-is-not-numeric-index.js")]
    public Task key_is_not_numeric_index() => ExecutionTestFromFile("key-is-not-numeric-index");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");

    [Fact(DisplayName = "resizable-array-buffer-auto.js")]
    public Task resizable_array_buffer_auto() => ExecutionTestFromFile("resizable-array-buffer-auto");

    [Fact(DisplayName = "resizable-array-buffer-fixed.js")]
    public Task resizable_array_buffer_fixed() => ExecutionTestFromFile("resizable-array-buffer-fixed");
}
