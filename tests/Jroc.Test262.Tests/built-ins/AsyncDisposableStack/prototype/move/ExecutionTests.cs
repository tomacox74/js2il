using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype.move;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype.move") { }

    [Fact(DisplayName = "does-not-dispose-resources.js")]
    public Task does_not_dispose_resources() => ExecutionTestFromFile("does-not-dispose-resources");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "returns-new-asyncdisposablestack-that-contains-moved-resources.js")]
    public Task returns_new_asyncdisposablestack_that_contains_moved_resources() => ExecutionTestFromFile("returns-new-asyncdisposablestack-that-contains-moved-resources");

    [Fact(DisplayName = "returns-new-asyncdisposablestack-that-is-still-pending.js")]
    public Task returns_new_asyncdisposablestack_that_is_still_pending() => ExecutionTestFromFile("returns-new-asyncdisposablestack-that-is-still-pending");

    [Fact(DisplayName = "returns-new-asyncdisposablestack.js")]
    public Task returns_new_asyncdisposablestack() => ExecutionTestFromFile("returns-new-asyncdisposablestack");

    [Fact(DisplayName = "sets-state-to-disposed.js")]
    public Task sets_state_to_disposed() => ExecutionTestFromFile("sets-state-to-disposed");

    [Fact(DisplayName = "still-returns-new-asyncdisposablestack-when-subclassed.js")]
    public Task still_returns_new_asyncdisposablestack_when_subclassed() => ExecutionTestFromFile("still-returns-new-asyncdisposablestack-when-subclassed");

    [Fact(DisplayName = "this-does-not-have-internal-asyncdisposablestate-throws.js")]
    public Task this_does_not_have_internal_asyncdisposablestate_throws() => ExecutionTestFromFile("this-does-not-have-internal-asyncdisposablestate-throws");

    [Fact(DisplayName = "this-not-object-throws.js")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed.js")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");
}
