using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.Symbol.species;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.Symbol.species") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "return-value")]
    public Task return_value()
        => ExecutionTestFromFile("return-value");

    [Fact(DisplayName = "symbol-species-name")]
    public Task symbol_species_name()
        => ExecutionTestFromFile("symbol-species-name");

    [Fact(DisplayName = "symbol-species")]
    public Task symbol_species()
        => ExecutionTestFromFile("symbol-species");
}
