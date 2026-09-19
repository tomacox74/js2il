using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype.has;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.WeakMap.prototype.has") { }

    [Fact(DisplayName = "has")]
    public Task has()
        => ExecutionTestFromFile("has");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
