using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype.move;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype.move") { }

    [Fact(DisplayName = "does-not-dispose-resources")]
    public Task does_not_dispose_resources() => ExecutionTestFromFile("does-not-dispose-resources");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "returns-new-disposablestack-that-contains-moved-resources")]
    public Task returns_new_disposablestack_that_contains_moved_resources() => ExecutionTestFromFile("returns-new-disposablestack-that-contains-moved-resources");

    [Fact(DisplayName = "returns-new-disposablestack-that-is-still-pending")]
    public Task returns_new_disposablestack_that_is_still_pending() => ExecutionTestFromFile("returns-new-disposablestack-that-is-still-pending");

    [Fact(DisplayName = "returns-new-disposablestack")]
    public Task returns_new_disposablestack() => ExecutionTestFromFile("returns-new-disposablestack");

    [Fact(DisplayName = "sets-state-to-disposed")]
    public Task sets_state_to_disposed() => ExecutionTestFromFile("sets-state-to-disposed");

    [Fact(DisplayName = "still-returns-new-disposablestack-when-subclassed")]
    public Task still_returns_new_disposablestack_when_subclassed() => ExecutionTestFromFile("still-returns-new-disposablestack-when-subclassed");

    [Fact(DisplayName = "this-does-not-have-internal-disposablestate-throws")]
    public Task this_does_not_have_internal_disposablestate_throws() => ExecutionTestFromFile("this-does-not-have-internal-disposablestate-throws");

    [Fact(DisplayName = "this-not-object-throws")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");
}
