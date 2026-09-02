using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.concat;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.concat") { }

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

    [Fact(DisplayName = "throws-typeerror-when-iterable-not-an-object.js")]
    public Task throws_typeerror_when_iterable_not_an_object() => ExecutionTestFromFile("throws-typeerror-when-iterable-not-an-object");

    [Fact(DisplayName = "throws-typeerror-when-iterator-method-not-callable.js")]
    public Task throws_typeerror_when_iterator_method_not_callable() => ExecutionTestFromFile("throws-typeerror-when-iterator-method-not-callable");

}
