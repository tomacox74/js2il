using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.fromCharCode;

public class NextBatch100Followup6ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100Followup6ExecutionTests() : base("built_ins.String.fromCharCode") { }

    [Fact(DisplayName = "S15.5.3.2_A1")]
    public Task S15_5_3_2_A1() => ExecutionTestFromFile("S15.5.3.2_A1");

    [Fact(DisplayName = "S15.5.3.2_A2")]
    public Task S15_5_3_2_A2() => ExecutionTestFromFile("S15.5.3.2_A2");

    [Fact(DisplayName = "S15.5.3.2_A3_T1")]
    public Task S15_5_3_2_A3_T1() => ExecutionTestFromFile("S15.5.3.2_A3_T1");

    [Fact(DisplayName = "S15.5.3.2_A3_T2")]
    public Task S15_5_3_2_A3_T2() => ExecutionTestFromFile("S15.5.3.2_A3_T2");

    [Fact(DisplayName = "S15.5.3.2_A4")]
    public Task S15_5_3_2_A4() => ExecutionTestFromFile("S15.5.3.2_A4");

    [Fact(DisplayName = "S9.7_A1")]
    public Task S9_7_A1() => ExecutionTestFromFile("S9.7_A1");

    [Fact(DisplayName = "S9.7_A2.2")]
    public Task S9_7_A2_2() => ExecutionTestFromFile("S9.7_A2.2");

    [Fact(DisplayName = "S9.7_A3.1_T1")]
    public Task S9_7_A3_1_T1() => ExecutionTestFromFile("S9.7_A3.1_T1");

    [Fact(DisplayName = "S9.7_A3.1_T2")]
    public Task S9_7_A3_1_T2() => ExecutionTestFromFile("S9.7_A3.1_T2");

    [Fact(DisplayName = "S9.7_A3.1_T3")]
    public Task S9_7_A3_1_T3() => ExecutionTestFromFile("S9.7_A3.1_T3");

    [Fact(DisplayName = "S9.7_A3.1_T4")]
    public Task S9_7_A3_1_T4() => ExecutionTestFromFile("S9.7_A3.1_T4");

    [Fact(DisplayName = "S9.7_A3.2_T1")]
    public Task S9_7_A3_2_T1() => ExecutionTestFromFile("S9.7_A3.2_T1");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

}
