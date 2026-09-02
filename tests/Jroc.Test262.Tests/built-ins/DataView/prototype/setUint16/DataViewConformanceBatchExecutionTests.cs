using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setUint16;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView.prototype.setUint16") { }

    [Fact(DisplayName = "index-is-out-of-range.js")]
    public Task index_is_out_of_range() => ExecutionTestFromFile("index-is-out-of-range");

    [Fact(DisplayName = "negative-byteoffset-throws.js")]
    public Task negative_byteoffset_throws() => ExecutionTestFromFile("negative-byteoffset-throws");

    [Fact(DisplayName = "no-value-arg.js")]
    public Task no_value_arg() => ExecutionTestFromFile("no-value-arg");

    [Fact(DisplayName = "range-check-after-value-conversion.js")]
    public Task range_check_after_value_conversion() => ExecutionTestFromFile("range-check-after-value-conversion");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-symbol.js")]
    public Task return_abrupt_from_tonumber_byteoffset_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset.js")]
    public Task return_abrupt_from_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "return-abrupt-from-tonumber-value-symbol.js")]
    public Task return_abrupt_from_tonumber_value_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-value-symbol");

    [Fact(DisplayName = "return-abrupt-from-tonumber-value.js")]
    public Task return_abrupt_from_tonumber_value() => ExecutionTestFromFile("return-abrupt-from-tonumber-value");

    [Fact(DisplayName = "set-values-little-endian-order.js")]
    public Task set_values_little_endian_order() => ExecutionTestFromFile("set-values-little-endian-order");

    [Fact(DisplayName = "set-values-return-undefined.js")]
    public Task set_values_return_undefined() => ExecutionTestFromFile("set-values-return-undefined");

    [Fact(DisplayName = "this-has-no-dataview-internal.js")]
    public Task this_has_no_dataview_internal() => ExecutionTestFromFile("this-has-no-dataview-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "to-boolean-littleendian.js")]
    public Task to_boolean_littleendian() => ExecutionTestFromFile("to-boolean-littleendian");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");

}
