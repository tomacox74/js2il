using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.parseFloat;

public class PortNext100ExecutionTests : DiskExecutionTestsBase
{
    public PortNext100ExecutionTests() : base("built_ins.parseFloat") { }

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.1.2.3_A1_T2")]
    public Task S15_1_2_3_A1_T2()
        => ExecutionTestFromFile("S15.1.2.3_A1_T2");

    [Fact(DisplayName = "S15.1.2.3_A1_T5")]
    public Task S15_1_2_3_A1_T5()
        => ExecutionTestFromFile("S15.1.2.3_A1_T5");

    [Fact(DisplayName = "S15.1.2.3_A1_T6")]
    public Task S15_1_2_3_A1_T6()
        => ExecutionTestFromFile("S15.1.2.3_A1_T6");

    [Fact(DisplayName = "S15.1.2.3_A1_T7")]
    public Task S15_1_2_3_A1_T7()
        => ExecutionTestFromFile("S15.1.2.3_A1_T7");

    [Fact(DisplayName = "S15.1.2.3_A2_T4")]
    public Task S15_1_2_3_A2_T4()
        => ExecutionTestFromFile("S15.1.2.3_A2_T4");

    [Fact(DisplayName = "S15.1.2.3_A2_T5")]
    public Task S15_1_2_3_A2_T5()
        => ExecutionTestFromFile("S15.1.2.3_A2_T5");

    [Fact(DisplayName = "S15.1.2.3_A2_T6")]
    public Task S15_1_2_3_A2_T6()
        => ExecutionTestFromFile("S15.1.2.3_A2_T6");

    [Fact(DisplayName = "S15.1.2.3_A2_T7")]
    public Task S15_1_2_3_A2_T7()
        => ExecutionTestFromFile("S15.1.2.3_A2_T7");

    [Fact(DisplayName = "S15.1.2.3_A2_T8")]
    public Task S15_1_2_3_A2_T8()
        => ExecutionTestFromFile("S15.1.2.3_A2_T8");

    [Fact(DisplayName = "S15.1.2.3_A2_T9")]
    public Task S15_1_2_3_A2_T9()
        => ExecutionTestFromFile("S15.1.2.3_A2_T9");

    [Fact(DisplayName = "S15.1.2.3_A2_T10")]
    public Task S15_1_2_3_A2_T10()
        => ExecutionTestFromFile("S15.1.2.3_A2_T10");

    [Fact(DisplayName = "S15.1.2.3_A3_T1")]
    public Task S15_1_2_3_A3_T1()
        => ExecutionTestFromFile("S15.1.2.3_A3_T1");

    [Fact(DisplayName = "S15.1.2.3_A3_T2")]
    public Task S15_1_2_3_A3_T2()
        => ExecutionTestFromFile("S15.1.2.3_A3_T2");

    [Fact(DisplayName = "S15.1.2.3_A3_T3")]
    public Task S15_1_2_3_A3_T3()
        => ExecutionTestFromFile("S15.1.2.3_A3_T3");

    [Fact(DisplayName = "S15.1.2.3_A4_T3")]
    public Task S15_1_2_3_A4_T3()
        => ExecutionTestFromFile("S15.1.2.3_A4_T3");

    [Fact(DisplayName = "S15.1.2.3_A4_T4")]
    public Task S15_1_2_3_A4_T4()
        => ExecutionTestFromFile("S15.1.2.3_A4_T4");

    [Fact(DisplayName = "S15.1.2.3_A5_T3")]
    public Task S15_1_2_3_A5_T3()
        => ExecutionTestFromFile("S15.1.2.3_A5_T3");

    [Fact(DisplayName = "S15.1.2.3_A5_T4")]
    public Task S15_1_2_3_A5_T4()
        => ExecutionTestFromFile("S15.1.2.3_A5_T4");

}
