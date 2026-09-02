using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.map;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.prototype.map") { }

    [Fact(DisplayName = "callable.js")]
    public Task callable() => ExecutionTestFromFile("callable");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "iterator-already-exhausted.js")]
    public Task iterator_already_exhausted() => ExecutionTestFromFile("iterator-already-exhausted");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "mapper-args.js")]
    public Task mapper_args() => ExecutionTestFromFile("mapper-args");

    [Fact(DisplayName = "mapper-this.js")]
    public Task mapper_this() => ExecutionTestFromFile("mapper-this");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-callable-mapper.js")]
    public Task non_callable_mapper() => ExecutionTestFromFile("non-callable-mapper");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "result-is-iterator.js")]
    public Task result_is_iterator() => ExecutionTestFromFile("result-is-iterator");

    [Fact(DisplayName = "returned-iterator-yields-mapper-return-values.js")]
    public Task returned_iterator_yields_mapper_return_values() => ExecutionTestFromFile("returned-iterator-yields-mapper-return-values");

    [Fact(DisplayName = "this-plain-iterator.js")]
    public Task this_plain_iterator() => ExecutionTestFromFile("this-plain-iterator");

    [Fact(DisplayName = "underlying-iterator-advanced-in-parallel.js")]
    public Task underlying_iterator_advanced_in_parallel() => ExecutionTestFromFile("underlying-iterator-advanced-in-parallel");

    [Fact(DisplayName = "underlying-iterator-closed-in-parallel.js")]
    public Task underlying_iterator_closed_in_parallel() => ExecutionTestFromFile("underlying-iterator-closed-in-parallel");

    [Fact(DisplayName = "underlying-iterator-closed.js")]
    public Task underlying_iterator_closed() => ExecutionTestFromFile("underlying-iterator-closed");

}
