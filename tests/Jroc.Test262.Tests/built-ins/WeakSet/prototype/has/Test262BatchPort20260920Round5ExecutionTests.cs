using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype.has;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.WeakSet.prototype.has") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
