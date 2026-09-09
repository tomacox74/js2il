using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype") { }

    [Fact(DisplayName = "exotic-array")]
    public Task exotic_array() => ExecutionTestFromFile("exotic-array");

}
