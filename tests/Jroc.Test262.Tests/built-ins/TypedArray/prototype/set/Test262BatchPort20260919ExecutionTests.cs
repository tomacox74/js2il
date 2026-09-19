using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.set") { }

    [Fact(DisplayName = "array-arg-offset-tointeger")]
    public Task array_arg_offset_tointeger()
        => ExecutionTestFromFile("array-arg-offset-tointeger");

    [Fact(DisplayName = "array-arg-return-abrupt-from-src-get-value")]
    public Task array_arg_return_abrupt_from_src_get_value()
        => ExecutionTestFromFile("array-arg-return-abrupt-from-src-get-value");

    [Fact(DisplayName = "array-arg-set-values-in-order")]
    public Task array_arg_set_values_in_order()
        => ExecutionTestFromFile("array-arg-set-values-in-order");

    [Fact(DisplayName = "array-arg-value-conversion-resizes-array-buffer")]
    public Task array_arg_value_conversion_resizes_array_buffer()
        => ExecutionTestFromFile("array-arg-value-conversion-resizes-array-buffer");

    [Fact(DisplayName = "invoked-as-func")]
    public Task invoked_as_func()
        => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method()
        => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

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
