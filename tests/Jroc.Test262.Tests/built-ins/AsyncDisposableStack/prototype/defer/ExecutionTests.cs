using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype.defer;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype.defer") { }

    [Fact(DisplayName = "adds-onDisposeAsync.js")]
    public Task adds_onDisposeAsync() => ExecutionTestFromFile("adds-onDisposeAsync");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "puts-onDisposeAsync-on-top-of-stack.js")]
    public Task puts_onDisposeAsync_on_top_of_stack() => ExecutionTestFromFile("puts-onDisposeAsync-on-top-of-stack");

    [Fact(DisplayName = "returns-undefined.js")]
    public Task returns_undefined() => ExecutionTestFromFile("returns-undefined");

    [Fact(DisplayName = "this-not-object-throws.js")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed.js")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");

    [Fact(DisplayName = "throws-if-onDisposeAsync-not-callable.js")]
    public Task throws_if_onDisposeAsync_not_callable() => ExecutionTestFromFile("throws-if-onDisposeAsync-not-callable");
}
