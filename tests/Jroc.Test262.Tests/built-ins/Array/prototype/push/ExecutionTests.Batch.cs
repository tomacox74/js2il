namespace Jroc.Test262.Tests.built_ins.Array.prototype.push;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "length-near-integer-limit-set-failure")]
    public Task length_near_integer_limit_set_failure() => ExecutionTestFromFile("length-near-integer-limit-set-failure");
    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "S15.4.4.7_A3")]
    public Task S15_4_4_7_A3() => ExecutionTestFromFile("S15.4.4.7_A3");
    [Fact(DisplayName = "set-length-array-is-frozen")]
    public Task set_length_array_is_frozen() => ExecutionTestFromFile("set-length-array-is-frozen");
    [Fact(DisplayName = "set-length-array-length-is-non-writable")]
    public Task set_length_array_length_is_non_writable() => ExecutionTestFromFile("set-length-array-length-is-non-writable");
    [Fact(DisplayName = "set-length-zero-array-is-frozen")]
    public Task set_length_zero_array_is_frozen() => ExecutionTestFromFile("set-length-zero-array-is-frozen");
    [Fact(DisplayName = "set-length-zero-array-length-is-non-writable")]
    public Task set_length_zero_array_length_is_non_writable() => ExecutionTestFromFile("set-length-zero-array-length-is-non-writable");
    [Fact(DisplayName = "throws-if-integer-limit-exceeded")]
    public Task throws_if_integer_limit_exceeded() => ExecutionTestFromFile("throws-if-integer-limit-exceeded");
    [Fact(DisplayName = "throws-with-string-receiver")]
    public Task throws_with_string_receiver() => ExecutionTestFromFile("throws-with-string-receiver");
}
