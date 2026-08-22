using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap;

public class PortNext200ExecutionTests : DiskExecutionTestsBase
{
    public PortNext200ExecutionTests() : base("built_ins.WeakMap") { }

    [Fact(DisplayName = "iterable-failure")]
    public Task iterable_failure()
        => ExecutionTestFromFile("iterable-failure");

    [Fact(DisplayName = "iterator-close-after-set-failure")]
    public Task iterator_close_after_set_failure()
        => ExecutionTestFromFile("iterator-close-after-set-failure");

    [Fact(DisplayName = "iterator-item-first-entry-returns-abrupt")]
    public Task iterator_item_first_entry_returns_abrupt()
        => ExecutionTestFromFile("iterator-item-first-entry-returns-abrupt");

    [Fact(DisplayName = "iterator-item-second-entry-returns-abrupt")]
    public Task iterator_item_second_entry_returns_abrupt()
        => ExecutionTestFromFile("iterator-item-second-entry-returns-abrupt");

    [Fact(DisplayName = "iterator-items-are-not-object-close-iterator")]
    public Task iterator_items_are_not_object_close_iterator()
        => ExecutionTestFromFile("iterator-items-are-not-object-close-iterator");

    [Fact(DisplayName = "iterator-items-keys-cannot-be-held-weakly")]
    public Task iterator_items_keys_cannot_be_held_weakly()
        => ExecutionTestFromFile("iterator-items-keys-cannot-be-held-weakly");

    [Fact(DisplayName = "iterator-next-failure")]
    public Task iterator_next_failure()
        => ExecutionTestFromFile("iterator-next-failure");

    [Fact(DisplayName = "iterator-value-failure")]
    public Task iterator_value_failure()
        => ExecutionTestFromFile("iterator-value-failure");

    [Fact(DisplayName = "prototype/delete/delete-entry-with-object-key-initial-iterable")]
    public Task prototype_delete_delete_entry_with_object_key_initial_iterable()
        => ExecutionTestFromFile("prototype/delete/delete-entry-with-object-key-initial-iterable");

    [Fact(DisplayName = "prototype/delete/delete-entry-with-symbol-key-initial-iterable")]
    public Task prototype_delete_delete_entry_with_symbol_key_initial_iterable()
        => ExecutionTestFromFile("prototype/delete/delete-entry-with-symbol-key-initial-iterable");

    [Fact(DisplayName = "prototype/delete/delete-entry-with-symbol-key")]
    public Task prototype_delete_delete_entry_with_symbol_key()
        => ExecutionTestFromFile("prototype/delete/delete-entry-with-symbol-key");

    [Fact(DisplayName = "prototype/delete/does-not-have-weakmapdata-internal-slot-array")]
    public Task prototype_delete_does_not_have_weakmapdata_internal_slot_array()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weakmapdata-internal-slot-array");

    [Fact(DisplayName = "prototype/delete/does-not-have-weakmapdata-internal-slot-map")]
    public Task prototype_delete_does_not_have_weakmapdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weakmapdata-internal-slot-map");

    [Fact(DisplayName = "prototype/delete/does-not-have-weakmapdata-internal-slot-object")]
    public Task prototype_delete_does_not_have_weakmapdata_internal_slot_object()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weakmapdata-internal-slot-object");

    [Fact(DisplayName = "prototype/delete/does-not-have-weakmapdata-internal-slot-set")]
    public Task prototype_delete_does_not_have_weakmapdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weakmapdata-internal-slot-set");

    [Fact(DisplayName = "prototype/delete/does-not-have-weakmapdata-internal-slot-weakmap-prototype")]
    public Task prototype_delete_does_not_have_weakmapdata_internal_slot_weakmap_prototype()
        => ExecutionTestFromFile("prototype/delete/does-not-have-weakmapdata-internal-slot-weakmap-prototype");

