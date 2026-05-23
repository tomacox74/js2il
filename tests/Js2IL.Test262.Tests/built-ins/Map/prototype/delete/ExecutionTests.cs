using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.delete") { }

    [Fact(DisplayName = "returns-false")]
    public Task returns_false()
        => ExecutionTestFromFile("returns-false");
}
