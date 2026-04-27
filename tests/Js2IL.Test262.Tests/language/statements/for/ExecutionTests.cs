using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_") { }

    [Fact(DisplayName = "12.6.3_2-3-a-ii-1")]
    public Task _12_6_3_2_3_a_ii_1()
        => ExecutionTest("12.6.3_2-3-a-ii-1");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-10")]
    public Task _12_6_3_2_3_a_ii_10()
        => ExecutionTest("12.6.3_2-3-a-ii-10");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-11")]
    public Task _12_6_3_2_3_a_ii_11()
        => ExecutionTest("12.6.3_2-3-a-ii-11");
}
