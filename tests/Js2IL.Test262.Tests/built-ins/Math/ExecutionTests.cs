using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Math;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math") { }

    [Fact(DisplayName = "ceil/not-a-constructor")]
    public Task ceil_not_a_constructor()
        => ExecutionTestFromFile("ceil/not-a-constructor");

    [Fact(DisplayName = "S15.8.2.6_A6")]
    public Task ceil_S15_8_2_6_A6()
        => ExecutionTestFromFile("ceil/S15.8.2.6_A6");

    [Fact(DisplayName = "S15.8.2.12_A1")]
    public Task min_S15_8_2_12_A1()
        => ExecutionTestFromFile("min/S15.8.2.12_A1");

    [Fact(DisplayName = "15.8.2.12-1")]
    public Task min_15_8_2_12_1()
        => ExecutionTestFromFile("min/15.8.2.12-1");

    [Fact(DisplayName = "round/not-a-constructor")]
    public Task round_not_a_constructor()
        => ExecutionTestFromFile("round/not-a-constructor");

    [Fact(DisplayName = "S15.8.2.15_A4")]
    public Task round_S15_8_2_15_A4()
        => ExecutionTestFromFile("round/S15.8.2.15_A4");

    [Fact(DisplayName = "sign-specialVals")]
    public Task sign_sign_specialVals()
        => ExecutionTestFromFile("sign/sign-specialVals");

    [Fact(DisplayName = "sign/not-a-constructor")]
    public Task sign_not_a_constructor()
        => ExecutionTestFromFile("sign/not-a-constructor");
    [Fact(DisplayName = "E/value")]
    public Task E_value()
        => ExecutionTestFromFile("E/value");

    [Fact(DisplayName = "LN10/value")]
    public Task LN10_value()
        => ExecutionTestFromFile("LN10/value");

    [Fact(DisplayName = "LN2/value")]
    public Task LN2_value()
        => ExecutionTestFromFile("LN2/value");

    [Fact(DisplayName = "LOG10E/value")]
    public Task LOG10E_value()
        => ExecutionTestFromFile("LOG10E/value");

    [Fact(DisplayName = "LOG2E/value")]
    public Task LOG2E_value()
        => ExecutionTestFromFile("LOG2E/value");

    [Fact(DisplayName = "PI/value")]
    public Task PI_value()
        => ExecutionTestFromFile("PI/value");

    [Fact(DisplayName = "SQRT2/value")]
    public Task SQRT2_value()
        => ExecutionTestFromFile("SQRT2/value");

    [Fact(DisplayName = "sqrt/S15.8.2.17_A1")]
    public Task sqrt_S15_8_2_17_A1()
        => ExecutionTestFromFile("sqrt/S15.8.2.17_A1");

    [Fact(DisplayName = "sqrt/S15.8.2.17_A2")]
    public Task sqrt_S15_8_2_17_A2()
        => ExecutionTestFromFile("sqrt/S15.8.2.17_A2");

    [Fact(DisplayName = "sqrt/S15.8.2.17_A5")]
    public Task sqrt_S15_8_2_17_A5()
        => ExecutionTestFromFile("sqrt/S15.8.2.17_A5");

    [Fact(DisplayName = "SQRT1_2/value")]
    public Task SQRT1_2_value()
        => ExecutionTestFromFile("SQRT1_2/value");

}
