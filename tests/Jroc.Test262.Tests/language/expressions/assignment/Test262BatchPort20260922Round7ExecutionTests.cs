using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.assignment;

public class Test262BatchPort20260922Round7ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260922Round7ExecutionTests() : base("language.expressions.assignment") { }

    [Fact(DisplayName = "target-member-computed-reference-null")]
    public Task target_member_computed_reference_null()
        => ExecutionTest("target-member-computed-reference-null");

    [Fact(DisplayName = "target-member-computed-reference-undefined")]
    public Task target_member_computed_reference_undefined()
        => ExecutionTest("target-member-computed-reference-undefined");

    [Fact(DisplayName = "target-member-identifier-reference-undefined")]
    public Task target_member_identifier_reference_undefined()
        => ExecutionTest("target-member-identifier-reference-undefined");

    [Fact(DisplayName = "white-space")]
    public Task white_space()
        => ExecutionTest("white-space");

}
