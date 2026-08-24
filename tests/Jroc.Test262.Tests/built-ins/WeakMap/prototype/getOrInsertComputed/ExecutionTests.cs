using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype.getOrInsertComputed;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.getOrInsertComputed") { }

    [Fact(DisplayName = "adds-object-element")]
    public Task adds_object_element()
        => ExecutionTestFromFile("adds-object-element");

    [Fact(DisplayName = "adds-value-different-callbackfn")]
    public Task adds_value_different_callbackfn()
        => ExecutionTestFromFile("adds-value-different-callbackfn");

    [Fact(DisplayName = "check-callback-fn-args")]
    public Task check_callback_fn_args()
        => ExecutionTestFromFile("check-callback-fn-args");

    [Fact(DisplayName = "check-state-after-callback-fn-throws")]
    public Task check_state_after_callback_fn_throws()
        => ExecutionTestFromFile("check-state-after-callback-fn-throws");

    [Fact(DisplayName = "does-not-evaluate-callbackfn-if-key-present")]
    public Task does_not_evaluate_callbackfn_if_key_present()
        => ExecutionTestFromFile("does-not-evaluate-callbackfn-if-key-present");

    [Fact(DisplayName = "getOrInsertComputed")]
    public Task getOrInsertComputed()
        => ExecutionTestFromFile("getOrInsertComputed");

    [Fact(DisplayName = "overwrites-mutation-from-callbackfn")]
    public Task overwrites_mutation_from_callbackfn()
        => ExecutionTestFromFile("overwrites-mutation-from-callbackfn");

    [Fact(DisplayName = "returns-value-if-key-is-not-present-object-key")]
    public Task returns_value_if_key_is_not_present_object_key()
        => ExecutionTestFromFile("returns-value-if-key-is-not-present-object-key");

    [Fact(DisplayName = "returns-value-if-key-is-present-object-key")]
    public Task returns_value_if_key_is_present_object_key()
        => ExecutionTestFromFile("returns-value-if-key-is-present-object-key");
}