    [Fact(DisplayName = "prototype/delete/returns-false-if-key-cannot-be-held-weakly")]
    public Task prototype_delete_returns_false_if_key_cannot_be_held_weakly()
        => ExecutionTestFromFile("prototype/delete/returns-false-if-key-cannot-be-held-weakly");

    [Fact(DisplayName = "prototype/delete/returns-false-when-object-key-not-present")]
    public Task prototype_delete_returns_false_when_object_key_not_present()
        => ExecutionTestFromFile("prototype/delete/returns-false-when-object-key-not-present");

    [Fact(DisplayName = "prototype/delete/returns-false-when-symbol-key-not-present")]
    public Task prototype_delete_returns_false_when_symbol_key_not_present()
        => ExecutionTestFromFile("prototype/delete/returns-false-when-symbol-key-not-present");

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

    [Fact(DisplayName = "prototype/get/does-not-have-weakmapdata-internal-slot-map")]
    public Task prototype_get_does_not_have_weakmapdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/get/does-not-have-weakmapdata-internal-slot-map");

    [Fact(DisplayName = "prototype/get/does-not-have-weakmapdata-internal-slot-set")]
    public Task prototype_get_does_not_have_weakmapdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/get/does-not-have-weakmapdata-internal-slot-set");

    [Fact(DisplayName = "prototype/get/does-not-have-weakmapdata-internal-slot")]
    public Task prototype_get_does_not_have_weakmapdata_internal_slot()
        => ExecutionTestFromFile("prototype/get/does-not-have-weakmapdata-internal-slot");

    [Fact(DisplayName = "prototype/get/returns-undefined-with-symbol-key")]
    public Task prototype_get_returns_undefined_with_symbol_key()
        => ExecutionTestFromFile("prototype/get/returns-undefined-with-symbol-key");

    [Fact(DisplayName = "prototype/get/returns-value-with-symbol-key")]
    public Task prototype_get_returns_value_with_symbol_key()
        => ExecutionTestFromFile("prototype/get/returns-value-with-symbol-key");

    [Fact(DisplayName = "prototype/has/does-not-have-weakmapdata-internal-slot-array")]
    public Task prototype_has_does_not_have_weakmapdata_internal_slot_array()
        => ExecutionTestFromFile("prototype/has/does-not-have-weakmapdata-internal-slot-array");

    [Fact(DisplayName = "prototype/has/does-not-have-weakmapdata-internal-slot-map")]
    public Task prototype_has_does_not_have_weakmapdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/has/does-not-have-weakmapdata-internal-slot-map");

    [Fact(DisplayName = "prototype/has/does-not-have-weakmapdata-internal-slot-object")]
    public Task prototype_has_does_not_have_weakmapdata_internal_slot_object()
        => ExecutionTestFromFile("prototype/has/does-not-have-weakmapdata-internal-slot-object");

    [Fact(DisplayName = "prototype/has/does-not-have-weakmapdata-internal-slot-set")]
    public Task prototype_has_does_not_have_weakmapdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/has/does-not-have-weakmapdata-internal-slot-set");

    [Fact(DisplayName = "prototype/has/does-not-have-weakmapdata-internal-slot-weakmap-prototype")]
    public Task prototype_has_does_not_have_weakmapdata_internal_slot_weakmap_prototype()
        => ExecutionTestFromFile("prototype/has/does-not-have-weakmapdata-internal-slot-weakmap-prototype");

    [Fact(DisplayName = "prototype/has/returns-false-when-key-cannot-be-held-weakly")]
    public Task prototype_has_returns_false_when_key_cannot_be_held_weakly()
        => ExecutionTestFromFile("prototype/has/returns-false-when-key-cannot-be-held-weakly");

    [Fact(DisplayName = "prototype/has/returns-false-when-object-key-not-present")]
    public Task prototype_has_returns_false_when_object_key_not_present()
        => ExecutionTestFromFile("prototype/has/returns-false-when-object-key-not-present");

