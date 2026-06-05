using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.size;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.size") { }

    [Fact(DisplayName = "size")]
    public Task size()
        => ExecutionTestFromFile("size");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "returns-count-of-present-values-by-insertion")]
    public Task returns_count_of_present_values_by_insertion()
        => ExecutionTestFromFile("returns-count-of-present-values-by-insertion");

    [Fact(DisplayName = "returns-count-of-present-values-by-iterable")]
    public Task returns_count_of_present_values_by_iterable()
        => ExecutionTestFromFile("returns-count-of-present-values-by-iterable");

    [Fact(DisplayName = "returns-count-of-present-values-before-after-set-delete")]
    public Task returns_count_of_present_values_before_after_set_delete()
        => ExecutionTestFromFile("returns-count-of-present-values-before-after-set-delete");

    [Fact(DisplayName = "returns-count-of-present-values-before-after-set-clear")]
    public Task returns_count_of_present_values_before_after_set_clear()
        => ExecutionTestFromFile("returns-count-of-present-values-before-after-set-clear");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot")]
    public Task does_not_have_mapdata_internal_slot()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-set")]
    public Task does_not_have_mapdata_internal_slot_set()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-set");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-weakmap")]
    public Task does_not_have_mapdata_internal_slot_weakmap()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-weakmap");

}
