using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.keys;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.keys") { }

    [Fact(DisplayName = "returns-iterator")]
    public Task returns_iterator()
        => ExecutionTestFromFile("returns-iterator");
}
