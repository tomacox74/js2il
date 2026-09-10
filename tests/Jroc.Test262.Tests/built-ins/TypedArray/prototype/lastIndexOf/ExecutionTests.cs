using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.lastIndexOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.lastIndexOf") { }

    [Fact(DisplayName = "resizable-buffer-special-float-values.js")]
    public Task resizable_buffer_special_float_values() => ExecutionTestFromFile("resizable-buffer-special-float-values");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

}
