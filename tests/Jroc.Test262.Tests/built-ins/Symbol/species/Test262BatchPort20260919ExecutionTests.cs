using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.species;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Symbol.species") { }

    [Fact(DisplayName = "basic")]
    public Task basic()
        => ExecutionTestFromFile("basic");

}
