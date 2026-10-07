using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.Symbol_species;

public sealed class RegExpMetadataExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpMetadataExecutionTests() : base("RegExp.Metadata") { }

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "symbol-species-name.js")]
    public Task symbol_species_name()
        => ExecutionTestFromFile("symbol-species-name");

}
