using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.function_code;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.function_code") { }

    [Fact(DisplayName = "10.4.3-1-21-s")]
    public Task _10_4_3_1_21_s()
        => ExecutionTest("10.4.3-1-21-s");

    [Fact(DisplayName = "10.4.3-1-22-s")]
    public Task _10_4_3_1_22_s()
        => ExecutionTest("10.4.3-1-22-s");

    [Fact(DisplayName = "10.4.3-1-23-s")]
    public Task _10_4_3_1_23_s()
        => ExecutionTest("10.4.3-1-23-s");

    [Fact(DisplayName = "10.4.3-1-24-s")]
    public Task _10_4_3_1_24_s()
        => ExecutionTest("10.4.3-1-24-s");

    [Fact(DisplayName = "10.4.3-1-25-s")]
    public Task _10_4_3_1_25_s()
        => ExecutionTest("10.4.3-1-25-s");

    [Fact(DisplayName = "10.4.3-1-26-s")]
    public Task _10_4_3_1_26_s()
        => ExecutionTest("10.4.3-1-26-s");

    [Fact(DisplayName = "10.4.3-1-33-s")]
    public Task _10_4_3_1_33_s()
        => ExecutionTest("10.4.3-1-33-s");

    [Fact(DisplayName = "10.4.3-1-34-s")]
    public Task _10_4_3_1_34_s()
        => ExecutionTest("10.4.3-1-34-s");

    [Fact(DisplayName = "10.4.3-1-42-s")]
    public Task _10_4_3_1_42_s()
        => ExecutionTest("10.4.3-1-42-s");

    [Fact(DisplayName = "10.4.3-1-43-s")]
    public Task _10_4_3_1_43_s()
        => ExecutionTest("10.4.3-1-43-s");
}
