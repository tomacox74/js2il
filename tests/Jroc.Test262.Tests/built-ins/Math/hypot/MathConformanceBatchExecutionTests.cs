using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.hypot;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.hypot") { }

    [Fact(DisplayName = "Math.hypot_Infinity.js")]
    public Task Math_hypot_Infinity() => ExecutionTestFromFile("Math.hypot_Infinity");

    [Fact(DisplayName = "Math.hypot_InfinityNaN.js")]
    public Task Math_hypot_InfinityNaN() => ExecutionTestFromFile("Math.hypot_InfinityNaN");

    [Fact(DisplayName = "Math.hypot_NaN.js")]
    public Task Math_hypot_NaN() => ExecutionTestFromFile("Math.hypot_NaN");

    [Fact(DisplayName = "Math.hypot_NegInfinity.js")]
    public Task Math_hypot_NegInfinity() => ExecutionTestFromFile("Math.hypot_NegInfinity");

    [Fact(DisplayName = "Math.hypot_NoArgs.js")]
    public Task Math_hypot_NoArgs() => ExecutionTestFromFile("Math.hypot_NoArgs");

    [Fact(DisplayName = "Math.hypot_Zero_2.js")]
    public Task Math_hypot_Zero_2() => ExecutionTestFromFile("Math.hypot_Zero_2");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
