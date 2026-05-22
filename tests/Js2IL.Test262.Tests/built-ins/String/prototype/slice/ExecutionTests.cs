using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.slice;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.slice") { }

    [Fact(DisplayName = "S15.5.4.13_A10")]
    public Task S15_5_4_13_A10()
        => ExecutionTestFromFile("S15.5.4.13_A10");

    [Fact(DisplayName = "S15.5.4.13_A11")]
    public Task S15_5_4_13_A11()
        => ExecutionTestFromFile("S15.5.4.13_A11");

    [Fact(DisplayName = "S15.5.4.13_A1_T1")]
    public Task S15_5_4_13_A1_T1()
        => ExecutionTestFromFile("S15.5.4.13_A1_T1");

    [Fact(DisplayName = "S15.5.4.13_A1_T10")]
    public Task S15_5_4_13_A1_T10()
        => ExecutionTestFromFile("S15.5.4.13_A1_T10");

    [Fact(DisplayName = "S15.5.4.13_A1_T11")]
    public Task S15_5_4_13_A1_T11()
        => ExecutionTestFromFile("S15.5.4.13_A1_T11");

    [Fact(DisplayName = "S15.5.4.13_A1_T12")]
    public Task S15_5_4_13_A1_T12()
        => ExecutionTestFromFile("S15.5.4.13_A1_T12");

    [Fact(DisplayName = "S15.5.4.13_A1_T13")]
    public Task S15_5_4_13_A1_T13()
        => ExecutionTestFromFile("S15.5.4.13_A1_T13");

    [Fact(DisplayName = "S15.5.4.13_A1_T14")]
    public Task S15_5_4_13_A1_T14()
        => ExecutionTestFromFile("S15.5.4.13_A1_T14");

    [Fact(DisplayName = "S15.5.4.13_A1_T15")]
    public Task S15_5_4_13_A1_T15()
        => ExecutionTestFromFile("S15.5.4.13_A1_T15");

    [Fact(DisplayName = "S15.5.4.13_A1_T2")]
    public Task S15_5_4_13_A1_T2()
        => ExecutionTestFromFile("S15.5.4.13_A1_T2");

    [Fact(DisplayName = "S15.5.4.13_A1_T4")]
    public Task S15_5_4_13_A1_T4()
        => ExecutionTestFromFile("S15.5.4.13_A1_T4");

    [Fact(DisplayName = "S15.5.4.13_A1_T5")]
    public Task S15_5_4_13_A1_T5()
        => ExecutionTestFromFile("S15.5.4.13_A1_T5");

    [Fact(DisplayName = "S15.5.4.13_A1_T6")]
    public Task S15_5_4_13_A1_T6()
        => ExecutionTestFromFile("S15.5.4.13_A1_T6");

    [Fact(DisplayName = "S15.5.4.13_A1_T7")]
    public Task S15_5_4_13_A1_T7()
        => ExecutionTestFromFile("S15.5.4.13_A1_T7");

    [Fact(DisplayName = "S15.5.4.13_A1_T8")]
    public Task S15_5_4_13_A1_T8()
        => ExecutionTestFromFile("S15.5.4.13_A1_T8");

    [Fact(DisplayName = "S15.5.4.13_A1_T9")]
    public Task S15_5_4_13_A1_T9()
        => ExecutionTestFromFile("S15.5.4.13_A1_T9");
}
