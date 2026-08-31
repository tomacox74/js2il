namespace Jroc.Test262.Tests.built_ins.Array.prototype.shift;

public partial class ExecutionTests
{
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "S15.4.4.9_A1.2_T1")]
    public Task S15_4_4_9_A1_2_T1() => ExecutionTestFromFile("S15.4.4.9_A1.2_T1");
    [Fact(DisplayName = "set-length-array-is-frozen")]
    public Task set_length_array_is_frozen() => ExecutionTestFromFile("set-length-array-is-frozen");
    [Fact(DisplayName = "set-length-array-length-is-non-writable")]
    public Task set_length_array_length_is_non_writable() => ExecutionTestFromFile("set-length-array-length-is-non-writable");
    [Fact(DisplayName = "throws-when-this-value-length-is-writable-false")]
    public Task throws_when_this_value_length_is_writable_false() => ExecutionTestFromFile("throws-when-this-value-length-is-writable-false");
}
