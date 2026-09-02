using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.drop;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.prototype.drop") { }

    [Fact(DisplayName = "callable.js")]
    public Task callable() => ExecutionTestFromFile("callable");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "limit-equals-total.js")]
    public Task limit_equals_total() => ExecutionTestFromFile("limit-equals-total");

    [Fact(DisplayName = "limit-greater-than-total.js")]
    public Task limit_greater_than_total() => ExecutionTestFromFile("limit-greater-than-total");

    [Fact(DisplayName = "limit-less-than-total.js")]
    public Task limit_less_than_total() => ExecutionTestFromFile("limit-less-than-total");

    [Fact(DisplayName = "limit-tonumber-throws.js")]
    public Task limit_tonumber_throws() => ExecutionTestFromFile("limit-tonumber-throws");

    [Fact(DisplayName = "limit-tonumber.js")]
    public Task limit_tonumber() => ExecutionTestFromFile("limit-tonumber");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "result-is-iterator.js")]
    public Task result_is_iterator() => ExecutionTestFromFile("result-is-iterator");

    [Fact(DisplayName = "this-plain-iterator.js")]
    public Task this_plain_iterator() => ExecutionTestFromFile("this-plain-iterator");

    [Fact(DisplayName = "underlying-iterator-advanced-in-parallel.js")]
    public Task underlying_iterator_advanced_in_parallel() => ExecutionTestFromFile("underlying-iterator-advanced-in-parallel");

    [Fact(DisplayName = "underlying-iterator-closed-in-parallel.js")]
    public Task underlying_iterator_closed_in_parallel() => ExecutionTestFromFile("underlying-iterator-closed-in-parallel");

    [Fact(DisplayName = "underlying-iterator-closed.js")]
    public Task underlying_iterator_closed() => ExecutionTestFromFile("underlying-iterator-closed");

}
