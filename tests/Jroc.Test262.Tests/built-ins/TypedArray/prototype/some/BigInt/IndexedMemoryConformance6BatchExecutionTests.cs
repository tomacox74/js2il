using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.some.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.some.BigInt") { }

    [Fact(DisplayName = "callbackfn-arguments-with-thisarg.js")]
    public Task callbackfn_arguments_with_thisarg() => ExecutionTestFromFile("callbackfn-arguments-with-thisarg");

    [Fact(DisplayName = "callbackfn-arguments-without-thisarg.js")]
    public Task callbackfn_arguments_without_thisarg() => ExecutionTestFromFile("callbackfn-arguments-without-thisarg");

    [Fact(DisplayName = "callbackfn-no-interaction-over-non-integer.js")]
    public Task callbackfn_no_interaction_over_non_integer() => ExecutionTestFromFile("callbackfn-no-interaction-over-non-integer");

    [Fact(DisplayName = "callbackfn-not-callable-throws.js")]
    public Task callbackfn_not_callable_throws() => ExecutionTestFromFile("callbackfn-not-callable-throws");

    [Fact(DisplayName = "callbackfn-not-called-on-empty.js")]
    public Task callbackfn_not_called_on_empty() => ExecutionTestFromFile("callbackfn-not-called-on-empty");

    [Fact(DisplayName = "callbackfn-return-does-not-change-instance.js")]
    public Task callbackfn_return_does_not_change_instance() => ExecutionTestFromFile("callbackfn-return-does-not-change-instance");

    [Fact(DisplayName = "callbackfn-returns-abrupt.js")]
    public Task callbackfn_returns_abrupt() => ExecutionTestFromFile("callbackfn-returns-abrupt");

    [Fact(DisplayName = "callbackfn-set-value-during-interaction.js")]
    public Task callbackfn_set_value_during_interaction() => ExecutionTestFromFile("callbackfn-set-value-during-interaction");

    [Fact(DisplayName = "callbackfn-this.js")]
    public Task callbackfn_this() => ExecutionTestFromFile("callbackfn-this");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

    [Fact(DisplayName = "returns-false-if-every-cb-returns-false.js")]
    public Task returns_false_if_every_cb_returns_false() => ExecutionTestFromFile("returns-false-if-every-cb-returns-false");

    [Fact(DisplayName = "returns-true-if-any-cb-returns-true.js")]
    public Task returns_true_if_any_cb_returns_true() => ExecutionTestFromFile("returns-true-if-any-cb-returns-true");

    [Fact(DisplayName = "values-are-not-cached.js")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");
}
