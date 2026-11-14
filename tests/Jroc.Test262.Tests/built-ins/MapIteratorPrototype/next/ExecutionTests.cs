using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.MapIteratorPrototype.next;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.MapIteratorPrototype.next") { }

    [Fact(DisplayName = "iteration-mutable.js")]
    public Task Port200_iteration_mutable()
        => ExecutionTestFromFile("iteration-mutable");
}
