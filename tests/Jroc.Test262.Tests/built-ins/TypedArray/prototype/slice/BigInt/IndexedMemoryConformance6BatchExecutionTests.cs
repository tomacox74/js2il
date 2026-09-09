using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.slice.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.slice.BigInt") { }

    [Fact(DisplayName = "return-abrupt-from-end-symbol.js")]
    public Task return_abrupt_from_end_symbol() => ExecutionTestFromFile("return-abrupt-from-end-symbol");

    [Fact(DisplayName = "return-abrupt-from-start-symbol.js")]
    public Task return_abrupt_from_start_symbol() => ExecutionTestFromFile("return-abrupt-from-start-symbol");

    [Fact(DisplayName = "speciesctor-destination-resizable.js")]
    public Task speciesctor_destination_resizable() => ExecutionTestFromFile("speciesctor-destination-resizable");

    [Fact(DisplayName = "speciesctor-get-ctor-returns-throws.js")]
    public Task speciesctor_get_ctor_returns_throws() => ExecutionTestFromFile("speciesctor-get-ctor-returns-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer.js")]
    public Task speciesctor_get_species_custom_ctor_length_throws_resizable_arraybuffer() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws.js")]
    public Task speciesctor_get_species_custom_ctor_length_throws() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-throws.js")]
    public Task speciesctor_get_species_custom_ctor_throws() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-throws");

    [Fact(DisplayName = "speciesctor-get-species-returns-throws.js")]
    public Task speciesctor_get_species_returns_throws() => ExecutionTestFromFile("speciesctor-get-species-returns-throws");
}
