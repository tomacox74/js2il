using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.size;

public class SetRuntimeSemanticsBatchExecutionTests : DiskExecutionTestsBase
{
    public SetRuntimeSemanticsBatchExecutionTests() : base("built_ins.Set.prototype.size") { }

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");
}
