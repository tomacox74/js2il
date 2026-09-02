using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.has;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.has") { }

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

    [Fact(DisplayName = "returns-false-when-undefined-added-deleted-not-present-undefined.js")]
    public Task returns_false_when_undefined_added_deleted_not_present_undefined() => ExecutionTestFromFile("returns-false-when-undefined-added-deleted-not-present-undefined");

    [Fact(DisplayName = "returns-false-when-value-not-present-nan.js")]
    public Task returns_false_when_value_not_present_nan() => ExecutionTestFromFile("returns-false-when-value-not-present-nan");

    [Fact(DisplayName = "returns-false-when-value-not-present-string.js")]
    public Task returns_false_when_value_not_present_string() => ExecutionTestFromFile("returns-false-when-value-not-present-string");

    [Fact(DisplayName = "returns-false-when-value-not-present-symbol.js")]
    public Task returns_false_when_value_not_present_symbol() => ExecutionTestFromFile("returns-false-when-value-not-present-symbol");

    [Fact(DisplayName = "returns-false-when-value-not-present-undefined.js")]
    public Task returns_false_when_value_not_present_undefined() => ExecutionTestFromFile("returns-false-when-value-not-present-undefined");

    [Fact(DisplayName = "returns-true-when-value-present-boolean.js")]
    public Task returns_true_when_value_present_boolean() => ExecutionTestFromFile("returns-true-when-value-present-boolean");

    [Fact(DisplayName = "returns-true-when-value-present-null.js")]
    public Task returns_true_when_value_present_null() => ExecutionTestFromFile("returns-true-when-value-present-null");

    [Fact(DisplayName = "returns-true-when-value-present-string.js")]
    public Task returns_true_when_value_present_string() => ExecutionTestFromFile("returns-true-when-value-present-string");

    [Fact(DisplayName = "returns-true-when-value-present-symbol.js")]
    public Task returns_true_when_value_present_symbol() => ExecutionTestFromFile("returns-true-when-value-present-symbol");

    [Fact(DisplayName = "returns-true-when-value-present-undefined.js")]
    public Task returns_true_when_value_present_undefined() => ExecutionTestFromFile("returns-true-when-value-present-undefined");

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
