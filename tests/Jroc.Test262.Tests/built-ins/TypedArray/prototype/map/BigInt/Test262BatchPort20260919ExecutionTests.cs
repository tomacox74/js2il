using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.map.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.map.BigInt") { }

    [Fact(DisplayName = "speciesctor-get-ctor-returns-throws")]
    public Task speciesctor_get_ctor_returns_throws()
        => ExecutionTestFromFile("speciesctor-get-ctor-returns-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-invocation")]
    public Task speciesctor_get_species_custom_ctor_invocation()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-invocation");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length")]
    public Task speciesctor_get_species_custom_ctor_length()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-returns-another-instance")]
    public Task speciesctor_get_species_custom_ctor_returns_another_instance()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-returns-another-instance");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor")]
    public Task speciesctor_get_species_custom_ctor()
        => ExecutionTestFromFile("speciesctor-get-species-custom-ctor");

}
