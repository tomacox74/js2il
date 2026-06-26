using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.values;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.values") { }

    [Fact(DisplayName = "returns-iterator")]
    public Task returns_iterator()
        => ExecutionTestFromFile("returns-iterator");

    [Fact(DisplayName = "returns-iterator-from-object")]
    public Task returns_iterator_from_object()
        => ExecutionTestFromFile("returns-iterator-from-object");
}
