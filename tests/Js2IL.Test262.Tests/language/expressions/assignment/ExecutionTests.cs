using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.assignment;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.assignment") { }

    [Fact(DisplayName = "8.12.5-3-b_1", Skip = "Known JS2IL defect")]
    public Task _8_12_5_3_b_1()
        => ExecutionTest("8.12.5-3-b_1");

    [Fact(DisplayName = "8.12.5-3-b_2", Skip = "Known JS2IL defect")]
    public Task _8_12_5_3_b_2()
        => ExecutionTest("8.12.5-3-b_2");

    [Fact(DisplayName = "8.12.5-5-b_1", Skip = "Known JS2IL defect")]
    public Task _8_12_5_5_b_1()
        => ExecutionTest("8.12.5-5-b_1");

    [Fact(DisplayName = "8.14.4-8-b_1", Skip = "Known JS2IL defect")]
    public Task _8_14_4_8_b_1()
        => ExecutionTest("8.14.4-8-b_1");
}
