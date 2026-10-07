using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.setFromBase64;

public sealed class TypedArrayConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConformanceExecutionTests() : base("TypedArray.Conformance") { }

    [Fact(DisplayName = "alphabet.js")]
    public Task alphabet()
        => ExecutionTestFromFile("alphabet");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer()
        => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "last-chunk-handling.js")]
    public Task last_chunk_handling()
        => ExecutionTestFromFile("last-chunk-handling");

    [Fact(DisplayName = "option-coercion.js")]
    public Task option_coercion()
        => ExecutionTestFromFile("option-coercion");

    [Fact(DisplayName = "subarray.js")]
    public Task subarray()
        => ExecutionTestFromFile("subarray");

    [Fact(DisplayName = "target-size.js")]
    public Task target_size()
        => ExecutionTestFromFile("target-size");

    [Fact(DisplayName = "trailing-garbage-empty.js")]
    public Task trailing_garbage_empty()
        => ExecutionTestFromFile("trailing-garbage-empty");

    [Fact(DisplayName = "trailing-garbage.js")]
    public Task trailing_garbage()
        => ExecutionTestFromFile("trailing-garbage");

    [Fact(DisplayName = "whitespace.js")]
    public Task whitespace()
        => ExecutionTestFromFile("whitespace");

    [Fact(DisplayName = "writes-up-to-error.js")]
    public Task writes_up_to_error()
        => ExecutionTestFromFile("writes-up-to-error");

}
