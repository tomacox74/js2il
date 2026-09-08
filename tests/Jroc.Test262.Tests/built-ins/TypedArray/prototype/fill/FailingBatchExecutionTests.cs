using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.fill;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.fill") { }

    [Fact(DisplayName = "coerced-end-detach.js")]
    public Task coerced_end_detach() => ExecutionTestFromFile("coerced-end-detach");

    [Fact(DisplayName = "coerced-start-detach.js")]
    public Task coerced_start_detach() => ExecutionTestFromFile("coerced-start-detach");

    [Fact(DisplayName = "coerced-value-detach.js")]
    public Task coerced_value_detach() => ExecutionTestFromFile("coerced-value-detach");

    [Fact(DisplayName = "coerced-value-start-end-resize.js")]
    public Task coerced_value_start_end_resize() => ExecutionTestFromFile("coerced-value-start-end-resize");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method.js")]
    public Task invoked_as_method() => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
