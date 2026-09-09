using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.OwnPropertyKeys;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.OwnPropertyKeys") { }

    [Fact(DisplayName = "integer-indexes-resizable-array-buffer-auto.js")]
    public Task integer_indexes_resizable_array_buffer_auto() => ExecutionTestFromFile("integer-indexes-resizable-array-buffer-auto");

    [Fact(DisplayName = "integer-indexes-resizable-array-buffer-fixed.js")]
    public Task integer_indexes_resizable_array_buffer_fixed() => ExecutionTestFromFile("integer-indexes-resizable-array-buffer-fixed");

    [Fact(DisplayName = "not-enumerable-keys.js")]
    public Task not_enumerable_keys() => ExecutionTestFromFile("not-enumerable-keys");
}
