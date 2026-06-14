using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.hasOwnProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.hasOwnProperty") { }

    [Fact(DisplayName = "8.12.1-1_1")]
    public Task _8_12_1_1_1()
        => ExecutionTestFromFile("8.12.1-1_1");

    [Fact(DisplayName = "8.12.1-1_10")]
    public Task _8_12_1_1_10()
        => ExecutionTestFromFile("8.12.1-1_10");

    [Fact(DisplayName = "8.12.1-1_11")]
    public Task _8_12_1_1_11()
        => ExecutionTestFromFile("8.12.1-1_11");

    [Fact(DisplayName = "8.12.1-1_12")]
    public Task _8_12_1_1_12()
        => ExecutionTestFromFile("8.12.1-1_12");

    [Fact(DisplayName = "8.12.1-1_13")]
    public Task _8_12_1_1_13()
        => ExecutionTestFromFile("8.12.1-1_13");

    [Fact(DisplayName = "8.12.1-1_14")]
    public Task _8_12_1_1_14()
        => ExecutionTestFromFile("8.12.1-1_14");

    [Fact(DisplayName = "8.12.1-1_15")]
    public Task _8_12_1_1_15()
        => ExecutionTestFromFile("8.12.1-1_15");

    [Fact(DisplayName = "8.12.1-1_2")]
    public Task _8_12_1_1_2()
        => ExecutionTestFromFile("8.12.1-1_2");

    [Fact(DisplayName = "8.12.1-1_3")]
    public Task _8_12_1_1_3()
        => ExecutionTestFromFile("8.12.1-1_3");

    [Fact(DisplayName = "8.12.1-1_4")]
    public Task _8_12_1_1_4()
        => ExecutionTestFromFile("8.12.1-1_4");

    [Fact(DisplayName = "8.12.1-1_5")]
    public Task _8_12_1_1_5()
        => ExecutionTestFromFile("8.12.1-1_5");

    [Fact(DisplayName = "8.12.1-1_6")]
    public Task _8_12_1_1_6()
        => ExecutionTestFromFile("8.12.1-1_6");

    [Fact(DisplayName = "8.12.1-1_7")]
    public Task _8_12_1_1_7()
        => ExecutionTestFromFile("8.12.1-1_7");

    [Fact(DisplayName = "8.12.1-1_8")]
    public Task _8_12_1_1_8()
        => ExecutionTestFromFile("8.12.1-1_8");

    [Fact(DisplayName = "8.12.1-1_9")]
    public Task _8_12_1_1_9()
        => ExecutionTestFromFile("8.12.1-1_9");

    [Fact(DisplayName = "S15.2.4.5_A12")]
    public Task S15_2_4_5_A12()
        => ExecutionTestFromFile("S15.2.4.5_A12");

    [Fact(DisplayName = "S15.2.4.5_A13")]
    public Task S15_2_4_5_A13()
        => ExecutionTestFromFile("S15.2.4.5_A13");

    [Fact(DisplayName = "S15.2.4.5_A1_T1")]
    public Task S15_2_4_5_A1_T1()
        => ExecutionTestFromFile("S15.2.4.5_A1_T1");

    [Fact(DisplayName = "S15.2.4.5_A1_T2")]
    public Task S15_2_4_5_A1_T2()
        => ExecutionTestFromFile("S15.2.4.5_A1_T2");

    [Fact(DisplayName = "S15.2.4.5_A1_T3")]
    public Task S15_2_4_5_A1_T3()
        => ExecutionTestFromFile("S15.2.4.5_A1_T3");

    [Fact(DisplayName = "symbol_own_property")]
    public Task symbol_own_property()
        => ExecutionTestFromFile("symbol_own_property");
}
