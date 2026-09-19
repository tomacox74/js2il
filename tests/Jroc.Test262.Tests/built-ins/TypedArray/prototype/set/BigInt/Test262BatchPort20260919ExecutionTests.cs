using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.set.BigInt") { }

    [Fact(DisplayName = "array-arg-offset-tointeger")]
    public Task array_arg_offset_tointeger()
        => ExecutionTestFromFile("array-arg-offset-tointeger");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-get-value")]
    public Task array_arg_return_abrupt_from_src_get_value()
        => ExecutionTestFromFile("array-arg-return-abrupt-from-src-get-value");

    [Fact(DisplayName = "array-arg-set-values-in-order")]
    public Task array_arg_set_values_in_order()
        => ExecutionTestFromFile("array-arg-set-values-in-order");

    [Fact(DisplayName = "typedarray-arg-offset-tointeger")]
    public Task typedarray_arg_offset_tointeger()
        => ExecutionTestFromFile("typedarray-arg-offset-tointeger");

    [Fact(DisplayName = "typedarray-arg-set-values-same-buffer-same-type-resized")]
    public Task typedarray_arg_set_values_same_buffer_same_type_resized()
        => ExecutionTestFromFile("typedarray-arg-set-values-same-buffer-same-type-resized");

    [Fact(DisplayName = "typedarray-arg-target-out-of-bounds")]
    public Task typedarray_arg_target_out_of_bounds()
        => ExecutionTestFromFile("typedarray-arg-target-out-of-bounds");

}
