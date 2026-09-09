using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray") { }

    [Fact(DisplayName = "resizable-buffer-length-tracking-1.js")]
    public Task resizable_buffer_length_tracking_1() => ExecutionTestFromFile("resizable-buffer-length-tracking-1");

    [Fact(DisplayName = "resizable-buffer-length-tracking-2.js")]
    public Task resizable_buffer_length_tracking_2() => ExecutionTestFromFile("resizable-buffer-length-tracking-2");

}
