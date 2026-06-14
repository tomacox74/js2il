using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.values;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.values") { }

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-array")]
    public Task does_not_have_setdata_internal_slot_array()
        => ExecutionTestFromFile("does-not-have-setdata-internal-slot-array");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-map")]
    public Task does_not_have_setdata_internal_slot_map()
        => ExecutionTestFromFile("does-not-have-setdata-internal-slot-map");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-object")]
    public Task does_not_have_setdata_internal_slot_object()
        => ExecutionTestFromFile("does-not-have-setdata-internal-slot-object");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-set-prototype")]
    public Task does_not_have_setdata_internal_slot_set_prototype()
        => ExecutionTestFromFile("does-not-have-setdata-internal-slot-set-prototype");

    [Fact(DisplayName = "does-not-have-setdata-internal-slot-weakset")]
    public Task does_not_have_setdata_internal_slot_weakset()
        => ExecutionTestFromFile("does-not-have-setdata-internal-slot-weakset");

    [Fact(DisplayName = "returns-iterator-empty")]
    public Task returns_iterator_empty()
        => ExecutionTestFromFile("returns-iterator-empty");

    [Fact(DisplayName = "values")]
    public Task values()
        => ExecutionTestFromFile("values");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

}
