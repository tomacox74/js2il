using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.GetOwnProperty.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.GetOwnProperty.BigInt") { }

    [Fact(DisplayName = "index-prop-desc.js")]
    public Task index_prop_desc() => ExecutionTestFromFile("index-prop-desc");

    [Fact(DisplayName = "key-is-minus-zero.js")]
    public Task key_is_minus_zero() => ExecutionTestFromFile("key-is-minus-zero");

    [Fact(DisplayName = "key-is-not-canonical-index.js")]
    public Task key_is_not_canonical_index() => ExecutionTestFromFile("key-is-not-canonical-index");

    [Fact(DisplayName = "key-is-not-integer.js")]
    public Task key_is_not_integer() => ExecutionTestFromFile("key-is-not-integer");

    [Fact(DisplayName = "key-is-not-numeric-index.js")]
    public Task key_is_not_numeric_index() => ExecutionTestFromFile("key-is-not-numeric-index");

    [Fact(DisplayName = "key-is-out-of-bounds.js")]
    public Task key_is_out_of_bounds() => ExecutionTestFromFile("key-is-out-of-bounds");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");
}
