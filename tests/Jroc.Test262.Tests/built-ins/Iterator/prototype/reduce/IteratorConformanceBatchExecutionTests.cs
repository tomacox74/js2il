using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.reduce;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.prototype.reduce") { }

    [Fact(DisplayName = "callable.js")]
    public Task callable() => ExecutionTestFromFile("callable");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "iterator-already-exhausted-initial-value.js")]
    public Task iterator_already_exhausted_initial_value() => ExecutionTestFromFile("iterator-already-exhausted-initial-value");

    [Fact(DisplayName = "iterator-already-exhausted-no-initial-value.js")]
    public Task iterator_already_exhausted_no_initial_value() => ExecutionTestFromFile("iterator-already-exhausted-no-initial-value");

    [Fact(DisplayName = "iterator-yields-once-initial-value.js")]
    public Task iterator_yields_once_initial_value() => ExecutionTestFromFile("iterator-yields-once-initial-value");

    [Fact(DisplayName = "iterator-yields-once-no-initial-value.js")]
    public Task iterator_yields_once_no_initial_value() => ExecutionTestFromFile("iterator-yields-once-no-initial-value");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-callable-reducer.js")]
    public Task non_callable_reducer() => ExecutionTestFromFile("non-callable-reducer");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "reducer-args-initial-value.js")]
    public Task reducer_args_initial_value() => ExecutionTestFromFile("reducer-args-initial-value");

    [Fact(DisplayName = "reducer-args-no-initial-value.js")]
    public Task reducer_args_no_initial_value() => ExecutionTestFromFile("reducer-args-no-initial-value");

    [Fact(DisplayName = "reducer-memo-can-be-any-type.js")]
    public Task reducer_memo_can_be_any_type() => ExecutionTestFromFile("reducer-memo-can-be-any-type");

    [Fact(DisplayName = "reducer-this.js")]
    public Task reducer_this() => ExecutionTestFromFile("reducer-this");

    [Fact(DisplayName = "this-non-callable-next.js")]
    public Task this_non_callable_next() => ExecutionTestFromFile("this-non-callable-next");

    [Fact(DisplayName = "this-plain-iterator.js")]
    public Task this_plain_iterator() => ExecutionTestFromFile("this-plain-iterator");

}
