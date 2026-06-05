using Js2IL.Test262.Tests.language.modules;

namespace Js2IL.Test262.Tests.language.module_code.top_level_await;

public class ExecutionTests : Js2IL.Test262.Tests.language.modules.FileSystemExecutionTestsBase
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

}
