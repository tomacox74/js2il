using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.clear;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.clear") { }

    [Fact(DisplayName = "clear.js")]
    public Task clear() => ExecutionTestFromFile("clear");

    [Fact(DisplayName = "clears-all-contents-from-iterable.js")]
    public Task clears_all_contents_from_iterable() => ExecutionTestFromFile("clears-all-contents-from-iterable");

    [Fact(DisplayName = "clears-all-contents.js")]
    public Task clears_all_contents() => ExecutionTestFromFile("clears-all-contents");

    [Fact(DisplayName = "clears-an-empty-set.js")]
    public Task clears_an_empty_set() => ExecutionTestFromFile("clears-an-empty-set");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-array.js")]
    public Task does_not_have_setdata_internal_slot_array() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-array");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-map.js")]
    public Task does_not_have_setdata_internal_slot_map() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-map");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-object.js")]
    public Task does_not_have_setdata_internal_slot_object() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-object");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-set.prototype.js")]
    public Task does_not_have_setdata_internal_slot_set_prototype() => ExecutionTestFromFile("does-not-have-setdata-internal-slot-set.prototype");

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

    [Fact(DisplayName = "this-not-object-throw-undefined.js")]
    public Task this_not_object_throw_undefined() => ExecutionTestFromFile("this-not-object-throw-undefined");

}
