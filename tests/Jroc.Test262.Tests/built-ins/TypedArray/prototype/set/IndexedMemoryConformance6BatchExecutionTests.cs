using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.@set;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.set") { }

    [Fact(DisplayName = "array-arg-src-tonumber-value-conversions.js")]
    public Task array_arg_src_tonumber_value_conversions() => ExecutionTestFromFile("array-arg-src-tonumber-value-conversions");

    [Fact(DisplayName = "src-typedarray-big-throws.js")]
    public Task src_typedarray_big_throws() => ExecutionTestFromFile("src-typedarray-big-throws");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance.js")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");

    [Fact(DisplayName = "typedarray-arg-negative-integer-offset-throws.js")]
    public Task typedarray_arg_negative_integer_offset_throws() => ExecutionTestFromFile("typedarray-arg-negative-integer-offset-throws");

    [Fact(DisplayName = "typedarray-arg-return-abrupt-from-tointeger-offset-symbol.js")]
    public Task typedarray_arg_return_abrupt_from_tointeger_offset_symbol() => ExecutionTestFromFile("typedarray-arg-return-abrupt-from-tointeger-offset-symbol");

    [Fact(DisplayName = "typedarray-arg-return-abrupt-from-tointeger-offset.js")]
    public Task typedarray_arg_return_abrupt_from_tointeger_offset() => ExecutionTestFromFile("typedarray-arg-return-abrupt-from-tointeger-offset");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-other-type-conversions-sab.js")]
    public Task typedarray_arg_set_values_diff_buffer_other_type_conversions_sab() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-other-type-conversions-sab");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-other-type-conversions.js")]
    public Task typedarray_arg_set_values_diff_buffer_other_type_conversions() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-other-type-conversions");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-other-type-sab.js")]
    public Task typedarray_arg_set_values_diff_buffer_other_type_sab() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-other-type-sab");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-other-type.js")]
    public Task typedarray_arg_set_values_diff_buffer_other_type() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-other-type");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-same-type-sab.js")]
    public Task typedarray_arg_set_values_diff_buffer_same_type_sab() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-same-type-sab");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-same-type.js")]
    public Task typedarray_arg_set_values_diff_buffer_same_type() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-same-type");

    [Fact(DisplayName = "typedarray-arg-set-values-same-buffer-other-type.js")]
    public Task typedarray_arg_set_values_same_buffer_other_type() => ExecutionTestFromFile("typedarray-arg-set-values-same-buffer-other-type");

    [Fact(DisplayName = "typedarray-arg-set-values-same-buffer-same-type-sab.js")]
    public Task typedarray_arg_set_values_same_buffer_same_type_sab() => ExecutionTestFromFile("typedarray-arg-set-values-same-buffer-same-type-sab");

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
}
