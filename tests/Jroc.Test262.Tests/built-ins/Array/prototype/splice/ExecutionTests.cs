using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.splice;


public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.splice") { }

    [Fact(DisplayName = "15.4.4.12-9-a-1")]
    public Task _15_4_4_12_9_a_1()
        => ExecutionTestFromFile("15.4.4.12-9-a-1");

    [Fact(DisplayName = "S15.4.4.12_A2.1_T5")]
    public Task S15_4_4_12_A2_1_T5()
        => ExecutionTestFromFile("S15.4.4.12_A2.1_T5");

    [Fact(DisplayName = "S15.4.4.12_A2.2_T5")]
    public Task S15_4_4_12_A2_2_T5()
        => ExecutionTestFromFile("S15.4.4.12_A2.2_T5");

    [Fact(DisplayName = "S15.4.4.12_A2_T1")]
    public Task S15_4_4_12_A2_T1()
        => ExecutionTestFromFile("S15.4.4.12_A2_T1");

    [Fact(DisplayName = "S15.4.4.12_A2_T2")]
    public Task S15_4_4_12_A2_T2()
        => ExecutionTestFromFile("S15.4.4.12_A2_T2");
}
