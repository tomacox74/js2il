using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.getInt16;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.getInt16") { }

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

}
