using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.catch_;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.prototype.catch") { }

    [Fact(DisplayName = "S25.4.5.1_A1.1_T1")]
    public Task S25_4_5_1_A1_1_T1() => ExecutionTestFromFile("S25.4.5.1_A1.1_T1");

    [Fact(DisplayName = "S25.4.5.1_A2.1_T1")]
    public Task S25_4_5_1_A2_1_T1() => ExecutionTestFromFile("S25.4.5.1_A2.1_T1");

    [Fact(DisplayName = "this-value-then-not-callable")]
    public Task this_value_then_not_callable() => ExecutionTestFromFile("this-value-then-not-callable");
}
