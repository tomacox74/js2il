using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set;

public class TypedArraySecondFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArraySecondFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.set") { }

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

    [Fact(DisplayName = "bit-precision.js")]
    public Task bit_precision() => ExecutionTestFromFile("bit-precision");

}
