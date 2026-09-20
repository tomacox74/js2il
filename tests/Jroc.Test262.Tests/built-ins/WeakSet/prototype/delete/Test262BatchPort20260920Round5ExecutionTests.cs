using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype.delete;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.WeakSet.prototype.delete") { }

    [Fact(DisplayName = "delete")]
    public Task delete()
        => ExecutionTestFromFile("delete");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
