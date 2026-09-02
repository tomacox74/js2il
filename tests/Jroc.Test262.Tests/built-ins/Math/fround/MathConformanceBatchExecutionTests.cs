using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.fround;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.fround") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "Math.fround_Infinity.js")]
    public Task Math_fround_Infinity() => ExecutionTestFromFile("Math.fround_Infinity");

    [Fact(DisplayName = "Math.fround_NaN.js")]
    public Task Math_fround_NaN() => ExecutionTestFromFile("Math.fround_NaN");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "ties.js")]
    public Task ties() => ExecutionTestFromFile("ties");

    [Fact(DisplayName = "value-convertion.js")]
    public Task value_convertion() => ExecutionTestFromFile("value-convertion");

}
