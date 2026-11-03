namespace Jroc.Test262.Tests.language.statements.generators;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.generators") { }

    [Fact(DisplayName = "cptn-decl.js", Skip = "eval is not supported.")]
    public Task test_cptn_decl()
        => ExecutionTest("cptn-decl");

    [Fact(DisplayName = "default-proto.js")]
    public Task test_default_proto()
        => ExecutionTest("default-proto");

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task test_dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task test_dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task test_dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_eval_var_scope_syntax_err()
        => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "scope-body-lex-distinct.js", Skip = "eval is not supported.")]
    public Task test_scope_body_lex_distinct()
        => ExecutionTest("scope-body-lex-distinct");

    [Fact(DisplayName = "unscopables-with-in-nested-fn.js")]
    public Task test_unscopables_with_in_nested_fn()
        => ExecutionTest("unscopables-with-in-nested-fn");

    [Fact(DisplayName = "unscopables-with.js")]
    public Task test_unscopables_with()
        => ExecutionTest("unscopables-with");

}
