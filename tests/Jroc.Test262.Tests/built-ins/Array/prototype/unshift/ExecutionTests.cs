using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.unshift;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.unshift") { }

    [Fact(DisplayName = "S15.4.4.13_A1_T1")]
    public Task S15_4_4_13_A1_T1()
        => ExecutionTestFromFile("S15.4.4.13_A1_T1");

    [Fact(DisplayName = "S15.4.4.13_A2_T1")]
    public Task S15_4_4_13_A2_T1()
        => ExecutionTestFromFile("S15.4.4.13_A2_T1");

    [Fact(DisplayName = "S15.4.4.13_A2_T2")]
    public Task S15_4_4_13_A2_T2()
        => ExecutionTestFromFile("S15.4.4.13_A2_T2");

    [Fact(DisplayName = "S15.4.4.13_A2_T3")]
    public Task S15_4_4_13_A2_T3()
        => ExecutionTestFromFile("S15.4.4.13_A2_T3");

    [Fact(DisplayName = "S15.4.4.13_A3_T2")]
    public Task S15_4_4_13_A3_T2()
        => ExecutionTestFromFile("S15.4.4.13_A3_T2");

    [Fact(DisplayName = "S15.4.4.13_A4_T1")]
    public Task S15_4_4_13_A4_T1()
        => ExecutionTestFromFile("S15.4.4.13_A4_T1");

    [Fact(DisplayName = "S15.4.4.13_A4_T2")]
    public Task S15_4_4_13_A4_T2()
        => ExecutionTestFromFile("S15.4.4.13_A4_T2");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "clamps-to-integer-limit")]
    public Task clamps_to_integer_limit()
        => ExecutionTestFromFile("clamps-to-integer-limit");

    [Fact(DisplayName = "length-near-integer-limit")]
    public Task length_near_integer_limit()
        => ExecutionTestFromFile("length-near-integer-limit");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "set-length-zero-array-is-frozen")]
    public Task set_length_zero_array_is_frozen()
        => ExecutionTestFromFile("set-length-zero-array-is-frozen");

    [Fact(DisplayName = "set-length-zero-array-length-is-non-writable")]
    public Task set_length_zero_array_length_is_non_writable()
        => ExecutionTestFromFile("set-length-zero-array-length-is-non-writable");
}
