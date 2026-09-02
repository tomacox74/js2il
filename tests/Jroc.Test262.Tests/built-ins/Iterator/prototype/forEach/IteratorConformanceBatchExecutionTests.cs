using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.forEach;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.prototype.forEach") { }

    [Fact(DisplayName = "callable.js")]
    public Task callable() => ExecutionTestFromFile("callable");

    [Fact(DisplayName = "fn-args.js")]
    public Task fn_args() => ExecutionTestFromFile("fn-args");

    [Fact(DisplayName = "fn-called-for-each-yielded-value.js")]
    public Task fn_called_for_each_yielded_value() => ExecutionTestFromFile("fn-called-for-each-yielded-value");

    [Fact(DisplayName = "fn-this.js")]
    public Task fn_this() => ExecutionTestFromFile("fn-this");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "iterator-already-exhausted.js")]
    public Task iterator_already_exhausted() => ExecutionTestFromFile("iterator-already-exhausted");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-callable-predicate.js")]
    public Task non_callable_predicate() => ExecutionTestFromFile("non-callable-predicate");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "result-is-undefined.js")]
    public Task result_is_undefined() => ExecutionTestFromFile("result-is-undefined");

    [Fact(DisplayName = "this-non-callable-next.js")]
    public Task this_non_callable_next() => ExecutionTestFromFile("this-non-callable-next");

    [Fact(DisplayName = "this-plain-iterator.js")]
    public Task this_plain_iterator() => ExecutionTestFromFile("this-plain-iterator");

}
