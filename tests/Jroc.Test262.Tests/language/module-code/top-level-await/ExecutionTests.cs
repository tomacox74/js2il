using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code.top_level_await;

public class ExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\module-code\top-level-await", "language.module_code.top_level_await") { }

    [Fact(DisplayName = "early-errors-await-not-simple-assignment-target")]
    public Task early_errors_await_not_simple_assignment_target()
        => CompilationFailureTest("early-errors-await-not-simple-assignment-target");

    [Fact(DisplayName = "syntax/early-does-not-propagate-to-fn-declaration-body")]
    public Task syntax_early_does_not_propagate_to_fn_declaration_body()
        => CompilationFailureTest("syntax/early-does-not-propagate-to-fn-declaration-body");

    [Fact(DisplayName = "syntax/early-does-not-propagate-to-fn-declaration-params")]
    public Task syntax_early_does_not_propagate_to_fn_declaration_params()
        => CompilationFailureTest("syntax/early-does-not-propagate-to-fn-declaration-params");

    [Fact(DisplayName = "syntax/early-does-not-propagate-to-fn-expr-body")]
    public Task syntax_early_does_not_propagate_to_fn_expr_body()
        => CompilationFailureTest("syntax/early-does-not-propagate-to-fn-expr-body");

    [Fact(DisplayName = "syntax/early-does-not-propagate-to-fn-expr-params")]
    public Task syntax_early_does_not_propagate_to_fn_expr_params()
        => CompilationFailureTest("syntax/early-does-not-propagate-to-fn-expr-params");

    [Fact(DisplayName = "syntax/early-no-escaped-await")]
    public Task syntax_early_no_escaped_await()
        => CompilationFailureTest("syntax/early-no-escaped-await");

    [Fact(DisplayName = "async-module-does-not-block-sibling-modules")]
    public Task async_module_does_not_block_sibling_modules() => ExecutionTest("async-module-does-not-block-sibling-modules");
    [Fact(DisplayName = "await-awaits-thenable-not-callable")]
    public Task await_awaits_thenable_not_callable() => ExecutionTest("await-awaits-thenable-not-callable");
    [Fact(DisplayName = "await-awaits-thenables-that-throw")]
    public Task await_awaits_thenables_that_throw() => ExecutionTest("await-awaits-thenables-that-throw");
    [Fact(DisplayName = "await-awaits-thenables")]
    public Task await_awaits_thenables() => ExecutionTest("await-awaits-thenables");
    [Fact(DisplayName = "await-dynamic-import-rejection")]
    public Task await_dynamic_import_rejection() => ExecutionTest("await-dynamic-import-rejection");
    [Fact(DisplayName = "await-dynamic-import-resolution")]
    public Task await_dynamic_import_resolution() => ExecutionTest("await-dynamic-import-resolution");
    [Fact(DisplayName = "await-expr-func-expression")]
    public Task await_expr_func_expression() => ExecutionTest("await-expr-func-expression");
    [Fact(DisplayName = "await-expr-new-expr-reject")]
    public Task await_expr_new_expr_reject() => ExecutionTest("await-expr-new-expr-reject");
    [Fact(DisplayName = "await-expr-new-expr")]
    public Task await_expr_new_expr() => ExecutionTest("await-expr-new-expr");
    [Fact(DisplayName = "await-expr-regexp")]
    public Task await_expr_regexp() => ExecutionTest("await-expr-regexp");
    [Fact(DisplayName = "await-expr-reject-throws")]
    public Task await_expr_reject_throws() => ExecutionTest("await-expr-reject-throws");
    [Fact(DisplayName = "await-expr-resolution")]
    public Task await_expr_resolution() => ExecutionTest("await-expr-resolution");
    [Fact(DisplayName = "await-void-expr")]
    public Task await_void_expr() => ExecutionTest("await-void-expr");
    [Fact(DisplayName = "dfs-invariant")]
    public Task dfs_invariant() => ExecutionTest("dfs-invariant");
    [Fact(DisplayName = "dynamic-import-of-waiting-module")]
    public Task dynamic_import_of_waiting_module() => ExecutionTest("dynamic-import-of-waiting-module");
    [Fact(DisplayName = "dynamic-import-rejection")]
    public Task dynamic_import_rejection() => ExecutionTest("dynamic-import-rejection");
    [Fact(DisplayName = "dynamic-import-resolution")]
    public Task dynamic_import_resolution() => ExecutionTest("dynamic-import-resolution");
    [Fact(DisplayName = "fulfillment-order")]
    public Task fulfillment_order() => ExecutionTest("fulfillment-order");
    [Fact(DisplayName = "if-await-expr")]
    public Task if_await_expr() => ExecutionTest("if-await-expr");
    [Fact(DisplayName = "module-async-import-async-resolution-ticks")]
    public Task module_async_import_async_resolution_ticks() => ExecutionTest("module-async-import-async-resolution-ticks");
    [Fact(DisplayName = "module-graphs-does-not-hang")]
    public Task module_graphs_does_not_hang() => ExecutionTest("module-graphs-does-not-hang");
    [Fact(DisplayName = "module-import-rejection-body")]
    public Task module_import_rejection_body() => ExecutionTest("module-import-rejection-body");
    [Fact(DisplayName = "module-import-rejection-tick")]
    public Task module_import_rejection_tick() => ExecutionTest("module-import-rejection-tick");
    [Fact(DisplayName = "module-import-rejection")]
    public Task module_import_rejection() => ExecutionTest("module-import-rejection");
    [Fact(DisplayName = "module-import-resolution")]
    public Task module_import_resolution() => ExecutionTest("module-import-resolution");
    [Fact(DisplayName = "module-import-unwrapped")]
    public Task module_import_unwrapped() => ExecutionTest("module-import-unwrapped");
    [Fact(DisplayName = "module-self-import-async-resolution-ticks")]
    public Task module_self_import_async_resolution_ticks() => ExecutionTest("module-self-import-async-resolution-ticks");
    [Fact(DisplayName = "module-sync-import-async-resolution-ticks")]
    public Task module_sync_import_async_resolution_ticks() => ExecutionTest("module-sync-import-async-resolution-ticks");
    [Fact(DisplayName = "new-await-parens")]
    public Task new_await_parens() => ExecutionTest("new-await-parens");
    [Fact(DisplayName = "new-await-script-code")]
    public Task new_await_script_code() => ExecutionTest("new-await-script-code");
    [Fact(DisplayName = "pending-async-dep-from-cycle")]
    public Task pending_async_dep_from_cycle() => ExecutionTest("pending-async-dep-from-cycle");
    [Fact(DisplayName = "rejection-order")]
    public Task rejection_order() => ExecutionTest("rejection-order");
}
