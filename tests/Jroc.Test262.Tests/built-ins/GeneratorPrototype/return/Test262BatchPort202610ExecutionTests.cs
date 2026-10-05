using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.@return;

public class Test262BatchPort202610ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202610ExecutionTests() : base("built_ins.GeneratorPrototype.return") { }

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
}
