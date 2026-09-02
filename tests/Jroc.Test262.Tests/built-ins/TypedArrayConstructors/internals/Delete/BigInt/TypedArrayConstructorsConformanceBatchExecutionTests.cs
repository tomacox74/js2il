using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Delete.BigInt;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.Delete.BigInt") { }

    [Fact(DisplayName = "indexed-value-ab-non-strict.js")]
    public Task indexed_value_ab_non_strict() => ExecutionTestFromFile("indexed-value-ab-non-strict");

    [Fact(DisplayName = "indexed-value-ab-strict.js")]
    public Task indexed_value_ab_strict() => ExecutionTestFromFile("indexed-value-ab-strict");

    [Fact(DisplayName = "indexed-value-sab-non-strict.js")]
    public Task indexed_value_sab_non_strict() => ExecutionTestFromFile("indexed-value-sab-non-strict");

    [Fact(DisplayName = "indexed-value-sab-strict.js")]
    public Task indexed_value_sab_strict() => ExecutionTestFromFile("indexed-value-sab-strict");

    [Fact(DisplayName = "key-is-not-canonical-index-non-strict.js")]
    public Task key_is_not_canonical_index_non_strict() => ExecutionTestFromFile("key-is-not-canonical-index-non-strict");

    [Fact(DisplayName = "key-is-not-canonical-index-strict.js")]
    public Task key_is_not_canonical_index_strict() => ExecutionTestFromFile("key-is-not-canonical-index-strict");

    [Fact(DisplayName = "key-is-not-minus-zero-non-strict.js")]
    public Task key_is_not_minus_zero_non_strict() => ExecutionTestFromFile("key-is-not-minus-zero-non-strict");

    [Fact(DisplayName = "key-is-not-minus-zero-strict.js")]
    public Task key_is_not_minus_zero_strict() => ExecutionTestFromFile("key-is-not-minus-zero-strict");

    [Fact(DisplayName = "key-is-not-numeric-index-get-throws.js")]
    public Task key_is_not_numeric_index_get_throws() => ExecutionTestFromFile("key-is-not-numeric-index-get-throws");

    [Fact(DisplayName = "key-is-not-numeric-index-non-strict.js")]
    public Task key_is_not_numeric_index_non_strict() => ExecutionTestFromFile("key-is-not-numeric-index-non-strict");

    [Fact(DisplayName = "key-is-not-numeric-index-strict.js")]
    public Task key_is_not_numeric_index_strict() => ExecutionTestFromFile("key-is-not-numeric-index-strict");

    [Fact(DisplayName = "key-is-out-of-bounds-non-strict.js")]
    public Task key_is_out_of_bounds_non_strict() => ExecutionTestFromFile("key-is-out-of-bounds-non-strict");

    [Fact(DisplayName = "key-is-out-of-bounds-strict.js")]
    public Task key_is_out_of_bounds_strict() => ExecutionTestFromFile("key-is-out-of-bounds-strict");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");

}
