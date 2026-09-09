using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype.disposeAsync;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype.disposeAsync") { }

    [Fact(DisplayName = "Symbol.asyncDispose-method-not-async.js")]
    public Task Symbol_asyncDispose_method_not_async() => ExecutionTestFromFile("Symbol.asyncDispose-method-not-async");

    [Fact(DisplayName = "disposes-resources-in-reverse-order.js")]
    public Task disposes_resources_in_reverse_order() => ExecutionTestFromFile("disposes-resources-in-reverse-order");

    [Fact(DisplayName = "does-not-reinvoke-disposers-if-already-disposed.js")]
    public Task does_not_reinvoke_disposers_if_already_disposed() => ExecutionTestFromFile("does-not-reinvoke-disposers-if-already-disposed");

    [Fact(DisplayName = "does-not-reinvoke-disposers-if-dispose-already-started.js")]
    public Task does_not_reinvoke_disposers_if_dispose_already_started() => ExecutionTestFromFile("does-not-reinvoke-disposers-if-dispose-already-started");

    [Fact(DisplayName = "does-not-reject-if-already-disposed.js")]
    public Task does_not_reject_if_already_disposed() => ExecutionTestFromFile("does-not-reject-if-already-disposed");

    [Fact(DisplayName = "explicit-await-for-null.js")]
    public Task explicit_await_for_null() => ExecutionTestFromFile("explicit-await-for-null");

    [Fact(DisplayName = "explicit-await-for-undefined.js")]
    public Task explicit_await_for_undefined() => ExecutionTestFromFile("explicit-await-for-undefined");

    [Fact(DisplayName = "explicit-await-skipped-when-empty.js")]
    public Task explicit_await_skipped_when_empty() => ExecutionTestFromFile("explicit-await-skipped-when-empty");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "rejects-with-error-as-is-if-only-one-error-during-disposal.js")]
    public Task rejects_with_error_as_is_if_only_one_error_during_disposal() => ExecutionTestFromFile("rejects-with-error-as-is-if-only-one-error-during-disposal");

    [Fact(DisplayName = "rejects-with-suppressederror-if-multiple-errors-during-disposal.js")]
    public Task rejects_with_suppressederror_if_multiple_errors_during_disposal() => ExecutionTestFromFile("rejects-with-suppressederror-if-multiple-errors-during-disposal");

    [Fact(DisplayName = "resolves-to-undefined.js")]
    public Task resolves_to_undefined() => ExecutionTestFromFile("resolves-to-undefined");

    [Fact(DisplayName = "returns-promise.js")]
    public Task returns_promise() => ExecutionTestFromFile("returns-promise");

    [Fact(DisplayName = "sets-state-to-disposed.js")]
    public Task sets_state_to_disposed() => ExecutionTestFromFile("sets-state-to-disposed");

    [Fact(DisplayName = "this-does-not-have-internal-asyncdisposablestate-rejects.js")]
    public Task this_does_not_have_internal_asyncdisposablestate_rejects() => ExecutionTestFromFile("this-does-not-have-internal-asyncdisposablestate-rejects");

    [Fact(DisplayName = "this-not-object-rejects.js")]
    public Task this_not_object_rejects() => ExecutionTestFromFile("this-not-object-rejects");
}
