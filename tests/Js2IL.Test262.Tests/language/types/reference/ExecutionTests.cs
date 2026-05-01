using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.types.reference;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.types.reference") { }

    [Fact(DisplayName = "8.7.2-2-s")]
    public Task _8_7_2_2_s()
        => ExecutionTest("8.7.2-2-s");

    [Fact(DisplayName = "8.7.2-6-s")]
    public Task _8_7_2_6_s()
        => ExecutionTest("8.7.2-6-s");

    [Fact(DisplayName = "8.7.2-7-s")]
    public Task _8_7_2_7_s()
        => ExecutionTest("8.7.2-7-s");

    [Fact(DisplayName = "8.7.2-8-s")]
    public Task _8_7_2_8_s()
        => ExecutionTest("8.7.2-8-s");

    [Fact(DisplayName = "8.7.2-3-a-1gs")]
    public Task _8_7_2_3_a_1gs()
        => ExecutionTest("8.7.2-3-a-1gs");

    [Fact(DisplayName = "8.7.2-3-a-2gs")]
    public Task _8_7_2_3_a_2gs()
        => ExecutionTest("8.7.2-3-a-2gs");

    [Fact(DisplayName = "8.7.2-5-s")]
    public Task _8_7_2_5_s()
        => ExecutionTest("8.7.2-5-s");

    [Fact(DisplayName = "get-value-prop-base-primitive")]
    public Task get_value_prop_base_primitive()
        => ExecutionTest("get-value-prop-base-primitive");
}
