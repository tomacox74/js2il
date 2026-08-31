using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.shift;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.shift") { }

    [Fact(DisplayName = "S15.4.4.9_A1.1_T1")]
    public Task S15_4_4_9_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.9_A1.1_T1");

    [Fact(DisplayName = "S15.4.4.9_A2_T1")]
    public Task S15_4_4_9_A2_T1()
        => ExecutionTestFromFile("S15.4.4.9_A2_T1");

    [Fact(DisplayName = "S15.4.4.9_A2_T2")]
    public Task S15_4_4_9_A2_T2()
        => ExecutionTestFromFile("S15.4.4.9_A2_T2");

    [Fact(DisplayName = "S15.4.4.9_A2_T3")]
    public Task S15_4_4_9_A2_T3()
        => ExecutionTestFromFile("S15.4.4.9_A2_T3");

    [Fact(DisplayName = "S15.4.4.9_A2_T4")]
    public Task S15_4_4_9_A2_T4()
        => ExecutionTestFromFile("S15.4.4.9_A2_T4");

    [Fact(DisplayName = "S15.4.4.9_A2_T5")]
    public Task S15_4_4_9_A2_T5()
        => ExecutionTestFromFile("S15.4.4.9_A2_T5");

    [Fact(DisplayName = "S15.4.4.9_A3_T3")]
    public Task S15_4_4_9_A3_T3()
        => ExecutionTestFromFile("S15.4.4.9_A3_T3");

    [Fact(DisplayName = "S15.4.4.9_A4_T1")]
    public Task S15_4_4_9_A4_T1()
        => ExecutionTestFromFile("S15.4.4.9_A4_T1");

    [Fact(DisplayName = "S15.4.4.9_A4_T2")]
    public Task S15_4_4_9_A4_T2()
        => ExecutionTestFromFile("S15.4.4.9_A4_T2");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

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
