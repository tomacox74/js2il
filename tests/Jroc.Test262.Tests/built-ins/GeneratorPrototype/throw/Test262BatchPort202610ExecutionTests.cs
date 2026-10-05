using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.@throw;

public class Test262BatchPort202610ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202610ExecutionTests() : base("built_ins.GeneratorPrototype.throw") { }

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
}
