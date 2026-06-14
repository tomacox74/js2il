using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.entries;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.entries") { }

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

    [Fact(DisplayName = "entries")]
    public Task entries()
        => ExecutionTestFromFile("entries");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "returns-iterator-empty")]
    public Task returns_iterator_empty()
        => ExecutionTestFromFile("returns-iterator-empty");

    [Fact(DisplayName = "returns-iterator")]
    public Task returns_iterator()
        => ExecutionTestFromFile("returns-iterator");

    [Fact(DisplayName = "this-not-object-throw-boolean")]
    public Task this_not_object_throw_boolean()
        => ExecutionTestFromFile("this-not-object-throw-boolean");

    [Fact(DisplayName = "this-not-object-throw-null")]
    public Task this_not_object_throw_null()
        => ExecutionTestFromFile("this-not-object-throw-null");

    [Fact(DisplayName = "this-not-object-throw-number")]
    public Task this_not_object_throw_number()
        => ExecutionTestFromFile("this-not-object-throw-number");

    [Fact(DisplayName = "this-not-object-throw-string")]
    public Task this_not_object_throw_string()
        => ExecutionTestFromFile("this-not-object-throw-string");

    [Fact(DisplayName = "this-not-object-throw-symbol")]
    public Task this_not_object_throw_symbol()
        => ExecutionTestFromFile("this-not-object-throw-symbol");

    [Fact(DisplayName = "this-not-object-throw-undefined")]
    public Task this_not_object_throw_undefined()
        => ExecutionTestFromFile("this-not-object-throw-undefined");

}
