using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.POSITIVE_INFINITY;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Number.POSITIVE_INFINITY") { }

    [Fact(DisplayName = "S15.7.3.6_A2")]
    public Task S15_7_3_6_A2() => ExecutionTestFromFile("S15.7.3.6_A2");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
}
