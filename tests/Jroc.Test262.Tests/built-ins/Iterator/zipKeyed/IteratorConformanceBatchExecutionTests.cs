using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.zipKeyed;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.zipKeyed") { }

    [Fact(DisplayName = "iterables-primitive.js")]
    public Task iterables_primitive() => ExecutionTestFromFile("iterables-primitive");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

}
