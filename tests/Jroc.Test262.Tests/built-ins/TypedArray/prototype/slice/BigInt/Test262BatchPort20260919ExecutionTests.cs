using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.slice.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.slice.BigInt") { }

    [Fact(DisplayName = "arraylength-internal")]
    public Task arraylength_internal()
        => ExecutionTestFromFile("arraylength-internal");

    [Fact(DisplayName = "infinity")]
    public Task infinity()
        => ExecutionTestFromFile("infinity");

    [Fact(DisplayName = "minus-zero")]
    public Task minus_zero()
        => ExecutionTestFromFile("minus-zero");

    [Fact(DisplayName = "result-does-not-copy-ordinary-properties")]
    public Task result_does_not_copy_ordinary_properties()
        => ExecutionTestFromFile("result-does-not-copy-ordinary-properties");

    [Fact(DisplayName = "results-with-different-length")]
    public Task results_with_different_length()
        => ExecutionTestFromFile("results-with-different-length");

    [Fact(DisplayName = "results-with-empty-length")]
    public Task results_with_empty_length()
        => ExecutionTestFromFile("results-with-empty-length");

    [Fact(DisplayName = "results-with-same-length")]
    public Task results_with_same_length()
        => ExecutionTestFromFile("results-with-same-length");

    [Fact(DisplayName = "return-abrupt-from-end")]
    public Task return_abrupt_from_end()
        => ExecutionTestFromFile("return-abrupt-from-end");

    [Fact(DisplayName = "return-abrupt-from-start")]
    public Task return_abrupt_from_start()
        => ExecutionTestFromFile("return-abrupt-from-start");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

    [Fact(DisplayName = "set-values-from-different-ctor-type")]
    public Task set_values_from_different_ctor_type()
        => ExecutionTestFromFile("set-values-from-different-ctor-type");

    [Fact(DisplayName = "speciesctor-get-ctor-abrupt")]
    public Task speciesctor_get_ctor_abrupt()
        => ExecutionTestFromFile("speciesctor-get-ctor-abrupt");

    [Fact(DisplayName = "speciesctor-get-ctor-inherited")]
    public Task speciesctor_get_ctor_inherited()
        => ExecutionTestFromFile("speciesctor-get-ctor-inherited");

    [Fact(DisplayName = "speciesctor-get-ctor")]
    public Task speciesctor_get_ctor()
        => ExecutionTestFromFile("speciesctor-get-ctor");

    [Fact(DisplayName = "speciesctor-get-species-abrupt")]
    public Task speciesctor_get_species_abrupt()
        => ExecutionTestFromFile("speciesctor-get-species-abrupt");

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

    [Fact(DisplayName = "speciesctor-get-species-use-default-ctor")]
    public Task speciesctor_get_species_use_default_ctor()
        => ExecutionTestFromFile("speciesctor-get-species-use-default-ctor");

    [Fact(DisplayName = "speciesctor-get-species")]
    public Task speciesctor_get_species()
        => ExecutionTestFromFile("speciesctor-get-species");

    [Fact(DisplayName = "tointeger-end")]
    public Task tointeger_end()
        => ExecutionTestFromFile("tointeger-end");

    [Fact(DisplayName = "tointeger-start")]
    public Task tointeger_start()
        => ExecutionTestFromFile("tointeger-start");

}
