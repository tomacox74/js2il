using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet;

public class PortNext200ExecutionTests : DiskExecutionTestsBase
{
    public PortNext200ExecutionTests() : base("built_ins.WeakSet") { }

    [Fact(DisplayName = "add-not-callable-throws")]
    public Task add_not_callable_throws()
        => ExecutionTestFromFile("add-not-callable-throws");

    [Fact(DisplayName = "iterable-failure")]
    public Task iterable_failure()
        => ExecutionTestFromFile("iterable-failure");

    [Fact(DisplayName = "iterator-close-after-add-failure")]
    public Task iterator_close_after_add_failure()
        => ExecutionTestFromFile("iterator-close-after-add-failure");

    [Fact(DisplayName = "iterator-next-failure")]
    public Task iterator_next_failure()
        => ExecutionTestFromFile("iterator-next-failure");

    [Fact(DisplayName = "iterator-value-failure")]
    public Task iterator_value_failure()
        => ExecutionTestFromFile("iterator-value-failure");

    [Fact(DisplayName = "prototype/add/adds-symbol-element")]
    public Task prototype_add_adds_symbol_element()
        => ExecutionTestFromFile("prototype/add/adds-symbol-element");

    [Fact(DisplayName = "prototype/add/does-not-have-weaksetdata-internal-slot-array")]
    public Task prototype_add_does_not_have_weaksetdata_internal_slot_array()
        => ExecutionTestFromFile("prototype/add/does-not-have-weaksetdata-internal-slot-array");

    [Fact(DisplayName = "prototype/add/does-not-have-weaksetdata-internal-slot-map")]
    public Task prototype_add_does_not_have_weaksetdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/add/does-not-have-weaksetdata-internal-slot-map");

    [Fact(DisplayName = "prototype/add/does-not-have-weaksetdata-internal-slot-object")]
    public Task prototype_add_does_not_have_weaksetdata_internal_slot_object()
        => ExecutionTestFromFile("prototype/add/does-not-have-weaksetdata-internal-slot-object");

    [Fact(DisplayName = "prototype/add/does-not-have-weaksetdata-internal-slot-set")]
    public Task prototype_add_does_not_have_weaksetdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/add/does-not-have-weaksetdata-internal-slot-set");

    [Fact(DisplayName = "prototype/add/does-not-have-weaksetdata-internal-slot-weakset-prototype")]
    public Task prototype_add_does_not_have_weaksetdata_internal_slot_weakset_prototype()
        => ExecutionTestFromFile("prototype/add/does-not-have-weaksetdata-internal-slot-weakset-prototype");

    [Fact(DisplayName = "prototype/add/returns-this-symbol")]
    public Task prototype_add_returns_this_symbol()
        => ExecutionTestFromFile("prototype/add/returns-this-symbol");

    [Fact(DisplayName = "prototype/add/returns-this-when-ignoring-duplicate-symbol")]
    public Task prototype_add_returns_this_when_ignoring_duplicate_symbol()
        => ExecutionTestFromFile("prototype/add/returns-this-when-ignoring-duplicate-symbol");

    [Fact(DisplayName = "prototype/add/this-not-object-throw-boolean")]
    public Task prototype_add_this_not_object_throw_boolean()
        => ExecutionTestFromFile("prototype/add/this-not-object-throw-boolean");

    [Fact(DisplayName = "prototype/add/this-not-object-throw-null")]
    public Task prototype_add_this_not_object_throw_null()
        => ExecutionTestFromFile("prototype/add/this-not-object-throw-null");

    [Fact(DisplayName = "prototype/add/this-not-object-throw-number")]
    public Task prototype_add_this_not_object_throw_number()
        => ExecutionTestFromFile("prototype/add/this-not-object-throw-number");

    [Fact(DisplayName = "prototype/add/this-not-object-throw-string")]
    public Task prototype_add_this_not_object_throw_string()
        => ExecutionTestFromFile("prototype/add/this-not-object-throw-string");

    [Fact(DisplayName = "prototype/add/this-not-object-throw-symbol")]
    public Task prototype_add_this_not_object_throw_symbol()
        => ExecutionTestFromFile("prototype/add/this-not-object-throw-symbol");

    [Fact(DisplayName = "prototype/add/this-not-object-throw-undefined")]
    public Task prototype_add_this_not_object_throw_undefined()
        => ExecutionTestFromFile("prototype/add/this-not-object-throw-undefined");

    [Fact(DisplayName = "prototype/add/throw-when-value-cannot-be-held-weakly")]
    public Task prototype_add_throw_when_value_cannot_be_held_weakly()
        => ExecutionTestFromFile("prototype/add/throw-when-value-cannot-be-held-weakly");

    [Fact(DisplayName = "prototype/constructor/weakset-prototype-constructor-intrinsic")]
    public Task prototype_constructor_weakset_prototype_constructor_intrinsic()
        => ExecutionTestFromFile("prototype/constructor/weakset-prototype-constructor-intrinsic");

    [Fact(DisplayName = "prototype/delete/delete-entry-initial-iterable")]
    public Task prototype_delete_delete_entry_initial_iterable()
        => ExecutionTestFromFile("prototype/delete/delete-entry-initial-iterable");

