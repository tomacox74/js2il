using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Function.prototype.bind;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype.bind") { }

[Fact(DisplayName = "15.3.4.5-0-1")]
    public Task _15_3_4_5_0_1()
        => ExecutionTest("15.3.4.5-0-1");

[Fact(DisplayName = "15.3.4.5-10-1")]
    public Task _15_3_4_5_10_1()
        => ExecutionTest("15.3.4.5-10-1");

[Fact(DisplayName = "15.3.4.5-11-1")]
    public Task _15_3_4_5_11_1()
        => ExecutionTest("15.3.4.5-11-1");

[Fact(DisplayName = "15.3.4.5-16-1")]
    public Task _15_3_4_5_16_1()
        => ExecutionTest("15.3.4.5-16-1");

[Fact(DisplayName = "15.3.4.5-16-2")]
    public Task _15_3_4_5_16_2()
        => ExecutionTest("15.3.4.5-16-2");

[Fact(DisplayName = "15.3.4.5-2-1")]
    public Task _15_3_4_5_2_1()
        => ExecutionTest("15.3.4.5-2-1");

[Fact(DisplayName = "15.3.4.5-2-10")]
    public Task _15_3_4_5_2_10()
        => ExecutionTest("15.3.4.5-2-10");

[Fact(DisplayName = "15.3.4.5-2-11")]
    public Task _15_3_4_5_2_11()
        => ExecutionTest("15.3.4.5-2-11");
}
