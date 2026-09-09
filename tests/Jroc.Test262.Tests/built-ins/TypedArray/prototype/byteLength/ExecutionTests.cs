using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.byteLength;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.byteLength") { }

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer()
        => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "resizable-buffer-assorted")]
    public Task resizable_buffer_assorted()
        => ExecutionTestFromFile("resizable-buffer-assorted");

    [Fact(DisplayName = "resized-out-of-bounds-1")]
    public Task resized_out_of_bounds_1()
        => ExecutionTestFromFile("resized-out-of-bounds-1");

    [Fact(DisplayName = "resized-out-of-bounds-2")]
    public Task resized_out_of_bounds_2()
        => ExecutionTestFromFile("resized-out-of-bounds-2");
}
