using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.sqrt;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.sqrt") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "results.js")]
    public Task results() => ExecutionTestFromFile("results");

    [Fact(DisplayName = "S15.8.2.17_A3.js")]
    public Task S15_8_2_17_A3() => ExecutionTestFromFile("S15.8.2.17_A3");

    [Fact(DisplayName = "S15.8.2.17_A4.js")]
    public Task S15_8_2_17_A4() => ExecutionTestFromFile("S15.8.2.17_A4");

}
