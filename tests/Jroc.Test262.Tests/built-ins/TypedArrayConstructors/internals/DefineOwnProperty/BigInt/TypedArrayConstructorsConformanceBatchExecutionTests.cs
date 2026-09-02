using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.DefineOwnProperty.BigInt;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.DefineOwnProperty.BigInt") { }

    [Fact(DisplayName = "key-is-not-canonical-index.js")]
    public Task key_is_not_canonical_index() => ExecutionTestFromFile("key-is-not-canonical-index");

    [Fact(DisplayName = "key-is-not-numeric-index.js")]
    public Task key_is_not_numeric_index() => ExecutionTestFromFile("key-is-not-numeric-index");

    [Fact(DisplayName = "key-is-numericindex-accessor-desc-throws.js")]
    public Task key_is_numericindex_accessor_desc_throws() => ExecutionTestFromFile("key-is-numericindex-accessor-desc-throws");

    [Fact(DisplayName = "key-is-numericindex-accessor-desc.js")]
    public Task key_is_numericindex_accessor_desc() => ExecutionTestFromFile("key-is-numericindex-accessor-desc");

    [Fact(DisplayName = "key-is-numericindex-desc-not-enumerable-throws.js")]
    public Task key_is_numericindex_desc_not_enumerable_throws() => ExecutionTestFromFile("key-is-numericindex-desc-not-enumerable-throws");

    [Fact(DisplayName = "key-is-numericindex-desc-not-enumerable.js")]
    public Task key_is_numericindex_desc_not_enumerable() => ExecutionTestFromFile("key-is-numericindex-desc-not-enumerable");

    [Fact(DisplayName = "key-is-numericindex-desc-not-writable-throws.js")]
    public Task key_is_numericindex_desc_not_writable_throws() => ExecutionTestFromFile("key-is-numericindex-desc-not-writable-throws");

    [Fact(DisplayName = "key-is-numericindex-desc-not-writable.js")]
    public Task key_is_numericindex_desc_not_writable() => ExecutionTestFromFile("key-is-numericindex-desc-not-writable");

    [Fact(DisplayName = "key-is-symbol.js")]
    public Task key_is_symbol() => ExecutionTestFromFile("key-is-symbol");

    [Fact(DisplayName = "non-extensible-new-key.js")]
    public Task non_extensible_new_key() => ExecutionTestFromFile("non-extensible-new-key");

    [Fact(DisplayName = "non-extensible-redefine-key.js")]
    public Task non_extensible_redefine_key() => ExecutionTestFromFile("non-extensible-redefine-key");

    [Fact(DisplayName = "this-is-not-extensible.js")]
    public Task this_is_not_extensible() => ExecutionTestFromFile("this-is-not-extensible");

}
