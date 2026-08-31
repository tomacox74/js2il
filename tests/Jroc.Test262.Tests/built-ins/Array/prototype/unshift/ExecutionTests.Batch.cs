namespace Jroc.Test262.Tests.built_ins.Array.prototype.unshift;

public partial class ExecutionTests
{
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "read-only-property")]
    public Task read_only_property() => ExecutionTestFromFile("read-only-property");
    [Fact(DisplayName = "S15.4.4.13_A1_T2")]
    public Task S15_4_4_13_A1_T2() => ExecutionTestFromFile("S15.4.4.13_A1_T2");
    [Fact(DisplayName = "set-length-array-is-frozen")]
    public Task set_length_array_is_frozen() => ExecutionTestFromFile("set-length-array-is-frozen");
    [Fact(DisplayName = "set-length-array-length-is-non-writable")]
    public Task set_length_array_length_is_non_writable() => ExecutionTestFromFile("set-length-array-length-is-non-writable");
    [Fact(DisplayName = "throws-if-integer-limit-exceeded")]
    public Task throws_if_integer_limit_exceeded() => ExecutionTestFromFile("throws-if-integer-limit-exceeded");
    [Fact(DisplayName = "throws-with-string-receiver")]
    public Task throws_with_string_receiver() => ExecutionTestFromFile("throws-with-string-receiver");
}
