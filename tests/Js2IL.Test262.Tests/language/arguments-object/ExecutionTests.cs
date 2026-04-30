using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.arguments_object;

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

    [Fact(DisplayName = "10.6-13-c-3-s", Skip = "Product defect: strict arguments.callee descriptor access crashes at runtime")]
    public Task _10_6_13_c_3_s()
        => ExecutionTest("10.6-13-c-3-s");
}
