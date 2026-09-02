using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.byteLength;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.byteLength") { }

    [Fact(DisplayName = "return-bytelength-sab.js")]
    public Task return_bytelength_sab() => ExecutionTestFromFile("return-bytelength-sab");

    [Fact(DisplayName = "this-has-no-dataview-internal-sab.js")]
    public Task this_has_no_dataview_internal_sab() => ExecutionTestFromFile("this-has-no-dataview-internal-sab");

}
