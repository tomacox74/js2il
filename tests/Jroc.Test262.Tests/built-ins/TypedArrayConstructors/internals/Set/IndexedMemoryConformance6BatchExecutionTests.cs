using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Set;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.Set") { }

    [Fact(DisplayName = "bigint-tonumber.js")]
    public Task bigint_tonumber() => ExecutionTestFromFile("bigint-tonumber");

    [Fact(DisplayName = "conversion-operation-consistent-nan.js")]
    public Task conversion_operation_consistent_nan() => ExecutionTestFromFile("conversion-operation-consistent-nan");

    [Fact(DisplayName = "conversion-operation.js")]
    public Task conversion_operation() => ExecutionTestFromFile("conversion-operation");

    [Fact(DisplayName = "indexed-value.js")]
    public Task indexed_value() => ExecutionTestFromFile("indexed-value");

    [Fact(DisplayName = "key-is-canonical-invalid-index-reflect-set.js")]
    public Task key_is_canonical_invalid_index_reflect_set() => ExecutionTestFromFile("key-is-canonical-invalid-index-reflect-set");

    [Fact(DisplayName = "key-is-minus-zero.js")]
    public Task key_is_minus_zero() => ExecutionTestFromFile("key-is-minus-zero");

    [Fact(DisplayName = "key-is-not-canonical-index.js")]
    public Task key_is_not_canonical_index() => ExecutionTestFromFile("key-is-not-canonical-index");

    [Fact(DisplayName = "key-is-not-integer.js")]
    public Task key_is_not_integer() => ExecutionTestFromFile("key-is-not-integer");

    [Fact(DisplayName = "key-is-not-numeric-index-set-throws.js")]
    public Task key_is_not_numeric_index_set_throws() => ExecutionTestFromFile("key-is-not-numeric-index-set-throws");

    [Fact(DisplayName = "key-is-not-numeric-index.js")]
    public Task key_is_not_numeric_index() => ExecutionTestFromFile("key-is-not-numeric-index");

    [Fact(DisplayName = "key-is-out-of-bounds-receiver-is-not-object.js")]
    public Task key_is_out_of_bounds_receiver_is_not_object() => ExecutionTestFromFile("key-is-out-of-bounds-receiver-is-not-object");

    [Fact(DisplayName = "key-is-out-of-bounds-receiver-is-not-typed-array.js")]
    public Task key_is_out_of_bounds_receiver_is_not_typed_array() => ExecutionTestFromFile("key-is-out-of-bounds-receiver-is-not-typed-array");

    [Fact(DisplayName = "key-is-out-of-bounds.js")]
    public Task key_is_out_of_bounds() => ExecutionTestFromFile("key-is-out-of-bounds");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");

    [Fact(DisplayName = "resized-out-of-bounds-to-in-bounds-index.js")]
    public Task resized_out_of_bounds_to_in_bounds_index() => ExecutionTestFromFile("resized-out-of-bounds-to-in-bounds-index");
}
