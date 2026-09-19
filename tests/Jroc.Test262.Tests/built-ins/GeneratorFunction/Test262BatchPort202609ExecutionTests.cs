using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorFunction;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.GeneratorFunction") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

}
