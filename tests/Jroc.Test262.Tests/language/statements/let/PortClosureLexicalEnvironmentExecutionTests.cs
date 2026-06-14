using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.let_;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.statements.let") { }

    [Fact(DisplayName = "block-local-closure-get-before-initialization")]
    public Task block_local_closure_get_before_initialization()
        => ExecutionTest("block-local-closure-get-before-initialization");

    [Fact(DisplayName = "block-local-use-before-initialization-in-declaration-statement")]
    public Task block_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("block-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "block-local-use-before-initialization-in-prior-statement")]
    public Task block_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("block-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "function-local-closure-get-before-initialization")]
    public Task function_local_closure_get_before_initialization()
        => ExecutionTest("function-local-closure-get-before-initialization");

    [Fact(DisplayName = "function-local-use-before-initialization-in-declaration-statement")]
    public Task function_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("function-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "function-local-use-before-initialization-in-prior-statement")]
    public Task function_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("function-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "global-closure-get-before-initialization")]
    public Task global_closure_get_before_initialization()
        => ExecutionTest("global-closure-get-before-initialization");

    [Fact(DisplayName = "global-use-before-initialization-in-declaration-statement")]
    public Task global_use_before_initialization_in_declaration_statement()
        => ExecutionTest("global-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "global-use-before-initialization-in-prior-statement")]
    public Task global_use_before_initialization_in_prior_statement()
        => ExecutionTest("global-use-before-initialization-in-prior-statement");
}
