using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype.defer;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype.defer") { }

    [Fact(DisplayName = "adds-onDispose")]
    public Task adds_onDispose() => ExecutionTestFromFile("adds-onDispose");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "puts-onDispose-on-top-of-stack")]
    public Task puts_onDispose_on_top_of_stack() => ExecutionTestFromFile("puts-onDispose-on-top-of-stack");

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined() => ExecutionTestFromFile("returns-undefined");

    [Fact(DisplayName = "this-does-not-have-internal-disposablestate-throws")]
    public Task this_does_not_have_internal_disposablestate_throws() => ExecutionTestFromFile("this-does-not-have-internal-disposablestate-throws");

    [Fact(DisplayName = "this-not-object-throws")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");

    [Fact(DisplayName = "throws-if-onDispose-not-callable")]
    public Task throws_if_onDispose_not_callable() => ExecutionTestFromFile("throws-if-onDispose-not-callable");
}
