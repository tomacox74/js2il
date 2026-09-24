using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.DefineOwnProperty.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArrayConstructors.internals.DefineOwnProperty.BigInt") { }

    [Fact(DisplayName = "detached-buffer-throws-realm.js")]
    public Task detached_buffer_throws_realm() => ExecutionTestFromFile("detached-buffer-throws-realm");

    [Fact(DisplayName = "detached-buffer-throws.js")]
    public Task detached_buffer_throws() => ExecutionTestFromFile("detached-buffer-throws");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "key-is-lower-than-zero.js")]
    public Task key_is_lower_than_zero() => ExecutionTestFromFile("key-is-lower-than-zero");

    [Fact(DisplayName = "key-is-minus-zero.js")]
    public Task key_is_minus_zero() => ExecutionTestFromFile("key-is-minus-zero");

    [Fact(DisplayName = "key-is-not-integer.js")]
    public Task key_is_not_integer() => ExecutionTestFromFile("key-is-not-integer");

    [Fact(DisplayName = "key-is-not-numeric-index-throws.js")]
    public Task key_is_not_numeric_index_throws() => ExecutionTestFromFile("key-is-not-numeric-index-throws");

    [Fact(DisplayName = "tonumber-value-detached-buffer.js")]
    public Task tonumber_value_detached_buffer() => ExecutionTestFromFile("tonumber-value-detached-buffer");
}
