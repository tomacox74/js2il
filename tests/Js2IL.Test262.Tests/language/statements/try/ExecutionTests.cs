using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.try_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.try_") { }

    [Fact(DisplayName = "12.14-10")]
    public Task _12_14_10()
        => ExecutionTest("12.14-10");

    [Fact(DisplayName = "12.14-11")]
    public Task _12_14_11()
        => ExecutionTest("12.14-11");

    [Fact(DisplayName = "12.14-12")]
    public Task _12_14_12()
        => ExecutionTest("12.14-12");
}
