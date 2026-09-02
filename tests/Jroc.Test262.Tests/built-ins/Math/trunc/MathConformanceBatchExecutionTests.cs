using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.trunc;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.trunc") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "Math.trunc_Infinity.js")]
    public Task Math_trunc_Infinity() => ExecutionTestFromFile("Math.trunc_Infinity");

    [Fact(DisplayName = "Math.trunc_NaN.js")]
    public Task Math_trunc_NaN() => ExecutionTestFromFile("Math.trunc_NaN");

    [Fact(DisplayName = "Math.trunc_NegDecimal.js")]
    public Task Math_trunc_NegDecimal() => ExecutionTestFromFile("Math.trunc_NegDecimal");

    [Fact(DisplayName = "Math.trunc_PosDecimal.js")]
    public Task Math_trunc_PosDecimal() => ExecutionTestFromFile("Math.trunc_PosDecimal");

    [Fact(DisplayName = "Math.trunc_Success.js")]
    public Task Math_trunc_Success() => ExecutionTestFromFile("Math.trunc_Success");

    [Fact(DisplayName = "Math.trunc_Zero.js")]
    public Task Math_trunc_Zero() => ExecutionTestFromFile("Math.trunc_Zero");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "trunc-sampleTests.js")]
    public Task trunc_sampleTests() => ExecutionTestFromFile("trunc-sampleTests");

}
