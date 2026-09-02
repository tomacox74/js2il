using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.buffer;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.buffer") { }

    [Fact(DisplayName = "return-buffer-sab.js")]
    public Task return_buffer_sab() => ExecutionTestFromFile("return-buffer-sab");

    [Fact(DisplayName = "this-has-no-dataview-internal-sab.js")]
    public Task this_has_no_dataview_internal_sab() => ExecutionTestFromFile("this-has-no-dataview-internal-sab");

}
