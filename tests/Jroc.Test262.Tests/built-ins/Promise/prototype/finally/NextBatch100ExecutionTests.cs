using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.finally_;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.prototype.finally") { }

    [Fact(DisplayName = "is-a-function")]
    public Task is_a_function() => ExecutionTestFromFile("is-a-function");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "species-constructor-throws")]
    public Task species_constructor_throws() => ExecutionTestFromFile("species-constructor-throws");

    [Fact(DisplayName = "this-value-non-object")]
    public Task this_value_non_object() => ExecutionTestFromFile("this-value-non-object");

    [Fact(DisplayName = "this-value-proxy")]
    public Task this_value_proxy() => ExecutionTestFromFile("this-value-proxy");

    [Fact(DisplayName = "this-value-then-poisoned")]
    public Task this_value_then_poisoned() => ExecutionTestFromFile("this-value-then-poisoned");

    [Fact(DisplayName = "this-value-then-throws")]
    public Task this_value_then_throws() => ExecutionTestFromFile("this-value-then-throws");

    [Fact(DisplayName = "this-value-thenable")]
    public Task this_value_thenable() => ExecutionTestFromFile("this-value-thenable");
}
