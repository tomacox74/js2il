using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.clear;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.clear") { }

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined()
        => ExecutionTestFromFile("returns-undefined");
}
