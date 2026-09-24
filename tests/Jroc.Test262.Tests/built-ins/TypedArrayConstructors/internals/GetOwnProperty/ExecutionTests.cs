using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.GetOwnProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArrayConstructors.internals.GetOwnProperty") { }

    [Fact(DisplayName = "detached-buffer-key-is-not-number.js")]
    public Task detached_buffer_key_is_not_number() => ExecutionTestFromFile("detached-buffer-key-is-not-number");

    [Fact(DisplayName = "detached-buffer-key-is-symbol.js")]
    public Task detached_buffer_key_is_symbol() => ExecutionTestFromFile("detached-buffer-key-is-symbol");

    [Fact(DisplayName = "detached-buffer-realm.js")]
    public Task detached_buffer_realm() => ExecutionTestFromFile("detached-buffer-realm");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "enumerate-detached-buffer.js")]
    public Task enumerate_detached_buffer() => ExecutionTestFromFile("enumerate-detached-buffer");
}
