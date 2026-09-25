using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setInt16;

public class DataViewConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public DataViewConformanceExecutionTests() : base("built_ins.DataView.prototype.setInt16") { }

    [Fact(DisplayName = "immutable-buffer.js")]
    public Task immutable_buffer() => ExecutionTestFromFile("immutable-buffer");

}
