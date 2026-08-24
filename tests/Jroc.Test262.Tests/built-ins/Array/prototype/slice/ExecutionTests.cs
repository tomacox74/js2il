using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.slice;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.slice") { }

    [Fact(DisplayName = "15.4.4.10-10-c-ii-1")]
    public Task _15_4_4_10_10_c_ii_1()
        => ExecutionTestFromFile("15.4.4.10-10-c-ii-1");

    [Fact(DisplayName = "S15.4.4.10_A2.1_T5")]
    public Task S15_4_4_10_A2_1_T5()
        => ExecutionTestFromFile("S15.4.4.10_A2.1_T5");

    [Fact(DisplayName = "S15.4.4.10_A2.2_T5")]
    public Task S15_4_4_10_A2_2_T5()
        => ExecutionTestFromFile("S15.4.4.10_A2.2_T5");

    [Fact(DisplayName = "S15.4.4.10_A2_T1")]
    public Task S15_4_4_10_A2_T1()
        => ExecutionTestFromFile("S15.4.4.10_A2_T1");

    [Fact(DisplayName = "S15.4.4.10_A2_T2")]
    public Task S15_4_4_10_A2_T2()
        => ExecutionTestFromFile("S15.4.4.10_A2_T2");

    [Fact(DisplayName = "S15.4.4.10_A2_T3")]
    public Task S15_4_4_10_A2_T3()
        => ExecutionTestFromFile("S15.4.4.10_A2_T3");

    [Fact(DisplayName = "S15.4.4.10_A2_T4")]
    public Task S15_4_4_10_A2_T4()
        => ExecutionTestFromFile("S15.4.4.10_A2_T4");

    [Fact(DisplayName = "S15.4.4.10_A2_T5")]
    public Task S15_4_4_10_A2_T5()
        => ExecutionTestFromFile("S15.4.4.10_A2_T5");

    [Fact(DisplayName = "S15.4.4.10_A2_T6")]
    public Task S15_4_4_10_A2_T6()
        => ExecutionTestFromFile("S15.4.4.10_A2_T6");

    [Fact(DisplayName = "create-ctor-non-object")]
    public Task create_ctor_non_object()
        => ExecutionTestFromFile("create-ctor-non-object");

    [Fact(DisplayName = "create-ctor-poisoned")]
    public Task create_ctor_poisoned()
        => ExecutionTestFromFile("create-ctor-poisoned");

    [Fact(DisplayName = "create-species-abrupt")]
    public Task create_species_abrupt()
        => ExecutionTestFromFile("create-species-abrupt");

    [Fact(DisplayName = "create-species-neg-zero")]
    public Task create_species_neg_zero()
        => ExecutionTestFromFile("create-species-neg-zero");

    [Fact(DisplayName = "create-species-non-ctor")]
    public Task create_species_non_ctor()
        => ExecutionTestFromFile("create-species-non-ctor");

    [Fact(DisplayName = "create-species-poisoned")]
    public Task create_species_poisoned()
        => ExecutionTestFromFile("create-species-poisoned");

    [Fact(DisplayName = "create-species")]
    public Task create_species()
        => ExecutionTestFromFile("create-species");

    [Fact(DisplayName = "length-exceeding-integer-limit")]
    public Task length_exceeding_integer_limit()
        => ExecutionTestFromFile("length-exceeding-integer-limit");

    [Fact(DisplayName = "target-array-non-extensible")]
    public Task target_array_non_extensible()
        => ExecutionTestFromFile("target-array-non-extensible");

    [Fact(DisplayName = "target-array-with-non-configurable-property")]
    public Task target_array_with_non_configurable_property()
        => ExecutionTestFromFile("target-array-with-non-configurable-property");
}
