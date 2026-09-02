using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.getInt32;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.getInt32") { }

    [Fact(DisplayName = "index-is-out-of-range-sab.js")]
    public Task index_is_out_of_range_sab() => ExecutionTestFromFile("index-is-out-of-range-sab");

    [Fact(DisplayName = "index-is-out-of-range.js")]
    public Task index_is_out_of_range() => ExecutionTestFromFile("index-is-out-of-range");

    [Fact(DisplayName = "negative-byteoffset-throws-sab.js")]
    public Task negative_byteoffset_throws_sab() => ExecutionTestFromFile("negative-byteoffset-throws-sab");

    [Fact(DisplayName = "negative-byteoffset-throws.js")]
    public Task negative_byteoffset_throws() => ExecutionTestFromFile("negative-byteoffset-throws");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-sab.js")]
    public Task return_abrupt_from_tonumber_byteoffset_sab() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-sab");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-symbol-sab.js")]
    public Task return_abrupt_from_tonumber_byteoffset_symbol_sab() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-symbol-sab");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-symbol.js")]
    public Task return_abrupt_from_tonumber_byteoffset_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset.js")]
    public Task return_abrupt_from_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "return-value-clean-arraybuffer-sab.js")]
    public Task return_value_clean_arraybuffer_sab() => ExecutionTestFromFile("return-value-clean-arraybuffer-sab");

    [Fact(DisplayName = "return-value-clean-arraybuffer.js")]
    public Task return_value_clean_arraybuffer() => ExecutionTestFromFile("return-value-clean-arraybuffer");

    [Fact(DisplayName = "return-values-custom-offset-sab.js")]
    public Task return_values_custom_offset_sab() => ExecutionTestFromFile("return-values-custom-offset-sab");

    [Fact(DisplayName = "return-values-custom-offset.js")]
    public Task return_values_custom_offset() => ExecutionTestFromFile("return-values-custom-offset");

    [Fact(DisplayName = "return-values-sab.js")]
    public Task return_values_sab() => ExecutionTestFromFile("return-values-sab");

    [Fact(DisplayName = "return-values.js")]
    public Task return_values() => ExecutionTestFromFile("return-values");

    [Fact(DisplayName = "this-has-no-dataview-internal-sab.js")]
    public Task this_has_no_dataview_internal_sab() => ExecutionTestFromFile("this-has-no-dataview-internal-sab");

    [Fact(DisplayName = "this-has-no-dataview-internal.js")]
    public Task this_has_no_dataview_internal() => ExecutionTestFromFile("this-has-no-dataview-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "to-boolean-littleendian-sab.js")]
    public Task to_boolean_littleendian_sab() => ExecutionTestFromFile("to-boolean-littleendian-sab");

    [Fact(DisplayName = "to-boolean-littleendian.js")]
    public Task to_boolean_littleendian() => ExecutionTestFromFile("to-boolean-littleendian");

    [Fact(DisplayName = "toindex-byteoffset-sab.js")]
    public Task toindex_byteoffset_sab() => ExecutionTestFromFile("toindex-byteoffset-sab");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");

}
