using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.StringIteratorPrototype.next;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.StringIteratorPrototype.next") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "next-iteration-surrogate-pairs")]
    public Task next_iteration_surrogate_pairs()
        => ExecutionTestFromFile("next-iteration-surrogate-pairs");

    [Fact(DisplayName = "next-iteration")]
    public Task next_iteration()
        => ExecutionTestFromFile("next-iteration");

    [Fact(DisplayName = "next-missing-internal-slots")]
    public Task next_missing_internal_slots()
        => ExecutionTestFromFile("next-missing-internal-slots");

}
