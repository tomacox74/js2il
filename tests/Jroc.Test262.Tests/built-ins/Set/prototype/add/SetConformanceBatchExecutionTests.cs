using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.add;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.add") { }

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

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

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

    [Fact(DisplayName = "will-not-add-duplicate-entry-initial-iterable.js")]
    public Task will_not_add_duplicate_entry_initial_iterable() => ExecutionTestFromFile("will-not-add-duplicate-entry-initial-iterable");

}
