namespace Jroc.Test262.Tests.language.global_code;

public class GlobalCodeCompletionExecutionTests : DiskExecutionTestsBase
{
    public GlobalCodeCompletionExecutionTests() : base("language.global_code") { }

    [Fact(DisplayName = "decl-func.js")]
    public Task decl_func() => ExecutionTest("decl-func");

    [Fact(DisplayName = "decl-lex-restricted-global.js")]
    public Task decl_lex_restricted_global() => ExecutionTest("decl-lex-restricted-global", allowUnhandledException: true);

    [Fact(DisplayName = "export.js")]
    public Task export() => CompilationFailureTest("export", "SyntaxError");

    [Fact(DisplayName = "return.js")]
    public Task @return() => CompilationFailureTest("return", "SyntaxError");

    [Fact(DisplayName = "script-decl-func-dups.js")]
    public Task script_decl_func_dups() => ExecutionTest("script-decl-func-dups");

    [Fact(DisplayName = "script-decl-func-err-non-configurable.js")]
    public Task script_decl_func_err_non_configurable() => ExecutionTest("script-decl-func-err-non-configurable");

    [Fact(DisplayName = "script-decl-func-err-non-extensible.js")]
    public Task script_decl_func_err_non_extensible() => ExecutionTest("script-decl-func-err-non-extensible");

    [Fact(DisplayName = "script-decl-func.js")]
    public Task script_decl_func() => ExecutionTest("script-decl-func");

    [Fact(DisplayName = "script-decl-lex-deletion.js")]
    public Task script_decl_lex_deletion() => ExecutionTest("script-decl-lex-deletion");

    [Fact(DisplayName = "script-decl-lex-lex.js")]
    public Task script_decl_lex_lex() => ExecutionTest("script-decl-lex-lex");

    [Fact(DisplayName = "script-decl-lex-restricted-global.js")]
    public Task script_decl_lex_restricted_global() => ExecutionTest("script-decl-lex-restricted-global");

    [Fact(DisplayName = "script-decl-lex-var-declared-via-eval.js", Skip = "eval is not supported.")]
    public Task script_decl_lex_var_declared_via_eval() => ExecutionTest("script-decl-lex-var-declared-via-eval");

    [Fact(DisplayName = "script-decl-lex-var.js")]
    public Task script_decl_lex_var() => ExecutionTest("script-decl-lex-var");

    [Fact(DisplayName = "script-decl-lex.js")]
    public Task script_decl_lex() => ExecutionTest("script-decl-lex");

    [Fact(DisplayName = "script-decl-var-collision.js")]
    public Task script_decl_var_collision() => ExecutionTest("script-decl-var-collision");

    [Fact(DisplayName = "script-decl-var-err.js")]
    public Task script_decl_var_err() => ExecutionTest("script-decl-var-err");

    [Fact(DisplayName = "script-decl-var.js")]
    public Task script_decl_var() => ExecutionTest("script-decl-var");
}
