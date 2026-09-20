using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.optional_chaining;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.optional_chaining") { }

    [Fact(DisplayName = "iteration-statement-for-of-type-error")]
    public Task iteration_statement_for_of_type_error()
        => ExecutionTest("iteration-statement-for-of-type-error");

    [Fact(DisplayName = "iteration-statement-for")]
    public Task iteration_statement_for()
        => ExecutionTest("iteration-statement-for");

    [Fact(DisplayName = "iteration-statement-while")]
    public Task iteration_statement_while()
        => ExecutionTest("iteration-statement-while");

    [Fact(DisplayName = "new-target-optional-call")]
    public Task new_target_optional_call()
        => ExecutionTest("new-target-optional-call");

    [Fact(DisplayName = "optional-chain-expression-optional-expression")]
    public Task optional_chain_expression_optional_expression()
        => ExecutionTest("optional-chain-expression-optional-expression");

    [Fact(DisplayName = "optional-chain-prod-arguments")]
    public Task optional_chain_prod_arguments()
        => ExecutionTest("optional-chain-prod-arguments");

    [Fact(DisplayName = "optional-chain-prod-expression")]
    public Task optional_chain_prod_expression()
        => ExecutionTest("optional-chain-prod-expression");

    [Fact(DisplayName = "optional-chain-prod-identifiername")]
    public Task optional_chain_prod_identifiername()
        => ExecutionTest("optional-chain-prod-identifiername");

    [Fact(DisplayName = "optional-chain")]
    public Task optional_chain()
        => ExecutionTest("optional-chain");

    [Fact(DisplayName = "optional-expression")]
    public Task optional_expression()
        => ExecutionTest("optional-expression");

    [Fact(DisplayName = "punctuator-decimal-lookahead")]
    public Task punctuator_decimal_lookahead()
        => ExecutionTest("punctuator-decimal-lookahead");

    [Fact(DisplayName = "runtime-semantics-evaluation")]
    public Task runtime_semantics_evaluation()
        => ExecutionTest("runtime-semantics-evaluation");

}
