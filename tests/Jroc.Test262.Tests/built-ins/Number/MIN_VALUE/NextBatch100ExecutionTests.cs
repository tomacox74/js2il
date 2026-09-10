using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.MIN_VALUE;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Number.MIN_VALUE") { }

    [Fact(DisplayName = "S15.7.3.3_A2")]
    public Task S15_7_3_3_A2() => ExecutionTestFromFile("S15.7.3.3_A2");
}
