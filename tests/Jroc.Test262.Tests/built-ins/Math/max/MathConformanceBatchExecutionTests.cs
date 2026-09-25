using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.max;

public class MathConformanceBatchExecutionTests : InMemoryExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.max") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

}
