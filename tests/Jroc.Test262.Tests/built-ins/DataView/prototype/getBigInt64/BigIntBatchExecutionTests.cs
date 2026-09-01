using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.getBigInt64;

public class BigIntBatchExecutionTests : DiskExecutionTestsBase
{
    public BigIntBatchExecutionTests() : base("built_ins.DataView.prototype.getBigInt64") { }

    [Fact(DisplayName = "detached-buffer-after-toindex-byteoffset.js")]
    public Task detached_buffer_after_toindex_byteoffset() => ExecutionTestFromFile("detached-buffer-after-toindex-byteoffset");

    [Fact(DisplayName = "detached-buffer-before-outofrange-byteoffset.js")]
    public Task detached_buffer_before_outofrange_byteoffset() => ExecutionTestFromFile("detached-buffer-before-outofrange-byteoffset");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "index-is-out-of-range.js")]
    public Task index_is_out_of_range() => ExecutionTestFromFile("index-is-out-of-range");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-byteoffset-throws.js")]
    public Task negative_byteoffset_throws() => ExecutionTestFromFile("negative-byteoffset-throws");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-symbol.js")]
    public Task return_abrupt_from_tonumber_byteoffset_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset.js")]
    public Task return_abrupt_from_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "return-value-clean-arraybuffer.js")]
    public Task return_value_clean_arraybuffer() => ExecutionTestFromFile("return-value-clean-arraybuffer");

    [Fact(DisplayName = "return-values-custom-offset.js")]
    public Task return_values_custom_offset() => ExecutionTestFromFile("return-values-custom-offset");

    [Fact(DisplayName = "return-values.js")]
    public Task return_values() => ExecutionTestFromFile("return-values");

    [Fact(DisplayName = "this-has-no-dataview-internal.js")]
    public Task this_has_no_dataview_internal() => ExecutionTestFromFile("this-has-no-dataview-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "to-boolean-littleendian.js")]
    public Task to_boolean_littleendian() => ExecutionTestFromFile("to-boolean-littleendian");

    [Fact(DisplayName = "toindex-byteoffset-errors.js")]
    public Task toindex_byteoffset_errors() => ExecutionTestFromFile("toindex-byteoffset-errors");

    [Fact(DisplayName = "toindex-byteoffset-toprimitive.js")]
    public Task toindex_byteoffset_toprimitive() => ExecutionTestFromFile("toindex-byteoffset-toprimitive");

    [Fact(DisplayName = "toindex-byteoffset-wrapped-values.js")]
    public Task toindex_byteoffset_wrapped_values() => ExecutionTestFromFile("toindex-byteoffset-wrapped-values");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");
}
