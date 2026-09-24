using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArrayConstructors.internals.Set") { }

    [Fact(DisplayName = "detached-buffer-key-is-not-numeric-index.js")]
    public Task detached_buffer_key_is_not_numeric_index() => ExecutionTestFromFile("detached-buffer-key-is-not-numeric-index");

    [Fact(DisplayName = "detached-buffer-key-is-symbol.js")]
    public Task detached_buffer_key_is_symbol() => ExecutionTestFromFile("detached-buffer-key-is-symbol");

    [Fact(DisplayName = "detached-buffer-realm.js")]
    public Task detached_buffer_realm() => ExecutionTestFromFile("detached-buffer-realm");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "key-is-canonical-invalid-index-prototype-chain-set.js")]
    public Task key_is_canonical_invalid_index_prototype_chain_set() => ExecutionTestFromFile("key-is-canonical-invalid-index-prototype-chain-set");

    [Fact(DisplayName = "key-is-in-bounds-receiver-is-not-typed-array.js")]
    public Task key_is_in_bounds_receiver_is_not_typed_array() => ExecutionTestFromFile("key-is-in-bounds-receiver-is-not-typed-array");

    [Fact(DisplayName = "key-is-out-of-bounds-receiver-is-proto.js")]
    public Task key_is_out_of_bounds_receiver_is_proto() => ExecutionTestFromFile("key-is-out-of-bounds-receiver-is-proto");

    [Fact(DisplayName = "key-is-valid-index-prototype-chain-set.js")]
    public Task key_is_valid_index_prototype_chain_set() => ExecutionTestFromFile("key-is-valid-index-prototype-chain-set");

    [Fact(DisplayName = "key-is-valid-index-reflect-set.js")]
    public Task key_is_valid_index_reflect_set() => ExecutionTestFromFile("key-is-valid-index-reflect-set");

    [Fact(DisplayName = "tonumber-value-detached-buffer.js")]
    public Task tonumber_value_detached_buffer() => ExecutionTestFromFile("tonumber-value-detached-buffer");

    [Fact(DisplayName = "tonumber-value-throws.js")]
    public Task tonumber_value_throws() => ExecutionTestFromFile("tonumber-value-throws");
}
