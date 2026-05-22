using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.indexOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.indexOf") { }

    [Fact(DisplayName = "S15.5.4.7_A10")]
    public Task S15_5_4_7_A10()
        => ExecutionTestFromFile("S15.5.4.7_A10");

    [Fact(DisplayName = "S15.5.4.7_A11")]
    public Task S15_5_4_7_A11()
        => ExecutionTestFromFile("S15.5.4.7_A11");

    [Fact(DisplayName = "S15.5.4.7_A1_T1")]
    public Task S15_5_4_7_A1_T1()
        => ExecutionTestFromFile("S15.5.4.7_A1_T1");

    [Fact(DisplayName = "S15.5.4.7_A1_T10")]
    public Task S15_5_4_7_A1_T10()
        => ExecutionTestFromFile("S15.5.4.7_A1_T10");

    [Fact(DisplayName = "S15.5.4.7_A1_T12")]
    public Task S15_5_4_7_A1_T12()
        => ExecutionTestFromFile("S15.5.4.7_A1_T12");

    [Fact(DisplayName = "S15.5.4.7_A1_T2")]
    public Task S15_5_4_7_A1_T2()
        => ExecutionTestFromFile("S15.5.4.7_A1_T2");

    [Fact(DisplayName = "S15.5.4.7_A1_T4")]
    public Task S15_5_4_7_A1_T4()
        => ExecutionTestFromFile("S15.5.4.7_A1_T4");

    [Fact(DisplayName = "S15.5.4.7_A1_T5")]
    public Task S15_5_4_7_A1_T5()
        => ExecutionTestFromFile("S15.5.4.7_A1_T5");

    [Fact(DisplayName = "S15.5.4.7_A1_T6")]
    public Task S15_5_4_7_A1_T6()
        => ExecutionTestFromFile("S15.5.4.7_A1_T6");

    [Fact(DisplayName = "S15.5.4.7_A1_T7")]
    public Task S15_5_4_7_A1_T7()
        => ExecutionTestFromFile("S15.5.4.7_A1_T7");

    [Fact(DisplayName = "S15.5.4.7_A1_T8")]
    public Task S15_5_4_7_A1_T8()
        => ExecutionTestFromFile("S15.5.4.7_A1_T8");

    [Fact(DisplayName = "S15.5.4.7_A1_T9")]
    public Task S15_5_4_7_A1_T9()
        => ExecutionTestFromFile("S15.5.4.7_A1_T9");

    [Fact(DisplayName = "S15.5.4.7_A2_T1")]
    public Task S15_5_4_7_A2_T1()
        => ExecutionTestFromFile("S15.5.4.7_A2_T1");

    [Fact(DisplayName = "S15.5.4.7_A2_T2")]
    public Task S15_5_4_7_A2_T2()
        => ExecutionTestFromFile("S15.5.4.7_A2_T2");

    [Fact(DisplayName = "S15.5.4.7_A2_T3")]
    public Task S15_5_4_7_A2_T3()
        => ExecutionTestFromFile("S15.5.4.7_A2_T3");

    [Fact(DisplayName = "S15.5.4.7_A2_T4")]
    public Task S15_5_4_7_A2_T4()
        => ExecutionTestFromFile("S15.5.4.7_A2_T4");
}
