using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.map;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.map") { }

    [Fact(DisplayName = "speciesctor-destination-resizable")]
    public Task speciesctor_destination_resizable()
        => ExecutionTestFromFile("speciesctor-destination-resizable");

    [Fact(DisplayName = "speciesctor-get-ctor-returns-throws")]
    public Task speciesctor_get_ctor_returns_throws()
        => ExecutionTestFromFile("speciesctor-get-ctor-returns-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-invocation")]
    public Task speciesctor_get_species_custom_ctor_invocation()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-invocation");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer")]
    public Task speciesctor_get_species_custom_ctor_length_throws_resizable_arraybuffer()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws")]
    public Task speciesctor_get_species_custom_ctor_length_throws()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length")]
    public Task speciesctor_get_species_custom_ctor_length()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-returns-another-instance")]
    public Task speciesctor_get_species_custom_ctor_returns_another_instance()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-returns-another-instance");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-throws")]
    public Task speciesctor_get_species_custom_ctor_throws()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor")]
    public Task speciesctor_get_species_custom_ctor()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor");

    [Fact(DisplayName = "speciesctor-get-species-returns-throws")]
    public Task speciesctor_get_species_returns_throws()
        => ExecutionTestFromFile("speciesctor-get-species-returns-throws");

}
