using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.isView;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer.isView") { }

    [Fact(DisplayName = "arg-is-dataview-subclass-instance.js")]
    public Task arg_is_dataview_subclass_instance() => ExecutionTestFromFile("arg-is-dataview-subclass-instance");

    [Fact(DisplayName = "arg-is-typedarray-buffer.js")]
    public Task arg_is_typedarray_buffer() => ExecutionTestFromFile("arg-is-typedarray-buffer");

    [Fact(DisplayName = "arg-is-typedarray-constructor.js")]
    public Task arg_is_typedarray_constructor() => ExecutionTestFromFile("arg-is-typedarray-constructor");

    [Fact(DisplayName = "arg-is-typedarray-subclass-instance.js")]
    public Task arg_is_typedarray_subclass_instance() => ExecutionTestFromFile("arg-is-typedarray-subclass-instance");

    [Fact(DisplayName = "invoked-as-a-fn.js")]
    public Task invoked_as_a_fn() => ExecutionTestFromFile("invoked-as-a-fn");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
