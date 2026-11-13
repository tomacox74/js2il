using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator.prototype") { }

    [Fact(DisplayName = "initial-value.js")]
    public Task Port200_initial_value()
        => ExecutionTestFromFile("initial-value");
}
