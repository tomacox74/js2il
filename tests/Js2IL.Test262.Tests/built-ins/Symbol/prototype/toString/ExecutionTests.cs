using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Symbol.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.toString") { }

    [Fact(DisplayName = "toString")]
    public Task toString()
        => ExecutionTestFromFile("toString");
}
