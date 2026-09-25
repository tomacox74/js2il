using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer") { }

    [Fact(DisplayName = "allocation-limit")]
    public Task allocation_limit()
        => ExecutionTest("allocation-limit");

    [Fact(DisplayName = "init-zero")]
    public Task init_zero()
        => ExecutionTest("init-zero");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTest("is-a-constructor");

    [Fact(DisplayName = "length-is-absent")]
    public Task length_is_absent()
        => ExecutionTest("length-is-absent");

    [Fact(DisplayName = "length-is-too-large-throws")]
    public Task length_is_too_large_throws()
        => ExecutionTest("length-is-too-large-throws");

    [Fact(DisplayName = "negative-length-throws")]
    public Task negative_length_throws()
        => ExecutionTest("negative-length-throws");

    [Fact(DisplayName = "return-abrupt-from-length")]
    public Task return_abrupt_from_length()
        => ExecutionTest("return-abrupt-from-length");

    [Fact(DisplayName = "zero-length")]
    public Task zero_length()
        => ExecutionTest("zero-length");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "newtarget-prototype-is-not-object.js")]
    public Task newtarget_prototype_is_not_object()
        => ExecutionTest("newtarget-prototype-is-not-object");

    [Fact(DisplayName = "options-maxbytelength-allocation-limit.js")]
    public Task ported_options_maxbytelength_allocation_limit() => ExecutionTest("options-maxbytelength-allocation-limit");

    [Fact(DisplayName = "options-maxbytelength-compared-before-object-creation.js")]
    public Task ported_options_maxbytelength_compared_before_object_creation() => ExecutionTest("options-maxbytelength-compared-before-object-creation");

    [Fact(DisplayName = "options-maxbytelength-diminuitive.js")]
    public Task ported_options_maxbytelength_diminuitive() => ExecutionTest("options-maxbytelength-diminuitive");

    [Fact(DisplayName = "options-maxbytelength-excessive.js")]
    public Task ported_options_maxbytelength_excessive() => ExecutionTest("options-maxbytelength-excessive");

    [Fact(DisplayName = "options-maxbytelength-negative.js")]
    public Task ported_options_maxbytelength_negative() => ExecutionTest("options-maxbytelength-negative");

    [Fact(DisplayName = "options-maxbytelength-object.js")]
    public Task ported_options_maxbytelength_object() => ExecutionTest("options-maxbytelength-object");

    [Fact(DisplayName = "options-maxbytelength-poisoned.js")]
    public Task ported_options_maxbytelength_poisoned() => ExecutionTest("options-maxbytelength-poisoned");
}
