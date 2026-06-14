using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.function_code;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.function_code") { }

    [Fact(DisplayName = "10.4.3-1-5-s")]
    public Task _10_4_3_1_5_s()
        => ExecutionTest("10.4.3-1-5-s");

    [Fact(DisplayName = "10.4.3-1-54-s")]
    public Task _10_4_3_1_54_s()
        => ExecutionTest("10.4.3-1-54-s");

    [Fact(DisplayName = "10.4.3-1-55-s")]
    public Task _10_4_3_1_55_s()
        => ExecutionTest("10.4.3-1-55-s");

    [Fact(DisplayName = "10.4.3-1-56-s")]
    public Task _10_4_3_1_56_s()
        => ExecutionTest("10.4.3-1-56-s");

    [Fact(DisplayName = "10.4.3-1-57-s")]
    public Task _10_4_3_1_57_s()
        => ExecutionTest("10.4.3-1-57-s");

    [Fact(DisplayName = "10.4.3-1-58-s")]
    public Task _10_4_3_1_58_s()
        => ExecutionTest("10.4.3-1-58-s");

    [Fact(DisplayName = "10.4.3-1-59-s")]
    public Task _10_4_3_1_59_s()
        => ExecutionTest("10.4.3-1-59-s");

    [Fact(DisplayName = "10.4.3-1-60-s")]
    public Task _10_4_3_1_60_s()
        => ExecutionTest("10.4.3-1-60-s");

    [Fact(DisplayName = "10.4.3-1-61-s")]
    public Task _10_4_3_1_61_s()
        => ExecutionTest("10.4.3-1-61-s");
}
