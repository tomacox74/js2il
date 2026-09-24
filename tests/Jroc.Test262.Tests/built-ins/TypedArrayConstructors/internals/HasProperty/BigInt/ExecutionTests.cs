using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.HasProperty.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArrayConstructors.internals.HasProperty.BigInt") { }

    [Fact(DisplayName = "abrupt-from-ordinary-has-parent-hasproperty.js")]
    public Task abrupt_from_ordinary_has_parent_hasproperty() => ExecutionTestFromFile("abrupt-from-ordinary-has-parent-hasproperty");

    [Fact(DisplayName = "detached-buffer-key-is-not-number.js")]
    public Task detached_buffer_key_is_not_number() => ExecutionTestFromFile("detached-buffer-key-is-not-number");

    [Fact(DisplayName = "detached-buffer-key-is-symbol.js")]
    public Task detached_buffer_key_is_symbol() => ExecutionTestFromFile("detached-buffer-key-is-symbol");

    [Fact(DisplayName = "detached-buffer-realm.js")]
    public Task detached_buffer_realm() => ExecutionTestFromFile("detached-buffer-realm");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "infinity-with-detached-buffer.js")]
    public Task infinity_with_detached_buffer() => ExecutionTestFromFile("infinity-with-detached-buffer");

    [Fact(DisplayName = "inherited-property.js")]
    public Task inherited_property() => ExecutionTestFromFile("inherited-property");

    [Fact(DisplayName = "key-is-greater-than-last-index.js")]
    public Task key_is_greater_than_last_index() => ExecutionTestFromFile("key-is-greater-than-last-index");

    [Fact(DisplayName = "key-is-lower-than-zero.js")]
    public Task key_is_lower_than_zero() => ExecutionTestFromFile("key-is-lower-than-zero");

    [Fact(DisplayName = "key-is-minus-zero.js")]
    public Task key_is_minus_zero() => ExecutionTestFromFile("key-is-minus-zero");

    [Fact(DisplayName = "key-is-not-integer.js")]
    public Task key_is_not_integer() => ExecutionTestFromFile("key-is-not-integer");
}
