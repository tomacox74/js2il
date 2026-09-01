using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.resize;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.resize") { }

    [Fact(DisplayName = "descriptor.js")]
    public Task descriptor() => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "extensible.js")]
    public Task extensible() => ExecutionTestFromFile("extensible");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nonconstructor.js")]
    public Task nonconstructor() => ExecutionTestFromFile("nonconstructor");

    [Fact(DisplayName = "resize-grow.js")]
    public Task resize_grow() => ExecutionTestFromFile("resize-grow");

    [Fact(DisplayName = "resize-same-size.js")]
    public Task resize_same_size() => ExecutionTestFromFile("resize-same-size");

    [Fact(DisplayName = "resize-same-size-zero-explicit.js")]
    public Task resize_same_size_zero_explicit() => ExecutionTestFromFile("resize-same-size-zero-explicit");

    [Fact(DisplayName = "resize-same-size-zero-implicit.js")]
    public Task resize_same_size_zero_implicit() => ExecutionTestFromFile("resize-same-size-zero-implicit");

    [Fact(DisplayName = "resize-shrink.js")]
    public Task resize_shrink() => ExecutionTestFromFile("resize-shrink");

    [Fact(DisplayName = "resize-shrink-zero-explicit.js")]
    public Task resize_shrink_zero_explicit() => ExecutionTestFromFile("resize-shrink-zero-explicit");

    [Fact(DisplayName = "resize-shrink-zero-implicit.js")]
    public Task resize_shrink_zero_implicit() => ExecutionTestFromFile("resize-shrink-zero-implicit");

    [Fact(DisplayName = "this-is-not-arraybuffer-object.js")]
    public Task this_is_not_arraybuffer_object() => ExecutionTestFromFile("this-is-not-arraybuffer-object");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-resizable-arraybuffer-object.js")]
    public Task this_is_not_resizable_arraybuffer_object() => ExecutionTestFromFile("this-is-not-resizable-arraybuffer-object");

}
