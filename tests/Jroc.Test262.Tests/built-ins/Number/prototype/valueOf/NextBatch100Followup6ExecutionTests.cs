using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype.valueOf;

public class NextBatch100Followup6ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100Followup6ExecutionTests() : base("built_ins.Number.prototype.valueOf") { }

    [Fact(DisplayName = "S15.7.4.4_A1_T01")]
    public Task S15_7_4_4_A1_T01() => ExecutionTestFromFile("S15.7.4.4_A1_T01");

    [Fact(DisplayName = "S15.7.4.4_A1_T02")]
    public Task S15_7_4_4_A1_T02() => ExecutionTestFromFile("S15.7.4.4_A1_T02");

    [Fact(DisplayName = "S15.7.4.4_A2_T02")]
    public Task S15_7_4_4_A2_T02() => ExecutionTestFromFile("S15.7.4.4_A2_T02");

    [Fact(DisplayName = "S15.7.4.4_A2_T03")]
    public Task S15_7_4_4_A2_T03() => ExecutionTestFromFile("S15.7.4.4_A2_T03");

    [Fact(DisplayName = "S15.7.4.4_A2_T04")]
    public Task S15_7_4_4_A2_T04() => ExecutionTestFromFile("S15.7.4.4_A2_T04");

    [Fact(DisplayName = "S15.7.4.4_A2_T05")]
    public Task S15_7_4_4_A2_T05() => ExecutionTestFromFile("S15.7.4.4_A2_T05");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

}
