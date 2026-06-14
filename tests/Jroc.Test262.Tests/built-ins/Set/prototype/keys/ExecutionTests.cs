using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.keys;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.keys") { }

    [Fact(DisplayName = "keys")]
    public Task keys()
        => ExecutionTestFromFile("keys");

}
