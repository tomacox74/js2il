namespace Jroc.Test262.Tests.language.statements.if_;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.if_") { }

    [Fact(DisplayName = "S12.5_A2.js", Skip = "eval is not supported.")]
    public Task test_S12_5_A2()
        => ExecutionTest("S12.5_A2");

    [Fact(DisplayName = "cptn-else-false-abrupt-empty.js", Skip = "eval is not supported.")]
    public Task test_cptn_else_false_abrupt_empty()
        => ExecutionTest("cptn-else-false-abrupt-empty");

    [Fact(DisplayName = "cptn-else-false-nrml.js", Skip = "eval is not supported.")]
    public Task test_cptn_else_false_nrml()
        => ExecutionTest("cptn-else-false-nrml");

    [Fact(DisplayName = "cptn-else-true-abrupt-empty.js", Skip = "eval is not supported.")]
    public Task test_cptn_else_true_abrupt_empty()
        => ExecutionTest("cptn-else-true-abrupt-empty");

    [Fact(DisplayName = "cptn-else-true-nrml.js", Skip = "eval is not supported.")]
    public Task test_cptn_else_true_nrml()
        => ExecutionTest("cptn-else-true-nrml");

    [Fact(DisplayName = "cptn-empty-statement.js", Skip = "eval is not supported.")]
    public Task test_cptn_empty_statement()
        => ExecutionTest("cptn-empty-statement");

    [Fact(DisplayName = "cptn-no-else-false.js", Skip = "eval is not supported.")]
    public Task test_cptn_no_else_false()
        => ExecutionTest("cptn-no-else-false");

    [Fact(DisplayName = "cptn-no-else-true-abrupt-empty.js", Skip = "eval is not supported.")]
    public Task test_cptn_no_else_true_abrupt_empty()
        => ExecutionTest("cptn-no-else-true-abrupt-empty");

    [Fact(DisplayName = "cptn-no-else-true-nrml.js", Skip = "eval is not supported.")]
    public Task test_cptn_no_else_true_nrml()
        => ExecutionTest("cptn-no-else-true-nrml");

    [Fact(DisplayName = "let-identifier-with-newline.js")]
    public Task test_let_identifier_with_newline()
        => ExecutionTest("let-identifier-with-newline");

}
