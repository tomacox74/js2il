using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer") { }

    [Fact(DisplayName = "allocation-limit.js")]
    public Task allocation_limit() => ExecutionTestFromFile("allocation-limit");

    [Fact(DisplayName = "init-zero.js")]
    public Task init_zero() => ExecutionTestFromFile("init-zero");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length-is-absent.js")]
    public Task length_is_absent() => ExecutionTestFromFile("length-is-absent");

    [Fact(DisplayName = "length-is-too-large-throws.js")]
    public Task length_is_too_large_throws() => ExecutionTestFromFile("length-is-too-large-throws");

    [Fact(DisplayName = "negative-length-throws.js")]
    public Task negative_length_throws() => ExecutionTestFromFile("negative-length-throws");

    [Fact(DisplayName = "options-maxbytelength-allocation-limit.js")]
    public Task options_maxbytelength_allocation_limit() => ExecutionTestFromFile("options-maxbytelength-allocation-limit");

    [Fact(DisplayName = "options-maxbytelength-compared-before-object-creation.js")]
    public Task options_maxbytelength_compared_before_object_creation() => ExecutionTestFromFile("options-maxbytelength-compared-before-object-creation");

    [Fact(DisplayName = "options-maxbytelength-data-allocation-after-object-creation.js")]
    public Task options_maxbytelength_data_allocation_after_object_creation() => ExecutionTestFromFile("options-maxbytelength-data-allocation-after-object-creation");

    [Fact(DisplayName = "options-maxbytelength-diminuitive.js")]
    public Task options_maxbytelength_diminuitive() => ExecutionTestFromFile("options-maxbytelength-diminuitive");

    [Fact(DisplayName = "options-maxbytelength-excessive.js")]
    public Task options_maxbytelength_excessive() => ExecutionTestFromFile("options-maxbytelength-excessive");

    [Fact(DisplayName = "options-maxbytelength-negative.js")]
    public Task options_maxbytelength_negative() => ExecutionTestFromFile("options-maxbytelength-negative");

    [Fact(DisplayName = "options-maxbytelength-object.js")]
    public Task options_maxbytelength_object() => ExecutionTestFromFile("options-maxbytelength-object");

    [Fact(DisplayName = "options-maxbytelength-poisoned.js")]
    public Task options_maxbytelength_poisoned() => ExecutionTestFromFile("options-maxbytelength-poisoned");

    [Fact(DisplayName = "options-maxbytelength-undefined.js")]
    public Task options_maxbytelength_undefined() => ExecutionTestFromFile("options-maxbytelength-undefined");

    [Fact(DisplayName = "options-non-object.js")]
    public Task options_non_object() => ExecutionTestFromFile("options-non-object");

    [Fact(DisplayName = "prototype-from-newtarget.js")]
    public Task prototype_from_newtarget() => ExecutionTestFromFile("prototype-from-newtarget");

    [Fact(DisplayName = "return-abrupt-from-length-symbol.js")]
    public Task return_abrupt_from_length_symbol() => ExecutionTestFromFile("return-abrupt-from-length-symbol");

    [Fact(DisplayName = "return-abrupt-from-length.js")]
    public Task return_abrupt_from_length() => ExecutionTestFromFile("return-abrupt-from-length");

    [Fact(DisplayName = "zero-length.js")]
    public Task zero_length() => ExecutionTestFromFile("zero-length");

}
