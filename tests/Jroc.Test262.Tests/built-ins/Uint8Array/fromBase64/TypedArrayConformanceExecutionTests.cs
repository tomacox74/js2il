using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.fromBase64;

public sealed class TypedArrayConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConformanceExecutionTests() : base("TypedArray.Conformance") { }

    [Fact(DisplayName = "alphabet.js")]
    public Task alphabet()
        => ExecutionTestFromFile("alphabet");

    [Fact(DisplayName = "last-chunk-handling.js")]
    public Task last_chunk_handling()
        => ExecutionTestFromFile("last-chunk-handling");

    [Fact(DisplayName = "last-chunk-invalid.js")]
    public Task last_chunk_invalid()
        => ExecutionTestFromFile("last-chunk-invalid");

    [Fact(DisplayName = "option-coercion.js")]
    public Task option_coercion()
        => ExecutionTestFromFile("option-coercion");

    [Fact(DisplayName = "whitespace.js")]
    public Task whitespace()
        => ExecutionTestFromFile("whitespace");

}
