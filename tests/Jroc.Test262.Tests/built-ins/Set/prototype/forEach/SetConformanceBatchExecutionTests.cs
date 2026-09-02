using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.forEach;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.forEach") { }

    [Fact(DisplayName = "callback-not-callable-boolean.js")]
    public Task callback_not_callable_boolean() => ExecutionTestFromFile("callback-not-callable-boolean");

    [Fact(DisplayName = "callback-not-callable-null.js")]
    public Task callback_not_callable_null() => ExecutionTestFromFile("callback-not-callable-null");

    [Fact(DisplayName = "callback-not-callable-number.js")]
    public Task callback_not_callable_number() => ExecutionTestFromFile("callback-not-callable-number");

    [Fact(DisplayName = "callback-not-callable-string.js")]
    public Task callback_not_callable_string() => ExecutionTestFromFile("callback-not-callable-string");

    [Fact(DisplayName = "callback-not-callable-symbol.js")]
    public Task callback_not_callable_symbol() => ExecutionTestFromFile("callback-not-callable-symbol");

    [Fact(DisplayName = "callback-not-callable-undefined.js")]
    public Task callback_not_callable_undefined() => ExecutionTestFromFile("callback-not-callable-undefined");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-array.js")]
    public Task does_not_have_setdata_internal_slot_array() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-array");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-map.js")]
    public Task does_not_have_setdata_internal_slot_map() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-map");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-object.js")]
    public Task does_not_have_setdata_internal_slot_object() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-object");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-set-prototype.js")]
    public Task does_not_have_setdata_internal_slot_set_prototype() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-set-prototype");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-weakset.js")]
    public Task does_not_have_setdata_internal_slot_weakset() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-weakset");

    [Fact(DisplayName = "iterates-in-iterable-entry-order.js")]
    public Task iterates_in_iterable_entry_order() => ExecutionTestFromFile("iterates-in-iterable-entry-order");

    [Fact(DisplayName = "iterates-values-deleted-then-readded.js")]
    public Task iterates_values_deleted_then_readded() => ExecutionTestFromFile("iterates-values-deleted-then-readded");

    [Fact(DisplayName = "iterates-values-not-deleted.js")]
    public Task iterates_values_not_deleted() => ExecutionTestFromFile("iterates-values-not-deleted");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "this-arg-explicit-cannot-override-lexical-this-arrow.js")]
    public Task this_arg_explicit_cannot_override_lexical_this_arrow() => ExecutionTestFromFile("this-arg-explicit-cannot-override-lexical-this-arrow");

    [Fact(DisplayName = "this-arg-explicit.js")]
    public Task this_arg_explicit() => ExecutionTestFromFile("this-arg-explicit");

    [Fact(DisplayName = "this-non-strict.js")]
    public Task this_non_strict() => ExecutionTestFromFile("this-non-strict");

    [Fact(DisplayName = "this-not-object-throw-boolean.js")]
    public Task this_not_object_throw_boolean() => ExecutionTestFromFile("this-not-object-throw-boolean");

    [Fact(DisplayName = "this-not-object-throw-null.js")]
    public Task this_not_object_throw_null() => ExecutionTestFromFile("this-not-object-throw-null");

    [Fact(DisplayName = "this-not-object-throw-number.js")]
    public Task this_not_object_throw_number() => ExecutionTestFromFile("this-not-object-throw-number");

    [Fact(DisplayName = "this-not-object-throw-string.js")]
    public Task this_not_object_throw_string() => ExecutionTestFromFile("this-not-object-throw-string");

    [Fact(DisplayName = "this-not-object-throw-symbol.js")]
    public Task this_not_object_throw_symbol() => ExecutionTestFromFile("this-not-object-throw-symbol");

    [Fact(DisplayName = "this-not-object-throw-undefined.js")]
    public Task this_not_object_throw_undefined() => ExecutionTestFromFile("this-not-object-throw-undefined");

    [Fact(DisplayName = "this-strict.js")]
    public Task this_strict() => ExecutionTestFromFile("this-strict");

    [Fact(DisplayName = "throws-when-callback-throws.js")]
    public Task throws_when_callback_throws() => ExecutionTestFromFile("throws-when-callback-throws");

}
