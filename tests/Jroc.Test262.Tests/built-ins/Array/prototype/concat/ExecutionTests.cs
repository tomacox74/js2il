using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.concat;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.concat") { }

    [Fact(DisplayName = "S15.4.4.4_A2_T1")]
    public Task S15_4_4_4_A2_T1()
        => ExecutionTestFromFile("S15.4.4.4_A2_T1");

    [Fact(DisplayName = "S15.4.4.4_A2_T2")]
    public Task S15_4_4_4_A2_T2()
        => ExecutionTestFromFile("S15.4.4.4_A2_T2");

    [Fact(DisplayName = "S15.4.4.4_A3_T1")]
    public Task S15_4_4_4_A3_T1()
        => ExecutionTestFromFile("S15.4.4.4_A3_T1");

    [Fact(DisplayName = "Array.prototype.concat_non-array")]
    public Task Array_prototype_concat_non_array()
        => ExecutionTestFromFile("Array.prototype.concat_non-array");

    [Fact(DisplayName = "Array.prototype.concat_array-like")]
    public Task Array_prototype_concat_array_like()
        => ExecutionTestFromFile("Array.prototype.concat_array-like");

    [Fact(DisplayName = "Array.prototype.concat_array-like-string-length")]
    public Task Array_prototype_concat_array_like_string_length()
        => ExecutionTestFromFile("Array.prototype.concat_array-like-string-length");

    [Fact(DisplayName = "Array.prototype.concat_no-prototype")]
    public Task Array_prototype_concat_no_prototype()
        => ExecutionTestFromFile("Array.prototype.concat_no-prototype");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "create-non-array")]
    public Task create_non_array()
        => ExecutionTestFromFile("create-non-array");

    [Fact(DisplayName = "15.4.4.4-5-b-iii-3-b-1")]
    public Task _15_4_4_4_5_b_iii_3_b_1()
        => ExecutionTestFromFile("15.4.4.4-5-b-iii-3-b-1");

    [Fact(DisplayName = "15.4.4.4-5-c-i-1")]
    public Task _15_4_4_4_5_c_i_1()
        => ExecutionTestFromFile("15.4.4.4-5-c-i-1");

    [Fact(DisplayName = "Array.prototype.concat_length-throws")]
    public Task Array_prototype_concat_length_throws()
        => ExecutionTestFromFile("Array.prototype.concat_length-throws");

    [Fact(DisplayName = "Array.prototype.concat_sloppy-arguments-throws")]
    public Task Array_prototype_concat_sloppy_arguments_throws()
        => ExecutionTestFromFile("Array.prototype.concat_sloppy-arguments-throws");

    [Fact(DisplayName = "Array.prototype.concat_spreadable-getter-throws")]
    public Task Array_prototype_concat_spreadable_getter_throws()
        => ExecutionTestFromFile("Array.prototype.concat_spreadable-getter-throws");

    [Fact(DisplayName = "create-ctor-non-object")]
    public Task create_ctor_non_object()
        => ExecutionTestFromFile("create-ctor-non-object");

    [Fact(DisplayName = "create-ctor-poisoned")]
    public Task create_ctor_poisoned()
        => ExecutionTestFromFile("create-ctor-poisoned");

    [Fact(DisplayName = "create-species-abrupt")]
    public Task create_species_abrupt()
        => ExecutionTestFromFile("create-species-abrupt");

    [Fact(DisplayName = "create-species-non-ctor")]
    public Task create_species_non_ctor()
        => ExecutionTestFromFile("create-species-non-ctor");

    [Fact(DisplayName = "create-species-non-extensible")]
    public Task create_species_non_extensible()
        => ExecutionTestFromFile("create-species-non-extensible");

    [Fact(DisplayName = "create-species-non-extensible-spreadable")]
    public Task create_species_non_extensible_spreadable()
        => ExecutionTestFromFile("create-species-non-extensible-spreadable");

    [Fact(DisplayName = "S15.4.4.4_A3_T2")]
    public Task S15_4_4_4_A3_T2()
        => ExecutionTestFromFile("S15.4.4.4_A3_T2");

    [Fact(DisplayName = "S15.4.4.4_A3_T3")]
    public Task S15_4_4_4_A3_T3()
        => ExecutionTestFromFile("S15.4.4.4_A3_T3");
}
