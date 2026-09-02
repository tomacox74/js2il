using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.find;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.prototype.find") { }

    [Fact(DisplayName = "callable.js")]
    public Task callable() => ExecutionTestFromFile("callable");

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

    [Fact(DisplayName = "predicate-args.js")]
    public Task predicate_args() => ExecutionTestFromFile("predicate-args");

    [Fact(DisplayName = "predicate-returns-falsey-then-truthy.js")]
    public Task predicate_returns_falsey_then_truthy() => ExecutionTestFromFile("predicate-returns-falsey-then-truthy");

    [Fact(DisplayName = "predicate-returns-falsey.js")]
    public Task predicate_returns_falsey() => ExecutionTestFromFile("predicate-returns-falsey");

    [Fact(DisplayName = "predicate-returns-non-boolean.js")]
    public Task predicate_returns_non_boolean() => ExecutionTestFromFile("predicate-returns-non-boolean");

    [Fact(DisplayName = "predicate-returns-truthy.js")]
    public Task predicate_returns_truthy() => ExecutionTestFromFile("predicate-returns-truthy");

    [Fact(DisplayName = "predicate-this.js")]
    public Task predicate_this() => ExecutionTestFromFile("predicate-this");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "this-non-callable-next.js")]
    public Task this_non_callable_next() => ExecutionTestFromFile("this-non-callable-next");

    [Fact(DisplayName = "this-plain-iterator.js")]
    public Task this_plain_iterator() => ExecutionTestFromFile("this-plain-iterator");

}
