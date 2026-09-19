using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.keys;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Map.prototype.keys") { }

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
