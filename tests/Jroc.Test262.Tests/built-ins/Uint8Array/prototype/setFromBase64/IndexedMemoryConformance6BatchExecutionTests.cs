using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.setFromBase64;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Uint8Array.prototype.setFromBase64") { }

    [Fact(DisplayName = "illegal-characters.js")]
    public Task illegal_characters() => ExecutionTestFromFile("illegal-characters");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nonconstructor.js")]
    public Task nonconstructor() => ExecutionTestFromFile("nonconstructor");

    [Fact(DisplayName = "string-coercion.js")]
    public Task string_coercion() => ExecutionTestFromFile("string-coercion");
}
