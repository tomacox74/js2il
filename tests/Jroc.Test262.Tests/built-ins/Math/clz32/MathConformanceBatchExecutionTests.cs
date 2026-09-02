using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.clz32;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.clz32") { }

    [Fact(DisplayName = "infinity.js")]
    public Task infinity() => ExecutionTestFromFile("infinity");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "Math.clz32_2.js")]
    public Task Math_clz32_2() => ExecutionTestFromFile("Math.clz32_2");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nan.js")]
    public Task nan() => ExecutionTestFromFile("nan");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
