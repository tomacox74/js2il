using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set.BigInt;

public class TypedArraySecondFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArraySecondFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.set.BigInt") { }

    [Fact(DisplayName = "array-arg-negative-integer-offset-throws.js")]
    public Task array_arg_negative_integer_offset_throws() => ExecutionTestFromFile("array-arg-negative-integer-offset-throws");

    [Fact(DisplayName = "array-arg-primitive-toobject.js")]
    public Task array_arg_primitive_toobject() => ExecutionTestFromFile("array-arg-primitive-toobject");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-get-length.js")]
    public Task array_arg_return_abrupt_from_src_get_length() => ExecutionTestFromFile("array-arg-return-abrupt-from-src-get-length");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-length-symbol.js")]
    public Task array_arg_return_abrupt_from_src_length_symbol() => ExecutionTestFromFile("array-arg-return-abrupt-from-src-length-symbol");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-length.js")]
    public Task array_arg_return_abrupt_from_src_length() => ExecutionTestFromFile("array-arg-return-abrupt-from-src-length");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-tonumber-value-symbol.js")]
    public Task array_arg_return_abrupt_from_src_tonumber_value_symbol() => ExecutionTestFromFile("array-arg-return-abrupt-from-src-tonumber-value-symbol");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-tonumber-value.js")]
    public Task array_arg_return_abrupt_from_src_tonumber_value() => ExecutionTestFromFile("array-arg-return-abrupt-from-src-tonumber-value");

    [Fact(DisplayName = "array-arg-return-abrupt-from-tointeger-offset-symbol.js")]
    public Task array_arg_return_abrupt_from_tointeger_offset_symbol() => ExecutionTestFromFile("array-arg-return-abrupt-from-tointeger-offset-symbol");

    [Fact(DisplayName = "array-arg-return-abrupt-from-tointeger-offset.js")]
    public Task array_arg_return_abrupt_from_tointeger_offset() => ExecutionTestFromFile("array-arg-return-abrupt-from-tointeger-offset");

    [Fact(DisplayName = "array-arg-return-abrupt-from-toobject-offset.js")]
    public Task array_arg_return_abrupt_from_toobject_offset() => ExecutionTestFromFile("array-arg-return-abrupt-from-toobject-offset");

    [Fact(DisplayName = "array-arg-set-values.js")]
    public Task array_arg_set_values() => ExecutionTestFromFile("array-arg-set-values");

    [Fact(DisplayName = "array-arg-src-tonumber-value-type-conversions.js")]
    public Task array_arg_src_tonumber_value_type_conversions() => ExecutionTestFromFile("array-arg-src-tonumber-value-type-conversions");

    [Fact(DisplayName = "array-arg-src-values-are-not-cached.js")]
    public Task array_arg_src_values_are_not_cached() => ExecutionTestFromFile("array-arg-src-values-are-not-cached");

    [Fact(DisplayName = "array-arg-target-arraylength-internal.js")]
    public Task array_arg_target_arraylength_internal() => ExecutionTestFromFile("array-arg-target-arraylength-internal");

    [Fact(DisplayName = "bigint-tobigint64.js")]
    public Task bigint_tobigint64() => ExecutionTestFromFile("bigint-tobigint64");

    [Fact(DisplayName = "bigint-tobiguint64.js")]
    public Task bigint_tobiguint64() => ExecutionTestFromFile("bigint-tobiguint64");

    [Fact(DisplayName = "boolean-tobigint.js")]
    public Task boolean_tobigint() => ExecutionTestFromFile("boolean-tobigint");

    [Fact(DisplayName = "null-tobigint.js")]
    public Task null_tobigint() => ExecutionTestFromFile("null-tobigint");

    [Fact(DisplayName = "number-tobigint.js")]
    public Task number_tobigint() => ExecutionTestFromFile("number-tobigint");

    [Fact(DisplayName = "src-typedarray-big.js")]
    public Task src_typedarray_big() => ExecutionTestFromFile("src-typedarray-big");

    [Fact(DisplayName = "src-typedarray-not-big-throws.js")]
    public Task src_typedarray_not_big_throws() => ExecutionTestFromFile("src-typedarray-not-big-throws");

    [Fact(DisplayName = "string-nan-tobigint.js")]
    public Task string_nan_tobigint() => ExecutionTestFromFile("string-nan-tobigint");

    [Fact(DisplayName = "string-tobigint.js")]
    public Task string_tobigint() => ExecutionTestFromFile("string-tobigint");

    [Fact(DisplayName = "symbol-tobigint.js")]
    public Task symbol_tobigint() => ExecutionTestFromFile("symbol-tobigint");

    [Fact(DisplayName = "typedarray-arg-negative-integer-offset-throws.js")]
    public Task typedarray_arg_negative_integer_offset_throws() => ExecutionTestFromFile("typedarray-arg-negative-integer-offset-throws");

    [Fact(DisplayName = "typedarray-arg-return-abrupt-from-tointeger-offset-symbol.js")]
    public Task typedarray_arg_return_abrupt_from_tointeger_offset_symbol() => ExecutionTestFromFile("typedarray-arg-return-abrupt-from-tointeger-offset-symbol");

    [Fact(DisplayName = "typedarray-arg-return-abrupt-from-tointeger-offset.js")]
    public Task typedarray_arg_return_abrupt_from_tointeger_offset() => ExecutionTestFromFile("typedarray-arg-return-abrupt-from-tointeger-offset");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-other-type.js")]
    public Task typedarray_arg_set_values_diff_buffer_other_type() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-other-type");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-same-type.js")]
    public Task typedarray_arg_set_values_diff_buffer_same_type() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-same-type");

    [Fact(DisplayName = "typedarray-arg-set-values-same-buffer-same-type.js")]
    public Task typedarray_arg_set_values_same_buffer_same_type() => ExecutionTestFromFile("typedarray-arg-set-values-same-buffer-same-type");

    [Fact(DisplayName = "typedarray-arg-src-arraylength-internal.js")]
    public Task typedarray_arg_src_arraylength_internal() => ExecutionTestFromFile("typedarray-arg-src-arraylength-internal");

    [Fact(DisplayName = "typedarray-arg-src-byteoffset-internal.js")]
    public Task typedarray_arg_src_byteoffset_internal() => ExecutionTestFromFile("typedarray-arg-src-byteoffset-internal");

    [Fact(DisplayName = "typedarray-arg-src-range-greather-than-target-throws-rangeerror.js")]
    public Task typedarray_arg_src_range_greather_than_target_throws_rangeerror() => ExecutionTestFromFile("typedarray-arg-src-range-greather-than-target-throws-rangeerror");

    [Fact(DisplayName = "typedarray-arg-target-arraylength-internal.js")]
    public Task typedarray_arg_target_arraylength_internal() => ExecutionTestFromFile("typedarray-arg-target-arraylength-internal");

    [Fact(DisplayName = "typedarray-arg-target-byteoffset-internal.js")]
    public Task typedarray_arg_target_byteoffset_internal() => ExecutionTestFromFile("typedarray-arg-target-byteoffset-internal");

    [Fact(DisplayName = "undefined-tobigint.js")]
    public Task undefined_tobigint() => ExecutionTestFromFile("undefined-tobigint");

}
