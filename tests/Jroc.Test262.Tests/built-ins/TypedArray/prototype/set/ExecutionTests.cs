using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.set") { }

    [Fact(DisplayName = "this-backed-by-resizable-buffer.js")]
    public Task this_backed_by_resizable_buffer() => ExecutionTestFromFile("this-backed-by-resizable-buffer");

    [Fact(DisplayName = "typedarray-arg-src-backed-by-resizable-buffer.js")]
    public Task typedarray_arg_src_backed_by_resizable_buffer() => ExecutionTestFromFile("typedarray-arg-src-backed-by-resizable-buffer");

}
