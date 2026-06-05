using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.toSorted;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.toSorted") { }

    [Fact(DisplayName = "immutable")]
    public Task immutable()
        => ExecutionTestFromFile("immutable");
}
