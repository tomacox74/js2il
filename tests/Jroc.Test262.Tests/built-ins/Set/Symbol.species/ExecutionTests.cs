using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.Symbol.species;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.Symbol.species") { }

    [Fact(DisplayName = "return-value")]
    public Task return_value()
        => ExecutionTestFromFile("return-value");
}
