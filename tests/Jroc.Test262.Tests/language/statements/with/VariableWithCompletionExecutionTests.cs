namespace Jroc.Test262.Tests.language.statements.with;

public sealed class VariableWithCompletionExecutionTests : DiskExecutionTestsBase
{
    public VariableWithCompletionExecutionTests() : base("language.statements.with") { }

    [Fact(DisplayName = "12.10-0-9.js")]
    public Task test_12_10_0_9()
        => ExecutionTest("12.10-0-9");

    [Fact(DisplayName = "12.10-2-1.js")]
    public Task test_12_10_2_1()
        => ExecutionTest("12.10-2-1");

    [Fact(DisplayName = "12.10-2-2.js")]
    public Task test_12_10_2_2()
        => ExecutionTest("12.10-2-2");

    [Fact(DisplayName = "12.10-2-3.js")]
    public Task test_12_10_2_3()
        => ExecutionTest("12.10-2-3");

    [Fact(DisplayName = "12.10.1-10-s.js", Skip = "eval is not supported.")]
    public Task test_12_10_1_10_s()
        => ExecutionTest("12.10.1-10-s");

    [Fact(DisplayName = "12.10.1-11gs.js")]
    public Task test_12_10_1_11gs()
        => CompilationFailureTest("12.10.1-11gs", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.10.1-12-s.js", Skip = "eval is not supported.")]
    public Task test_12_10_1_12_s()
        => ExecutionTest("12.10.1-12-s");

    [Fact(DisplayName = "S12.10_A1.10_T1.js")]
    public Task S12_10_A1_10_T1()
        => ExecutionTest("S12.10_A1.10_T1");

    [Fact(DisplayName = "S12.10_A1.10_T2.js")]
    public Task S12_10_A1_10_T2()
        => ExecutionTest("S12.10_A1.10_T2");

    [Fact(DisplayName = "S12.10_A1.10_T4.js")]
    public Task S12_10_A1_10_T4()
        => ExecutionTest("S12.10_A1.10_T4");

    [Fact(DisplayName = "S12.10_A1.12_T1.js")]
    public Task S12_10_A1_12_T1()
        => ExecutionTest("S12.10_A1.12_T1");

    [Fact(DisplayName = "S12.10_A1.12_T2.js")]
    public Task S12_10_A1_12_T2()
        => ExecutionTest("S12.10_A1.12_T2");

    [Fact(DisplayName = "S12.10_A1.12_T4.js")]
    public Task S12_10_A1_12_T4()
        => ExecutionTest("S12.10_A1.12_T4");

    [Fact(DisplayName = "S12.10_A1.1_T1.js")]
    public Task S12_10_A1_1_T1()
        => ExecutionTest("S12.10_A1.1_T1");

    [Fact(DisplayName = "S12.10_A1.1_T2.js")]
    public Task S12_10_A1_1_T2()
        => ExecutionTest("S12.10_A1.1_T2");

    [Fact(DisplayName = "S12.10_A1.2_T1.js")]
    public Task S12_10_A1_2_T1()
        => ExecutionTest("S12.10_A1.2_T1");

    [Fact(DisplayName = "S12.10_A1.2_T2.js")]
    public Task S12_10_A1_2_T2()
        => ExecutionTest("S12.10_A1.2_T2");

    [Fact(DisplayName = "S12.10_A1.2_T4.js")]
    public Task S12_10_A1_2_T4()
        => ExecutionTest("S12.10_A1.2_T4");

    [Fact(DisplayName = "S12.10_A1.3_T1.js")]
    public Task S12_10_A1_3_T1()
        => ExecutionTest("S12.10_A1.3_T1");

    [Fact(DisplayName = "S12.10_A1.3_T2.js")]
    public Task S12_10_A1_3_T2()
        => ExecutionTest("S12.10_A1.3_T2");

    [Fact(DisplayName = "S12.10_A1.3_T4.js")]
    public Task S12_10_A1_3_T4()
        => ExecutionTest("S12.10_A1.3_T4");

    [Fact(DisplayName = "S12.10_A1.4_T1.js")]
    public Task S12_10_A1_4_T1()
        => ExecutionTest("S12.10_A1.4_T1");

    [Fact(DisplayName = "S12.10_A1.4_T2.js")]
    public Task S12_10_A1_4_T2()
        => ExecutionTest("S12.10_A1.4_T2");

    [Fact(DisplayName = "S12.10_A1.4_T4.js")]
    public Task S12_10_A1_4_T4()
        => ExecutionTest("S12.10_A1.4_T4");

    [Fact(DisplayName = "S12.10_A1.5_T1.js")]
    public Task S12_10_A1_5_T1()
        => ExecutionTest("S12.10_A1.5_T1");

    [Fact(DisplayName = "S12.10_A1.5_T2.js")]
    public Task S12_10_A1_5_T2()
        => ExecutionTest("S12.10_A1.5_T2");

    [Fact(DisplayName = "S12.10_A1.5_T4.js")]
    public Task S12_10_A1_5_T4()
        => ExecutionTest("S12.10_A1.5_T4");

    [Fact(DisplayName = "S12.10_A1.6_T1.js")]
    public Task S12_10_A1_6_T1()
        => ExecutionTest("S12.10_A1.6_T1");

    [Fact(DisplayName = "S12.10_A1.6_T2.js")]
    public Task S12_10_A1_6_T2()
        => ExecutionTest("S12.10_A1.6_T2");

    [Fact(DisplayName = "S12.10_A1.7_T1.js")]
    public Task S12_10_A1_7_T1()
        => ExecutionTest("S12.10_A1.7_T1");

    [Fact(DisplayName = "S12.10_A1.7_T2.js")]
    public Task S12_10_A1_7_T2()
        => ExecutionTest("S12.10_A1.7_T2");

    [Fact(DisplayName = "S12.10_A1.7_T4.js")]
    public Task S12_10_A1_7_T4()
        => ExecutionTest("S12.10_A1.7_T4");

    [Fact(DisplayName = "S12.10_A1.8_T1.js")]
    public Task S12_10_A1_8_T1()
        => ExecutionTest("S12.10_A1.8_T1");

    [Fact(DisplayName = "S12.10_A1.8_T2.js")]
    public Task S12_10_A1_8_T2()
        => ExecutionTest("S12.10_A1.8_T2");

    [Fact(DisplayName = "S12.10_A1.8_T4.js")]
    public Task S12_10_A1_8_T4()
        => ExecutionTest("S12.10_A1.8_T4");

    [Fact(DisplayName = "S12.10_A1.9_T1.js")]
    public Task S12_10_A1_9_T1()
        => ExecutionTest("S12.10_A1.9_T1");

    [Fact(DisplayName = "S12.10_A1.9_T2.js")]
    public Task S12_10_A1_9_T2()
        => ExecutionTest("S12.10_A1.9_T2");

    [Fact(DisplayName = "S12.10_A4_T1.js", Skip = "eval is not supported.")]
    public Task S12_10_A4_T1()
        => ExecutionTest("S12.10_A4_T1");

    [Fact(DisplayName = "S12.10_A4_T2.js", Skip = "eval is not supported.")]
    public Task S12_10_A4_T2()
        => ExecutionTest("S12.10_A4_T2");

    [Fact(DisplayName = "S12.10_A4_T3.js", Skip = "eval is not supported.")]
    public Task S12_10_A4_T3()
        => ExecutionTest("S12.10_A4_T3");

    [Fact(DisplayName = "S12.10_A4_T4.js", Skip = "eval is not supported.")]
    public Task S12_10_A4_T4()
        => ExecutionTest("S12.10_A4_T4");

    [Fact(DisplayName = "S12.10_A4_T5.js", Skip = "eval is not supported.")]
    public Task S12_10_A4_T5()
        => ExecutionTest("S12.10_A4_T5");

    [Fact(DisplayName = "S12.10_A4_T6.js", Skip = "eval is not supported.")]
    public Task S12_10_A4_T6()
        => ExecutionTest("S12.10_A4_T6");

    [Fact(DisplayName = "S12.10_A5_T1.js", Skip = "eval is not supported.")]
    public Task S12_10_A5_T1()
        => ExecutionTest("S12.10_A5_T1");

    [Fact(DisplayName = "S12.10_A5_T2.js", Skip = "eval is not supported.")]
    public Task S12_10_A5_T2()
        => ExecutionTest("S12.10_A5_T2");

    [Fact(DisplayName = "S12.10_A5_T3.js", Skip = "eval is not supported.")]
    public Task S12_10_A5_T3()
        => ExecutionTest("S12.10_A5_T3");

    [Fact(DisplayName = "S12.10_A5_T4.js", Skip = "eval is not supported.")]
    public Task S12_10_A5_T4()
        => ExecutionTest("S12.10_A5_T4");

    [Fact(DisplayName = "S12.10_A5_T5.js", Skip = "eval is not supported.")]
    public Task S12_10_A5_T5()
        => ExecutionTest("S12.10_A5_T5");

    [Fact(DisplayName = "S12.10_A5_T6.js", Skip = "eval is not supported.")]
    public Task S12_10_A5_T6()
        => ExecutionTest("S12.10_A5_T6");

    [Fact(DisplayName = "binding-blocked-by-unscopables.js")]
    public Task binding_blocked_by_unscopables()
        => ExecutionTest("binding-blocked-by-unscopables");

    [Fact(DisplayName = "cptn-abrupt-empty.js", Skip = "eval is not supported.")]
    public Task cptn_abrupt_empty()
        => ExecutionTest("cptn-abrupt-empty");

    [Fact(DisplayName = "cptn-nrml.js", Skip = "eval is not supported.")]
    public Task cptn_nrml()
        => ExecutionTest("cptn-nrml");

    [Fact(DisplayName = "decl-async-fun.js")]
    public Task decl_async_fun()
        => CompilationFailureTest("decl-async-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-async-gen.js")]
    public Task decl_async_gen()
        => CompilationFailureTest("decl-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-cls.js")]
    public Task decl_cls()
        => CompilationFailureTest("decl-cls", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-const.js")]
    public Task decl_const()
        => CompilationFailureTest("decl-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-fun.js")]
    public Task decl_fun()
        => CompilationFailureTest("decl-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-gen.js")]
    public Task decl_gen()
        => CompilationFailureTest("decl-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-let.js")]
    public Task decl_let()
        => CompilationFailureTest("decl-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "get-binding-value-call-with-proxy-env.js")]
    public Task get_binding_value_call_with_proxy_env()
        => ExecutionTest("get-binding-value-call-with-proxy-env");

    [Fact(DisplayName = "get-binding-value-idref-with-proxy-env.js")]
    public Task get_binding_value_idref_with_proxy_env()
        => ExecutionTest("get-binding-value-idref-with-proxy-env");

    [Fact(DisplayName = "get-mutable-binding-binding-deleted-in-get-unscopables-strict-mode.js")]
    public Task get_mutable_binding_binding_deleted_in_get_unscopables_strict_mode()
        => ExecutionTest("get-mutable-binding-binding-deleted-in-get-unscopables-strict-mode");

    [Fact(DisplayName = "get-mutable-binding-binding-deleted-in-get-unscopables.js")]
    public Task get_mutable_binding_binding_deleted_in_get_unscopables()
        => ExecutionTest("get-mutable-binding-binding-deleted-in-get-unscopables");

    [Fact(DisplayName = "has-binding-call-with-proxy-env.js")]
    public Task has_binding_call_with_proxy_env()
        => ExecutionTest("has-binding-call-with-proxy-env");

    [Fact(DisplayName = "labelled-fn-stmt.js")]
    public Task labelled_fn_stmt()
        => CompilationFailureTest("labelled-fn-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-array-with-newline.js")]
    public Task let_array_with_newline()
        => CompilationFailureTest("let-array-with-newline", "Failed to parse JavaScript");

    [Fact(DisplayName = "scope-var-open.js", Skip = "eval is not supported.")]
    public Task scope_var_open()
        => ExecutionTest("scope-var-open");

    [Fact(DisplayName = "set-mutable-binding-binding-deleted-in-get-unscopables-strict-mode.js")]
    public Task set_mutable_binding_binding_deleted_in_get_unscopables_strict_mode()
        => ExecutionTest("set-mutable-binding-binding-deleted-in-get-unscopables-strict-mode");

    [Fact(DisplayName = "set-mutable-binding-binding-deleted-in-get-unscopables.js")]
    public Task set_mutable_binding_binding_deleted_in_get_unscopables()
        => ExecutionTest("set-mutable-binding-binding-deleted-in-get-unscopables");

    [Fact(DisplayName = "set-mutable-binding-binding-deleted-with-typed-array-in-proto-chain.js")]
    public Task set_mutable_binding_binding_deleted_with_typed_array_in_proto_chain()
        => ExecutionTest("set-mutable-binding-binding-deleted-with-typed-array-in-proto-chain");

    [Fact(DisplayName = "set-mutable-binding-idref-compound-assign-with-proxy-env.js")]
    public Task set_mutable_binding_idref_compound_assign_with_proxy_env()
        => ExecutionTest("set-mutable-binding-idref-compound-assign-with-proxy-env");

    [Fact(DisplayName = "set-mutable-binding-idref-with-proxy-env.js")]
    public Task set_mutable_binding_idref_with_proxy_env()
        => ExecutionTest("set-mutable-binding-idref-with-proxy-env");

    [Fact(DisplayName = "strict-fn-decl-nested-1.js")]
    public Task strict_fn_decl_nested_1()
        => CompilationFailureTest("strict-fn-decl-nested-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "strict-fn-decl-nested-2.js")]
    public Task strict_fn_decl_nested_2()
        => CompilationFailureTest("strict-fn-decl-nested-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "strict-fn-decl.js")]
    public Task strict_fn_decl()
        => CompilationFailureTest("strict-fn-decl", "Failed to parse JavaScript");

    [Fact(DisplayName = "strict-fn-expr.js")]
    public Task strict_fn_expr()
        => CompilationFailureTest("strict-fn-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "strict-fn-method.js")]
    public Task strict_fn_method()
        => CompilationFailureTest("strict-fn-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "strict-script.js")]
    public Task strict_script()
        => CompilationFailureTest("strict-script", "Failed to parse JavaScript");

    [Fact(DisplayName = "unscopables-get-err.js")]
    public Task unscopables_get_err()
        => ExecutionTest("unscopables-get-err");

    [Fact(DisplayName = "unscopables-inc-dec.js")]
    public Task unscopables_inc_dec()
        => ExecutionTest("unscopables-inc-dec");

    [Fact(DisplayName = "unscopables-prop-get-err.js")]
    public Task unscopables_prop_get_err()
        => ExecutionTest("unscopables-prop-get-err");
}
