using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.getOrInsertComputed;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Map.prototype.getOrInsertComputed") { }

    [Fact(DisplayName = "append-new-values-normalizes-zero-key")]
    public Task append_new_values_normalizes_zero_key()
        => ExecutionTestFromFile("append-new-values-normalizes-zero-key");

    [Fact(DisplayName = "append-new-values")]
    public Task append_new_values()
        => ExecutionTestFromFile("append-new-values");

    [Fact(DisplayName = "append-value-if-key-is-not-present-different-key-types")]
    public Task append_value_if_key_is_not_present_different_key_types()
        => ExecutionTestFromFile("append-value-if-key-is-not-present-different-key-types");

    [Fact(DisplayName = "callbackfn-throws")]
    public Task callbackfn_throws()
        => ExecutionTestFromFile("callbackfn-throws");

    [Fact(DisplayName = "check-callback-fn-args")]
    public Task check_callback_fn_args()
        => ExecutionTestFromFile("check-callback-fn-args");

    [Fact(DisplayName = "check-state-after-callback-fn-throws")]
    public Task check_state_after_callback_fn_throws()
        => ExecutionTestFromFile("check-state-after-callback-fn-throws");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-set")]
    public Task does_not_have_mapdata_internal_slot_set()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-set");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-weakmap")]
    public Task does_not_have_mapdata_internal_slot_weakmap()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-weakmap");

    [Fact(DisplayName = "getOrInsertComputed")]
    public Task getOrInsertComputed()
        => ExecutionTestFromFile("getOrInsertComputed");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "not-a-function-callbackfn-throws")]
    public Task not_a_function_callbackfn_throws()
        => ExecutionTestFromFile("not-a-function-callbackfn-throws");

    [Fact(DisplayName = "returns-value-if-key-is-not-present-different-key-types")]
    public Task returns_value_if_key_is_not_present_different_key_types()
        => ExecutionTestFromFile("returns-value-if-key-is-not-present-different-key-types");

    [Fact(DisplayName = "returns-value-if-key-is-present-different-key-types")]
    public Task returns_value_if_key_is_present_different_key_types()
        => ExecutionTestFromFile("returns-value-if-key-is-present-different-key-types");

    [Fact(DisplayName = "returns-value-normalized-zero-key")]
    public Task returns_value_normalized_zero_key()
        => ExecutionTestFromFile("returns-value-normalized-zero-key");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");

}
