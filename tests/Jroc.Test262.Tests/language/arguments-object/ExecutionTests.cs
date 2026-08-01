using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.arguments_object;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.arguments_object") { }

    [Fact(DisplayName = "ArgumentsObject_callee-descriptor-non-strict")]
    public Task ArgumentsObject_callee_descriptor_non_strict()
        => ExecutionTest("ArgumentsObject_callee-descriptor-non-strict");

    [Fact(DisplayName = "ArgumentsObject_global-TypeError")]
    public Task ArgumentsObject_global_TypeError()
        => ExecutionTest("ArgumentsObject_global-TypeError");

    [Fact(DisplayName = "10.6-12-2")]
    public Task _10_6_12_2()
        => ExecutionTest("10.6-12-2");

    [Fact(DisplayName = "10.6-13-a-2")]
    public Task _10_6_13_a_2()
        => ExecutionTest("10.6-13-a-2");

    [Fact(DisplayName = "10.6-13-a-3")]
    public Task _10_6_13_a_3()
        => ExecutionTest("10.6-13-a-3");

    [Fact(DisplayName = "10.6-13-c-2-s")]
    public Task _10_6_13_c_2_s()
        => ExecutionTest("10.6-13-c-2-s");

    [Fact(DisplayName = "10.6-13-c-3-s")]
    public Task _10_6_13_c_3_s()
        => ExecutionTest("10.6-13-c-3-s");

    [Fact(DisplayName = "10.5-7-b-2-s")]
    public Task _10_5_7_b_2_s()
        => ExecutionTest("10.5-7-b-2-s");

    [Fact(DisplayName = "10.5-7-b-3-s")]
    public Task _10_5_7_b_3_s()
        => ExecutionTest("10.5-7-b-3-s");

    [Fact(DisplayName = "10.5-7-b-4-s")]
    public Task _10_5_7_b_4_s()
        => ExecutionTest("10.5-7-b-4-s");

    [Fact(DisplayName = "10.6-5-1")]
    public Task _10_6_5_1()
        => ExecutionTest("10.6-5-1");

    [Fact(DisplayName = "10.6-6-1")]
    public Task _10_6_6_1()
        => ExecutionTest("10.6-6-1");

    [Fact(DisplayName = "10.6-10-c-ii-1-s")]
    public Task _10_6_10_c_ii_1_s()
        => ExecutionTest("10.6-10-c-ii-1-s");

    [Fact(DisplayName = "10.6-10-c-ii-1")]
    public Task _10_6_10_c_ii_1()
        => ExecutionTest("10.6-10-c-ii-1");

    [Fact(DisplayName = "10.6-10-c-ii-2")]
    public Task _10_6_10_c_ii_2()
        => ExecutionTest("10.6-10-c-ii-2");

    [Fact(DisplayName = "10.6-11-b-1")]
    public Task _10_6_11_b_1()
        => ExecutionTest("10.6-11-b-1");

    [Fact(DisplayName = "10.6-13-a-1")]
    public Task _10_6_13_a_1()
        => ExecutionTest("10.6-13-a-1");
}
