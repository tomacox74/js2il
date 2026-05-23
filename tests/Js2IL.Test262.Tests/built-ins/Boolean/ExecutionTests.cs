using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Boolean;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Boolean") { }

    [Fact(DisplayName = "S15.6.1.1_A2")]
    public Task S15_6_1_1_A2()
        => ExecutionTest("S15.6.1.1_A2");

    [Fact(DisplayName = "S15.6.2.1_A1")]
    public Task S15_6_2_1_A1()
        => ExecutionTest("S15.6.2.1_A1");

    [Fact(DisplayName = "S15.6.1.1_A1_T1")]
    public Task S15_6_1_1_A1_T1()
        => ExecutionTest("S15.6.1.1_A1_T1");

    [Fact(DisplayName = "S15.6.1.1_A1_T2")]
    public Task S15_6_1_1_A1_T2()
        => ExecutionTest("S15.6.1.1_A1_T2");

    [Fact(DisplayName = "S15.6.1.1_A1_T3")]
    public Task S15_6_1_1_A1_T3()
        => ExecutionTest("S15.6.1.1_A1_T3");

    [Fact(DisplayName = "S15.6.1.1_A1_T4")]
    public Task S15_6_1_1_A1_T4()
        => ExecutionTest("S15.6.1.1_A1_T4");

    [Fact(DisplayName = "S15.6.1.1_A1_T5")]
    public Task S15_6_1_1_A1_T5()
        => ExecutionTest("S15.6.1.1_A1_T5");

}
