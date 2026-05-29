using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.get;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.get") { }

    [Fact(DisplayName = "get")]
    public Task get()
        => ExecutionTestFromFile("get");

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined()
        => ExecutionTestFromFile("returns-undefined");

    [Fact(DisplayName = "returns-value-normalized-zero-key")]
    public Task returns_value_normalized_zero_key()
        => ExecutionTestFromFile("returns-value-normalized-zero-key");

    [Fact(DisplayName = "returns-value-different-key-types")]
    public Task returns_value_different_key_types()
        => ExecutionTestFromFile("returns-value-different-key-types");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");
    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-set")]
    public Task does_not_have_mapdata_internal_slot_set()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-set");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-weakmap")]
    public Task does_not_have_mapdata_internal_slot_weakmap()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-weakmap");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot")]
    public Task does_not_have_mapdata_internal_slot()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot");

}
