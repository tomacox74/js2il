using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.sumPrecise;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.sumPrecise") { }

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "sum-is-infinite.js")]
    public Task sum_is_infinite() => ExecutionTestFromFile("sum-is-infinite");

    [Fact(DisplayName = "sum-is-minus-zero.js")]
    public Task sum_is_minus_zero() => ExecutionTestFromFile("sum-is-minus-zero");

    [Fact(DisplayName = "sum-is-NaN.js")]
    public Task sum_is_NaN() => ExecutionTestFromFile("sum-is-NaN");

    [Fact(DisplayName = "sum.js")]
    public Task sum() => ExecutionTestFromFile("sum");

    [Fact(DisplayName = "takes-iterable.js")]
    public Task takes_iterable() => ExecutionTestFromFile("takes-iterable");

    [Fact(DisplayName = "throws-on-non-number.js")]
    public Task throws_on_non_number() => ExecutionTestFromFile("throws-on-non-number");

}
