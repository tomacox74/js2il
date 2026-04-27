using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_in;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_in") { }

    [Fact(DisplayName = "12.6.4-1", Skip = "Known JS2IL defect")]
    public Task _12_6_4_1()
        => ExecutionTest("12.6.4-1");

    [Fact(DisplayName = "12.6.4-2", Skip = "Known JS2IL defect")]
    public Task _12_6_4_2()
        => ExecutionTest("12.6.4-2");

    [Fact(DisplayName = "head-const-fresh-binding-per-iteration")]
    public Task head_const_fresh_binding_per_iteration()
        => ExecutionTest("head-const-fresh-binding-per-iteration");
}
