using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setFloat16;

public class DataViewConformanceExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceExecutionTests() : base("built_ins.DataView.prototype.setFloat16") { }

    [Fact(DisplayName = "detached-buffer-after-number-value.js")]
    public Task detached_buffer_after_number_value() => ExecutionTestFromFile("detached-buffer-after-number-value");

    [Fact(DisplayName = "detached-buffer-after-toindex-byteoffset.js")]
    public Task detached_buffer_after_toindex_byteoffset() => ExecutionTestFromFile("detached-buffer-after-toindex-byteoffset");

    [Fact(DisplayName = "immutable-buffer.js")]
    public Task immutable_buffer() => ExecutionTestFromFile("immutable-buffer");

    [Fact(DisplayName = "index-check-before-value-conversion.js")]
    public Task index_check_before_value_conversion() => ExecutionTestFromFile("index-check-before-value-conversion");

    [Fact(DisplayName = "index-is-out-of-range.js")]
    public Task index_is_out_of_range() => ExecutionTestFromFile("index-is-out-of-range");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-byteoffset-throws.js")]
    public Task negative_byteoffset_throws() => ExecutionTestFromFile("negative-byteoffset-throws");

    [Fact(DisplayName = "no-value-arg.js")]
    public Task no_value_arg() => ExecutionTestFromFile("no-value-arg");

    [Fact(DisplayName = "range-check-after-value-conversion.js")]
    public Task range_check_after_value_conversion() => ExecutionTestFromFile("range-check-after-value-conversion");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset.js")]
    public Task return_abrupt_from_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "return-abrupt-from-tonumber-value.js")]
    public Task return_abrupt_from_tonumber_value() => ExecutionTestFromFile("return-abrupt-from-tonumber-value");

    [Fact(DisplayName = "set-values-little-endian-order.js")]
    public Task set_values_little_endian_order() => ExecutionTestFromFile("set-values-little-endian-order");

    [Fact(DisplayName = "set-values-return-undefined.js")]
    public Task set_values_return_undefined() => ExecutionTestFromFile("set-values-return-undefined");

    [Fact(DisplayName = "to-boolean-littleendian.js")]
    public Task to_boolean_littleendian() => ExecutionTestFromFile("to-boolean-littleendian");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");

}
