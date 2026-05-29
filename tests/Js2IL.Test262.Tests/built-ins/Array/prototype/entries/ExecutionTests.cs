using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.entries;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.entries") { }

    [Fact(DisplayName = "returns-iterator")]
    public Task returns_iterator()
        => ExecutionTestFromFile("returns-iterator");

    [Fact(DisplayName = "returns-iterator-from-object")]
    public Task returns_iterator_from_object()
        => ExecutionTestFromFile("returns-iterator-from-object");
}
