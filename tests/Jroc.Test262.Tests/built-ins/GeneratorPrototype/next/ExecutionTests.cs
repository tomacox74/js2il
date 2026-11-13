using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.next;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorPrototype.next") { }

    [Fact(DisplayName = "from-state-executing.js")]
    public Task Port200_from_state_executing()
        => ExecutionTestFromFile("from-state-executing");

    [Fact(DisplayName = "length.js")]
    public Task Port200_length()
        => ExecutionTestFromFile("length");
}
