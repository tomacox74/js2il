namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public class ArrowFunctionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ArrowFunctionCompletionExecutionTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task eval_var_scope_syntax_err() => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "scope-body-lex-distinct.js", Skip = "eval is not supported.")]
    public Task scope_body_lex_distinct() => ExecutionTest("scope-body-lex-distinct");

    [Fact(DisplayName = "unscopables-with-in-nested-fn.js")]
    public Task unscopables_with_in_nested_fn() => ExecutionTest("unscopables-with-in-nested-fn");

    [Fact(DisplayName = "unscopables-with.js")]
    public Task unscopables_with() => ExecutionTest("unscopables-with");
}
