using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorPrototype.@throw;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorPrototype.throw") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
