using Js2IL.Tests;

namespace Js2IL.Test262.Tests.Integration;

public class GeneratorTests : GeneratorTestsBase
{
    public GeneratorTests() : base("Integration") { }

    [Fact]
    public Task Compile_Scripts_Test262Bootstrap() => GenerateTest(nameof(Compile_Scripts_Test262Bootstrap));

    [Fact]
    public Task Compile_Scripts_Test262MetadataParser()
        => GenerateTest(nameof(Compile_Scripts_Test262MetadataParser), ["test262/metadataParser"]);
}
