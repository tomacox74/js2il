using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.replace;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.replace") { }

    [Fact(DisplayName = "15.5.4.11-1")]
    public Task _15_5_4_11_1()
        => ExecutionTestFromFile("15.5.4.11-1");

    [Fact(DisplayName = "S15.5.4.11_A1_T1")]
    public Task S15_5_4_11_A1_T1()
        => ExecutionTestFromFile("S15.5.4.11_A1_T1");

    [Fact(DisplayName = "S15.5.4.11_A1_T4")]
    public Task S15_5_4_11_A1_T4()
        => ExecutionTestFromFile("S15.5.4.11_A1_T4");

    [Fact(DisplayName = "S15.5.4.11_A2_T1")]
    public Task S15_5_4_11_A2_T1()
        => ExecutionTestFromFile("S15.5.4.11_A2_T1");

    [Fact(DisplayName = "S15.5.4.11_A2_T3")]
    public Task S15_5_4_11_A2_T3()
        => ExecutionTestFromFile("S15.5.4.11_A2_T3");

    [Fact(DisplayName = "S15.5.4.11_A2_T6")]
    public Task S15_5_4_11_A2_T6()
        => ExecutionTestFromFile("S15.5.4.11_A2_T6");
    [Fact(DisplayName = "S15.5.4.11_A12")]
    public Task S15_5_4_11_A12()
        => ExecutionTestFromFile("S15.5.4.11_A12");
    [Fact(DisplayName = "S15.5.4.11_A2_T5")]
    public Task S15_5_4_11_A2_T5()
        => ExecutionTestFromFile("S15.5.4.11_A2_T5");
    [Fact(DisplayName = "S15.5.4.11_A2_T2")]
    public Task S15_5_4_11_A2_T2()
        => ExecutionTestFromFile("S15.5.4.11_A2_T2");
    [Fact(DisplayName = "S15.5.4.11_A2_T4")]
    public Task S15_5_4_11_A2_T4()
        => ExecutionTestFromFile("S15.5.4.11_A2_T4");

    [Fact(DisplayName = "S15.5.4.11_A1_T8")]
    public Task S15_5_4_11_A1_T8()
        => ExecutionTestFromFile("S15.5.4.11_A1_T8");

    [Fact(DisplayName = "S15.5.4.11_A1_T14")]
    public Task S15_5_4_11_A1_T14()
        => ExecutionTestFromFile("S15.5.4.11_A1_T14");

}
