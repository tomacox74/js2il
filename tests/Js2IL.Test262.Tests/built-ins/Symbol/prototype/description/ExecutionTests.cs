using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Symbol.prototype.description;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.description") { }

    [Fact(DisplayName = "get")]
    public Task get()
        => ExecutionTestFromFile("get");
}
