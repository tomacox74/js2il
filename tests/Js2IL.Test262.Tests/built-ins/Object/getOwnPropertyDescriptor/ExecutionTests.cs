using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Object.getOwnPropertyDescriptor;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "15.2.3.3-0-1")]
    public Task _15_2_3_3_0_1()
        => ExecutionTest("15.2.3.3-0-1");

    [Fact(DisplayName = "15.2.3.3-1-1")]
    public Task _15_2_3_3_1_1()
        => ExecutionTest("15.2.3.3-1-1");

    [Fact(DisplayName = "15.2.3.3-1-2")]
    public Task _15_2_3_3_1_2()
        => ExecutionTest("15.2.3.3-1-2");

    [Fact(DisplayName = "15.2.3.3-1-3")]
    public Task _15_2_3_3_1_3()
        => ExecutionTest("15.2.3.3-1-3");
}
