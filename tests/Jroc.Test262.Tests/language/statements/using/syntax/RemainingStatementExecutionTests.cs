namespace Jroc.Test262.Tests.language.statements.using_.syntax;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.using_.syntax") { }

    [Fact(DisplayName = "using-allowed-at-top-level-of-module.js")]
    public Task test_using_allowed_at_top_level_of_module()
        => ExecutionTest("using-allowed-at-top-level-of-module");

    [Fact(DisplayName = "using-allows-bindingidentifier.js")]
    public Task test_using_allows_bindingidentifier()
        => ExecutionTest("using-allows-bindingidentifier");

    [Fact(DisplayName = "using-allows-multiple-bindings.js")]
    public Task test_using_allows_multiple_bindings()
        => ExecutionTest("using-allows-multiple-bindings");

    [Fact(DisplayName = "using-for-statement.js")]
    public Task test_using_for_statement()
        => ExecutionTest("using-for-statement");

    [Fact(DisplayName = "using-invalid-assignment-next-expression-for.js")]
    public Task test_using_invalid_assignment_next_expression_for()
        => ExecutionTest("using-invalid-assignment-next-expression-for");

    [Fact(DisplayName = "using-invalid-assignment-statement-body-for-of.js")]
    public Task test_using_invalid_assignment_statement_body_for_of()
        => ExecutionTest("using-invalid-assignment-statement-body-for-of");

    [Fact(DisplayName = "using-not-allowed-at-top-level-of-eval.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_using_not_allowed_at_top_level_of_eval()
        => ExecutionTest("using-not-allowed-at-top-level-of-eval");

    [Fact(DisplayName = "using-outer-inner-using-bindings.js")]
    public Task test_using_outer_inner_using_bindings()
        => ExecutionTest("using-outer-inner-using-bindings");

    [Fact(DisplayName = "using.js")]
    public Task test_using()
        => ExecutionTest("using");

    [Fact(DisplayName = "without-initializer-for-statement.js")]
    public Task test_without_initializer_for_statement()
        => CompilationFailureTest("without-initializer-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-if-expression-statement-else-statement.js")]
    public Task test_without_initializer_if_expression_statement_else_statement()
        => CompilationFailureTest("without-initializer-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-if-expression-statement.js")]
    public Task test_without_initializer_if_expression_statement()
        => CompilationFailureTest("without-initializer-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-label-statement.js")]
    public Task test_without_initializer_label_statement()
        => CompilationFailureTest("without-initializer-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-while-expression-statement.js")]
    public Task test_without_initializer_while_expression_statement()
        => CompilationFailureTest("without-initializer-while-expression-statement", "Failed to parse JavaScript");

}
