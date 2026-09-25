using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.toString;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.toString") { }

    [Fact(DisplayName = "toString")]
    public Task toString()
        => ExecutionTestFromFile("toString");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
