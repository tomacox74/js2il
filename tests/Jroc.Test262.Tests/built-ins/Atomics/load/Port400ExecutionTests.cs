using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.load;

public class Port400ExecutionTests : InMemoryExecutionTestsBase
{
    public Port400ExecutionTests() : base("built_ins.Atomics.load") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port400_not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");
}
