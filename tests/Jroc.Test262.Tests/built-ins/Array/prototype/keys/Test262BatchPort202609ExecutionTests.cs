using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.keys;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.keys") { }

    [Fact(DisplayName = "iteration")]
    public Task iteration()
        => ExecutionTestFromFile("iteration");

}
