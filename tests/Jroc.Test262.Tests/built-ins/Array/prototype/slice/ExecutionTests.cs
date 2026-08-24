using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.slice;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.slice") { }

    [Fact(DisplayName = "15.4.4.10-10-c-ii-1")]
    public Task _15_4_4_10_10_c_ii_1()
        => ExecutionTestFromFile("15.4.4.10-10-c-ii-1");

    [Fact(DisplayName = "S15.4.4.10_A2.1_T5")]
    public Task S15_4_4_10_A2_1_T5()
        => ExecutionTestFromFile("S15.4.4.10_A2.1_T5");

    [Fact(DisplayName = "S15.4.4.10_A2.2_T5")]
    public Task S15_4_4_10_A2_2_T5()
        => ExecutionTestFromFile("S15.4.4.10_A2.2_T5");

    [Fact(DisplayName = "S15.4.4.10_A2_T1")]
    public Task S15_4_4_10_A2_T1()
        => ExecutionTestFromFile("S15.4.4.10_A2_T1");

    [Fact(DisplayName = "S15.4.4.10_A2_T2")]
    public Task S15_4_4_10_A2_T2()
        => ExecutionTestFromFile("S15.4.4.10_A2_T2");

    [Fact(DisplayName = "S15.4.4.10_A2_T3")]
    public Task S15_4_4_10_A2_T3()
        => ExecutionTestFromFile("S15.4.4.10_A2_T3");

    [Fact(DisplayName = "S15.4.4.10_A2_T4")]
    public Task S15_4_4_10_A2_T4()
        => ExecutionTestFromFile("S15.4.4.10_A2_T4");

    [Fact(DisplayName = "S15.4.4.10_A2_T5")]
    public Task S15_4_4_10_A2_T5()
        => ExecutionTestFromFile("S15.4.4.10_A2_T5");

    [Fact(DisplayName = "S15.4.4.10_A2_T6")]
    public Task S15_4_4_10_A2_T6()
        => ExecutionTestFromFile("S15.4.4.10_A2_T6");
}
