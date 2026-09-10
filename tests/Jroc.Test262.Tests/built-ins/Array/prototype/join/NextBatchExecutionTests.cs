using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.join;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.join") { }

    [Fact(DisplayName = "S15.4.4.5_A1.2_T1")]
    public Task S15_4_4_5_A1_2_T1() => ExecutionTestFromFile("S15.4.4.5_A1.2_T1");

    [Fact(DisplayName = "S15.4.4.5_A1.2_T2")]
    public Task S15_4_4_5_A1_2_T2() => ExecutionTestFromFile("S15.4.4.5_A1.2_T2");

    [Fact(DisplayName = "S15.4.4.5_A1.3_T1")]
    public Task S15_4_4_5_A1_3_T1() => ExecutionTestFromFile("S15.4.4.5_A1.3_T1");

    [Fact(DisplayName = "S15.4.4.5_A3.2_T1")]
    public Task S15_4_4_5_A3_2_T1() => ExecutionTestFromFile("S15.4.4.5_A3.2_T1");

    [Fact(DisplayName = "coerced-separator-grow")]
    public Task coerced_separator_grow() => ExecutionTestFromFile("coerced-separator-grow");

    [Fact(DisplayName = "coerced-separator-shrink")]
    public Task coerced_separator_shrink() => ExecutionTestFromFile("coerced-separator-shrink");

}