    [Fact(DisplayName = "prototype/has/returns-false-when-symbol-key-not-present")]
    public Task prototype_has_returns_false_when_symbol_key_not_present()
        => ExecutionTestFromFile("prototype/has/returns-false-when-symbol-key-not-present");

    [Fact(DisplayName = "prototype/has/returns-true-when-symbol-key-present")]
    public Task prototype_has_returns_true_when_symbol_key_present()
        => ExecutionTestFromFile("prototype/has/returns-true-when-symbol-key-present");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-boolean")]
    public Task prototype_has_this_not_object_throw_boolean()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-boolean");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-null")]
    public Task prototype_has_this_not_object_throw_null()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-null");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-number")]
    public Task prototype_has_this_not_object_throw_number()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-number");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-string")]
    public Task prototype_has_this_not_object_throw_string()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-string");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-symbol")]
    public Task prototype_has_this_not_object_throw_symbol()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-symbol");

    [Fact(DisplayName = "prototype/has/this-not-object-throw-undefined")]
    public Task prototype_has_this_not_object_throw_undefined()
        => ExecutionTestFromFile("prototype/has/this-not-object-throw-undefined");

    [Fact(DisplayName = "prototype/set/adds-symbol-element")]
    public Task prototype_set_adds_symbol_element()
        => ExecutionTestFromFile("prototype/set/adds-symbol-element");

    [Fact(DisplayName = "prototype/set/does-not-have-weakmapdata-internal-slot-map")]
    public Task prototype_set_does_not_have_weakmapdata_internal_slot_map()
        => ExecutionTestFromFile("prototype/set/does-not-have-weakmapdata-internal-slot-map");

    [Fact(DisplayName = "prototype/set/does-not-have-weakmapdata-internal-slot-object")]
    public Task prototype_set_does_not_have_weakmapdata_internal_slot_object()
        => ExecutionTestFromFile("prototype/set/does-not-have-weakmapdata-internal-slot-object");

    [Fact(DisplayName = "prototype/set/does-not-have-weakmapdata-internal-slot-set")]
    public Task prototype_set_does_not_have_weakmapdata_internal_slot_set()
        => ExecutionTestFromFile("prototype/set/does-not-have-weakmapdata-internal-slot-set");

    [Fact(DisplayName = "prototype/set/does-not-have-weakmapdata-internal-slot-weakmap-prototype")]
    public Task prototype_set_does_not_have_weakmapdata_internal_slot_weakmap_prototype()
        => ExecutionTestFromFile("prototype/set/does-not-have-weakmapdata-internal-slot-weakmap-prototype");

    [Fact(DisplayName = "prototype/set/this-not-object-throw-boolean")]
    public Task prototype_set_this_not_object_throw_boolean()
        => ExecutionTestFromFile("prototype/set/this-not-object-throw-boolean");

    [Fact(DisplayName = "prototype/set/this-not-object-throw-number")]
    public Task prototype_set_this_not_object_throw_number()
        => ExecutionTestFromFile("prototype/set/this-not-object-throw-number");

    [Fact(DisplayName = "prototype/set/this-not-object-throw-string")]
    public Task prototype_set_this_not_object_throw_string()
        => ExecutionTestFromFile("prototype/set/this-not-object-throw-string");

    [Fact(DisplayName = "prototype/set/this-not-object-throw-symbol")]
    public Task prototype_set_this_not_object_throw_symbol()
        => ExecutionTestFromFile("prototype/set/this-not-object-throw-symbol");

    [Fact(DisplayName = "prototype/set/throw-if-key-cannot-be-held-weakly")]
    public Task prototype_set_throw_if_key_cannot_be_held_weakly()
        => ExecutionTestFromFile("prototype/set/throw-if-key-cannot-be-held-weakly");

    [Fact(DisplayName = "set-not-callable-throws")]
    public Task set_not_callable_throws()
        => ExecutionTestFromFile("set-not-callable-throws");

}
