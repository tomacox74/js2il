using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype.dispose;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype.dispose") { }

    [Fact(DisplayName = "disposes-resources-in-reverse-order")]
    public Task disposes_resources_in_reverse_order() => ExecutionTestFromFile("disposes-resources-in-reverse-order");

    [Fact(DisplayName = "does-not-reinvoke-disposers-if-already-disposed")]
    public Task does_not_reinvoke_disposers_if_already_disposed() => ExecutionTestFromFile("does-not-reinvoke-disposers-if-already-disposed");

    [Fact(DisplayName = "does-not-throw-if-already-disposed")]
    public Task does_not_throw_if_already_disposed() => ExecutionTestFromFile("does-not-throw-if-already-disposed");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined() => ExecutionTestFromFile("returns-undefined");

    [Fact(DisplayName = "sets-state-to-disposed")]
    public Task sets_state_to_disposed() => ExecutionTestFromFile("sets-state-to-disposed");

    [Fact(DisplayName = "this-does-not-have-internal-disposablestate-throws")]
    public Task this_does_not_have_internal_disposablestate_throws() => ExecutionTestFromFile("this-does-not-have-internal-disposablestate-throws");

    [Fact(DisplayName = "this-not-object-throws")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-error-as-is-if-only-one-error-during-disposal")]
    public Task throws_error_as_is_if_only_one_error_during_disposal() => ExecutionTestFromFile("throws-error-as-is-if-only-one-error-during-disposal");

    [Fact(DisplayName = "throws-suppressederror-if-multiple-errors-during-disposal")]
    public Task throws_suppressederror_if_multiple_errors_during_disposal() => ExecutionTestFromFile("throws-suppressederror-if-multiple-errors-during-disposal");
}
