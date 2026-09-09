using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Set.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.Set.BigInt") { }

    [Fact(DisplayName = "bigint-tobigint64.js")]
    public Task bigint_tobigint64() => ExecutionTestFromFile("bigint-tobigint64");

    [Fact(DisplayName = "bigint-tobiguint64.js")]
    public Task bigint_tobiguint64() => ExecutionTestFromFile("bigint-tobiguint64");

    [Fact(DisplayName = "boolean-tobigint.js")]
    public Task boolean_tobigint() => ExecutionTestFromFile("boolean-tobigint");

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

    [Fact(DisplayName = "key-is-out-of-bounds.js")]
    public Task key_is_out_of_bounds() => ExecutionTestFromFile("key-is-out-of-bounds");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");

    [Fact(DisplayName = "null-tobigint.js")]
    public Task null_tobigint() => ExecutionTestFromFile("null-tobigint");

    [Fact(DisplayName = "number-tobigint.js")]
    public Task number_tobigint() => ExecutionTestFromFile("number-tobigint");

    [Fact(DisplayName = "string-nan-tobigint.js")]
    public Task string_nan_tobigint() => ExecutionTestFromFile("string-nan-tobigint");

    [Fact(DisplayName = "string-tobigint.js")]
    public Task string_tobigint() => ExecutionTestFromFile("string-tobigint");

    [Fact(DisplayName = "symbol-tobigint.js")]
    public Task symbol_tobigint() => ExecutionTestFromFile("symbol-tobigint");

    [Fact(DisplayName = "undefined-tobigint.js")]
    public Task undefined_tobigint() => ExecutionTestFromFile("undefined-tobigint");
}
