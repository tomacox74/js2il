using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.values;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.values") { }

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-set")]
    public Task does_not_have_mapdata_internal_slot_set()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-set");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-weakmap")]
    public Task does_not_have_mapdata_internal_slot_weakmap()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-weakmap");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot")]
    public Task does_not_have_mapdata_internal_slot()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "returns-iterator-empty")]
    public Task returns_iterator_empty()
        => ExecutionTestFromFile("returns-iterator-empty");

    [Fact(DisplayName = "returns-iterator")]
    public Task returns_iterator()
        => ExecutionTestFromFile("returns-iterator");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");

    [Fact(DisplayName = "values")]
    public Task values()
        => ExecutionTestFromFile("values");

}
