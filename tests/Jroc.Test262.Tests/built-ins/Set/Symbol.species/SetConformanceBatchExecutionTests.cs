using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.Symbol.species;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.Symbol.species") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "symbol-species-name.js")]
    public Task symbol_species_name() => ExecutionTestFromFile("symbol-species-name");

    [Fact(DisplayName = "symbol-species.js")]
    public Task symbol_species() => ExecutionTestFromFile("symbol-species");

}
