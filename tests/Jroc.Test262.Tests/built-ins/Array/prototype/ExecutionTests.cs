using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype") { }

    [Fact(DisplayName = "Array_prototype_entries_sparse")]
    public Task Array_prototype_entries_sparse()
        => ExecutionTest("Array_prototype_entries_sparse");
}
