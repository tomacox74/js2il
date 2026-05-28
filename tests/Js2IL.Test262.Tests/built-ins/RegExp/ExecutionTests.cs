using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.RegExp;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp") { }

    [Fact(DisplayName = "15.10.4.1-1")]
    public Task _15_10_4_1_1()
        => ExecutionTestFromFile("15.10.4.1-1");

    [Fact(DisplayName = "call_with_non_regexp_same_constructor")]
    public Task call_with_non_regexp_same_constructor()
        => ExecutionTestFromFile("call_with_non_regexp_same_constructor");

    [Fact(DisplayName = "15.10.2.15-6-1")]
    public Task _15_10_2_15_6_1()
        => ExecutionTestFromFile("15.10.2.15-6-1");

    [Fact(DisplayName = "15.10.2.5-3-1")]
    public Task _15_10_2_5_3_1()
        => ExecutionTestFromFile("15.10.2.5-3-1");

    [Fact(DisplayName = "15.10.4.1-2")]
    public Task _15_10_4_1_2()
        => ExecutionTestFromFile("15.10.4.1-2");

    [Fact(DisplayName = "15.10.4.1-3")]
    public Task _15_10_4_1_3()
        => ExecutionTestFromFile("15.10.4.1-3");

    [Fact(DisplayName = "15.10.4.1-4")]
    public Task _15_10_4_1_4()
        => ExecutionTestFromFile("15.10.4.1-4");

    [Fact(DisplayName = "S15.10.1_A1_T1")]
    public Task S15_10_1_A1_T1()
        => ExecutionTestFromFile("S15.10.1_A1_T1");

    [Fact(DisplayName = "S15.10.1_A1_T10")]
    public Task S15_10_1_A1_T10()
        => ExecutionTestFromFile("S15.10.1_A1_T10");

    [Fact(DisplayName = "S15.10.2.10_A1.1_T1")]
    public Task S15_10_2_10_A1_1_T1()
        => ExecutionTestFromFile("S15.10.2.10_A1.1_T1");

    [Fact(DisplayName = "S15.10.2.11_A1_T1")]
    public Task S15_10_2_11_A1_T1()
        => ExecutionTestFromFile("S15.10.2.11_A1_T1");

    [Fact(DisplayName = "S15.10.2.6_A1_T5")]
    public Task S15_10_2_6_A1_T5()
        => ExecutionTestFromFile("S15.10.2.6_A1_T5");

    [Fact(DisplayName = "S15.10.2.15_A1_T20")]
    public Task S15_10_2_15_A1_T20()
        => ExecutionTestFromFile("S15.10.2.15_A1_T20");
    [Fact(DisplayName = "S15.10.1_A1_T2")]
    public Task S15_10_1_A1_T2()
        => ExecutionTestFromFile("S15.10.1_A1_T2");
    [Fact(DisplayName = "S15.10.1_A1_T3")]
    public Task S15_10_1_A1_T3()
        => ExecutionTestFromFile("S15.10.1_A1_T3");
    [Fact(DisplayName = "S15.10.1_A1_T4")]
    public Task S15_10_1_A1_T4()
        => ExecutionTestFromFile("S15.10.1_A1_T4");
    [Fact(DisplayName = "S15.10.1_A1_T5")]
    public Task S15_10_1_A1_T5()
        => ExecutionTestFromFile("S15.10.1_A1_T5");
    [Fact(DisplayName = "S15.10.1_A1_T6")]
    public Task S15_10_1_A1_T6()
        => ExecutionTestFromFile("S15.10.1_A1_T6");
    [Fact(DisplayName = "S15.10.1_A1_T7")]
    public Task S15_10_1_A1_T7()
        => ExecutionTestFromFile("S15.10.1_A1_T7");
    [Fact(DisplayName = "S15.10.1_A1_T8")]
    public Task S15_10_1_A1_T8()
        => ExecutionTestFromFile("S15.10.1_A1_T8");
    [Fact(DisplayName = "S15.10.1_A1_T9")]
    public Task S15_10_1_A1_T9()
        => ExecutionTestFromFile("S15.10.1_A1_T9");

}