    [Fact(DisplayName = "prototype/delete/delete-symbol-entry")]
    public Task prototype_delete_delete_symbol_entry()
        => ExecutionTestFromFile("prototype/delete/delete-symbol-entry");

    [Fact(DisplayName = "prototype/delete/does-not-have-weaksetdata-internal-slot-array")]
    public Task prototype_delete_does_not_have_weaksetdata_internal_slot_array()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weaksetdata-internal-slot-array");

    [Fact(DisplayName = "prototype/delete/does-not-have-weaksetdata-internal-slot-map")]
    public Task prototype_delete_does_not_have_weaksetdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weaksetdata-internal-slot-map");

    [Fact(DisplayName = "prototype/delete/does-not-have-weaksetdata-internal-slot-object")]
    public Task prototype_delete_does_not_have_weaksetdata_internal_slot_object()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weaksetdata-internal-slot-object");

    [Fact(DisplayName = "prototype/delete/does-not-have-weaksetdata-internal-slot-set")]
    public Task prototype_delete_does_not_have_weaksetdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weaksetdata-internal-slot-set");

    [Fact(DisplayName = "prototype/delete/does-not-have-weaksetdata-internal-slot-weakset-prototype")]
    public Task prototype_delete_does_not_have_weaksetdata_internal_slot_weakset_prototype()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weaksetdata-internal-slot-weakset-prototype");

    [Fact(DisplayName = "prototype/delete/this-not-object-throw-boolean")]
    public Task prototype_delete_this_not_object_throw_boolean()
        => ExecutionTestFromFile("prototype/delete/this-not-object-throw-boolean");

    [Fact(DisplayName = "prototype/delete/this-not-object-throw-null")]
    public Task prototype_delete_this_not_object_throw_null()
        => ExecutionTestFromFile("prototype/delete/this-not-object-throw-null");

    [Fact(DisplayName = "prototype/delete/this-not-object-throw-number")]
    public Task prototype_delete_this_not_object_throw_number()
        => ExecutionTestFromFile("prototype/delete/this-not-object-throw-number");

    [Fact(DisplayName = "prototype/delete/this-not-object-throw-string")]
    public Task prototype_delete_this_not_object_throw_string()
        => ExecutionTestFromFile("prototype/delete/this-not-object-throw-string");

    [Fact(DisplayName = "prototype/delete/this-not-object-throw-symbol")]
    public Task prototype_delete_this_not_object_throw_symbol()
        => ExecutionTestFromFile("prototype/delete/this-not-object-throw-symbol");

    [Fact(DisplayName = "prototype/delete/this-not-object-throw-undefined")]
    public Task prototype_delete_this_not_object_throw_undefined()
        => ExecutionTestFromFile("prototype/delete/this-not-object-throw-undefined");

    [Fact(DisplayName = "prototype/has/does-not-have-weaksetdata-internal-slot-array")]
    public Task prototype_has_does_not_have_weaksetdata_internal_slot_array()
        => ExecutionTestFromFile("prototype/has/does-not-have-weaksetdata-internal-slot-array");

    [Fact(DisplayName = "prototype/has/does-not-have-weaksetdata-internal-slot-map")]
    public Task prototype_has_does_not_have_weaksetdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/has/does-not-have-weaksetdata-internal-slot-map");

    [Fact(DisplayName = "prototype/has/does-not-have-weaksetdata-internal-slot-object")]
    public Task prototype_has_does_not_have_weaksetdata_internal_slot_object()
        => ExecutionTestFromFile("prototype/has/does-not-have-weaksetdata-internal-slot-object");

    [Fact(DisplayName = "prototype/has/does-not-have-weaksetdata-internal-slot-set")]
    public Task prototype_has_does_not_have_weaksetdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/has/does-not-have-weaksetdata-internal-slot-set");

    [Fact(DisplayName = "prototype/has/does-not-have-weaksetdata-internal-slot-weakset-prototype")]
    public Task prototype_has_does_not_have_weaksetdata_internal_slot_weakset_prototype()
        => ExecutionTestFromFile("prototype/has/does-not-have-weaksetdata-internal-slot-weakset-prototype");

    [Fact(DisplayName = "prototype/has/returns-false-when-symbol-value-not-present")]
    public Task prototype_has_returns_false_when_symbol_value_not_present()
        => ExecutionTestFromFile("prototype/has/returns-false-when-symbol-value-not-present");

    [Fact(DisplayName = "prototype/has/returns-true-when-symbol-value-present")]
    public Task prototype_has_returns_true_when_symbol_value_present()
        => ExecutionTestFromFile("prototype/has/returns-true-when-symbol-value-present");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-boolean")]
    public Task prototype_has_this_not_object_throw_boolean()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-boolean");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-number")]
    public Task prototype_has_this_not_object_throw_number()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-number");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-string")]
    public Task prototype_has_this_not_object_throw_string()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-string");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-symbol")]
    public Task prototype_has_this_not_object_throw_symbol()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-symbol");

}
