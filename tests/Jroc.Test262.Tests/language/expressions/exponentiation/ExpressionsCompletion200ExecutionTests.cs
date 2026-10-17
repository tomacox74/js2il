namespace Jroc.Test262.Tests.language.expressions.exponentiation;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.exponentiation") { }

    [Fact(DisplayName = "exp-operator-syntax-error-bitnot-unary-expression-base.js")]
    public Task exp_operator_syntax_error_bitnot_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-bitnot-unary-expression-base", "Failed to parse JavaScript");

    [Fact(DisplayName = "exp-operator-syntax-error-delete-unary-expression-base.js")]
    public Task exp_operator_syntax_error_delete_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-delete-unary-expression-base", "Failed to parse JavaScript");

    [Fact(DisplayName = "exp-operator-syntax-error-logical-not-unary-expression-base.js")]
    public Task exp_operator_syntax_error_logical_not_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-logical-not-unary-expression-base", "Failed to parse JavaScript");

    [Fact(DisplayName = "exp-operator-syntax-error-negate-unary-expression-base.js")]
    public Task exp_operator_syntax_error_negate_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-negate-unary-expression-base", "Failed to parse JavaScript");

    [Fact(DisplayName = "exp-operator-syntax-error-plus-unary-expression-base.js")]
    public Task exp_operator_syntax_error_plus_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-plus-unary-expression-base", "Failed to parse JavaScript");

    [Fact(DisplayName = "exp-operator-syntax-error-typeof-unary-expression-base.js")]
    public Task exp_operator_syntax_error_typeof_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-typeof-unary-expression-base", "Failed to parse JavaScript");

    [Fact(DisplayName = "exp-operator-syntax-error-void-unary-expression-base.js")]
    public Task exp_operator_syntax_error_void_unary_expression_base()
        => CompilationFailureTest("exp-operator-syntax-error-void-unary-expression-base", "Failed to parse JavaScript");
}
