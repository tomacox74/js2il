using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.set;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Map.prototype.set") { }

    [Fact(DisplayName = "append-new-values")]
    public Task append_new_values()
        => ExecutionTestFromFile("append-new-values");

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

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "replaces-a-value-normalizes-zero-key")]
    public Task replaces_a_value_normalizes_zero_key()
        => ExecutionTestFromFile("replaces-a-value-normalizes-zero-key");

    [Fact(DisplayName = "replaces-a-value-returns-map")]
    public Task replaces_a_value_returns_map()
        => ExecutionTestFromFile("replaces-a-value-returns-map");

}
