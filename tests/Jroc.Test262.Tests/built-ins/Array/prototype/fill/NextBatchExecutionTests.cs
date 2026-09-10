using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.fill;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.fill") { }

    [Fact(DisplayName = "coerced-indexes")]
    public Task coerced_indexes() => ExecutionTestFromFile("coerced-indexes");

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

}
