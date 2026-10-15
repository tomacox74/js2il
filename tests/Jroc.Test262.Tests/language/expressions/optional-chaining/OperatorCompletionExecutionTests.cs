using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.optional_chaining;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.optional_chaining") { }

    [Fact(DisplayName = "call-expression-super-no-base.js")]
    public Task test_call_expression_super_no_base() => CompilationFailureTest("call-expression-super-no-base", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-null-op-template-string-esi.js")]
    public Task test_early_errors_tail_position_null_op_template_string_esi() => CompilationFailureTest("early-errors-tail-position-null-op-template-string-esi", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-null-op-template-string.js")]
    public Task test_early_errors_tail_position_null_op_template_string() => CompilationFailureTest("early-errors-tail-position-null-op-template-string", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-null-optchain-template-string-esi.js")]
    public Task test_early_errors_tail_position_null_optchain_template_string_esi() => CompilationFailureTest("early-errors-tail-position-null-optchain-template-string-esi", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-null-optchain-template-string.js")]
    public Task test_early_errors_tail_position_null_optchain_template_string() => CompilationFailureTest("early-errors-tail-position-null-optchain-template-string", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-op-template-string-esi.js")]
    public Task test_early_errors_tail_position_op_template_string_esi() => CompilationFailureTest("early-errors-tail-position-op-template-string-esi", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-op-template-string.js")]
    public Task test_early_errors_tail_position_op_template_string() => CompilationFailureTest("early-errors-tail-position-op-template-string", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-optchain-template-string-esi.js")]
    public Task test_early_errors_tail_position_optchain_template_string_esi() => CompilationFailureTest("early-errors-tail-position-optchain-template-string-esi", string.Empty);

    [Fact(DisplayName = "early-errors-tail-position-optchain-template-string.js")]
    public Task test_early_errors_tail_position_optchain_template_string() => CompilationFailureTest("early-errors-tail-position-optchain-template-string", string.Empty);

    [Fact(DisplayName = "eval-optional-call.js", Skip = "eval is not supported.")]
    public Task test_eval_optional_call() => ExecutionTest("eval-optional-call");

    [Fact(DisplayName = "iteration-statement-for-await-of.js")]
    public Task test_iteration_statement_for_await_of() => ExecutionTest("iteration-statement-for-await-of");

    [Fact(DisplayName = "member-expression-async-identifier.js")]
    public Task test_member_expression_async_identifier() => ExecutionTest("member-expression-async-identifier");

    [Fact(DisplayName = "member-expression-async-literal.js")]
    public Task test_member_expression_async_literal() => ExecutionTest("member-expression-async-literal");

    [Fact(DisplayName = "member-expression-async-this.js")]
    public Task test_member_expression_async_this() => ExecutionTest("member-expression-async-this");

    [Fact(DisplayName = "member-expression.js")]
    public Task test_member_expression() => ExecutionTest("member-expression");

    [Fact(DisplayName = "optional-chain-async-optional-chain-square-brackets.js")]
    public Task test_optional_chain_async_optional_chain_square_brackets() => ExecutionTest("optional-chain-async-optional-chain-square-brackets");

    [Fact(DisplayName = "optional-chain-async-square-brackets.js")]
    public Task test_optional_chain_async_square_brackets() => ExecutionTest("optional-chain-async-square-brackets");

    [Fact(DisplayName = "static-semantics-simple-assignment.js")]
    public Task test_static_semantics_simple_assignment() => CompilationFailureTest("static-semantics-simple-assignment", string.Empty);

    [Fact(DisplayName = "update-expression-postfix.js")]
    public Task test_update_expression_postfix() => CompilationFailureTest("update-expression-postfix", string.Empty);

    [Fact(DisplayName = "update-expression-prefix.js")]
    public Task test_update_expression_prefix() => CompilationFailureTest("update-expression-prefix", string.Empty);
}
