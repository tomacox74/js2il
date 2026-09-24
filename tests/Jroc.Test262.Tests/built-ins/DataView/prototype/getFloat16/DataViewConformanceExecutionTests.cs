using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.getFloat16;

public class DataViewConformanceExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceExecutionTests() : base("built_ins.DataView.prototype.getFloat16") { }

    [Fact(DisplayName = "detached-buffer-after-toindex-byteoffset.js")]
    public Task detached_buffer_after_toindex_byteoffset() => ExecutionTestFromFile("detached-buffer-after-toindex-byteoffset");

    [Fact(DisplayName = "index-is-out-of-range.js")]
    public Task index_is_out_of_range() => ExecutionTestFromFile("index-is-out-of-range");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "minus-zero.js")]
    public Task minus_zero() => ExecutionTestFromFile("minus-zero");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-byteoffset-throws.js")]
    public Task negative_byteoffset_throws() => ExecutionTestFromFile("negative-byteoffset-throws");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset.js")]
    public Task return_abrupt_from_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "return-infinity.js")]
    public Task return_infinity() => ExecutionTestFromFile("return-infinity");

    [Fact(DisplayName = "return-nan.js")]
    public Task return_nan() => ExecutionTestFromFile("return-nan");

    [Fact(DisplayName = "return-value-clean-arraybuffer.js")]
    public Task return_value_clean_arraybuffer() => ExecutionTestFromFile("return-value-clean-arraybuffer");

    [Fact(DisplayName = "return-values-custom-offset.js")]
    public Task return_values_custom_offset() => ExecutionTestFromFile("return-values-custom-offset");

    [Fact(DisplayName = "return-values.js")]
    public Task return_values() => ExecutionTestFromFile("return-values");

    [Fact(DisplayName = "to-boolean-littleendian.js")]
    public Task to_boolean_littleendian() => ExecutionTestFromFile("to-boolean-littleendian");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");

}
