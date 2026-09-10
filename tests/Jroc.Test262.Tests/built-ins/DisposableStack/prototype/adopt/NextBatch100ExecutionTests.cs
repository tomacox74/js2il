using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype.adopt;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype.adopt") { }

    [Fact(DisplayName = "adds-value-onDispose")]
    public Task adds_value_onDispose() => ExecutionTestFromFile("adds-value-onDispose");

    [Fact(DisplayName = "allows-any-value")]
    public Task allows_any_value() => ExecutionTestFromFile("allows-any-value");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "puts-value-onDispose-on-top-of-stack")]
    public Task puts_value_onDispose_on_top_of_stack() => ExecutionTestFromFile("puts-value-onDispose-on-top-of-stack");

    [Fact(DisplayName = "returns-value")]
    public Task returns_value() => ExecutionTestFromFile("returns-value");

    [Fact(DisplayName = "this-does-not-have-internal-disposablestate-throws")]
    public Task this_does_not_have_internal_disposablestate_throws() => ExecutionTestFromFile("this-does-not-have-internal-disposablestate-throws");

    [Fact(DisplayName = "this-not-object-throws")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");

    [Fact(DisplayName = "throws-if-onDispose-not-callable")]
    public Task throws_if_onDispose_not_callable() => ExecutionTestFromFile("throws-if-onDispose-not-callable");
}
