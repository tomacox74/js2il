using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.forEach;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Map.prototype.forEach") { }

    [Fact(DisplayName = "callback-this-non-strict")]
    public Task callback_this_non_strict()
        => ExecutionTestFromFile("callback-this-non-strict");

    [Fact(DisplayName = "forEach")]
    public Task forEach()
        => ExecutionTestFromFile("forEach");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
