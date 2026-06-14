using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.charCodeAt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.charCodeAt") { }

    [Fact(DisplayName = "pos-coerce-string")]
    public Task pos_coerce_string()
        => ExecutionTestFromFile("pos-coerce-string");

    [Fact(DisplayName = "pos-rounding")]
    public Task pos_rounding()
        => ExecutionTestFromFile("pos-rounding");

    [Fact(DisplayName = "S15.5.4.5_A2")]
    public Task S15_5_4_5_A2()
        => ExecutionTestFromFile("S15.5.4.5_A2");

    [Fact(DisplayName = "S15.5.4.5_A3")]
    public Task S15_5_4_5_A3()
        => ExecutionTestFromFile("S15.5.4.5_A3");
    [Fact(DisplayName = "S15.5.4.5_A1_T2")]
    public Task S15_5_4_5_A1_T2()
        => ExecutionTestFromFile("S15.5.4.5_A1_T2");
    [Fact(DisplayName = "S15.5.4.5_A11")]
    public Task S15_5_4_5_A11()
        => ExecutionTestFromFile("S15.5.4.5_A11");
    [Fact(DisplayName = "S15.5.4.5_A1_T1")]
    public Task S15_5_4_5_A1_T1()
        => ExecutionTestFromFile("S15.5.4.5_A1_T1");
    [Fact(DisplayName = "S15.5.4.5_A1_T10")]
    public Task S15_5_4_5_A1_T10()
        => ExecutionTestFromFile("S15.5.4.5_A1_T10");

    [Fact(DisplayName = "S15.5.4.5_A1_T4")]
    public Task S15_5_4_5_A1_T4()
        => ExecutionTestFromFile("S15.5.4.5_A1_T4");

    [Fact(DisplayName = "S15.5.4.5_A1_T5")]
    public Task S15_5_4_5_A1_T5()
        => ExecutionTestFromFile("S15.5.4.5_A1_T5");

    [Fact(DisplayName = "S15.5.4.5_A1_T6")]
    public Task S15_5_4_5_A1_T6()
        => ExecutionTestFromFile("S15.5.4.5_A1_T6");

    [Fact(DisplayName = "S15.5.4.5_A1_T7")]
    public Task S15_5_4_5_A1_T7()
        => ExecutionTestFromFile("S15.5.4.5_A1_T7");

    [Fact(DisplayName = "S15.5.4.5_A1_T8")]
    public Task S15_5_4_5_A1_T8()
        => ExecutionTestFromFile("S15.5.4.5_A1_T8");

    [Fact(DisplayName = "S15.5.4.5_A1_T9")]
    public Task S15_5_4_5_A1_T9()
        => ExecutionTestFromFile("S15.5.4.5_A1_T9");

    [Fact(DisplayName = "S15.5.4.5_A4")]
    public Task S15_5_4_5_A4()
        => ExecutionTestFromFile("S15.5.4.5_A4");

}
