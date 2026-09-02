using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.min;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.min") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "Math.min_each-element-coerced.js")]
    public Task Math_min_each_element_coerced() => ExecutionTestFromFile("Math.min_each-element-coerced");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.8.2.12_A2.js")]
    public Task S15_8_2_12_A2() => ExecutionTestFromFile("S15.8.2.12_A2");

    [Fact(DisplayName = "S15.8.2.12_A4.js")]
    public Task S15_8_2_12_A4() => ExecutionTestFromFile("S15.8.2.12_A4");

    [Fact(DisplayName = "zeros.js")]
    public Task zeros() => ExecutionTestFromFile("zeros");

}
