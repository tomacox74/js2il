using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.from;

public class IteratorConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public IteratorConformanceBatchExecutionTests() : base("built_ins.Iterator.from") { }

    [Fact(DisplayName = "callable.js")]
    public Task callable() => ExecutionTestFromFile("callable");

    [Fact(DisplayName = "get-next-method-only-once.js")]
    public Task get_next_method_only_once() => ExecutionTestFromFile("get-next-method-only-once");

    [Fact(DisplayName = "get-next-method-throws.js")]
    public Task get_next_method_throws() => ExecutionTestFromFile("get-next-method-throws");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-constructible.js")]
    public Task non_constructible() => ExecutionTestFromFile("non-constructible");

    [Fact(DisplayName = "primitives.js")]
    public Task primitives() => ExecutionTestFromFile("primitives");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "supports-iterable.js")]
    public Task supports_iterable() => ExecutionTestFromFile("supports-iterable");

    [Fact(DisplayName = "supports-iterator.js")]
    public Task supports_iterator() => ExecutionTestFromFile("supports-iterator");

}
