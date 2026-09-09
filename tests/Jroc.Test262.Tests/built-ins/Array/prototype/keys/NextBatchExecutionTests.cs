using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.keys;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.keys") { }

    [Fact(DisplayName = "iteration-mutable")]
    public Task iteration_mutable() => ExecutionTestFromFile("iteration-mutable");

}
