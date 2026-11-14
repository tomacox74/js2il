using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.set") { }

    [Fact(DisplayName = "this-backed-by-resizable-buffer.js")]
    public Task this_backed_by_resizable_buffer() => ExecutionTestFromFile("this-backed-by-resizable-buffer");

    [Fact(DisplayName = "typedarray-arg-src-backed-by-resizable-buffer.js")]
    public Task typedarray_arg_src_backed_by_resizable_buffer() => ExecutionTestFromFile("typedarray-arg-src-backed-by-resizable-buffer");


    [Fact(DisplayName = "array-arg-targetbuffer-detached-on-get-src-value-no-throw.js")]
    public Task Port400_array_arg_targetbuffer_detached_on_get_src_value_no_throw()
        => ExecutionTestFromFile("array-arg-targetbuffer-detached-on-get-src-value-no-throw");

    [Fact(DisplayName = "array-arg-targetbuffer-detached-on-tointeger-offset-throws.js")]
    public Task Port400_array_arg_targetbuffer_detached_on_tointeger_offset_throws()
        => ExecutionTestFromFile("array-arg-targetbuffer-detached-on-tointeger-offset-throws");

    [Fact(DisplayName = "array-arg-targetbuffer-detached-throws.js")]
    public Task Port400_array_arg_targetbuffer_detached_throws()
        => ExecutionTestFromFile("array-arg-targetbuffer-detached-throws");

    [Fact(DisplayName = "typedarray-arg-srcbuffer-detached-during-tointeger-offset-throws.js")]
    public Task Port400_typedarray_arg_srcbuffer_detached_during_tointeger_offset_throws()
        => ExecutionTestFromFile("typedarray-arg-srcbuffer-detached-during-tointeger-offset-throws");

    [Fact(DisplayName = "typedarray-arg-targetbuffer-detached-during-tointeger-offset-throws.js")]
    public Task Port400_typedarray_arg_targetbuffer_detached_during_tointeger_offset_throws()
        => ExecutionTestFromFile("typedarray-arg-targetbuffer-detached-during-tointeger-offset-throws");
}
