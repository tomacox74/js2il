namespace Jroc.Test262.Tests.language.statements.function;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.function") { }

    [Fact(DisplayName = "13.1-29-s.js", Skip = "eval is not supported.")]
    public Task test_13_1_29_s()
        => ExecutionTest("13.1-29-s");

    [Fact(DisplayName = "13.1-31-s.js", Skip = "eval is not supported.")]
    public Task test_13_1_31_s()
        => ExecutionTest("13.1-31-s");

    [Fact(DisplayName = "13.1-33-s.js", Skip = "eval is not supported.")]
    public Task test_13_1_33_s()
        => ExecutionTest("13.1-33-s");

    [Fact(DisplayName = "13.1-39-s.js", Skip = "eval is not supported.")]
    public Task test_13_1_39_s()
        => ExecutionTest("13.1-39-s");

    [Fact(DisplayName = "13.1-4-s.js", Skip = "eval is not supported.")]
    public Task test_13_1_4_s()
        => ExecutionTest("13.1-4-s");

    [Fact(DisplayName = "13.1-41-s.js", Skip = "eval is not supported.")]
    public Task test_13_1_41_s()
        => ExecutionTest("13.1-41-s");

    [Fact(DisplayName = "13.2-30-s.js")]
    public Task test_13_2_30_s()
        => ExecutionTest("13.2-30-s");

    [Fact(DisplayName = "S13.2.2_A13.js", Skip = "eval is not supported.")]
    public Task test_S13_2_2_A13()
        => ExecutionTest("S13.2.2_A13");

    [Fact(DisplayName = "S13.2.2_A14.js", Skip = "eval is not supported.")]
    public Task test_S13_2_2_A14()
        => ExecutionTest("S13.2.2_A14");

    [Fact(DisplayName = "S13.2.2_A18_T1.js")]
    public Task test_S13_2_2_A18_T1()
        => ExecutionTest("S13.2.2_A18_T1");

    [Fact(DisplayName = "S13.2.2_A18_T2.js")]
    public Task test_S13_2_2_A18_T2()
        => ExecutionTest("S13.2.2_A18_T2");

    [Fact(DisplayName = "S13.2.2_A8_T3.js")]
    public Task test_S13_2_2_A8_T3()
        => ExecutionTest("S13.2.2_A8_T3");

    [Fact(DisplayName = "S13.2_A8_T1.js")]
    public Task test_S13_2_A8_T1()
        => ExecutionTest("S13.2_A8_T1");

    [Fact(DisplayName = "S13.2_A8_T2.js")]
    public Task test_S13_2_A8_T2()
        => ExecutionTest("S13.2_A8_T2");

    [Fact(DisplayName = "S13_A14_T1.js", Skip = "eval is not supported.")]
    public Task test_S13_A14_T1()
        => ExecutionTest("S13_A14_T1");

    [Fact(DisplayName = "S13_A15_T4.js")]
    public Task test_S13_A15_T4()
        => ExecutionTest("S13_A15_T4");

    [Fact(DisplayName = "S13_A17_T1.js")]
    public Task test_S13_A17_T1()
        => ExecutionTest("S13_A17_T1");

    [Fact(DisplayName = "S13_A17_T2.js")]
    public Task test_S13_A17_T2()
        => ExecutionTest("S13_A17_T2");

    [Fact(DisplayName = "cptn-decl.js", Skip = "eval is not supported.")]
    public Task test_cptn_decl()
        => ExecutionTest("cptn-decl");

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_eval_var_scope_syntax_err()
        => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "name-eval-stricteval.js", Skip = "eval is not supported.")]
    public Task test_name_eval_stricteval()
        => ExecutionTest("name-eval-stricteval");

    [Fact(DisplayName = "name-unicode.js", Skip = "eval is not supported.")]
    public Task test_name_unicode()
        => ExecutionTest("name-unicode");

    [Fact(DisplayName = "param-eval-stricteval.js", Skip = "eval is not supported.")]
    public Task test_param_eval_stricteval()
        => ExecutionTest("param-eval-stricteval");

    [Fact(DisplayName = "scope-body-lex-distinct.js", Skip = "eval is not supported.")]
    public Task test_scope_body_lex_distinct()
        => ExecutionTest("scope-body-lex-distinct");

    [Fact(DisplayName = "static-init-await-binding-valid.js")]
    public Task test_static_init_await_binding_valid()
        => ExecutionTest("static-init-await-binding-valid");

    [Fact(DisplayName = "unscopables-with-in-nested-fn.js")]
    public Task test_unscopables_with_in_nested_fn()
        => ExecutionTest("unscopables-with-in-nested-fn");

    [Fact(DisplayName = "unscopables-with.js")]
    public Task test_unscopables_with()
        => ExecutionTest("unscopables-with");

}
