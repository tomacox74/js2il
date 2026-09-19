using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.from;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Iterator.from") { }

    [Fact(DisplayName = "iterable-primitives")]
    public Task iterable_primitives()
        => ExecutionTestFromFile("iterable-primitives");

    [Fact(DisplayName = "iterable-to-iterator-fallback")]
    public Task iterable_to_iterator_fallback()
        => ExecutionTestFromFile("iterable-to-iterator-fallback");

    [Fact(DisplayName = "result-proto")]
    public Task result_proto()
        => ExecutionTestFromFile("result-proto");

    [Fact(DisplayName = "return-method-returns-iterator-result")]
    public Task return_method_returns_iterator_result()
        => ExecutionTestFromFile("return-method-returns-iterator-result");

}
