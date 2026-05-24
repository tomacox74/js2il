using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Number;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "15.7.3-1")]
    public Task _15_7_3_1()
        => ExecutionTestFromFile("15.7.3-1");

    [Fact(DisplayName = "15.7.3-2")]
    public Task _15_7_3_2()
        => ExecutionTestFromFile("15.7.3-2");

    [Fact(DisplayName = "15.7.4-1")]
    public Task _15_7_4_1()
        => ExecutionTestFromFile("15.7.4-1");

    [Fact(DisplayName = "EPSILON")]
    public Task EPSILON()
        => ExecutionTestFromFile("EPSILON");

    [Fact(DisplayName = "NaN")]
    public Task NaN()
        => ExecutionTestFromFile("NaN");

    [Fact(DisplayName = "parseFloat")]
    public Task parseFloat()
        => ExecutionTestFromFile("parseFloat");

    [Fact(DisplayName = "parseInt")]
    public Task parseInt()
        => ExecutionTestFromFile("parseInt");

    [Fact(DisplayName = "return-abrupt-tonumber-value")]
    public Task return_abrupt_tonumber_value()
        => ExecutionTestFromFile("return-abrupt-tonumber-value");

    [Fact(DisplayName = "value")]
    public Task POSITIVE_INFINITY_value()
        => ExecutionTestFromFile("POSITIVE_INFINITY/value");

    [Fact(DisplayName = "value")]
    public Task NEGATIVE_INFINITY_value()
        => ExecutionTestFromFile("NEGATIVE_INFINITY/value");

    [Fact(DisplayName = "S9.3_A1_T1")]
    public Task S9_3_A1_T1()
        => ExecutionTestFromFile("S9.3_A1_T1");

    [Fact(DisplayName = "S9.3.1_A17")]
    public Task S9_3_1_A17()
        => ExecutionTestFromFile("S9.3.1_A17");

}
