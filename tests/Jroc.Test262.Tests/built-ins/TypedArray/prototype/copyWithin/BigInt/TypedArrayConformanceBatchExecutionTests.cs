using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.copyWithin.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.copyWithin.BigInt") { }

    [Fact(DisplayName = "coerced-values-end.js")]
    public Task coerced_values_end() => ExecutionTestFromFile("coerced-values-end");

    [Fact(DisplayName = "coerced-values-start.js")]
    public Task coerced_values_start() => ExecutionTestFromFile("coerced-values-start");

    [Fact(DisplayName = "coerced-values-target.js")]
    public Task coerced_values_target() => ExecutionTestFromFile("coerced-values-target");

    [Fact(DisplayName = "get-length-ignores-length-prop.js")]
    public Task get_length_ignores_length_prop() => ExecutionTestFromFile("get-length-ignores-length-prop");

    [Fact(DisplayName = "negative-end.js")]
    public Task negative_end() => ExecutionTestFromFile("negative-end");

    [Fact(DisplayName = "negative-out-of-bounds-end.js")]
    public Task negative_out_of_bounds_end() => ExecutionTestFromFile("negative-out-of-bounds-end");

    [Fact(DisplayName = "negative-out-of-bounds-start.js")]
    public Task negative_out_of_bounds_start() => ExecutionTestFromFile("negative-out-of-bounds-start");

    [Fact(DisplayName = "negative-out-of-bounds-target.js")]
    public Task negative_out_of_bounds_target() => ExecutionTestFromFile("negative-out-of-bounds-target");

    [Fact(DisplayName = "negative-start.js")]
    public Task negative_start() => ExecutionTestFromFile("negative-start");

    [Fact(DisplayName = "negative-target.js")]
    public Task negative_target() => ExecutionTestFromFile("negative-target");

    [Fact(DisplayName = "non-negative-out-of-bounds-end.js")]
    public Task non_negative_out_of_bounds_end() => ExecutionTestFromFile("non-negative-out-of-bounds-end");

    [Fact(DisplayName = "non-negative-out-of-bounds-target-and-start.js")]
    public Task non_negative_out_of_bounds_target_and_start() => ExecutionTestFromFile("non-negative-out-of-bounds-target-and-start");

    [Fact(DisplayName = "non-negative-target-and-start.js")]
    public Task non_negative_target_and_start() => ExecutionTestFromFile("non-negative-target-and-start");

    [Fact(DisplayName = "non-negative-target-start-and-end.js")]
    public Task non_negative_target_start_and_end() => ExecutionTestFromFile("non-negative-target-start-and-end");

    [Fact(DisplayName = "return-abrupt-from-end-is-symbol.js")]
    public Task return_abrupt_from_end_is_symbol() => ExecutionTestFromFile("return-abrupt-from-end-is-symbol");

    [Fact(DisplayName = "return-abrupt-from-end.js")]
    public Task return_abrupt_from_end() => ExecutionTestFromFile("return-abrupt-from-end");

    [Fact(DisplayName = "return-abrupt-from-start-is-symbol.js")]
    public Task return_abrupt_from_start_is_symbol() => ExecutionTestFromFile("return-abrupt-from-start-is-symbol");

    [Fact(DisplayName = "return-abrupt-from-start.js")]
    public Task return_abrupt_from_start() => ExecutionTestFromFile("return-abrupt-from-start");

    [Fact(DisplayName = "return-abrupt-from-target-is-symbol.js")]
    public Task return_abrupt_from_target_is_symbol() => ExecutionTestFromFile("return-abrupt-from-target-is-symbol");

    [Fact(DisplayName = "return-abrupt-from-target.js")]
    public Task return_abrupt_from_target() => ExecutionTestFromFile("return-abrupt-from-target");

    [Fact(DisplayName = "return-this.js")]
    public Task return_this() => ExecutionTestFromFile("return-this");

    [Fact(DisplayName = "undefined-end.js")]
    public Task undefined_end() => ExecutionTestFromFile("undefined-end");

}
