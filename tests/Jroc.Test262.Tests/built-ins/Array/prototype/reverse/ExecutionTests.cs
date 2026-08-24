using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.reverse;


public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.reverse") { }

    [Fact(DisplayName = "S15.4.4.8_A1_T1")]
    public Task S15_4_4_8_A1_T1()
        => ExecutionTestFromFile("S15.4.4.8_A1_T1");

    [Fact(DisplayName = "S15.4.4.8_A2_T1")]
    public Task S15_4_4_8_A2_T1()
        => ExecutionTestFromFile("S15.4.4.8_A2_T1");

    [Fact(DisplayName = "S15.4.4.8_A2_T2")]
    public Task S15_4_4_8_A2_T2()
        => ExecutionTestFromFile("S15.4.4.8_A2_T2");

    [Fact(DisplayName = "S15.4.4.8_A2_T3")]
    public Task S15_4_4_8_A2_T3()
        => ExecutionTestFromFile("S15.4.4.8_A2_T3");

    [Fact(DisplayName = "S15.4.4.8_A3_T3")]
    public Task S15_4_4_8_A3_T3()
        => ExecutionTestFromFile("S15.4.4.8_A3_T3");

    [Fact(DisplayName = "S15.4.4.8_A4_T1")]
    public Task S15_4_4_8_A4_T1()
        => ExecutionTestFromFile("S15.4.4.8_A4_T1");

    [Fact(DisplayName = "S15.4.4.8_A4_T2")]
    public Task S15_4_4_8_A4_T2()
        => ExecutionTestFromFile("S15.4.4.8_A4_T2");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "get_if_present_with_delete")]
    public Task get_if_present_with_delete()
        => ExecutionTestFromFile("get_if_present_with_delete");

    [Fact(DisplayName = "length-exceeding-integer-limit-with-object")]
    public Task length_exceeding_integer_limit_with_object()
        => ExecutionTestFromFile("length-exceeding-integer-limit-with-object");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");
}
