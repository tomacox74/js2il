namespace Jroc.Test262.Tests.language.statements.let;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.let") { }

    [Fact(DisplayName = "block-local-closure-set-before-initialization.js")]
    public Task test_block_local_closure_set_before_initialization()
        => ExecutionTest("block-local-closure-set-before-initialization");

    [Fact(DisplayName = "cptn-value.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_value()
        => ExecutionTest("cptn-value");

    [Fact(DisplayName = "function-local-closure-set-before-initialization.js")]
    public Task test_function_local_closure_set_before_initialization()
        => ExecutionTest("function-local-closure-set-before-initialization");

    [Fact(DisplayName = "global-closure-set-before-initialization.js")]
    public Task test_global_closure_set_before_initialization()
        => ExecutionTest("global-closure-set-before-initialization");

    [Fact(DisplayName = "static-init-await-binding-valid.js")]
    public Task test_static_init_await_binding_valid()
        => ExecutionTest("static-init-await-binding-valid");

}
