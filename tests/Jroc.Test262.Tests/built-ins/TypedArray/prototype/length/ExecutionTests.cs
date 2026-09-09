using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.length;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.length") { }

    [Fact(DisplayName = "resizable-buffer-assorted.js")]
    public Task resizable_buffer_assorted() => ExecutionTestFromFile("resizable-buffer-assorted");

}
