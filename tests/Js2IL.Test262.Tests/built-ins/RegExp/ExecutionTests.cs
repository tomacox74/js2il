using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.RegExp;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp") { }

    [Fact(DisplayName = "15.10.4.1-1")]
    public Task _15_10_4_1_1()
        => ExecutionTest("15.10.4.1-1");

    [Fact(DisplayName = "call_with_non_regexp_same_constructor")]
    public Task call_with_non_regexp_same_constructor()
        => ExecutionTest("call_with_non_regexp_same_constructor");

    [Fact(DisplayName = "15.10.2.15-6-1")]
    public Task _15_10_2_15_6_1()
        => ExecutionTest("15.10.2.15-6-1");

    [Fact(DisplayName = "15.10.2.5-3-1")]
    public Task _15_10_2_5_3_1()
        => ExecutionTest("15.10.2.5-3-1");

    [Fact(DisplayName = "15.10.4.1-2")]
    public Task _15_10_4_1_2()
        => ExecutionTest("15.10.4.1-2");

    [Fact(DisplayName = "15.10.4.1-3")]
    public Task _15_10_4_1_3()
        => ExecutionTest("15.10.4.1-3");

    [Fact(DisplayName = "15.10.4.1-4")]
    public Task _15_10_4_1_4()
        => ExecutionTest("15.10.4.1-4");

}
