using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Number;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "15.7.3-1")]
    public Task _15_7_3_1()
        => ExecutionTest("15.7.3-1");

    [Fact(DisplayName = "15.7.3-2")]
    public Task _15_7_3_2()
        => ExecutionTest("15.7.3-2");

    [Fact(DisplayName = "15.7.4-1")]
    public Task _15_7_4_1()
        => ExecutionTest("15.7.4-1");

    [Fact(DisplayName = "EPSILON", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task EPSILON()
        => ExecutionTest("EPSILON");

    [Fact(DisplayName = "NaN")]
    public Task NaN()
        => ExecutionTest("NaN");

    [Fact(DisplayName = "parseFloat", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task parseFloat()
        => ExecutionTest("parseFloat");

    [Fact(DisplayName = "parseInt", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task parseInt()
        => ExecutionTest("parseInt");

    [Fact(DisplayName = "return-abrupt-tonumber-value", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task return_abrupt_tonumber_value()
        => ExecutionTest("return-abrupt-tonumber-value");

}
