using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.subarray.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.subarray.BigInt") { }

    [Fact(DisplayName = "return-abrupt-from-begin-symbol.js")]
    public Task return_abrupt_from_begin_symbol() => ExecutionTestFromFile("return-abrupt-from-begin-symbol");

    [Fact(DisplayName = "return-abrupt-from-end-symbol.js")]
    public Task return_abrupt_from_end_symbol() => ExecutionTestFromFile("return-abrupt-from-end-symbol");

    [Fact(DisplayName = "speciesctor-get-ctor-returns-throws.js")]
    public Task speciesctor_get_ctor_returns_throws() => ExecutionTestFromFile("speciesctor-get-ctor-returns-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-throws.js")]
    public Task speciesctor_get_species_custom_ctor_throws() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-throws");

    [Fact(DisplayName = "speciesctor-get-species-returns-throws.js")]
    public Task speciesctor_get_species_returns_throws() => ExecutionTestFromFile("speciesctor-get-species-returns-throws");
}
