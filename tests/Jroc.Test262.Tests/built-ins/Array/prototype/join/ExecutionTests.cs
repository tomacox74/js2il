using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.join;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.join") { }

    [Fact(DisplayName = "S15.4.4.5_A1.1_T1")]
    public Task S15_4_4_5_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.5_A1.1_T1");

    [Fact(DisplayName = "S15.4.4.5_A2_T1")]
    public Task S15_4_4_5_A2_T1()
        => ExecutionTestFromFile("S15.4.4.5_A2_T1");

    [Fact(DisplayName = "S15.4.4.5_A2_T2")]
    public Task S15_4_4_5_A2_T2()
        => ExecutionTestFromFile("S15.4.4.5_A2_T2");

    [Fact(DisplayName = "S15.4.4.5_A2_T3")]
    public Task S15_4_4_5_A2_T3()
        => ExecutionTestFromFile("S15.4.4.5_A2_T3");

    [Fact(DisplayName = "S15.4.4.5_A2_T4")]
    public Task S15_4_4_5_A2_T4()
        => ExecutionTestFromFile("S15.4.4.5_A2_T4");

    [Fact(DisplayName = "S15.4.4.5_A4_T3")]
    public Task S15_4_4_5_A4_T3()
        => ExecutionTestFromFile("S15.4.4.5_A4_T3");

    [Fact(DisplayName = "S15.4.4.5_A5_T1")]
    public Task S15_4_4_5_A5_T1()
        => ExecutionTestFromFile("S15.4.4.5_A5_T1");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

}
