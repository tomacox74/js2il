using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
}
