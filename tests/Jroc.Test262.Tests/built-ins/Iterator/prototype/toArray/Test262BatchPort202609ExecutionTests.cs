using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.toArray;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Iterator.prototype.toArray") { }

    [Fact(DisplayName = "get-next-method-only-once")]
    public Task get_next_method_only_once()
        => ExecutionTestFromFile("get-next-method-only-once");

    [Fact(DisplayName = "get-next-method-throws")]
    public Task get_next_method_throws()
        => ExecutionTestFromFile("get-next-method-throws");

    [Fact(DisplayName = "next-method-returns-non-object")]
    public Task next_method_returns_non_object()
        => ExecutionTestFromFile("next-method-returns-non-object");

    [Fact(DisplayName = "next-method-returns-throwing-done")]
    public Task next_method_returns_throwing_done()
        => ExecutionTestFromFile("next-method-returns-throwing-done");

    [Fact(DisplayName = "next-method-returns-throwing-value-done")]
    public Task next_method_returns_throwing_value_done()
        => ExecutionTestFromFile("next-method-returns-throwing-value-done");

    [Fact(DisplayName = "next-method-returns-throwing-value")]
    public Task next_method_returns_throwing_value()
        => ExecutionTestFromFile("next-method-returns-throwing-value");

    [Fact(DisplayName = "next-method-throws")]
    public Task next_method_throws()
        => ExecutionTestFromFile("next-method-throws");

    [Fact(DisplayName = "this-non-object")]
    public Task this_non_object()
        => ExecutionTestFromFile("this-non-object");

}
