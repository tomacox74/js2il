using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setBigInt64;

public class BigIntBatchExecutionTests : DiskExecutionTestsBase
{
    public BigIntBatchExecutionTests() : base("built_ins.DataView.prototype.setBigInt64") { }

    [Fact(DisplayName = "detached-buffer-after-bigint-value.js")]
    public Task detached_buffer_after_bigint_value() => ExecutionTestFromFile("detached-buffer-after-bigint-value");

    [Fact(DisplayName = "detached-buffer-after-toindex-byteoffset.js")]
    public Task detached_buffer_after_toindex_byteoffset() => ExecutionTestFromFile("detached-buffer-after-toindex-byteoffset");

    [Fact(DisplayName = "detached-buffer-before-outofrange-byteoffset.js")]
    public Task detached_buffer_before_outofrange_byteoffset() => ExecutionTestFromFile("detached-buffer-before-outofrange-byteoffset");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

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

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "range-check-after-value-conversion.js")]
    public Task range_check_after_value_conversion() => ExecutionTestFromFile("range-check-after-value-conversion");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-tobigint-value-symbol.js")]
    public Task return_abrupt_from_tobigint_value_symbol() => ExecutionTestFromFile("return-abrupt-from-tobigint-value-symbol");

    [Fact(DisplayName = "return-abrupt-from-tobigint-value.js")]
    public Task return_abrupt_from_tobigint_value() => ExecutionTestFromFile("return-abrupt-from-tobigint-value");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset-symbol.js")]
    public Task return_abrupt_from_tonumber_byteoffset_symbol() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "return-abrupt-from-tonumber-byteoffset.js")]
    public Task return_abrupt_from_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-from-tonumber-byteoffset");

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
