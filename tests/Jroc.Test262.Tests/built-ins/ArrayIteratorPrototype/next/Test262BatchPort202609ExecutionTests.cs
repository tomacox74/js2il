using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayIteratorPrototype.next;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.ArrayIteratorPrototype.next") { }

    [Fact(DisplayName = "Uint16Array")]
    public Task Uint16Array()
        => ExecutionTestFromFile("Uint16Array");

    [Fact(DisplayName = "Uint32Array")]
    public Task Uint32Array()
        => ExecutionTestFromFile("Uint32Array");

    [Fact(DisplayName = "Uint8ClampedArray")]
    public Task Uint8ClampedArray()
        => ExecutionTestFromFile("Uint8ClampedArray");

    [Fact(DisplayName = "iteration-mutable")]
    public Task iteration_mutable()
        => ExecutionTestFromFile("iteration-mutable");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-own-slots")]
    public Task non_own_slots()
        => ExecutionTestFromFile("non-own-slots");

}
