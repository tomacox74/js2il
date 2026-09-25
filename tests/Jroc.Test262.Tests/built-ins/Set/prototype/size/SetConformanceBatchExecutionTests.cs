using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.size;

public class SetConformanceBatchExecutionTests : InMemoryExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.size") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

}
