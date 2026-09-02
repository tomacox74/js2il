using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.byteOffset;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.byteOffset") { }

    [Fact(DisplayName = "return-byteoffset-sab.js")]
    public Task return_byteoffset_sab() => ExecutionTestFromFile("return-byteoffset-sab");

    [Fact(DisplayName = "this-has-no-dataview-internal-sab.js")]
    public Task this_has_no_dataview_internal_sab() => ExecutionTestFromFile("this-has-no-dataview-internal-sab");

}
