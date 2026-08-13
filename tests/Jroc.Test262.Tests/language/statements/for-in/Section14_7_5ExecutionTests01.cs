namespace Jroc.Test262.Tests.language.statements.for_in;

public class Section14_7_5ExecutionTests01 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests01() : base("language/statements/for-in", "language.statements.for_in") { }

    [Fact(DisplayName = "S12.6.4_A15.js")]
    public Task S12_6_4_A15_1()
        => CompilationFailureTest("S12.6.4_A15", string.Empty);

    [Fact(DisplayName = "cptn-decl-zero-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_decl_zero_itr_2()
        => ExecutionTest("cptn-decl-zero-itr");

    [Fact(DisplayName = "cptn-expr-abrupt-empty.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_abrupt_empty_3()
        => ExecutionTest("cptn-expr-abrupt-empty");

    [Fact(DisplayName = "cptn-expr-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_itr_4()
        => ExecutionTest("cptn-expr-itr");

    [Fact(DisplayName = "cptn-expr-skip-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_skip_itr_5()
        => ExecutionTest("cptn-expr-skip-itr");

    [Fact(DisplayName = "cptn-expr-zero-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_zero_itr_6()
        => ExecutionTest("cptn-expr-zero-itr");

    [Fact(DisplayName = "decl-async-fun.js")]
    public Task decl_async_fun_7()
        => CompilationFailureTest("decl-async-fun", string.Empty);

    [Fact(DisplayName = "decl-async-gen.js")]
    public Task decl_async_gen_8()
        => CompilationFailureTest("decl-async-gen", string.Empty);

    [Fact(DisplayName = "decl-cls.js")]
    public Task decl_cls_9()
        => CompilationFailureTest("decl-cls", string.Empty);

    [Fact(DisplayName = "decl-const.js")]
    public Task decl_const_10()
        => CompilationFailureTest("decl-const", string.Empty);

    [Fact(DisplayName = "decl-fun.js")]
    public Task decl_fun_11()
        => CompilationFailureTest("decl-fun", string.Empty);

    [Fact(DisplayName = "decl-gen.js")]
    public Task decl_gen_12()
        => CompilationFailureTest("decl-gen", string.Empty);

    [Fact(DisplayName = "decl-let.js")]
    public Task decl_let_13()
        => CompilationFailureTest("decl-let", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_init_yield_ident_invalid_14()
        => CompilationFailureTest("dstr/array-elem-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-array-invalid.js")]
    public Task dstr_array_elem_nested_array_invalid_15()
        => CompilationFailureTest("dstr/array-elem-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_nested_array_yield_ident_invalid_16()
        => CompilationFailureTest("dstr/array-elem-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-memberexpr-optchain-prop-ref-init.js")]
    public Task dstr_array_elem_nested_memberexpr_optchain_prop_ref_init_17()
        => CompilationFailureTest("dstr/array-elem-nested-memberexpr-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-obj-invalid.js")]
    public Task dstr_array_elem_nested_obj_invalid_18()
        => CompilationFailureTest("dstr/array-elem-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_nested_obj_yield_ident_invalid_19()
        => CompilationFailureTest("dstr/array-elem-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-put-obj-literal-optchain-prop-ref-init.js")]
    public Task dstr_array_elem_put_obj_literal_optchain_prop_ref_init_20()
        => CompilationFailureTest("dstr/array-elem-put-obj-literal-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-target-simple-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_target_simple_strict_21()
        => CompilationFailureTest("dstr/array-elem-target-simple-strict", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-target-yield-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_target_yield_invalid_22()
        => CompilationFailureTest("dstr/array-elem-target-yield-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-before-element.js")]
    public Task dstr_array_rest_before_element_23()
        => CompilationFailureTest("dstr/array-rest-before-element", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-before-elision.js")]
    public Task dstr_array_rest_before_elision_24()
        => CompilationFailureTest("dstr/array-rest-before-elision", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-before-rest.js")]
    public Task dstr_array_rest_before_rest_25()
        => CompilationFailureTest("dstr/array-rest-before-rest", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-elision-invalid.js")]
    public Task dstr_array_rest_elision_invalid_26()
        => CompilationFailureTest("dstr/array-rest-elision-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-init.js")]
    public Task dstr_array_rest_init_27()
        => CompilationFailureTest("dstr/array-rest-init", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-array-invalid.js")]
    public Task dstr_array_rest_nested_array_invalid_28()
        => CompilationFailureTest("dstr/array-rest-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_rest_nested_array_yield_ident_invalid_29()
        => CompilationFailureTest("dstr/array-rest-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-obj-invalid.js")]
    public Task dstr_array_rest_nested_obj_invalid_30()
        => CompilationFailureTest("dstr/array-rest-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_rest_nested_obj_yield_ident_invalid_31()
        => CompilationFailureTest("dstr/array-rest-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_rest_yield_ident_invalid_32()
        => CompilationFailureTest("dstr/array-rest-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-identifier-yield-expr.js")]
    public Task dstr_obj_id_identifier_yield_expr_33()
        => CompilationFailureTest("dstr/obj-id-identifier-yield-expr", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-identifier-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_identifier_yield_ident_invalid_34()
        => CompilationFailureTest("dstr/obj-id-identifier-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-init-simple-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_init_simple_strict_35()
        => CompilationFailureTest("dstr/obj-id-init-simple-strict", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_init_yield_ident_invalid_36()
        => CompilationFailureTest("dstr/obj-id-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-simple-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_simple_strict_37()
        => CompilationFailureTest("dstr/obj-id-simple-strict", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_elem_init_yield_ident_invalid_38()
        => CompilationFailureTest("dstr/obj-prop-elem-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-target-memberexpr-optchain-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_memberexpr_optchain_prop_ref_init_39()
        => CompilationFailureTest("dstr/obj-prop-elem-target-memberexpr-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-target-obj-literal-optchain-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_obj_literal_optchain_prop_ref_init_40()
        => CompilationFailureTest("dstr/obj-prop-elem-target-obj-literal-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-target-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_elem_target_yield_ident_invalid_41()
        => CompilationFailureTest("dstr/obj-prop-elem-target-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-array-invalid.js")]
    public Task dstr_obj_prop_nested_array_invalid_42()
        => CompilationFailureTest("dstr/obj-prop-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_nested_array_yield_ident_invalid_43()
        => CompilationFailureTest("dstr/obj-prop-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-invalid.js")]
    public Task dstr_obj_prop_nested_obj_invalid_44()
        => CompilationFailureTest("dstr/obj-prop-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_nested_obj_yield_ident_invalid_45()
        => CompilationFailureTest("dstr/obj-prop-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-rest-not-last-element-invalid.js")]
    public Task dstr_obj_rest_not_last_element_invalid_46()
        => CompilationFailureTest("dstr/obj-rest-not-last-element-invalid", string.Empty);

    [Fact(DisplayName = "head-const-bound-names-dup.js")]
    public Task head_const_bound_names_dup_47()
        => CompilationFailureTest("head-const-bound-names-dup", string.Empty);

    [Fact(DisplayName = "head-const-bound-names-in-stmt.js")]
    public Task head_const_bound_names_in_stmt_48()
        => CompilationFailureTest("head-const-bound-names-in-stmt", string.Empty);

    [Fact(DisplayName = "head-const-bound-names-let.js")]
    public Task head_const_bound_names_let_49()
        => CompilationFailureTest("head-const-bound-names-let", string.Empty);

    [Fact(DisplayName = "head-let-bound-names-dup.js")]
    public Task head_let_bound_names_dup_50()
        => CompilationFailureTest("head-let-bound-names-dup", string.Empty);

    [Fact(DisplayName = "head-let-bound-names-in-stmt.js")]
    public Task head_let_bound_names_in_stmt_51()
        => CompilationFailureTest("head-let-bound-names-in-stmt", string.Empty);

    [Fact(DisplayName = "head-let-bound-names-let.js")]
    public Task head_let_bound_names_let_52()
        => CompilationFailureTest("head-let-bound-names-let", string.Empty);

    [Fact(DisplayName = "head-lhs-cover-non-asnmt-trgt.js")]
    public Task head_lhs_cover_non_asnmt_trgt_53()
        => CompilationFailureTest("head-lhs-cover-non-asnmt-trgt", string.Empty);

    [Fact(DisplayName = "head-lhs-invalid-asnmt-ptrn-ary.js")]
    public Task head_lhs_invalid_asnmt_ptrn_ary_54()
        => CompilationFailureTest("head-lhs-invalid-asnmt-ptrn-ary", string.Empty);

    [Fact(DisplayName = "head-lhs-invalid-asnmt-ptrn-obj.js")]
    public Task head_lhs_invalid_asnmt_ptrn_obj_55()
        => CompilationFailureTest("head-lhs-invalid-asnmt-ptrn-obj", string.Empty);

    [Fact(DisplayName = "head-lhs-member.js")]
    public Task head_lhs_member_56()
        => ExecutionTest("head-lhs-member");

    [Fact(DisplayName = "head-lhs-non-asnmt-trgt.js")]
    public Task head_lhs_non_asnmt_trgt_57()
        => CompilationFailureTest("head-lhs-non-asnmt-trgt", string.Empty);

    [Fact(DisplayName = "head-var-bound-names-dup.js")]
    public Task head_var_bound_names_dup_58()
        => ExecutionTest("head-var-bound-names-dup");

    [Fact(DisplayName = "head-var-bound-names-in-stmt.js")]
    public Task head_var_bound_names_in_stmt_59()
        => ExecutionTest("head-var-bound-names-in-stmt");

    [Fact(DisplayName = "head-var-bound-names-let.js")]
    public Task head_var_bound_names_let_60()
        => ExecutionTest("head-var-bound-names-let");

    [Fact(DisplayName = "head-var-expr.js")]
    public Task head_var_expr_61()
        => ExecutionTest("head-var-expr");

    [Fact(DisplayName = "identifier-let-allowed-as-lefthandside-expression-not-strict.js")]
    public Task identifier_let_allowed_as_lefthandside_expression_not_strict_62()
        => ExecutionTest("identifier-let-allowed-as-lefthandside-expression-not-strict");

    [Fact(DisplayName = "labelled-fn-stmt-const.js")]
    public Task labelled_fn_stmt_const_63()
        => CompilationFailureTest("labelled-fn-stmt-const", string.Empty);

    [Fact(DisplayName = "labelled-fn-stmt-let.js")]
    public Task labelled_fn_stmt_let_64()
        => CompilationFailureTest("labelled-fn-stmt-let", string.Empty);

    [Fact(DisplayName = "labelled-fn-stmt-lhs.js")]
    public Task labelled_fn_stmt_lhs_65()
        => CompilationFailureTest("labelled-fn-stmt-lhs", string.Empty);

    [Fact(DisplayName = "labelled-fn-stmt-var.js")]
    public Task labelled_fn_stmt_var_66()
        => CompilationFailureTest("labelled-fn-stmt-var", string.Empty);

    [Fact(DisplayName = "let-array-with-newline.js")]
    public Task let_array_with_newline_67()
        => CompilationFailureTest("let-array-with-newline", string.Empty);

    [Fact(DisplayName = "let-block-with-newline.js")]
    public Task let_block_with_newline_68()
        => ExecutionTest("let-block-with-newline");

    [Fact(DisplayName = "let-identifier-with-newline.js")]
    public Task let_identifier_with_newline_69()
        => ExecutionTest("let-identifier-with-newline");

    [Fact(DisplayName = "order-after-define-property.js")]
    public Task order_after_define_property_70()
        => ExecutionTest("order-after-define-property");

    [Fact(DisplayName = "order-enumerable-shadowed.js")]
    public Task order_enumerable_shadowed_71()
        => ExecutionTest("order-enumerable-shadowed");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer_72()
        => ExecutionTest("resizable-buffer");

    [Fact(DisplayName = "scope-body-lex-close.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_body_lex_close_73()
        => ExecutionTest("scope-body-lex-close");

    [Fact(DisplayName = "scope-body-lex-open.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_body_lex_open_74()
        => ExecutionTest("scope-body-lex-open");

    [Fact(DisplayName = "scope-body-var-none.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_body_var_none_75()
        => ExecutionTest("scope-body-var-none");

    [Fact(DisplayName = "scope-head-lex-close.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_head_lex_close_76()
        => ExecutionTest("scope-head-lex-close");

    [Fact(DisplayName = "scope-head-lex-open.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_head_lex_open_77()
        => ExecutionTest("scope-head-lex-open");

    [Fact(DisplayName = "scope-head-var-none.js", Skip = "eval is not supported by JROC.")]
    public Task scope_head_var_none_78()
        => ExecutionTest("scope-head-var-none");

    [Fact(DisplayName = "var-arguments-fn-strict-init.js")]
    public Task var_arguments_fn_strict_init_79()
        => CompilationFailureTest("var-arguments-fn-strict-init", string.Empty);

    [Fact(DisplayName = "var-arguments-fn-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task var_arguments_fn_strict_80()
        => CompilationFailureTest("var-arguments-fn-strict", string.Empty);

    [Fact(DisplayName = "var-arguments-strict-init.js")]
    public Task var_arguments_strict_init_81()
        => CompilationFailureTest("var-arguments-strict-init", string.Empty);

    [Fact(DisplayName = "var-arguments-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task var_arguments_strict_82()
        => CompilationFailureTest("var-arguments-strict", string.Empty);

    [Fact(DisplayName = "var-eval-strict-init.js")]
    public Task var_eval_strict_init_83()
        => CompilationFailureTest("var-eval-strict-init", string.Empty);

    [Fact(DisplayName = "var-eval-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task var_eval_strict_84()
        => CompilationFailureTest("var-eval-strict", string.Empty);
}
