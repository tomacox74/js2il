using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.MapIteratorPrototype.next;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.MapIteratorPrototype.next") { }

    [Fact(DisplayName = "does-not-have-mapiterator-internal-slots-map")]
    public Task does_not_have_mapiterator_internal_slots_map()
        => ExecutionTestFromFile("does-not-have-mapiterator-internal-slots-map");

    [Fact(DisplayName = "does-not-have-mapiterator-internal-slots")]
    public Task does_not_have_mapiterator_internal_slots()
        => ExecutionTestFromFile("does-not-have-mapiterator-internal-slots");

    [Fact(DisplayName = "iteration")]
    public Task iteration()
        => ExecutionTestFromFile("iteration");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

}
