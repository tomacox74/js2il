namespace Jroc.Test262.Tests.language.expressions.assignment;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.assignment") { }

    [Fact(DisplayName = "S11.13.1_A6_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_1_A6_T1()
        => ExecutionTest("S11.13.1_A6_T1");

    [Fact(DisplayName = "S11.13.1_A6_T2.js", Skip = "eval is not supported.")]
    public Task S11_13_1_A6_T2()
        => ExecutionTest("S11.13.1_A6_T2");

    [Fact(DisplayName = "fn-name-lhs-cover.js")]
    public Task fn_name_lhs_cover()
        => ExecutionTest("fn-name-lhs-cover");

    [Fact(DisplayName = "target-super-computed-reference-null.js")]
    public Task target_super_computed_reference_null()
        => ExecutionTest("target-super-computed-reference-null");

    [Fact(DisplayName = "target-super-computed-reference.js")]
    public Task target_super_computed_reference()
        => ExecutionTest("target-super-computed-reference");

    [Fact(DisplayName = "target-super-identifier-reference-null.js")]
    public Task target_super_identifier_reference_null()
        => ExecutionTest("target-super-identifier-reference-null");
}
