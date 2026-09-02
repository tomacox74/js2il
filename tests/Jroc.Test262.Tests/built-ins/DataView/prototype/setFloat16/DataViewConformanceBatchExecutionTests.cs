using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setFloat16;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.setFloat16") { }

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-symbol.js")]
    public Task return_abrupt_from_tonumber_byteoffset_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "return-abrupt-from-tonumber-value-symbol.js")]
    public Task return_abrupt_from_tonumber_value_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-value-symbol");

    [Fact(DisplayName = "this-has-no-dataview-internal.js")]
    public Task this_has_no_dataview_internal() => ExecutionTestFromFile("this-has-no-dataview-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

}
