using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.at;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.at") { }

    [Fact(DisplayName = "coerced-index-resize")]
    public Task coerced_index_resize() => ExecutionTestFromFile("coerced-index-resize");

}
