using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.Symbol_species;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.Symbol.species") { }

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "return-value.js")]
    public Task return_value()
        => ExecutionTestFromFile("return-value");

    [Fact(DisplayName = "symbol-species-name.js")]
    public Task symbol_species_name()
        => ExecutionTestFromFile("symbol-species-name");

    [Fact(DisplayName = "symbol-species.js")]
    public Task symbol_species()
        => ExecutionTestFromFile("symbol-species");
}
