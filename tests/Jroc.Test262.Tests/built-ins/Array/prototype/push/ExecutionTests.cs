using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.push;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.push") { }

    [Fact(DisplayName = "S15.4.4.7_A1_T1")]
    public Task S15_4_4_7_A1_T1()
        => ExecutionTestFromFile("S15.4.4.7_A1_T1");
    [Fact(DisplayName = "S15.4.4.7_A1_T2")]
    public Task S15_4_4_7_A1_T2()
        => ExecutionTestFromFile("S15.4.4.7_A1_T2");

    [Fact(DisplayName = "S15.4.4.7_A2_T1")]
    public Task S15_4_4_7_A2_T1()
        => ExecutionTestFromFile("S15.4.4.7_A2_T1");

    [Fact(DisplayName = "S15.4.4.7_A2_T2")]
    public Task S15_4_4_7_A2_T2()
        => ExecutionTestFromFile("S15.4.4.7_A2_T2");

    [Fact(DisplayName = "S15.4.4.7_A2_T3")]
    public Task S15_4_4_7_A2_T3()
        => ExecutionTestFromFile("S15.4.4.7_A2_T3");

    [Fact(DisplayName = "S15.4.4.7_A4_T1")]
    public Task S15_4_4_7_A4_T1()
        => ExecutionTestFromFile("S15.4.4.7_A4_T1");

    [Fact(DisplayName = "S15.4.4.7_A4_T2")]
    public Task S15_4_4_7_A4_T2()
        => ExecutionTestFromFile("S15.4.4.7_A4_T2");

    [Fact(DisplayName = "S15.4.4.7_A4_T3")]
    public Task S15_4_4_7_A4_T3()
        => ExecutionTestFromFile("S15.4.4.7_A4_T3");

    [Fact(DisplayName = "S15.4.4.7_A5_T1")]
    public Task S15_4_4_7_A5_T1()
        => ExecutionTestFromFile("S15.4.4.7_A5_T1");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "clamps-to-integer-limit")]
    public Task clamps_to_integer_limit()
        => ExecutionTestFromFile("clamps-to-integer-limit");

    [Fact(DisplayName = "length-near-integer-limit")]
    public Task length_near_integer_limit()
        => ExecutionTestFromFile("length-near-integer-limit");

}
