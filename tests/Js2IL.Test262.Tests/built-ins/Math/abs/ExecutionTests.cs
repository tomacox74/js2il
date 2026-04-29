using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Math.abs;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.abs") { }

    [Fact(DisplayName = "absolute-value")]
    public Task absolute_value()
        => ExecutionTest("absolute-value");

    [Fact(DisplayName = "S15.8.2.1_A1")]
    public Task S15_8_2_1_A1()
        => ExecutionTest("S15.8.2.1_A1");

    [Fact(DisplayName = "S15.8.2.1_A2")]
    public Task S15_8_2_1_A2()
        => ExecutionTest("S15.8.2.1_A2");

    [Fact(DisplayName = "S15.8.2.1_A3")]
    public Task S15_8_2_1_A3()
        => ExecutionTest("S15.8.2.1_A3");
}
