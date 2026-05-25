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
}
