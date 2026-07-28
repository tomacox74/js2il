using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.copyWithin;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.copyWithin") { }

    [Fact(DisplayName = "bit-precision")]
    public Task bit_precision() => ExecutionTestFromFile("bit-precision");

    [Fact(DisplayName = "byteoffset")]
    public Task byteoffset() => ExecutionTestFromFile("byteoffset");

    [Fact(DisplayName = "coerced-values-end")]
    public Task coerced_values_end() => ExecutionTestFromFile("coerced-values-end");

    [Fact(DisplayName = "coerced-values-start")]
    public Task coerced_values_start() => ExecutionTestFromFile("coerced-values-start");

    [Fact(DisplayName = "coerced-values-target")]
    public Task coerced_values_target() => ExecutionTestFromFile("coerced-values-target");

    [Fact(DisplayName = "get-length-ignores-length-prop")]
    public Task get_length_ignores_length_prop() => ExecutionTestFromFile("get-length-ignores-length-prop");

    [Fact(DisplayName = "invoked-as-func")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method() => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-end")]
    public Task negative_end() => ExecutionTestFromFile("negative-end");

    [Fact(DisplayName = "negative-out-of-bounds-end")]
    public Task negative_out_of_bounds_end() => ExecutionTestFromFile("negative-out-of-bounds-end");

    [Fact(DisplayName = "negative-out-of-bounds-start")]
    public Task negative_out_of_bounds_start() => ExecutionTestFromFile("negative-out-of-bounds-start");

    [Fact(DisplayName = "negative-out-of-bounds-target")]
    public Task negative_out_of_bounds_target() => ExecutionTestFromFile("negative-out-of-bounds-target");

    [Fact(DisplayName = "negative-start")]
    public Task negative_start() => ExecutionTestFromFile("negative-start");

    [Fact(DisplayName = "negative-target")]
    public Task negative_target() => ExecutionTestFromFile("negative-target");

    [Fact(DisplayName = "non-negative-out-of-bounds-end")]
    public Task non_negative_out_of_bounds_end() => ExecutionTestFromFile("non-negative-out-of-bounds-end");

    [Fact(DisplayName = "non-negative-out-of-bounds-target-and-start")]
    public Task non_negative_out_of_bounds_target_and_start() => ExecutionTestFromFile("non-negative-out-of-bounds-target-and-start");

    [Fact(DisplayName = "non-negative-target-and-start")]
    public Task non_negative_target_and_start() => ExecutionTestFromFile("non-negative-target-and-start");

    [Fact(DisplayName = "non-negative-target-start-and-end")]
    public Task non_negative_target_start_and_end() => ExecutionTestFromFile("non-negative-target-start-and-end");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-abrupt-from-target")]
    public Task return_abrupt_from_target() => ExecutionTestFromFile("return-abrupt-from-target");

    [Fact(DisplayName = "return-this")]
    public Task return_this() => ExecutionTestFromFile("return-this");

    [Fact(DisplayName = "undefined-end")]
    public Task undefined_end() => ExecutionTestFromFile("undefined-end");
}
