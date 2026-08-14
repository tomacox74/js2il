namespace Jroc.Test262.Tests.language.statements.for_of;

public class Section14_7_5ExecutionTests01 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests01() : base("language/statements/for-of", "language.statements.for_of") { }

    [Fact(DisplayName = "body-put-error.js")]
    public Task body_put_error_1()
        => ExecutionTest("body-put-error");

    [Fact(DisplayName = "break-from-catch.js")]
    public Task break_from_catch_2()
        => ExecutionTest("break-from-catch");

    [Fact(DisplayName = "break-from-finally.js")]
    public Task break_from_finally_3()
        => ExecutionTest("break-from-finally");

    [Fact(DisplayName = "break-from-try.js")]
    public Task break_from_try_4()
        => ExecutionTest("break-from-try");

    [Fact(DisplayName = "break-label-from-catch.js")]
    public Task break_label_from_catch_5()
        => ExecutionTest("break-label-from-catch");

    [Fact(DisplayName = "break-label-from-finally.js")]
    public Task break_label_from_finally_6()
        => ExecutionTest("break-label-from-finally");

    [Fact(DisplayName = "break-label-from-try.js")]
    public Task break_label_from_try_7()
        => ExecutionTest("break-label-from-try");

    [Fact(DisplayName = "continue-from-catch.js")]
    public Task continue_from_catch_8()
        => ExecutionTest("continue-from-catch");

    [Fact(DisplayName = "continue-from-finally.js")]
    public Task continue_from_finally_9()
        => ExecutionTest("continue-from-finally");

    [Fact(DisplayName = "continue-from-try.js")]
    public Task continue_from_try_10()
        => ExecutionTest("continue-from-try");

    [Fact(DisplayName = "continue-label-from-catch.js")]
    public Task continue_label_from_catch_11()
        => ExecutionTest("continue-label-from-catch");

    [Fact(DisplayName = "continue-label-from-finally.js")]
    public Task continue_label_from_finally_12()
        => ExecutionTest("continue-label-from-finally");

    [Fact(DisplayName = "continue-label-from-try.js")]
    public Task continue_label_from_try_13()
        => ExecutionTest("continue-label-from-try");

    [Fact(DisplayName = "cptn-decl-abrupt-empty.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_decl_abrupt_empty_14()
        => ExecutionTest("cptn-decl-abrupt-empty");

    [Fact(DisplayName = "cptn-decl-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_decl_itr_15()
        => ExecutionTest("cptn-decl-itr");

    [Fact(DisplayName = "cptn-decl-no-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_decl_no_itr_16()
        => ExecutionTest("cptn-decl-no-itr");

    [Fact(DisplayName = "cptn-expr-abrupt-empty.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_abrupt_empty_17()
        => ExecutionTest("cptn-expr-abrupt-empty");

    [Fact(DisplayName = "cptn-expr-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_itr_18()
        => ExecutionTest("cptn-expr-itr");

    [Fact(DisplayName = "cptn-expr-no-itr.js", Skip = "eval is not supported by JROC.")]
    public Task cptn_expr_no_itr_19()
        => ExecutionTest("cptn-expr-no-itr");

    [Fact(DisplayName = "decl-async-fun.js")]
    public Task decl_async_fun_20()
        => CompilationFailureTest("decl-async-fun", string.Empty);

    [Fact(DisplayName = "decl-async-gen.js")]
    public Task decl_async_gen_21()
        => CompilationFailureTest("decl-async-gen", string.Empty);

    [Fact(DisplayName = "decl-cls.js")]
    public Task decl_cls_22()
        => CompilationFailureTest("decl-cls", string.Empty);

    [Fact(DisplayName = "decl-const.js")]
    public Task decl_const_23()
        => CompilationFailureTest("decl-const", string.Empty);

    [Fact(DisplayName = "decl-fun.js")]
    public Task decl_fun_24()
        => CompilationFailureTest("decl-fun", string.Empty);

    [Fact(DisplayName = "decl-gen.js")]
    public Task decl_gen_25()
        => CompilationFailureTest("decl-gen", string.Empty);

    [Fact(DisplayName = "decl-let.js")]
    public Task decl_let_26()
        => CompilationFailureTest("decl-let", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-init-evaluation.js")]
    public Task dstr_array_elem_init_evaluation_27()
        => ExecutionTest("dstr/array-elem-init-evaluation");

    [Fact(DisplayName = "dstr/array-elem-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_array_elem_init_fn_name_arrow_28()
        => ExecutionTest("dstr/array-elem-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/array-elem-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_array_elem_init_fn_name_class_29()
        => ExecutionTest("dstr/array-elem-init-fn-name-class");

    [Fact(DisplayName = "dstr/array-elem-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_array_elem_init_fn_name_cover_30()
        => ExecutionTest("dstr/array-elem-init-fn-name-cover");

    [Fact(DisplayName = "dstr/array-elem-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_array_elem_init_fn_name_fn_31()
        => ExecutionTest("dstr/array-elem-init-fn-name-fn");

    [Fact(DisplayName = "dstr/array-elem-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_array_elem_init_fn_name_gen_32()
        => ExecutionTest("dstr/array-elem-init-fn-name-gen");

    [Fact(DisplayName = "dstr/array-elem-init-simple-no-strict.js")]
    public Task dstr_array_elem_init_simple_no_strict_33()
        => ExecutionTest("dstr/array-elem-init-simple-no-strict");

    [Fact(DisplayName = "dstr/array-elem-init-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_elem_init_yield_expr_34()
        => ExecutionTest("dstr/array-elem-init-yield-expr");

    [Fact(DisplayName = "dstr/array-elem-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_init_yield_ident_invalid_35()
        => CompilationFailureTest("dstr/array-elem-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-init-yield-ident-valid.js")]
    public Task dstr_array_elem_init_yield_ident_valid_36()
        => ExecutionTest("dstr/array-elem-init-yield-ident-valid");

    [Fact(DisplayName = "dstr/array-elem-iter-get-err.js")]
    public Task dstr_array_elem_iter_get_err_37()
        => ExecutionTest("dstr/array-elem-iter-get-err");

    [Fact(DisplayName = "dstr/array-elem-iter-nrml-close-err.js")]
    public Task dstr_array_elem_iter_nrml_close_err_38()
        => ExecutionTest("dstr/array-elem-iter-nrml-close-err");

    [Fact(DisplayName = "dstr/array-elem-iter-rtrn-close-err.js")]
    public Task dstr_array_elem_iter_rtrn_close_err_39()
        => ExecutionTest("dstr/array-elem-iter-rtrn-close-err");

    [Fact(DisplayName = "dstr/array-elem-iter-rtrn-close.js")]
    public Task dstr_array_elem_iter_rtrn_close_40()
        => ExecutionTest("dstr/array-elem-iter-rtrn-close");

    [Fact(DisplayName = "dstr/array-elem-iter-thrw-close-err.js")]
    public Task dstr_array_elem_iter_thrw_close_err_41()
        => ExecutionTest("dstr/array-elem-iter-thrw-close-err");

    [Fact(DisplayName = "dstr/array-elem-iter-thrw-close-skip.js")]
    public Task dstr_array_elem_iter_thrw_close_skip_42()
        => ExecutionTest("dstr/array-elem-iter-thrw-close-skip");

    [Fact(DisplayName = "dstr/array-elem-iter-thrw-close.js")]
    public Task dstr_array_elem_iter_thrw_close_43()
        => ExecutionTest("dstr/array-elem-iter-thrw-close");

    [Fact(DisplayName = "dstr/array-elem-nested-array-invalid.js")]
    public Task dstr_array_elem_nested_array_invalid_44()
        => CompilationFailureTest("dstr/array-elem-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-array-null.js")]
    public Task dstr_array_elem_nested_array_null_45()
        => ExecutionTest("dstr/array-elem-nested-array-null");

    [Fact(DisplayName = "dstr/array-elem-nested-array-undefined-hole.js")]
    public Task dstr_array_elem_nested_array_undefined_hole_46()
        => ExecutionTest("dstr/array-elem-nested-array-undefined-hole");

    [Fact(DisplayName = "dstr/array-elem-nested-array-undefined-own.js")]
    public Task dstr_array_elem_nested_array_undefined_own_47()
        => ExecutionTest("dstr/array-elem-nested-array-undefined-own");

    [Fact(DisplayName = "dstr/array-elem-nested-array-undefined.js")]
    public Task dstr_array_elem_nested_array_undefined_48()
        => ExecutionTest("dstr/array-elem-nested-array-undefined");

    [Fact(DisplayName = "dstr/array-elem-nested-array-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_elem_nested_array_yield_expr_49()
        => ExecutionTest("dstr/array-elem-nested-array-yield-expr");

    [Fact(DisplayName = "dstr/array-elem-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_nested_array_yield_ident_invalid_50()
        => CompilationFailureTest("dstr/array-elem-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-array-yield-ident-valid.js")]
    public Task dstr_array_elem_nested_array_yield_ident_valid_51()
        => ExecutionTest("dstr/array-elem-nested-array-yield-ident-valid");

    [Fact(DisplayName = "dstr/array-elem-nested-array.js")]
    public Task dstr_array_elem_nested_array_52()
        => ExecutionTest("dstr/array-elem-nested-array");

    [Fact(DisplayName = "dstr/array-elem-nested-memberexpr-optchain-prop-ref-init.js")]
    public Task dstr_array_elem_nested_memberexpr_optchain_prop_ref_init_53()
        => CompilationFailureTest("dstr/array-elem-nested-memberexpr-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-obj-invalid.js")]
    public Task dstr_array_elem_nested_obj_invalid_54()
        => CompilationFailureTest("dstr/array-elem-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-obj-null.js")]
    public Task dstr_array_elem_nested_obj_null_55()
        => ExecutionTest("dstr/array-elem-nested-obj-null");

    [Fact(DisplayName = "dstr/array-elem-nested-obj-undefined-hole.js")]
    public Task dstr_array_elem_nested_obj_undefined_hole_56()
        => ExecutionTest("dstr/array-elem-nested-obj-undefined-hole");

    [Fact(DisplayName = "dstr/array-elem-nested-obj-undefined-own.js")]
    public Task dstr_array_elem_nested_obj_undefined_own_57()
        => ExecutionTest("dstr/array-elem-nested-obj-undefined-own");

    [Fact(DisplayName = "dstr/array-elem-nested-obj-undefined.js")]
    public Task dstr_array_elem_nested_obj_undefined_58()
        => ExecutionTest("dstr/array-elem-nested-obj-undefined");

    [Fact(DisplayName = "dstr/array-elem-nested-obj-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_elem_nested_obj_yield_expr_59()
        => ExecutionTest("dstr/array-elem-nested-obj-yield-expr");

    [Fact(DisplayName = "dstr/array-elem-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_elem_nested_obj_yield_ident_invalid_60()
        => CompilationFailureTest("dstr/array-elem-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-nested-obj-yield-ident-valid.js")]
    public Task dstr_array_elem_nested_obj_yield_ident_valid_61()
        => ExecutionTest("dstr/array-elem-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "dstr/array-elem-nested-obj.js")]
    public Task dstr_array_elem_nested_obj_62()
        => ExecutionTest("dstr/array-elem-nested-obj");

    [Fact(DisplayName = "dstr/array-elem-put-const.js")]
    public Task dstr_array_elem_put_const_63()
        => ExecutionTest("dstr/array-elem-put-const");

    [Fact(DisplayName = "dstr/array-elem-put-let.js")]
    public Task dstr_array_elem_put_let_64()
        => ExecutionTest("dstr/array-elem-put-let");

    [Fact(DisplayName = "dstr/array-elem-put-obj-literal-optchain-prop-ref-init.js")]
    public Task dstr_array_elem_put_obj_literal_optchain_prop_ref_init_65()
        => CompilationFailureTest("dstr/array-elem-put-obj-literal-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-put-obj-literal-prop-ref-init-active.js")]
    public Task dstr_array_elem_put_obj_literal_prop_ref_init_active_66()
        => ExecutionTest("dstr/array-elem-put-obj-literal-prop-ref-init-active");

    [Fact(DisplayName = "dstr/array-elem-put-obj-literal-prop-ref-init.js")]
    public Task dstr_array_elem_put_obj_literal_prop_ref_init_67()
        => ExecutionTest("dstr/array-elem-put-obj-literal-prop-ref-init");

    [Fact(DisplayName = "dstr/array-elem-put-obj-literal-prop-ref.js")]
    public Task dstr_array_elem_put_obj_literal_prop_ref_68()
        => ExecutionTest("dstr/array-elem-put-obj-literal-prop-ref");

    [Fact(DisplayName = "dstr/array-elem-put-prop-ref-no-get.js")]
    public Task dstr_array_elem_put_prop_ref_no_get_69()
        => ExecutionTest("dstr/array-elem-put-prop-ref-no-get");

    [Fact(DisplayName = "dstr/array-elem-put-prop-ref-user-err.js")]
    public Task dstr_array_elem_put_prop_ref_user_err_70()
        => ExecutionTest("dstr/array-elem-put-prop-ref-user-err");

    [Fact(DisplayName = "dstr/array-elem-put-prop-ref.js")]
    public Task dstr_array_elem_put_prop_ref_71()
        => ExecutionTest("dstr/array-elem-put-prop-ref");

    [Fact(DisplayName = "dstr/array-elem-put-unresolvable-no-strict.js")]
    public Task dstr_array_elem_put_unresolvable_no_strict_72()
        => ExecutionTest("dstr/array-elem-put-unresolvable-no-strict");

    [Fact(DisplayName = "dstr/array-elem-put-unresolvable-strict.js")]
    public Task dstr_array_elem_put_unresolvable_strict_73()
        => ExecutionTest("dstr/array-elem-put-unresolvable-strict");

    [Fact(DisplayName = "dstr/array-elem-target-identifier.js")]
    public Task dstr_array_elem_target_identifier_74()
        => ExecutionTest("dstr/array-elem-target-identifier");

    [Fact(DisplayName = "dstr/array-elem-target-simple-no-strict.js")]
    public Task dstr_array_elem_target_simple_no_strict_75()
        => ExecutionTest("dstr/array-elem-target-simple-no-strict");

    [Fact(DisplayName = "dstr/array-elem-target-simple-strict.js")]
    public Task dstr_array_elem_target_simple_strict_76()
        => CompilationFailureTest("dstr/array-elem-target-simple-strict", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-target-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_elem_target_yield_expr_77()
        => ExecutionTest("dstr/array-elem-target-yield-expr");

    [Fact(DisplayName = "dstr/array-elem-target-yield-invalid.js")]
    public Task dstr_array_elem_target_yield_invalid_78()
        => CompilationFailureTest("dstr/array-elem-target-yield-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-elem-target-yield-valid.js")]
    public Task dstr_array_elem_target_yield_valid_79()
        => ExecutionTest("dstr/array-elem-target-yield-valid");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-elision-iter-abpt.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_abpt_80()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-abpt");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-elision-iter-nrml-close-err.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_err_81()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-elision-iter-nrml-close-null.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_null_82()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close-null");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-elision-iter-nrml-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_skip_83()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close-skip");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-elision-iter-nrml-close.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_84()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-get-err.js")]
    public Task dstr_array_elem_trlg_iter_get_err_85()
        => ExecutionTest("dstr/array-elem-trlg-iter-get-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-nrml-close-err.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_err_86()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-nrml-close-null.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_null_87()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close-null");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-nrml-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_skip_88()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close-skip");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-nrml-close.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_89()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-rtrn-close-err.js")]
    public Task dstr_array_elem_trlg_iter_list_rtrn_close_err_90()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-rtrn-close-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-rtrn-close-null.js")]
    public Task dstr_array_elem_trlg_iter_list_rtrn_close_null_91()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-rtrn-close-null");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-rtrn-close.js")]
    public Task dstr_array_elem_trlg_iter_list_rtrn_close_92()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-rtrn-close");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-thrw-close-err.js")]
    public Task dstr_array_elem_trlg_iter_list_thrw_close_err_93()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-thrw-close-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-thrw-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_list_thrw_close_skip_94()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-thrw-close-skip");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-list-thrw-close.js")]
    public Task dstr_array_elem_trlg_iter_list_thrw_close_95()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-thrw-close");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-nrml-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_rest_nrml_close_skip_96()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-nrml-close-skip");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-rtrn-close-err.js")]
    public Task dstr_array_elem_trlg_iter_rest_rtrn_close_err_97()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-rtrn-close-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-rtrn-close-null.js")]
    public Task dstr_array_elem_trlg_iter_rest_rtrn_close_null_98()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-rtrn-close-null");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-rtrn-close.js")]
    public Task dstr_array_elem_trlg_iter_rest_rtrn_close_99()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-rtrn-close");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-thrw-close-err.js")]
    public Task dstr_array_elem_trlg_iter_rest_thrw_close_err_100()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-thrw-close-err");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-thrw-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_rest_thrw_close_skip_101()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-thrw-close-skip");

    [Fact(DisplayName = "dstr/array-elem-trlg-iter-rest-thrw-close.js")]
    public Task dstr_array_elem_trlg_iter_rest_thrw_close_102()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-thrw-close");

    [Fact(DisplayName = "dstr/array-elision-iter-abpt.js")]
    public Task dstr_array_elision_iter_abpt_103()
        => ExecutionTest("dstr/array-elision-iter-abpt");

    [Fact(DisplayName = "dstr/array-elision-iter-get-err.js")]
    public Task dstr_array_elision_iter_get_err_104()
        => ExecutionTest("dstr/array-elision-iter-get-err");

    [Fact(DisplayName = "dstr/array-elision-iter-nrml-close-err.js")]
    public Task dstr_array_elision_iter_nrml_close_err_105()
        => ExecutionTest("dstr/array-elision-iter-nrml-close-err");

    [Fact(DisplayName = "dstr/array-elision-iter-nrml-close-null.js")]
    public Task dstr_array_elision_iter_nrml_close_null_106()
        => ExecutionTest("dstr/array-elision-iter-nrml-close-null");

    [Fact(DisplayName = "dstr/array-elision-iter-nrml-close-skip.js")]
    public Task dstr_array_elision_iter_nrml_close_skip_107()
        => ExecutionTest("dstr/array-elision-iter-nrml-close-skip");

    [Fact(DisplayName = "dstr/array-elision-iter-nrml-close.js")]
    public Task dstr_array_elision_iter_nrml_close_108()
        => ExecutionTest("dstr/array-elision-iter-nrml-close");

    [Fact(DisplayName = "dstr/array-elision-val-array.js")]
    public Task dstr_array_elision_val_array_109()
        => ExecutionTest("dstr/array-elision-val-array");

    [Fact(DisplayName = "dstr/array-elision-val-bool.js")]
    public Task dstr_array_elision_val_bool_110()
        => ExecutionTest("dstr/array-elision-val-bool");

    [Fact(DisplayName = "dstr/array-elision-val-null.js")]
    public Task dstr_array_elision_val_null_111()
        => ExecutionTest("dstr/array-elision-val-null");

    [Fact(DisplayName = "dstr/array-elision-val-num.js")]
    public Task dstr_array_elision_val_num_112()
        => ExecutionTest("dstr/array-elision-val-num");

    [Fact(DisplayName = "dstr/array-elision-val-string.js")]
    public Task dstr_array_elision_val_string_113()
        => ExecutionTest("dstr/array-elision-val-string");

    [Fact(DisplayName = "dstr/array-elision-val-symbol.js")]
    public Task dstr_array_elision_val_symbol_114()
        => ExecutionTest("dstr/array-elision-val-symbol");

    [Fact(DisplayName = "dstr/array-elision-val-undef.js")]
    public Task dstr_array_elision_val_undef_115()
        => ExecutionTest("dstr/array-elision-val-undef");

    [Fact(DisplayName = "dstr/array-empty-iter-close-err.js")]
    public Task dstr_array_empty_iter_close_err_116()
        => ExecutionTest("dstr/array-empty-iter-close-err");

    [Fact(DisplayName = "dstr/array-empty-iter-close-null.js")]
    public Task dstr_array_empty_iter_close_null_117()
        => ExecutionTest("dstr/array-empty-iter-close-null");

    [Fact(DisplayName = "dstr/array-empty-iter-close.js")]
    public Task dstr_array_empty_iter_close_118()
        => ExecutionTest("dstr/array-empty-iter-close");

    [Fact(DisplayName = "dstr/array-empty-iter-get-err.js")]
    public Task dstr_array_empty_iter_get_err_119()
        => ExecutionTest("dstr/array-empty-iter-get-err");

    [Fact(DisplayName = "dstr/array-empty-val-array.js")]
    public Task dstr_array_empty_val_array_120()
        => ExecutionTest("dstr/array-empty-val-array");

    [Fact(DisplayName = "dstr/array-empty-val-bool.js")]
    public Task dstr_array_empty_val_bool_121()
        => ExecutionTest("dstr/array-empty-val-bool");

    [Fact(DisplayName = "dstr/array-empty-val-null.js")]
    public Task dstr_array_empty_val_null_122()
        => ExecutionTest("dstr/array-empty-val-null");

    [Fact(DisplayName = "dstr/array-empty-val-num.js")]
    public Task dstr_array_empty_val_num_123()
        => ExecutionTest("dstr/array-empty-val-num");

    [Fact(DisplayName = "dstr/array-empty-val-string.js")]
    public Task dstr_array_empty_val_string_124()
        => ExecutionTest("dstr/array-empty-val-string");

    [Fact(DisplayName = "dstr/array-empty-val-symbol.js")]
    public Task dstr_array_empty_val_symbol_125()
        => ExecutionTest("dstr/array-empty-val-symbol");

    [Fact(DisplayName = "dstr/array-empty-val-undef.js")]
    public Task dstr_array_empty_val_undef_126()
        => ExecutionTest("dstr/array-empty-val-undef");

    [Fact(DisplayName = "dstr/array-iteration.js")]
    public Task dstr_array_iteration_127()
        => ExecutionTest("dstr/array-iteration");

    [Fact(DisplayName = "dstr/array-rest-after-element.js")]
    public Task dstr_array_rest_after_element_128()
        => ExecutionTest("dstr/array-rest-after-element");

    [Fact(DisplayName = "dstr/array-rest-after-elision.js")]
    public Task dstr_array_rest_after_elision_129()
        => ExecutionTest("dstr/array-rest-after-elision");

    [Fact(DisplayName = "dstr/array-rest-before-element.js")]
    public Task dstr_array_rest_before_element_130()
        => CompilationFailureTest("dstr/array-rest-before-element", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-before-elision.js")]
    public Task dstr_array_rest_before_elision_131()
        => CompilationFailureTest("dstr/array-rest-before-elision", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-before-rest.js")]
    public Task dstr_array_rest_before_rest_132()
        => CompilationFailureTest("dstr/array-rest-before-rest", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-elision-invalid.js")]
    public Task dstr_array_rest_elision_invalid_133()
        => CompilationFailureTest("dstr/array-rest-elision-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-elision-iter-abpt.js")]
    public Task dstr_array_rest_elision_iter_abpt_134()
        => ExecutionTest("dstr/array-rest-elision-iter-abpt");

    [Fact(DisplayName = "dstr/array-rest-elision.js")]
    public Task dstr_array_rest_elision_135()
        => ExecutionTest("dstr/array-rest-elision");

    [Fact(DisplayName = "dstr/array-rest-init.js")]
    public Task dstr_array_rest_init_136()
        => CompilationFailureTest("dstr/array-rest-init", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-iter-get-err.js")]
    public Task dstr_array_rest_iter_get_err_137()
        => ExecutionTest("dstr/array-rest-iter-get-err");

    [Fact(DisplayName = "dstr/array-rest-iter-nrml-close-skip.js")]
    public Task dstr_array_rest_iter_nrml_close_skip_138()
        => ExecutionTest("dstr/array-rest-iter-nrml-close-skip");

    [Fact(DisplayName = "dstr/array-rest-iter-rtrn-close-err.js")]
    public Task dstr_array_rest_iter_rtrn_close_err_139()
        => ExecutionTest("dstr/array-rest-iter-rtrn-close-err");

    [Fact(DisplayName = "dstr/array-rest-iter-rtrn-close-null.js")]
    public Task dstr_array_rest_iter_rtrn_close_null_140()
        => ExecutionTest("dstr/array-rest-iter-rtrn-close-null");

    [Fact(DisplayName = "dstr/array-rest-iter-rtrn-close.js")]
    public Task dstr_array_rest_iter_rtrn_close_141()
        => ExecutionTest("dstr/array-rest-iter-rtrn-close");

    [Fact(DisplayName = "dstr/array-rest-iter-thrw-close-err.js")]
    public Task dstr_array_rest_iter_thrw_close_err_142()
        => ExecutionTest("dstr/array-rest-iter-thrw-close-err");

    [Fact(DisplayName = "dstr/array-rest-iter-thrw-close-skip.js")]
    public Task dstr_array_rest_iter_thrw_close_skip_143()
        => ExecutionTest("dstr/array-rest-iter-thrw-close-skip");

    [Fact(DisplayName = "dstr/array-rest-iter-thrw-close.js")]
    public Task dstr_array_rest_iter_thrw_close_144()
        => ExecutionTest("dstr/array-rest-iter-thrw-close");

    [Fact(DisplayName = "dstr/array-rest-iteration.js")]
    public Task dstr_array_rest_iteration_145()
        => ExecutionTest("dstr/array-rest-iteration");

    [Fact(DisplayName = "dstr/array-rest-lref-err.js")]
    public Task dstr_array_rest_lref_err_146()
        => ExecutionTest("dstr/array-rest-lref-err");

    [Fact(DisplayName = "dstr/array-rest-lref.js")]
    public Task dstr_array_rest_lref_147()
        => ExecutionTest("dstr/array-rest-lref");

    [Fact(DisplayName = "dstr/array-rest-nested-array-invalid.js")]
    public Task dstr_array_rest_nested_array_invalid_148()
        => CompilationFailureTest("dstr/array-rest-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-array-iter-thrw-close-skip.js")]
    public Task dstr_array_rest_nested_array_iter_thrw_close_skip_149()
        => ExecutionTest("dstr/array-rest-nested-array-iter-thrw-close-skip");

    [Fact(DisplayName = "dstr/array-rest-nested-array-null.js")]
    public Task dstr_array_rest_nested_array_null_150()
        => ExecutionTest("dstr/array-rest-nested-array-null");

    [Fact(DisplayName = "dstr/array-rest-nested-array-undefined-hole.js")]
    public Task dstr_array_rest_nested_array_undefined_hole_151()
        => ExecutionTest("dstr/array-rest-nested-array-undefined-hole");

    [Fact(DisplayName = "dstr/array-rest-nested-array-undefined-own.js")]
    public Task dstr_array_rest_nested_array_undefined_own_152()
        => ExecutionTest("dstr/array-rest-nested-array-undefined-own");

    [Fact(DisplayName = "dstr/array-rest-nested-array-undefined.js")]
    public Task dstr_array_rest_nested_array_undefined_153()
        => ExecutionTest("dstr/array-rest-nested-array-undefined");

    [Fact(DisplayName = "dstr/array-rest-nested-array-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_rest_nested_array_yield_expr_154()
        => ExecutionTest("dstr/array-rest-nested-array-yield-expr");

    [Fact(DisplayName = "dstr/array-rest-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_rest_nested_array_yield_ident_invalid_155()
        => CompilationFailureTest("dstr/array-rest-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-array-yield-ident-valid.js")]
    public Task dstr_array_rest_nested_array_yield_ident_valid_156()
        => ExecutionTest("dstr/array-rest-nested-array-yield-ident-valid");

    [Fact(DisplayName = "dstr/array-rest-nested-array.js")]
    public Task dstr_array_rest_nested_array_157()
        => ExecutionTest("dstr/array-rest-nested-array");

    [Fact(DisplayName = "dstr/array-rest-nested-obj-invalid.js")]
    public Task dstr_array_rest_nested_obj_invalid_158()
        => CompilationFailureTest("dstr/array-rest-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-obj-null.js")]
    public Task dstr_array_rest_nested_obj_null_159()
        => ExecutionTest("dstr/array-rest-nested-obj-null");

    [Fact(DisplayName = "dstr/array-rest-nested-obj-undefined-hole.js")]
    public Task dstr_array_rest_nested_obj_undefined_hole_160()
        => ExecutionTest("dstr/array-rest-nested-obj-undefined-hole");

    [Fact(DisplayName = "dstr/array-rest-nested-obj-undefined-own.js")]
    public Task dstr_array_rest_nested_obj_undefined_own_161()
        => ExecutionTest("dstr/array-rest-nested-obj-undefined-own");

    [Fact(DisplayName = "dstr/array-rest-nested-obj-undefined.js")]
    public Task dstr_array_rest_nested_obj_undefined_162()
        => ExecutionTest("dstr/array-rest-nested-obj-undefined");

    [Fact(DisplayName = "dstr/array-rest-nested-obj-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_rest_nested_obj_yield_expr_163()
        => ExecutionTest("dstr/array-rest-nested-obj-yield-expr");

    [Fact(DisplayName = "dstr/array-rest-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_rest_nested_obj_yield_ident_invalid_164()
        => CompilationFailureTest("dstr/array-rest-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-nested-obj-yield-ident-valid.js")]
    public Task dstr_array_rest_nested_obj_yield_ident_valid_165()
        => ExecutionTest("dstr/array-rest-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "dstr/array-rest-nested-obj.js")]
    public Task dstr_array_rest_nested_obj_166()
        => ExecutionTest("dstr/array-rest-nested-obj");

    [Fact(DisplayName = "dstr/array-rest-put-const.js")]
    public Task dstr_array_rest_put_const_167()
        => ExecutionTest("dstr/array-rest-put-const");

    [Fact(DisplayName = "dstr/array-rest-put-let.js")]
    public Task dstr_array_rest_put_let_168()
        => ExecutionTest("dstr/array-rest-put-let");

    [Fact(DisplayName = "dstr/array-rest-put-prop-ref-no-get.js")]
    public Task dstr_array_rest_put_prop_ref_no_get_169()
        => ExecutionTest("dstr/array-rest-put-prop-ref-no-get");

    [Fact(DisplayName = "dstr/array-rest-put-prop-ref-user-err-iter-close-skip.js")]
    public Task dstr_array_rest_put_prop_ref_user_err_iter_close_skip_170()
        => ExecutionTest("dstr/array-rest-put-prop-ref-user-err-iter-close-skip");

    [Fact(DisplayName = "dstr/array-rest-put-prop-ref-user-err.js")]
    public Task dstr_array_rest_put_prop_ref_user_err_171()
        => ExecutionTest("dstr/array-rest-put-prop-ref-user-err");

    [Fact(DisplayName = "dstr/array-rest-put-prop-ref.js")]
    public Task dstr_array_rest_put_prop_ref_172()
        => ExecutionTest("dstr/array-rest-put-prop-ref");

    [Fact(DisplayName = "dstr/array-rest-put-unresolvable-no-strict.js")]
    public Task dstr_array_rest_put_unresolvable_no_strict_173()
        => ExecutionTest("dstr/array-rest-put-unresolvable-no-strict");

    [Fact(DisplayName = "dstr/array-rest-put-unresolvable-strict.js")]
    public Task dstr_array_rest_put_unresolvable_strict_174()
        => ExecutionTest("dstr/array-rest-put-unresolvable-strict");

    [Fact(DisplayName = "dstr/array-rest-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_array_rest_yield_expr_175()
        => ExecutionTest("dstr/array-rest-yield-expr");

    [Fact(DisplayName = "dstr/array-rest-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_array_rest_yield_ident_invalid_176()
        => CompilationFailureTest("dstr/array-rest-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/array-rest-yield-ident-valid.js")]
    public Task dstr_array_rest_yield_ident_valid_177()
        => ExecutionTest("dstr/array-rest-yield-ident-valid");

    [Fact(DisplayName = "dstr/const-ary-init-iter-close.js")]
    public Task dstr_const_ary_init_iter_close_178()
        => ExecutionTest("dstr/const-ary-init-iter-close");

    [Fact(DisplayName = "dstr/const-ary-init-iter-get-err-array-prototype.js")]
    public Task dstr_const_ary_init_iter_get_err_array_prototype_179()
        => ExecutionTest("dstr/const-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dstr/const-ary-init-iter-get-err.js")]
    public Task dstr_const_ary_init_iter_get_err_180()
        => ExecutionTest("dstr/const-ary-init-iter-get-err");

    [Fact(DisplayName = "dstr/const-ary-init-iter-no-close.js")]
    public Task dstr_const_ary_init_iter_no_close_181()
        => ExecutionTest("dstr/const-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/const-ary-name-iter-val.js")]
    public Task dstr_const_ary_name_iter_val_182()
        => ExecutionTest("dstr/const-ary-name-iter-val");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-elem-init.js")]
    public Task dstr_const_ary_ptrn_elem_ary_elem_init_183()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-elem-iter.js")]
    public Task dstr_const_ary_ptrn_elem_ary_elem_iter_184()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-elision-init.js")]
    public Task dstr_const_ary_ptrn_elem_ary_elision_init_185()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-elision-iter.js")]
    public Task dstr_const_ary_ptrn_elem_ary_elision_iter_186()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_ary_empty_init_187()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_ary_empty_iter_188()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-rest-init.js")]
    public Task dstr_const_ary_ptrn_elem_ary_rest_init_189()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_ary_rest_iter_190()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-ary-val-null.js")]
    public Task dstr_const_ary_ptrn_elem_ary_val_null_191()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-exhausted.js")]
    public Task dstr_const_ary_ptrn_elem_id_init_exhausted_192()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_id_init_fn_name_arrow_193()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_id_init_fn_name_class_194()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_id_init_fn_name_cover_195()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_id_init_fn_name_fn_196()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_id_init_fn_name_gen_197()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-hole.js")]
    public Task dstr_const_ary_ptrn_elem_id_init_hole_198()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-skipped.js")]
    public Task dstr_const_ary_ptrn_elem_id_init_skipped_199()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-throws.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_ary_ptrn_elem_id_init_throws_200()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-undef.js")]
    public Task dstr_const_ary_ptrn_elem_id_init_undef_201()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task dstr_const_ary_ptrn_elem_id_init_unresolvable_202()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-iter-complete.js")]
    public Task dstr_const_ary_ptrn_elem_id_iter_complete_203()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-iter-done.js")]
    public Task dstr_const_ary_ptrn_elem_id_iter_done_204()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-iter-step-err.js")]
    public Task dstr_const_ary_ptrn_elem_id_iter_step_err_205()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task dstr_const_ary_ptrn_elem_id_iter_val_array_prototype_206()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-iter-val-err.js")]
    public Task dstr_const_ary_ptrn_elem_id_iter_val_err_207()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-id-iter-val.js")]
    public Task dstr_const_ary_ptrn_elem_id_iter_val_208()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-obj-id-init.js")]
    public Task dstr_const_ary_ptrn_elem_obj_id_init_209()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-obj-id.js")]
    public Task dstr_const_ary_ptrn_elem_obj_id_210()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task dstr_const_ary_ptrn_elem_obj_prop_id_init_211()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-obj-prop-id.js")]
    public Task dstr_const_ary_ptrn_elem_obj_prop_id_212()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-obj-val-null.js")]
    public Task dstr_const_ary_ptrn_elem_obj_val_null_213()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elem-obj-val-undef.js")]
    public Task dstr_const_ary_ptrn_elem_obj_val_undef_214()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elision-exhausted.js")]
    public Task dstr_const_ary_ptrn_elision_exhausted_215()
        => ExecutionTest("dstr/const-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elision-iter-close.js")]
    public Task dstr_const_ary_ptrn_elision_iter_close_216()
        => ExecutionTest("dstr/const-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elision-step-err.js")]
    public Task dstr_const_ary_ptrn_elision_step_err_217()
        => ExecutionTest("dstr/const-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dstr/const-ary-ptrn-elision.js")]
    public Task dstr_const_ary_ptrn_elision_218()
        => ExecutionTest("dstr/const-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/const-ary-ptrn-empty.js")]
    public Task dstr_const_ary_ptrn_empty_219()
        => ExecutionTest("dstr/const-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/const-ary-ptrn-init-err.js")]
    public Task dstr_const_ary_ptrn_init_err_220()
        => CompilationFailureTest("dstr/const-ary-ptrn-init-err", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-ary-elem.js")]
    public Task dstr_const_ary_ptrn_rest_ary_elem_221()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-ary-elision.js")]
    public Task dstr_const_ary_ptrn_rest_ary_elision_222()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-ary-empty.js")]
    public Task dstr_const_ary_ptrn_rest_ary_empty_223()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-ary-rest.js")]
    public Task dstr_const_ary_ptrn_rest_ary_rest_224()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-direct.js")]
    public Task dstr_const_ary_ptrn_rest_id_direct_225()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-elision-next-err.js")]
    public Task dstr_const_ary_ptrn_rest_id_elision_next_err_226()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-elision.js")]
    public Task dstr_const_ary_ptrn_rest_id_elision_227()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-exhausted.js")]
    public Task dstr_const_ary_ptrn_rest_id_exhausted_228()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-iter-close.js")]
    public Task dstr_const_ary_ptrn_rest_id_iter_close_229()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-iter-step-err.js")]
    public Task dstr_const_ary_ptrn_rest_id_iter_step_err_230()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id-iter-val-err.js")]
    public Task dstr_const_ary_ptrn_rest_id_iter_val_err_231()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-id.js")]
    public Task dstr_const_ary_ptrn_rest_id_232()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-init-ary.js")]
    public Task dstr_const_ary_ptrn_rest_init_ary_233()
        => CompilationFailureTest("dstr/const-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-init-id.js")]
    public Task dstr_const_ary_ptrn_rest_init_id_234()
        => CompilationFailureTest("dstr/const-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-init-obj.js")]
    public Task dstr_const_ary_ptrn_rest_init_obj_235()
        => CompilationFailureTest("dstr/const-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_const_ary_ptrn_rest_not_final_ary_236()
        => CompilationFailureTest("dstr/const-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-not-final-id.js")]
    public Task dstr_const_ary_ptrn_rest_not_final_id_237()
        => CompilationFailureTest("dstr/const-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_const_ary_ptrn_rest_not_final_obj_238()
        => CompilationFailureTest("dstr/const-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-obj-id.js")]
    public Task dstr_const_ary_ptrn_rest_obj_id_239()
        => ExecutionTest("dstr/const-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/const-ary-ptrn-rest-obj-prop-id.js")]
    public Task dstr_const_ary_ptrn_rest_obj_prop_id_240()
        => ExecutionTest("dstr/const-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/const-obj-init-null.js")]
    public Task dstr_const_obj_init_null_241()
        => ExecutionTest("dstr/const-obj-init-null");

    [Fact(DisplayName = "dstr/const-obj-init-undefined.js")]
    public Task dstr_const_obj_init_undefined_242()
        => ExecutionTest("dstr/const-obj-init-undefined");

    [Fact(DisplayName = "dstr/const-obj-ptrn-empty.js")]
    public Task dstr_const_obj_ptrn_empty_243()
        => ExecutionTest("dstr/const-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-get-value-err.js")]
    public Task dstr_const_obj_ptrn_id_get_value_err_244()
        => ExecutionTest("dstr/const-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_id_init_fn_name_arrow_245()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_id_init_fn_name_class_246()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_id_init_fn_name_cover_247()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_id_init_fn_name_fn_248()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_id_init_fn_name_gen_249()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-skipped.js")]
    public Task dstr_const_obj_ptrn_id_init_skipped_250()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-skipped");
}
