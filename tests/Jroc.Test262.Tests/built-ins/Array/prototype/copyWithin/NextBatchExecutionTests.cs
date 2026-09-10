using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.copyWithin;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.copyWithin") { }

    [Fact(DisplayName = "coerced-values-end")]
    public Task coerced_values_end() => ExecutionTestFromFile("coerced-values-end");

    [Fact(DisplayName = "coerced-values-start-change-start")]
    public Task coerced_values_start_change_start() => ExecutionTestFromFile("coerced-values-start-change-start");

    [Fact(DisplayName = "coerced-values-start-change-target")]
    public Task coerced_values_start_change_target() => ExecutionTestFromFile("coerced-values-start-change-target");

    [Fact(DisplayName = "fill-holes")]
    public Task fill_holes() => ExecutionTestFromFile("fill-holes");

    [Fact(DisplayName = "length-near-integer-limit")]
    public Task length_near_integer_limit() => ExecutionTestFromFile("length-near-integer-limit");

    [Fact(DisplayName = "return-abrupt-from-end-as-symbol")]
    public Task return_abrupt_from_end_as_symbol() => ExecutionTestFromFile("return-abrupt-from-end-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-end")]
    public Task return_abrupt_from_end() => ExecutionTestFromFile("return-abrupt-from-end");

    [Fact(DisplayName = "return-abrupt-from-start-as-symbol")]
    public Task return_abrupt_from_start_as_symbol() => ExecutionTestFromFile("return-abrupt-from-start-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-start")]
    public Task return_abrupt_from_start() => ExecutionTestFromFile("return-abrupt-from-start");

    [Fact(DisplayName = "return-abrupt-from-target-as-symbol")]
    public Task return_abrupt_from_target_as_symbol() => ExecutionTestFromFile("return-abrupt-from-target-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-target")]
    public Task return_abrupt_from_target() => ExecutionTestFromFile("return-abrupt-from-target");

}
