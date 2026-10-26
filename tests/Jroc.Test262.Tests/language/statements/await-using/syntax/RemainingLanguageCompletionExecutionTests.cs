namespace Jroc.Test262.Tests.language.statements.await_using.syntax;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.await_using.syntax") { }

    [Fact(DisplayName = "await-using-allowed-at-top-level-of-module.js")]
    public Task await_using_allowed_at_top_level_of_module()
        => ExecutionTest("await-using-allowed-at-top-level-of-module");

    [Fact(DisplayName = "await-using-allows-bindingidentifier.js")]
    public Task await_using_allows_bindingidentifier()
        => ExecutionTest("await-using-allows-bindingidentifier");

    [Fact(DisplayName = "await-using-allows-multiple-bindings.js")]
    public Task await_using_allows_multiple_bindings()
        => ExecutionTest("await-using-allows-multiple-bindings");

    [Fact(DisplayName = "await-using-declaring-let-split-across-two-lines.js")]
    public Task await_using_declaring_let_split_across_two_lines()
        => ExecutionTest("await-using-declaring-let-split-across-two-lines");

    [Fact(DisplayName = "await-using-invalid-arraybindingpattern-does-not-break-element-access.js")]
    public Task await_using_invalid_arraybindingpattern_does_not_break_element_access()
        => ExecutionTest("await-using-invalid-arraybindingpattern-does-not-break-element-access");

    [Fact(DisplayName = "await-using-invalid-assignment-next-expression-for.js")]
    public Task await_using_invalid_assignment_next_expression_for()
        => ExecutionTest("await-using-invalid-assignment-next-expression-for");

    [Fact(DisplayName = "await-using-invalid-assignment-statement-body-for-of.js")]
    public Task await_using_invalid_assignment_statement_body_for_of()
        => ExecutionTest("await-using-invalid-assignment-statement-body-for-of");

    [Fact(DisplayName = "await-using-not-allowed-at-top-level-of-eval.js", Skip = "Requires unsupported eval.")]
    public Task await_using_not_allowed_at_top_level_of_eval()
        => ExecutionTest("await-using-not-allowed-at-top-level-of-eval");

    [Fact(DisplayName = "await-using-outer-inner-using-bindings.js")]
    public Task await_using_outer_inner_using_bindings()
        => ExecutionTest("await-using-outer-inner-using-bindings");

    [Fact(DisplayName = "await-using.js")]
    public Task await_using()
        => ExecutionTest("await-using");
}
