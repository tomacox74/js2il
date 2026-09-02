using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.zip;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.zip") { }

    [Fact(DisplayName = "iterables-primitive.js")]
    public Task iterables_primitive() => ExecutionTestFromFile("iterables-primitive");

    [Fact(DisplayName = "iterator-non-iterable.js")]
    public Task iterator_non_iterable() => ExecutionTestFromFile("iterator-non-iterable");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

}
