using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.atan2;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.atan2") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.8.2.5_A14.js")]
    public Task S15_8_2_5_A14() => ExecutionTestFromFile("S15.8.2.5_A14");

    [Fact(DisplayName = "S15.8.2.5_A16.js")]
    public Task S15_8_2_5_A16() => ExecutionTestFromFile("S15.8.2.5_A16");

    [Fact(DisplayName = "S15.8.2.5_A4.js")]
    public Task S15_8_2_5_A4() => ExecutionTestFromFile("S15.8.2.5_A4");

    [Fact(DisplayName = "S15.8.2.5_A5.js")]
    public Task S15_8_2_5_A5() => ExecutionTestFromFile("S15.8.2.5_A5");

    [Fact(DisplayName = "S15.8.2.5_A8.js")]
    public Task S15_8_2_5_A8() => ExecutionTestFromFile("S15.8.2.5_A8");

    [Fact(DisplayName = "S15.8.2.5_A9.js")]
    public Task S15_8_2_5_A9() => ExecutionTestFromFile("S15.8.2.5_A9");

}
