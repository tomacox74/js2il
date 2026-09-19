using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype.getOrInsert;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.WeakMap.prototype.getOrInsert") { }

    [Fact(DisplayName = "adds-symbol-element")]
    public Task adds_symbol_element()
        => ExecutionTestFromFile("adds-symbol-element");

    [Fact(DisplayName = "does-not-have-weakmapdata-internal-slot-array")]
    public Task does_not_have_weakmapdata_internal_slot_array()
        => ExecutionTestFromFile("does-not-have-weakmapdata-internal-slot-array");

    [Fact(DisplayName = "does-not-have-weakmapdata-internal-slot-map")]
    public Task does_not_have_weakmapdata_internal_slot_map()
        => ExecutionTestFromFile("does-not-have-weakmapdata-internal-slot-map");

    [Fact(DisplayName = "does-not-have-weakmapdata-internal-slot-object")]
    public Task does_not_have_weakmapdata_internal_slot_object()
        => ExecutionTestFromFile("does-not-have-weakmapdata-internal-slot-object");

    [Fact(DisplayName = "does-not-have-weakmapdata-internal-slot-set")]
    public Task does_not_have_weakmapdata_internal_slot_set()
        => ExecutionTestFromFile("does-not-have-weakmapdata-internal-slot-set");

    [Fact(DisplayName = "does-not-have-weakmapdata-internal-slot-weakmap-prototype")]
    public Task does_not_have_weakmapdata_internal_slot_weakmap_prototype()
        => ExecutionTestFromFile("does-not-have-weakmapdata-internal-slot-weakmap-prototype");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "returns-value-if-key-is-not-present-symbol-key")]
    public Task returns_value_if_key_is_not_present_symbol_key()
        => ExecutionTestFromFile("returns-value-if-key-is-not-present-symbol-key");

    [Fact(DisplayName = "returns-value-if-key-is-present-symbol-key")]
    public Task returns_value_if_key_is_present_symbol_key()
        => ExecutionTestFromFile("returns-value-if-key-is-present-symbol-key");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");

    [Fact(DisplayName = "throw-if-key-cannot-be-held-weakly")]
    public Task throw_if_key_cannot_be_held_weakly()
        => ExecutionTestFromFile("throw-if-key-cannot-be-held-weakly");

}
