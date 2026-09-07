using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.filter;

public class SpeciesExecutionTests : InMemoryExecutionTestsBase
{
    public SpeciesExecutionTests() : base("built_ins.Array.prototype.filter") { }

    [Fact(DisplayName = "create-ctor-non-object.js")]
    public Task create_ctor_non_object()
        => ExecutionTestFromFile("create-ctor-non-object");

    [Fact(DisplayName = "create-ctor-poisoned.js")]
    public Task create_ctor_poisoned()
        => ExecutionTestFromFile("create-ctor-poisoned");

    [Fact(DisplayName = "create-species-abrupt.js")]
    public Task create_species_abrupt()
        => ExecutionTestFromFile("create-species-abrupt");

    [Fact(DisplayName = "create-species-non-ctor.js")]
    public Task create_species_non_ctor()
        => ExecutionTestFromFile("create-species-non-ctor");

    [Fact(DisplayName = "create-species-poisoned.js")]
    public Task create_species_poisoned()
        => ExecutionTestFromFile("create-species-poisoned");

    [Fact(DisplayName = "create-species.js")]
    public Task create_species()
        => ExecutionTestFromFile("create-species");

    [Fact(DisplayName = "target-array-non-extensible.js")]
    public Task target_array_non_extensible()
        => ExecutionTestFromFile("target-array-non-extensible");

    [Fact(DisplayName = "target-array-with-non-configurable-property.js")]
    public Task target_array_with_non_configurable_property()
        => ExecutionTestFromFile("target-array-with-non-configurable-property");

    [Fact(DisplayName = "15.4.4.20-5-10.js")]
    public Task _15_4_4_20_5_10()
        => ExecutionTestFromFile("15.4.4.20-5-10");

    [Fact(DisplayName = "15.4.4.20-5-11.js")]
    public Task _15_4_4_20_5_11()
        => ExecutionTestFromFile("15.4.4.20-5-11");
}
