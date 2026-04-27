using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.types.reference;

public class ExecutionTests : ExecutionTestsBase
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
}
