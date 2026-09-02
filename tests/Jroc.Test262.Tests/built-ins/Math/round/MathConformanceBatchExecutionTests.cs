using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.round;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.round") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.8.2.15_A1.js")]
    public Task S15_8_2_15_A1() => ExecutionTestFromFile("S15.8.2.15_A1");

    [Fact(DisplayName = "S15.8.2.15_A2.js")]
    public Task S15_8_2_15_A2() => ExecutionTestFromFile("S15.8.2.15_A2");

    [Fact(DisplayName = "S15.8.2.15_A3.js")]
    public Task S15_8_2_15_A3() => ExecutionTestFromFile("S15.8.2.15_A3");

    [Fact(DisplayName = "S15.8.2.15_A5.js")]
    public Task S15_8_2_15_A5() => ExecutionTestFromFile("S15.8.2.15_A5");

    [Fact(DisplayName = "S15.8.2.15_A6.js")]
    public Task S15_8_2_15_A6() => ExecutionTestFromFile("S15.8.2.15_A6");

}
