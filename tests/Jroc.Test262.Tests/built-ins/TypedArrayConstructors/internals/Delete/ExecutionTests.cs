using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArrayConstructors.internals.Delete") { }

    [Fact(DisplayName = "detached-buffer-key-is-not-numeric-index.js")]
    public Task detached_buffer_key_is_not_numeric_index() => ExecutionTestFromFile("detached-buffer-key-is-not-numeric-index");

    [Fact(DisplayName = "detached-buffer-key-is-symbol.js")]
    public Task detached_buffer_key_is_symbol() => ExecutionTestFromFile("detached-buffer-key-is-symbol");

    [Fact(DisplayName = "detached-buffer-realm.js")]
    public Task detached_buffer_realm() => ExecutionTestFromFile("detached-buffer-realm");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "infinity-detached-buffer.js")]
    public Task infinity_detached_buffer() => ExecutionTestFromFile("infinity-detached-buffer");
}
