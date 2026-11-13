using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.length;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.length") { }

    [Fact(DisplayName = "S15.3.5.1_A1_T1.js")]
    public Task Port200_S15_3_5_1_A1_T1()
        => ExecutionTestFromFile("S15.3.5.1_A1_T1");

    [Fact(DisplayName = "S15.3.5.1_A1_T2.js")]
    public Task Port200_S15_3_5_1_A1_T2()
        => ExecutionTestFromFile("S15.3.5.1_A1_T2");

    [Fact(DisplayName = "S15.3.5.1_A1_T3.js")]
    public Task Port200_S15_3_5_1_A1_T3()
        => ExecutionTestFromFile("S15.3.5.1_A1_T3");

    [Fact(DisplayName = "S15.3.5.1_A2_T1.js")]
    public Task Port200_S15_3_5_1_A2_T1()
        => ExecutionTestFromFile("S15.3.5.1_A2_T1");

    [Fact(DisplayName = "S15.3.5.1_A2_T2.js")]
    public Task Port200_S15_3_5_1_A2_T2()
        => ExecutionTestFromFile("S15.3.5.1_A2_T2");

    [Fact(DisplayName = "S15.3.5.1_A2_T3.js")]
    public Task Port200_S15_3_5_1_A2_T3()
        => ExecutionTestFromFile("S15.3.5.1_A2_T3");

    [Fact(DisplayName = "S15.3.5.1_A3_T1.js")]
    public Task Port200_S15_3_5_1_A3_T1()
        => ExecutionTestFromFile("S15.3.5.1_A3_T1");

    [Fact(DisplayName = "S15.3.5.1_A3_T2.js")]
    public Task Port200_S15_3_5_1_A3_T2()
        => ExecutionTestFromFile("S15.3.5.1_A3_T2");

    [Fact(DisplayName = "S15.3.5.1_A3_T3.js")]
    public Task Port200_S15_3_5_1_A3_T3()
        => ExecutionTestFromFile("S15.3.5.1_A3_T3");

    [Fact(DisplayName = "S15.3.5.1_A4_T1.js")]
    public Task Port200_S15_3_5_1_A4_T1()
        => ExecutionTestFromFile("S15.3.5.1_A4_T1");

    [Fact(DisplayName = "S15.3.5.1_A4_T2.js")]
    public Task Port200_S15_3_5_1_A4_T2()
        => ExecutionTestFromFile("S15.3.5.1_A4_T2");

    [Fact(DisplayName = "S15.3.5.1_A4_T3.js")]
    public Task Port200_S15_3_5_1_A4_T3()
        => ExecutionTestFromFile("S15.3.5.1_A4_T3");
}
