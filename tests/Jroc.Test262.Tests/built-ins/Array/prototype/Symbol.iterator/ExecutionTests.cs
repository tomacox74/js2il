using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.Symbol.iterator;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.Symbol.iterator") { }

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");
}
