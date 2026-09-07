using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.assignment;

public class AssignmentExpressionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public AssignmentExpressionConformanceBatchExecutionTests() : base("language/expressions/assignment", "language.expressions.assignment") { }

    [Fact(DisplayName = "S11.13.1_A2.1_T1.js")]
    public Task S11_13_1_A2_1_T1()
        => ExecutionTest("S11.13.1_A2.1_T1");

    [Fact(DisplayName = "S11.13.1_A2.1_T2.js")]
    public Task S11_13_1_A2_1_T2()
        => ExecutionTest("S11.13.1_A2.1_T2");

    [Fact(DisplayName = "S11.13.1_A3.1.js")]
    public Task S11_13_1_A3_1()
        => ExecutionTest("S11.13.1_A3.1");

    [Fact(DisplayName = "S11.13.1_A3.2.js")]
    public Task S11_13_1_A3_2()
        => ExecutionTest("S11.13.1_A3.2");

    [Fact(DisplayName = "S11.13.1_A4_T1.js")]
    public Task S11_13_1_A4_T1()
        => ExecutionTest("S11.13.1_A4_T1");

    [Fact(DisplayName = "S11.13.1_A4_T2.js")]
    public Task S11_13_1_A4_T2()
        => ExecutionTest("S11.13.1_A4_T2");

    [Fact(DisplayName = "S11.13.1_A6_T3.js")]
    public Task S11_13_1_A6_T3()
        => ExecutionTest("S11.13.1_A6_T3");

    [Fact(DisplayName = "S11.13.1_A7_T4.js")]
    public Task S11_13_1_A7_T4()
        => ExecutionTest("S11.13.1_A7_T4");

    [Fact(DisplayName = "S8.12.4_A1.js")]
    public Task S8_12_4_A1()
        => ExecutionTest("S8.12.4_A1");

    [Fact(DisplayName = "S8.12.5_A1.js")]
    public Task S8_12_5_A1()
        => ExecutionTest("S8.12.5_A1");

    [Fact(DisplayName = "S8.12.5_A2.js")]
    public Task S8_12_5_A2()
        => ExecutionTest("S8.12.5_A2");

    [Fact(DisplayName = "array-elem-init-assignment.js")]
    public Task dstr_array_elem_init_assignment()
        => ExecutionTest("dstr/array-elem-init-assignment");

    [Fact(DisplayName = "array-elem-init-evaluation.js")]
    public Task dstr_array_elem_init_evaluation()
        => ExecutionTest("dstr/array-elem-init-evaluation");

    [Fact(DisplayName = "array-elem-init-fn-name-arrow.js")]
    public Task dstr_array_elem_init_fn_name_arrow()
        => ExecutionTest("dstr/array-elem-init-fn-name-arrow");

    [Fact(DisplayName = "array-elem-init-fn-name-class.js")]
    public Task dstr_array_elem_init_fn_name_class()
        => ExecutionTest("dstr/array-elem-init-fn-name-class");

    [Fact(DisplayName = "array-elem-init-fn-name-cover.js")]
    public Task dstr_array_elem_init_fn_name_cover()
        => ExecutionTest("dstr/array-elem-init-fn-name-cover");

    [Fact(DisplayName = "array-elem-init-fn-name-fn.js")]
    public Task dstr_array_elem_init_fn_name_fn()
        => ExecutionTest("dstr/array-elem-init-fn-name-fn");

    [Fact(DisplayName = "array-elem-init-fn-name-gen.js")]
    public Task dstr_array_elem_init_fn_name_gen()
        => ExecutionTest("dstr/array-elem-init-fn-name-gen");

    [Fact(DisplayName = "array-elem-init-in.js")]
    public Task dstr_array_elem_init_in()
        => ExecutionTest("dstr/array-elem-init-in");

    [Fact(DisplayName = "array-elem-init-let.js")]
    public Task dstr_array_elem_init_let()
        => ExecutionTest("dstr/array-elem-init-let");

    [Fact(DisplayName = "array-elem-init-order.js")]
    public Task dstr_array_elem_init_order()
        => ExecutionTest("dstr/array-elem-init-order");

    [Fact(DisplayName = "array-elem-init-simple-no-strict.js")]
    public Task dstr_array_elem_init_simple_no_strict()
        => ExecutionTest("dstr/array-elem-init-simple-no-strict");

    [Fact(DisplayName = "array-elem-init-yield-expr.js")]
    public Task dstr_array_elem_init_yield_expr()
        => ExecutionTest("dstr/array-elem-init-yield-expr");

    [Fact(DisplayName = "array-elem-init-yield-ident-valid.js")]
    public Task dstr_array_elem_init_yield_ident_valid()
        => ExecutionTest("dstr/array-elem-init-yield-ident-valid");

    [Fact(DisplayName = "array-elem-iter-get-err.js")]
    public Task dstr_array_elem_iter_get_err()
        => ExecutionTest("dstr/array-elem-iter-get-err");

    [Fact(DisplayName = "array-elem-iter-nrml-close-err.js")]
    public Task dstr_array_elem_iter_nrml_close_err()
        => ExecutionTest("dstr/array-elem-iter-nrml-close-err");

    [Fact(DisplayName = "array-elem-iter-nrml-close-null.js")]
    public Task dstr_array_elem_iter_nrml_close_null()
        => ExecutionTest("dstr/array-elem-iter-nrml-close-null");

    [Fact(DisplayName = "array-elem-iter-nrml-close-skip.js")]
    public Task dstr_array_elem_iter_nrml_close_skip()
        => ExecutionTest("dstr/array-elem-iter-nrml-close-skip");

    [Fact(DisplayName = "array-elem-iter-nrml-close.js")]
    public Task dstr_array_elem_iter_nrml_close()
        => ExecutionTest("dstr/array-elem-iter-nrml-close");

    [Fact(DisplayName = "array-elem-iter-rtrn-close-err.js")]
    public Task dstr_array_elem_iter_rtrn_close_err()
        => ExecutionTest("dstr/array-elem-iter-rtrn-close-err");

    [Fact(DisplayName = "array-elem-iter-rtrn-close-null.js")]
    public Task dstr_array_elem_iter_rtrn_close_null()
        => ExecutionTest("dstr/array-elem-iter-rtrn-close-null");

    [Fact(DisplayName = "array-elem-iter-rtrn-close.js")]
    public Task dstr_array_elem_iter_rtrn_close()
        => ExecutionTest("dstr/array-elem-iter-rtrn-close");

    [Fact(DisplayName = "array-elem-iter-thrw-close-err.js")]
    public Task dstr_array_elem_iter_thrw_close_err()
        => ExecutionTest("dstr/array-elem-iter-thrw-close-err");

    [Fact(DisplayName = "array-elem-iter-thrw-close-skip.js")]
    public Task dstr_array_elem_iter_thrw_close_skip()
        => ExecutionTest("dstr/array-elem-iter-thrw-close-skip");

    [Fact(DisplayName = "array-elem-iter-thrw-close.js")]
    public Task dstr_array_elem_iter_thrw_close()
        => ExecutionTest("dstr/array-elem-iter-thrw-close");

    [Fact(DisplayName = "array-elem-nested-array-null.js")]
    public Task dstr_array_elem_nested_array_null()
        => ExecutionTest("dstr/array-elem-nested-array-null");

    [Fact(DisplayName = "array-elem-nested-array-undefined-hole.js")]
    public Task dstr_array_elem_nested_array_undefined_hole()
        => ExecutionTest("dstr/array-elem-nested-array-undefined-hole");

    [Fact(DisplayName = "array-elem-nested-array-undefined-own.js")]
    public Task dstr_array_elem_nested_array_undefined_own()
        => ExecutionTest("dstr/array-elem-nested-array-undefined-own");

    [Fact(DisplayName = "array-elem-nested-array-undefined.js")]
    public Task dstr_array_elem_nested_array_undefined()
        => ExecutionTest("dstr/array-elem-nested-array-undefined");

    [Fact(DisplayName = "array-elem-nested-array-yield-expr.js")]
    public Task dstr_array_elem_nested_array_yield_expr()
        => ExecutionTest("dstr/array-elem-nested-array-yield-expr");

    [Fact(DisplayName = "array-elem-nested-array-yield-ident-valid.js")]
    public Task dstr_array_elem_nested_array_yield_ident_valid()
        => ExecutionTest("dstr/array-elem-nested-array-yield-ident-valid");

    [Fact(DisplayName = "array-elem-nested-array.js")]
    public Task dstr_array_elem_nested_array()
        => ExecutionTest("dstr/array-elem-nested-array");

    [Fact(DisplayName = "array-elem-nested-obj-null.js")]
    public Task dstr_array_elem_nested_obj_null()
        => ExecutionTest("dstr/array-elem-nested-obj-null");

    [Fact(DisplayName = "array-elem-nested-obj-undefined-hole.js")]
    public Task dstr_array_elem_nested_obj_undefined_hole()
        => ExecutionTest("dstr/array-elem-nested-obj-undefined-hole");

    [Fact(DisplayName = "array-elem-nested-obj-undefined-own.js")]
    public Task dstr_array_elem_nested_obj_undefined_own()
        => ExecutionTest("dstr/array-elem-nested-obj-undefined-own");

    [Fact(DisplayName = "array-elem-nested-obj-undefined.js")]
    public Task dstr_array_elem_nested_obj_undefined()
        => ExecutionTest("dstr/array-elem-nested-obj-undefined");

    [Fact(DisplayName = "array-elem-nested-obj-yield-expr.js")]
    public Task dstr_array_elem_nested_obj_yield_expr()
        => ExecutionTest("dstr/array-elem-nested-obj-yield-expr");

    [Fact(DisplayName = "array-elem-nested-obj-yield-ident-valid.js")]
    public Task dstr_array_elem_nested_obj_yield_ident_valid()
        => ExecutionTest("dstr/array-elem-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "array-elem-nested-obj.js")]
    public Task dstr_array_elem_nested_obj()
        => ExecutionTest("dstr/array-elem-nested-obj");

    [Fact(DisplayName = "array-elem-put-const.js")]
    public Task dstr_array_elem_put_const()
        => ExecutionTest("dstr/array-elem-put-const");

    [Fact(DisplayName = "array-elem-put-let.js")]
    public Task dstr_array_elem_put_let()
        => ExecutionTest("dstr/array-elem-put-let");

    [Fact(DisplayName = "array-elem-put-obj-literal-prop-ref-init-active.js")]
    public Task dstr_array_elem_put_obj_literal_prop_ref_init_active()
        => ExecutionTest("dstr/array-elem-put-obj-literal-prop-ref-init-active");

    [Fact(DisplayName = "array-elem-put-obj-literal-prop-ref-init.js")]
    public Task dstr_array_elem_put_obj_literal_prop_ref_init()
        => ExecutionTest("dstr/array-elem-put-obj-literal-prop-ref-init");

    [Fact(DisplayName = "array-elem-put-obj-literal-prop-ref.js")]
    public Task dstr_array_elem_put_obj_literal_prop_ref()
        => ExecutionTest("dstr/array-elem-put-obj-literal-prop-ref");

    [Fact(DisplayName = "array-elem-put-prop-ref-no-get.js")]
    public Task dstr_array_elem_put_prop_ref_no_get()
        => ExecutionTest("dstr/array-elem-put-prop-ref-no-get");

    [Fact(DisplayName = "array-elem-put-prop-ref-user-err.js")]
    public Task dstr_array_elem_put_prop_ref_user_err()
        => ExecutionTest("dstr/array-elem-put-prop-ref-user-err");

    [Fact(DisplayName = "array-elem-put-prop-ref.js")]
    public Task dstr_array_elem_put_prop_ref()
        => ExecutionTest("dstr/array-elem-put-prop-ref");

    [Fact(DisplayName = "array-elem-put-unresolvable-no-strict.js")]
    public Task dstr_array_elem_put_unresolvable_no_strict()
        => ExecutionTest("dstr/array-elem-put-unresolvable-no-strict");

    [Fact(DisplayName = "array-elem-put-unresolvable-strict.js")]
    public Task dstr_array_elem_put_unresolvable_strict()
        => ExecutionTest("dstr/array-elem-put-unresolvable-strict");

    [Fact(DisplayName = "array-elem-target-identifier.js")]
    public Task dstr_array_elem_target_identifier()
        => ExecutionTest("dstr/array-elem-target-identifier");

    [Fact(DisplayName = "array-elem-target-simple-no-strict.js")]
    public Task dstr_array_elem_target_simple_no_strict()
        => ExecutionTest("dstr/array-elem-target-simple-no-strict");

    [Fact(DisplayName = "array-elem-target-yield-expr.js")]
    public Task dstr_array_elem_target_yield_expr()
        => ExecutionTest("dstr/array-elem-target-yield-expr");

    [Fact(DisplayName = "array-elem-target-yield-valid.js")]
    public Task dstr_array_elem_target_yield_valid()
        => ExecutionTest("dstr/array-elem-target-yield-valid");

    [Fact(DisplayName = "array-elem-trlg-iter-elision-iter-abpt.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_abpt()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-abpt");

    [Fact(DisplayName = "array-elem-trlg-iter-elision-iter-nrml-close-err.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close-err");

    [Fact(DisplayName = "array-elem-trlg-iter-elision-iter-nrml-close-null.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_null()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close-null");

    [Fact(DisplayName = "array-elem-trlg-iter-elision-iter-nrml-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close_skip()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close-skip");

    [Fact(DisplayName = "array-elem-trlg-iter-elision-iter-nrml-close.js")]
    public Task dstr_array_elem_trlg_iter_elision_iter_nrml_close()
        => ExecutionTest("dstr/array-elem-trlg-iter-elision-iter-nrml-close");

    [Fact(DisplayName = "array-elem-trlg-iter-get-err.js")]
    public Task dstr_array_elem_trlg_iter_get_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-get-err");

    [Fact(DisplayName = "array-elem-trlg-iter-list-nrml-close-err.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close-err");

    [Fact(DisplayName = "array-elem-trlg-iter-list-nrml-close-null.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_null()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close-null");

    [Fact(DisplayName = "array-elem-trlg-iter-list-nrml-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close_skip()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close-skip");

    [Fact(DisplayName = "array-elem-trlg-iter-list-nrml-close.js")]
    public Task dstr_array_elem_trlg_iter_list_nrml_close()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-nrml-close");

    [Fact(DisplayName = "array-elem-trlg-iter-list-rtrn-close-err.js")]
    public Task dstr_array_elem_trlg_iter_list_rtrn_close_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-rtrn-close-err");

    [Fact(DisplayName = "array-elem-trlg-iter-list-rtrn-close-null.js")]
    public Task dstr_array_elem_trlg_iter_list_rtrn_close_null()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-rtrn-close-null");

    [Fact(DisplayName = "array-elem-trlg-iter-list-rtrn-close.js")]
    public Task dstr_array_elem_trlg_iter_list_rtrn_close()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-rtrn-close");

    [Fact(DisplayName = "array-elem-trlg-iter-list-thrw-close-err.js")]
    public Task dstr_array_elem_trlg_iter_list_thrw_close_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-thrw-close-err");

    [Fact(DisplayName = "array-elem-trlg-iter-list-thrw-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_list_thrw_close_skip()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-thrw-close-skip");

    [Fact(DisplayName = "array-elem-trlg-iter-list-thrw-close.js")]
    public Task dstr_array_elem_trlg_iter_list_thrw_close()
        => ExecutionTest("dstr/array-elem-trlg-iter-list-thrw-close");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-nrml-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_rest_nrml_close_skip()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-nrml-close-skip");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-rtrn-close-err.js")]
    public Task dstr_array_elem_trlg_iter_rest_rtrn_close_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-rtrn-close-err");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-rtrn-close-null.js")]
    public Task dstr_array_elem_trlg_iter_rest_rtrn_close_null()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-rtrn-close-null");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-rtrn-close.js")]
    public Task dstr_array_elem_trlg_iter_rest_rtrn_close()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-rtrn-close");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-thrw-close-err.js")]
    public Task dstr_array_elem_trlg_iter_rest_thrw_close_err()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-thrw-close-err");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-thrw-close-skip.js")]
    public Task dstr_array_elem_trlg_iter_rest_thrw_close_skip()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-thrw-close-skip");

    [Fact(DisplayName = "array-elem-trlg-iter-rest-thrw-close.js")]
    public Task dstr_array_elem_trlg_iter_rest_thrw_close()
        => ExecutionTest("dstr/array-elem-trlg-iter-rest-thrw-close");

    [Fact(DisplayName = "array-elision-iter-abpt.js")]
    public Task dstr_array_elision_iter_abpt()
        => ExecutionTest("dstr/array-elision-iter-abpt");

    [Fact(DisplayName = "array-elision-iter-get-err.js")]
    public Task dstr_array_elision_iter_get_err()
        => ExecutionTest("dstr/array-elision-iter-get-err");

    [Fact(DisplayName = "array-elision-iter-nrml-close-err.js")]
    public Task dstr_array_elision_iter_nrml_close_err()
        => ExecutionTest("dstr/array-elision-iter-nrml-close-err");

    [Fact(DisplayName = "array-elision-iter-nrml-close-null.js")]
    public Task dstr_array_elision_iter_nrml_close_null()
        => ExecutionTest("dstr/array-elision-iter-nrml-close-null");

    [Fact(DisplayName = "array-elision-iter-nrml-close-skip.js")]
    public Task dstr_array_elision_iter_nrml_close_skip()
        => ExecutionTest("dstr/array-elision-iter-nrml-close-skip");

    [Fact(DisplayName = "array-elision-iter-nrml-close.js")]
    public Task dstr_array_elision_iter_nrml_close()
        => ExecutionTest("dstr/array-elision-iter-nrml-close");

    [Fact(DisplayName = "array-elision-val-null.js")]
    public Task dstr_array_elision_val_null()
        => ExecutionTest("dstr/array-elision-val-null");

    [Fact(DisplayName = "array-elision-val-symbol.js")]
    public Task dstr_array_elision_val_symbol()
        => ExecutionTest("dstr/array-elision-val-symbol");

    [Fact(DisplayName = "array-empty-iter-close-err.js")]
    public Task dstr_array_empty_iter_close_err()
        => ExecutionTest("dstr/array-empty-iter-close-err");

    [Fact(DisplayName = "array-empty-iter-close-null.js")]
    public Task dstr_array_empty_iter_close_null()
        => ExecutionTest("dstr/array-empty-iter-close-null");

    [Fact(DisplayName = "array-empty-iter-close.js")]
    public Task dstr_array_empty_iter_close()
        => ExecutionTest("dstr/array-empty-iter-close");

    [Fact(DisplayName = "array-empty-iter-get-err.js")]
    public Task dstr_array_empty_iter_get_err()
        => ExecutionTest("dstr/array-empty-iter-get-err");

    [Fact(DisplayName = "array-empty-val-null.js")]
    public Task dstr_array_empty_val_null()
        => ExecutionTest("dstr/array-empty-val-null");

    [Fact(DisplayName = "array-empty-val-symbol.js")]
    public Task dstr_array_empty_val_symbol()
        => ExecutionTest("dstr/array-empty-val-symbol");

    [Fact(DisplayName = "array-iteration.js")]
    public Task dstr_array_iteration()
        => ExecutionTest("dstr/array-iteration");

    [Fact(DisplayName = "array-rest-after-element.js")]
    public Task dstr_array_rest_after_element()
        => ExecutionTest("dstr/array-rest-after-element");

    [Fact(DisplayName = "array-rest-after-elision.js")]
    public Task dstr_array_rest_after_elision()
        => ExecutionTest("dstr/array-rest-after-elision");

    [Fact(DisplayName = "array-rest-elision-iter-abpt.js")]
    public Task dstr_array_rest_elision_iter_abpt()
        => ExecutionTest("dstr/array-rest-elision-iter-abpt");

    [Fact(DisplayName = "array-rest-elision.js")]
    public Task dstr_array_rest_elision()
        => ExecutionTest("dstr/array-rest-elision");

    [Fact(DisplayName = "array-rest-iter-get-err.js")]
    public Task dstr_array_rest_iter_get_err()
        => ExecutionTest("dstr/array-rest-iter-get-err");

    [Fact(DisplayName = "array-rest-iter-nrml-close-skip.js")]
    public Task dstr_array_rest_iter_nrml_close_skip()
        => ExecutionTest("dstr/array-rest-iter-nrml-close-skip");

    [Fact(DisplayName = "array-rest-iter-rtrn-close-err.js")]
    public Task dstr_array_rest_iter_rtrn_close_err()
        => ExecutionTest("dstr/array-rest-iter-rtrn-close-err");

    [Fact(DisplayName = "array-rest-iter-rtrn-close-null.js")]
    public Task dstr_array_rest_iter_rtrn_close_null()
        => ExecutionTest("dstr/array-rest-iter-rtrn-close-null");

    [Fact(DisplayName = "array-rest-iter-rtrn-close.js")]
    public Task dstr_array_rest_iter_rtrn_close()
        => ExecutionTest("dstr/array-rest-iter-rtrn-close");

    [Fact(DisplayName = "array-rest-iter-thrw-close-err.js")]
    public Task dstr_array_rest_iter_thrw_close_err()
        => ExecutionTest("dstr/array-rest-iter-thrw-close-err");

    [Fact(DisplayName = "array-rest-iter-thrw-close-skip.js")]
    public Task dstr_array_rest_iter_thrw_close_skip()
        => ExecutionTest("dstr/array-rest-iter-thrw-close-skip");

    [Fact(DisplayName = "array-rest-iter-thrw-close.js")]
    public Task dstr_array_rest_iter_thrw_close()
        => ExecutionTest("dstr/array-rest-iter-thrw-close");

    [Fact(DisplayName = "array-rest-iteration.js")]
    public Task dstr_array_rest_iteration()
        => ExecutionTest("dstr/array-rest-iteration");

    [Fact(DisplayName = "array-rest-lref-err.js")]
    public Task dstr_array_rest_lref_err()
        => ExecutionTest("dstr/array-rest-lref-err");

    [Fact(DisplayName = "array-rest-lref.js")]
    public Task dstr_array_rest_lref()
        => ExecutionTest("dstr/array-rest-lref");

    [Fact(DisplayName = "array-rest-nested-array-iter-thrw-close-skip.js")]
    public Task dstr_array_rest_nested_array_iter_thrw_close_skip()
        => ExecutionTest("dstr/array-rest-nested-array-iter-thrw-close-skip");

    [Fact(DisplayName = "array-rest-nested-array-null.js")]
    public Task dstr_array_rest_nested_array_null()
        => ExecutionTest("dstr/array-rest-nested-array-null");

    [Fact(DisplayName = "array-rest-nested-array-undefined-hole.js")]
    public Task dstr_array_rest_nested_array_undefined_hole()
        => ExecutionTest("dstr/array-rest-nested-array-undefined-hole");

    [Fact(DisplayName = "array-rest-nested-array-undefined-own.js")]
    public Task dstr_array_rest_nested_array_undefined_own()
        => ExecutionTest("dstr/array-rest-nested-array-undefined-own");

    [Fact(DisplayName = "array-rest-nested-array-undefined.js")]
    public Task dstr_array_rest_nested_array_undefined()
        => ExecutionTest("dstr/array-rest-nested-array-undefined");

    [Fact(DisplayName = "array-rest-nested-array-yield-expr.js")]
    public Task dstr_array_rest_nested_array_yield_expr()
        => ExecutionTest("dstr/array-rest-nested-array-yield-expr");

    [Fact(DisplayName = "array-rest-nested-array-yield-ident-valid.js")]
    public Task dstr_array_rest_nested_array_yield_ident_valid()
        => ExecutionTest("dstr/array-rest-nested-array-yield-ident-valid");

    [Fact(DisplayName = "array-rest-nested-array.js")]
    public Task dstr_array_rest_nested_array()
        => ExecutionTest("dstr/array-rest-nested-array");

    [Fact(DisplayName = "array-rest-nested-obj-null.js")]
    public Task dstr_array_rest_nested_obj_null()
        => ExecutionTest("dstr/array-rest-nested-obj-null");

    [Fact(DisplayName = "array-rest-nested-obj-undefined-hole.js")]
    public Task dstr_array_rest_nested_obj_undefined_hole()
        => ExecutionTest("dstr/array-rest-nested-obj-undefined-hole");

    [Fact(DisplayName = "array-rest-nested-obj-undefined-own.js")]
    public Task dstr_array_rest_nested_obj_undefined_own()
        => ExecutionTest("dstr/array-rest-nested-obj-undefined-own");

    [Fact(DisplayName = "array-rest-nested-obj-undefined.js")]
    public Task dstr_array_rest_nested_obj_undefined()
        => ExecutionTest("dstr/array-rest-nested-obj-undefined");

    [Fact(DisplayName = "array-rest-nested-obj-yield-expr.js")]
    public Task dstr_array_rest_nested_obj_yield_expr()
        => ExecutionTest("dstr/array-rest-nested-obj-yield-expr");

    [Fact(DisplayName = "array-rest-nested-obj-yield-ident-valid.js")]
    public Task dstr_array_rest_nested_obj_yield_ident_valid()
        => ExecutionTest("dstr/array-rest-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "array-rest-nested-obj.js")]
    public Task dstr_array_rest_nested_obj()
        => ExecutionTest("dstr/array-rest-nested-obj");

    [Fact(DisplayName = "array-rest-put-const.js")]
    public Task dstr_array_rest_put_const()
        => ExecutionTest("dstr/array-rest-put-const");

    [Fact(DisplayName = "array-rest-put-let.js")]
    public Task dstr_array_rest_put_let()
        => ExecutionTest("dstr/array-rest-put-let");

    [Fact(DisplayName = "array-rest-put-prop-ref-no-get.js")]
    public Task dstr_array_rest_put_prop_ref_no_get()
        => ExecutionTest("dstr/array-rest-put-prop-ref-no-get");

    [Fact(DisplayName = "array-rest-put-prop-ref-user-err-iter-close-skip.js")]
    public Task dstr_array_rest_put_prop_ref_user_err_iter_close_skip()
        => ExecutionTest("dstr/array-rest-put-prop-ref-user-err-iter-close-skip");

    [Fact(DisplayName = "array-rest-put-prop-ref-user-err.js")]
    public Task dstr_array_rest_put_prop_ref_user_err()
        => ExecutionTest("dstr/array-rest-put-prop-ref-user-err");

    [Fact(DisplayName = "array-rest-put-prop-ref.js")]
    public Task dstr_array_rest_put_prop_ref()
        => ExecutionTest("dstr/array-rest-put-prop-ref");

    [Fact(DisplayName = "array-rest-put-unresolvable-no-strict.js")]
    public Task dstr_array_rest_put_unresolvable_no_strict()
        => ExecutionTest("dstr/array-rest-put-unresolvable-no-strict");

    [Fact(DisplayName = "array-rest-put-unresolvable-strict.js")]
    public Task dstr_array_rest_put_unresolvable_strict()
        => ExecutionTest("dstr/array-rest-put-unresolvable-strict");

    [Fact(DisplayName = "array-rest-yield-expr.js")]
    public Task dstr_array_rest_yield_expr()
        => ExecutionTest("dstr/array-rest-yield-expr");

    [Fact(DisplayName = "array-rest-yield-ident-valid.js")]
    public Task dstr_array_rest_yield_ident_valid()
        => ExecutionTest("dstr/array-rest-yield-ident-valid");

    [Fact(DisplayName = "ident-name-prop-name-literal-break-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_break_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-break-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-case-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_case_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-case-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-catch-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_catch_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-catch-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-class-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_class_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-class-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-const-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_const_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-const-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-continue-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_continue_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-continue-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-debugger-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_debugger_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-debugger-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-default-escaped-ext.js")]
    public Task dstr_ident_name_prop_name_literal_default_escaped_ext()
        => ExecutionTest("dstr/ident-name-prop-name-literal-default-escaped-ext");

    [Fact(DisplayName = "ident-name-prop-name-literal-default-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_default_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-default-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-default.js")]
    public Task dstr_ident_name_prop_name_literal_default()
        => ExecutionTest("dstr/ident-name-prop-name-literal-default");

    [Fact(DisplayName = "ident-name-prop-name-literal-delete-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_delete_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-delete-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-do-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_do_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-do-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-else-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_else_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-else-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-enum-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_enum_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-enum-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-export-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_export_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-export-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-extends-escaped-ext.js")]
    public Task dstr_ident_name_prop_name_literal_extends_escaped_ext()
        => ExecutionTest("dstr/ident-name-prop-name-literal-extends-escaped-ext");

    [Fact(DisplayName = "ident-name-prop-name-literal-extends-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_extends_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-extends-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-extends.js")]
    public Task dstr_ident_name_prop_name_literal_extends()
        => ExecutionTest("dstr/ident-name-prop-name-literal-extends");

    [Fact(DisplayName = "ident-name-prop-name-literal-finally-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_finally_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-finally-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-for-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_for_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-for-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-function-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_function_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-function-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-if-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_if_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-if-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-implements-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_implements_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-implements-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-import-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_import_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-import-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-in-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_in_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-in-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-instanceof-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_instanceof_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-instanceof-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-interface-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_interface_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-interface-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-let-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_let_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-let-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-new-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_new_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-new-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-package-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_package_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-package-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-private-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_private_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-private-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-protected-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_protected_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-protected-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-public-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_public_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-public-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-return-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_return_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-return-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-static-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_static_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-static-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-super-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_super_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-super-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-switch-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_switch_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-switch-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-this-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_this_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-this-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-throw-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_throw_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-throw-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-try-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_try_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-try-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-typeof-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_typeof_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-typeof-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-var-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_var_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-var-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-void-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_void_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-void-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-while-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_while_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-while-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-with-escaped.js")]
    public Task dstr_ident_name_prop_name_literal_with_escaped()
        => ExecutionTest("dstr/ident-name-prop-name-literal-with-escaped");

    [Fact(DisplayName = "obj-empty-bool.js")]
    public Task dstr_obj_empty_bool()
        => ExecutionTest("dstr/obj-empty-bool");

    [Fact(DisplayName = "obj-empty-null.js")]
    public Task dstr_obj_empty_null()
        => ExecutionTest("dstr/obj-empty-null");

    [Fact(DisplayName = "obj-empty-num.js")]
    public Task dstr_obj_empty_num()
        => ExecutionTest("dstr/obj-empty-num");

    [Fact(DisplayName = "obj-empty-obj.js")]
    public Task dstr_obj_empty_obj()
        => ExecutionTest("dstr/obj-empty-obj");

    [Fact(DisplayName = "obj-empty-string.js")]
    public Task dstr_obj_empty_string()
        => ExecutionTest("dstr/obj-empty-string");

    [Fact(DisplayName = "obj-empty-symbol.js")]
    public Task dstr_obj_empty_symbol()
        => ExecutionTest("dstr/obj-empty-symbol");

    [Fact(DisplayName = "obj-empty-undef.js")]
    public Task dstr_obj_empty_undef()
        => ExecutionTest("dstr/obj-empty-undef");

    [Fact(DisplayName = "obj-id-identifier-resolution-first.js")]
    public Task dstr_obj_id_identifier_resolution_first()
        => ExecutionTest("dstr/obj-id-identifier-resolution-first");

    [Fact(DisplayName = "obj-id-identifier-resolution-last.js")]
    public Task dstr_obj_id_identifier_resolution_last()
        => ExecutionTest("dstr/obj-id-identifier-resolution-last");

    [Fact(DisplayName = "obj-id-identifier-resolution-lone.js")]
    public Task dstr_obj_id_identifier_resolution_lone()
        => ExecutionTest("dstr/obj-id-identifier-resolution-lone");

    [Fact(DisplayName = "obj-id-identifier-resolution-middle.js")]
    public Task dstr_obj_id_identifier_resolution_middle()
        => ExecutionTest("dstr/obj-id-identifier-resolution-middle");

    [Fact(DisplayName = "obj-id-identifier-resolution-trlng.js")]
    public Task dstr_obj_id_identifier_resolution_trlng()
        => ExecutionTest("dstr/obj-id-identifier-resolution-trlng");

    [Fact(DisplayName = "obj-id-identifier-yield-ident-valid.js")]
    public Task dstr_obj_id_identifier_yield_ident_valid()
        => ExecutionTest("dstr/obj-id-identifier-yield-ident-valid");

    [Fact(DisplayName = "obj-id-init-assignment-missing.js")]
    public Task dstr_obj_id_init_assignment_missing()
        => ExecutionTest("dstr/obj-id-init-assignment-missing");

    [Fact(DisplayName = "obj-id-init-assignment-null.js")]
    public Task dstr_obj_id_init_assignment_null()
        => ExecutionTest("dstr/obj-id-init-assignment-null");

    [Fact(DisplayName = "obj-id-init-assignment-truthy.js")]
    public Task dstr_obj_id_init_assignment_truthy()
        => ExecutionTest("dstr/obj-id-init-assignment-truthy");

    [Fact(DisplayName = "obj-id-init-assignment-undef.js")]
    public Task dstr_obj_id_init_assignment_undef()
        => ExecutionTest("dstr/obj-id-init-assignment-undef");

    [Fact(DisplayName = "obj-id-init-evaluation.js")]
    public Task dstr_obj_id_init_evaluation()
        => ExecutionTest("dstr/obj-id-init-evaluation");

    [Fact(DisplayName = "obj-id-init-fn-name-arrow.js")]
    public Task dstr_obj_id_init_fn_name_arrow()
        => ExecutionTest("dstr/obj-id-init-fn-name-arrow");

    [Fact(DisplayName = "obj-id-init-fn-name-class.js")]
    public Task dstr_obj_id_init_fn_name_class()
        => ExecutionTest("dstr/obj-id-init-fn-name-class");

    [Fact(DisplayName = "obj-id-init-fn-name-cover.js")]
    public Task dstr_obj_id_init_fn_name_cover()
        => ExecutionTest("dstr/obj-id-init-fn-name-cover");

    [Fact(DisplayName = "obj-id-init-fn-name-fn.js")]
    public Task dstr_obj_id_init_fn_name_fn()
        => ExecutionTest("dstr/obj-id-init-fn-name-fn");

    [Fact(DisplayName = "obj-id-init-fn-name-gen.js")]
    public Task dstr_obj_id_init_fn_name_gen()
        => ExecutionTest("dstr/obj-id-init-fn-name-gen");

    [Fact(DisplayName = "obj-id-init-in.js")]
    public Task dstr_obj_id_init_in()
        => ExecutionTest("dstr/obj-id-init-in");

    [Fact(DisplayName = "obj-id-init-let.js")]
    public Task dstr_obj_id_init_let()
        => ExecutionTest("dstr/obj-id-init-let");

    [Fact(DisplayName = "obj-id-init-order.js")]
    public Task dstr_obj_id_init_order()
        => ExecutionTest("dstr/obj-id-init-order");

    [Fact(DisplayName = "obj-id-init-simple-no-strict.js")]
    public Task dstr_obj_id_init_simple_no_strict()
        => ExecutionTest("dstr/obj-id-init-simple-no-strict");

    [Fact(DisplayName = "obj-id-init-yield-expr.js")]
    public Task dstr_obj_id_init_yield_expr()
        => ExecutionTest("dstr/obj-id-init-yield-expr");

    [Fact(DisplayName = "obj-id-init-yield-ident-valid.js")]
    public Task dstr_obj_id_init_yield_ident_valid()
        => ExecutionTest("dstr/obj-id-init-yield-ident-valid");

    [Fact(DisplayName = "obj-id-put-const.js")]
    public Task dstr_obj_id_put_const()
        => ExecutionTest("dstr/obj-id-put-const");

    [Fact(DisplayName = "obj-id-put-let.js")]
    public Task dstr_obj_id_put_let()
        => ExecutionTest("dstr/obj-id-put-let");

    [Fact(DisplayName = "obj-id-put-unresolvable-no-strict.js")]
    public Task dstr_obj_id_put_unresolvable_no_strict()
        => ExecutionTest("dstr/obj-id-put-unresolvable-no-strict");

    [Fact(DisplayName = "obj-id-put-unresolvable-strict.js")]
    public Task dstr_obj_id_put_unresolvable_strict()
        => ExecutionTest("dstr/obj-id-put-unresolvable-strict");

    [Fact(DisplayName = "obj-id-simple-no-strict.js")]
    public Task dstr_obj_id_simple_no_strict()
        => ExecutionTest("dstr/obj-id-simple-no-strict");

    [Fact(DisplayName = "obj-prop-elem-init-assignment-missing.js")]
    public Task dstr_obj_prop_elem_init_assignment_missing()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-missing");

    [Fact(DisplayName = "obj-prop-elem-init-assignment-null.js")]
    public Task dstr_obj_prop_elem_init_assignment_null()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-null");

    [Fact(DisplayName = "obj-prop-elem-init-assignment-truthy.js")]
    public Task dstr_obj_prop_elem_init_assignment_truthy()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-truthy");

    [Fact(DisplayName = "obj-prop-elem-init-assignment-undef.js")]
    public Task dstr_obj_prop_elem_init_assignment_undef()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-undef");

    [Fact(DisplayName = "obj-prop-elem-init-evaluation.js")]
    public Task dstr_obj_prop_elem_init_evaluation()
        => ExecutionTest("dstr/obj-prop-elem-init-evaluation");

    [Fact(DisplayName = "obj-prop-elem-init-fn-name-arrow.js")]
    public Task dstr_obj_prop_elem_init_fn_name_arrow()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-arrow");

    [Fact(DisplayName = "obj-prop-elem-init-fn-name-class.js")]
    public Task dstr_obj_prop_elem_init_fn_name_class()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-class");

    [Fact(DisplayName = "obj-prop-elem-init-fn-name-cover.js")]
    public Task dstr_obj_prop_elem_init_fn_name_cover()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-cover");

    [Fact(DisplayName = "obj-prop-elem-init-fn-name-fn.js")]
    public Task dstr_obj_prop_elem_init_fn_name_fn()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-fn");

    [Fact(DisplayName = "obj-prop-elem-init-fn-name-gen.js")]
    public Task dstr_obj_prop_elem_init_fn_name_gen()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-gen");

    [Fact(DisplayName = "obj-prop-elem-init-in.js")]
    public Task dstr_obj_prop_elem_init_in()
        => ExecutionTest("dstr/obj-prop-elem-init-in");

    [Fact(DisplayName = "obj-prop-elem-init-let.js")]
    public Task dstr_obj_prop_elem_init_let()
        => ExecutionTest("dstr/obj-prop-elem-init-let");

    [Fact(DisplayName = "obj-prop-elem-init-yield-expr.js")]
    public Task dstr_obj_prop_elem_init_yield_expr()
        => ExecutionTest("dstr/obj-prop-elem-init-yield-expr");

    [Fact(DisplayName = "obj-prop-elem-init-yield-ident-valid.js")]
    public Task dstr_obj_prop_elem_init_yield_ident_valid()
        => ExecutionTest("dstr/obj-prop-elem-init-yield-ident-valid");

    [Fact(DisplayName = "obj-prop-elem-target-obj-literal-prop-ref-init-active.js")]
    public Task dstr_obj_prop_elem_target_obj_literal_prop_ref_init_active()
        => ExecutionTest("dstr/obj-prop-elem-target-obj-literal-prop-ref-init-active");

    [Fact(DisplayName = "obj-prop-elem-target-obj-literal-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_obj_literal_prop_ref_init()
        => ExecutionTest("dstr/obj-prop-elem-target-obj-literal-prop-ref-init");

    [Fact(DisplayName = "obj-prop-elem-target-obj-literal-prop-ref.js")]
    public Task dstr_obj_prop_elem_target_obj_literal_prop_ref()
        => ExecutionTest("dstr/obj-prop-elem-target-obj-literal-prop-ref");

    [Fact(DisplayName = "obj-prop-elem-target-yield-expr.js")]
    public Task dstr_obj_prop_elem_target_yield_expr()
        => ExecutionTest("dstr/obj-prop-elem-target-yield-expr");

    [Fact(DisplayName = "obj-prop-elem-target-yield-ident-valid.js")]
    public Task dstr_obj_prop_elem_target_yield_ident_valid()
        => ExecutionTest("dstr/obj-prop-elem-target-yield-ident-valid");

    [Fact(DisplayName = "obj-prop-identifier-resolution-first.js")]
    public Task dstr_obj_prop_identifier_resolution_first()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-first");

    [Fact(DisplayName = "obj-prop-identifier-resolution-last.js")]
    public Task dstr_obj_prop_identifier_resolution_last()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-last");

    [Fact(DisplayName = "obj-prop-identifier-resolution-lone.js")]
    public Task dstr_obj_prop_identifier_resolution_lone()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-lone");

    [Fact(DisplayName = "obj-prop-identifier-resolution-middle.js")]
    public Task dstr_obj_prop_identifier_resolution_middle()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-middle");

    [Fact(DisplayName = "obj-prop-identifier-resolution-trlng.js")]
    public Task dstr_obj_prop_identifier_resolution_trlng()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-trlng");

    [Fact(DisplayName = "obj-prop-name-evaluation-error.js")]
    public Task dstr_obj_prop_name_evaluation_error()
        => ExecutionTest("dstr/obj-prop-name-evaluation-error");

    [Fact(DisplayName = "obj-prop-name-evaluation.js")]
    public Task dstr_obj_prop_name_evaluation()
        => ExecutionTest("dstr/obj-prop-name-evaluation");

    [Fact(DisplayName = "obj-prop-nested-array-null.js")]
    public Task dstr_obj_prop_nested_array_null()
        => ExecutionTest("dstr/obj-prop-nested-array-null");

    [Fact(DisplayName = "obj-prop-nested-array-undefined-own.js")]
    public Task dstr_obj_prop_nested_array_undefined_own()
        => ExecutionTest("dstr/obj-prop-nested-array-undefined-own");

    [Fact(DisplayName = "obj-prop-nested-array-undefined.js")]
    public Task dstr_obj_prop_nested_array_undefined()
        => ExecutionTest("dstr/obj-prop-nested-array-undefined");

    [Fact(DisplayName = "obj-prop-nested-array-yield-expr.js")]
    public Task dstr_obj_prop_nested_array_yield_expr()
        => ExecutionTest("dstr/obj-prop-nested-array-yield-expr");

    [Fact(DisplayName = "obj-prop-nested-array-yield-ident-valid.js")]
    public Task dstr_obj_prop_nested_array_yield_ident_valid()
        => ExecutionTest("dstr/obj-prop-nested-array-yield-ident-valid");

    [Fact(DisplayName = "obj-prop-nested-array.js")]
    public Task dstr_obj_prop_nested_array()
        => ExecutionTest("dstr/obj-prop-nested-array");

    [Fact(DisplayName = "obj-prop-nested-obj-null.js")]
    public Task dstr_obj_prop_nested_obj_null()
        => ExecutionTest("dstr/obj-prop-nested-obj-null");

    [Fact(DisplayName = "obj-prop-nested-obj-undefined-own.js")]
    public Task dstr_obj_prop_nested_obj_undefined_own()
        => ExecutionTest("dstr/obj-prop-nested-obj-undefined-own");

    [Fact(DisplayName = "obj-prop-nested-obj-undefined.js")]
    public Task dstr_obj_prop_nested_obj_undefined()
        => ExecutionTest("dstr/obj-prop-nested-obj-undefined");

    [Fact(DisplayName = "obj-prop-nested-obj-yield-expr.js")]
    public Task dstr_obj_prop_nested_obj_yield_expr()
        => ExecutionTest("dstr/obj-prop-nested-obj-yield-expr");

    [Fact(DisplayName = "obj-prop-nested-obj-yield-ident-valid.js")]
    public Task dstr_obj_prop_nested_obj_yield_ident_valid()
        => ExecutionTest("dstr/obj-prop-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "obj-prop-nested-obj.js")]
    public Task dstr_obj_prop_nested_obj()
        => ExecutionTest("dstr/obj-prop-nested-obj");

    [Fact(DisplayName = "obj-prop-put-const.js")]
    public Task dstr_obj_prop_put_const()
        => ExecutionTest("dstr/obj-prop-put-const");

    [Fact(DisplayName = "obj-prop-put-let.js")]
    public Task dstr_obj_prop_put_let()
        => ExecutionTest("dstr/obj-prop-put-let");

    [Fact(DisplayName = "obj-prop-put-order.js")]
    public Task dstr_obj_prop_put_order()
        => ExecutionTest("dstr/obj-prop-put-order");

    [Fact(DisplayName = "obj-prop-put-prop-ref-no-get.js")]
    public Task dstr_obj_prop_put_prop_ref_no_get()
        => ExecutionTest("dstr/obj-prop-put-prop-ref-no-get");

    [Fact(DisplayName = "obj-prop-put-prop-ref-user-err.js")]
    public Task dstr_obj_prop_put_prop_ref_user_err()
        => ExecutionTest("dstr/obj-prop-put-prop-ref-user-err");

    [Fact(DisplayName = "obj-prop-put-prop-ref.js")]
    public Task dstr_obj_prop_put_prop_ref()
        => ExecutionTest("dstr/obj-prop-put-prop-ref");

    [Fact(DisplayName = "obj-prop-put-unresolvable-no-strict.js")]
    public Task dstr_obj_prop_put_unresolvable_no_strict()
        => ExecutionTest("dstr/obj-prop-put-unresolvable-no-strict");

    [Fact(DisplayName = "obj-prop-put-unresolvable-strict.js")]
    public Task dstr_obj_prop_put_unresolvable_strict()
        => ExecutionTest("dstr/obj-prop-put-unresolvable-strict");

    [Fact(DisplayName = "obj-rest-computed-property-no-strict.js")]
    public Task dstr_obj_rest_computed_property_no_strict()
        => ExecutionTest("dstr/obj-rest-computed-property-no-strict");

    [Fact(DisplayName = "obj-rest-computed-property.js")]
    public Task dstr_obj_rest_computed_property()
        => ExecutionTest("dstr/obj-rest-computed-property");

    [Fact(DisplayName = "obj-rest-descriptors.js")]
    public Task dstr_obj_rest_descriptors()
        => ExecutionTest("dstr/obj-rest-descriptors");

    [Fact(DisplayName = "obj-rest-empty-obj.js")]
    public Task dstr_obj_rest_empty_obj()
        => ExecutionTest("dstr/obj-rest-empty-obj");

    [Fact(DisplayName = "obj-rest-getter-abrupt-get-error.js")]
    public Task dstr_obj_rest_getter_abrupt_get_error()
        => ExecutionTest("dstr/obj-rest-getter-abrupt-get-error");

    [Fact(DisplayName = "obj-rest-getter.js")]
    public Task dstr_obj_rest_getter()
        => ExecutionTest("dstr/obj-rest-getter");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-1.js")]
    public Task dstr_obj_rest_non_string_computed_property_1()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-1dot.js")]
    public Task dstr_obj_rest_non_string_computed_property_1dot()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1dot");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-1dot0.js")]
    public Task dstr_obj_rest_non_string_computed_property_1dot0()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1dot0");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-1e0.js")]
    public Task dstr_obj_rest_non_string_computed_property_1e0()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1e0");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-array-1.js")]
    public Task dstr_obj_rest_non_string_computed_property_array_1()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-array-1");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-array-1e0.js")]
    public Task dstr_obj_rest_non_string_computed_property_array_1e0()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-array-1e0");

    [Fact(DisplayName = "obj-rest-non-string-computed-property-string-1.js")]
    public Task dstr_obj_rest_non_string_computed_property_string_1()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-string-1");

    [Fact(DisplayName = "obj-rest-number.js")]
    public Task dstr_obj_rest_number()
        => ExecutionTest("dstr/obj-rest-number");

    [Fact(DisplayName = "obj-rest-order.js")]
    public Task dstr_obj_rest_order()
        => ExecutionTest("dstr/obj-rest-order");

    [Fact(DisplayName = "obj-rest-put-const.js")]
    public Task dstr_obj_rest_put_const()
        => ExecutionTest("dstr/obj-rest-put-const");

    [Fact(DisplayName = "obj-rest-same-name.js")]
    public Task dstr_obj_rest_same_name()
        => ExecutionTest("dstr/obj-rest-same-name");

    [Fact(DisplayName = "obj-rest-skip-non-enumerable.js")]
    public Task dstr_obj_rest_skip_non_enumerable()
        => ExecutionTest("dstr/obj-rest-skip-non-enumerable");

    [Fact(DisplayName = "obj-rest-str-val.js")]
    public Task dstr_obj_rest_str_val()
        => ExecutionTest("dstr/obj-rest-str-val");

    [Fact(DisplayName = "obj-rest-symbol-val.js")]
    public Task dstr_obj_rest_symbol_val()
        => ExecutionTest("dstr/obj-rest-symbol-val");

    [Fact(DisplayName = "obj-rest-to-property-with-setter.js")]
    public Task dstr_obj_rest_to_property_with_setter()
        => ExecutionTest("dstr/obj-rest-to-property-with-setter");

    [Fact(DisplayName = "obj-rest-to-property.js")]
    public Task dstr_obj_rest_to_property()
        => ExecutionTest("dstr/obj-rest-to-property");

    [Fact(DisplayName = "obj-rest-val-null.js")]
    public Task dstr_obj_rest_val_null()
        => ExecutionTest("dstr/obj-rest-val-null");

    [Fact(DisplayName = "obj-rest-val-undefined.js")]
    public Task dstr_obj_rest_val_undefined()
        => ExecutionTest("dstr/obj-rest-val-undefined");

    [Fact(DisplayName = "obj-rest-valid-object.js")]
    public Task dstr_obj_rest_valid_object()
        => ExecutionTest("dstr/obj-rest-valid-object");

    [Fact(DisplayName = "fn-name-arrow.js")]
    public Task fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class.js")]
    public Task fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover.js")]
    public Task fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn.js")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen.js")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

    [Fact(DisplayName = "fn-name-lhs-member.js")]
    public Task fn_name_lhs_member()
        => ExecutionTest("fn-name-lhs-member");

    [Fact(DisplayName = "line-terminator.js")]
    public Task line_terminator()
        => ExecutionTest("line-terminator");

    [Fact(DisplayName = "member-expr-ident-name-break-escaped.js")]
    public Task member_expr_ident_name_break_escaped()
        => ExecutionTest("member-expr-ident-name-break-escaped");

    [Fact(DisplayName = "member-expr-ident-name-case-escaped.js")]
    public Task member_expr_ident_name_case_escaped()
        => ExecutionTest("member-expr-ident-name-case-escaped");

    [Fact(DisplayName = "member-expr-ident-name-catch-escaped.js")]
    public Task member_expr_ident_name_catch_escaped()
        => ExecutionTest("member-expr-ident-name-catch-escaped");

    [Fact(DisplayName = "member-expr-ident-name-class-escaped.js")]
    public Task member_expr_ident_name_class_escaped()
        => ExecutionTest("member-expr-ident-name-class-escaped");

    [Fact(DisplayName = "member-expr-ident-name-const-escaped.js")]
    public Task member_expr_ident_name_const_escaped()
        => ExecutionTest("member-expr-ident-name-const-escaped");

    [Fact(DisplayName = "member-expr-ident-name-continue-escaped.js")]
    public Task member_expr_ident_name_continue_escaped()
        => ExecutionTest("member-expr-ident-name-continue-escaped");

    [Fact(DisplayName = "member-expr-ident-name-debugger-escaped.js")]
    public Task member_expr_ident_name_debugger_escaped()
        => ExecutionTest("member-expr-ident-name-debugger-escaped");

    [Fact(DisplayName = "member-expr-ident-name-default-escaped-ext.js")]
    public Task member_expr_ident_name_default_escaped_ext()
        => ExecutionTest("member-expr-ident-name-default-escaped-ext");

    [Fact(DisplayName = "member-expr-ident-name-default-escaped.js")]
    public Task member_expr_ident_name_default_escaped()
        => ExecutionTest("member-expr-ident-name-default-escaped");

    [Fact(DisplayName = "member-expr-ident-name-default.js")]
    public Task member_expr_ident_name_default()
        => ExecutionTest("member-expr-ident-name-default");

    [Fact(DisplayName = "member-expr-ident-name-delete-escaped.js")]
    public Task member_expr_ident_name_delete_escaped()
        => ExecutionTest("member-expr-ident-name-delete-escaped");

    [Fact(DisplayName = "member-expr-ident-name-do-escaped.js")]
    public Task member_expr_ident_name_do_escaped()
        => ExecutionTest("member-expr-ident-name-do-escaped");

    [Fact(DisplayName = "member-expr-ident-name-else-escaped.js")]
    public Task member_expr_ident_name_else_escaped()
        => ExecutionTest("member-expr-ident-name-else-escaped");

    [Fact(DisplayName = "member-expr-ident-name-enum-escaped.js")]
    public Task member_expr_ident_name_enum_escaped()
        => ExecutionTest("member-expr-ident-name-enum-escaped");

    [Fact(DisplayName = "member-expr-ident-name-export-escaped.js")]
    public Task member_expr_ident_name_export_escaped()
        => ExecutionTest("member-expr-ident-name-export-escaped");

    [Fact(DisplayName = "member-expr-ident-name-extends-escaped-ext.js")]
    public Task member_expr_ident_name_extends_escaped_ext()
        => ExecutionTest("member-expr-ident-name-extends-escaped-ext");

    [Fact(DisplayName = "member-expr-ident-name-extends-escaped.js")]
    public Task member_expr_ident_name_extends_escaped()
        => ExecutionTest("member-expr-ident-name-extends-escaped");

    [Fact(DisplayName = "member-expr-ident-name-extends.js")]
    public Task member_expr_ident_name_extends()
        => ExecutionTest("member-expr-ident-name-extends");

    [Fact(DisplayName = "member-expr-ident-name-finally-escaped.js")]
    public Task member_expr_ident_name_finally_escaped()
        => ExecutionTest("member-expr-ident-name-finally-escaped");

    [Fact(DisplayName = "member-expr-ident-name-for-escaped.js")]
    public Task member_expr_ident_name_for_escaped()
        => ExecutionTest("member-expr-ident-name-for-escaped");

    [Fact(DisplayName = "member-expr-ident-name-function-escaped.js")]
    public Task member_expr_ident_name_function_escaped()
        => ExecutionTest("member-expr-ident-name-function-escaped");

    [Fact(DisplayName = "member-expr-ident-name-if-escaped.js")]
    public Task member_expr_ident_name_if_escaped()
        => ExecutionTest("member-expr-ident-name-if-escaped");

    [Fact(DisplayName = "member-expr-ident-name-implements-escaped.js")]
    public Task member_expr_ident_name_implements_escaped()
        => ExecutionTest("member-expr-ident-name-implements-escaped");

    [Fact(DisplayName = "member-expr-ident-name-import-escaped.js")]
    public Task member_expr_ident_name_import_escaped()
        => ExecutionTest("member-expr-ident-name-import-escaped");

    [Fact(DisplayName = "member-expr-ident-name-in-escaped.js")]
    public Task member_expr_ident_name_in_escaped()
        => ExecutionTest("member-expr-ident-name-in-escaped");

    [Fact(DisplayName = "member-expr-ident-name-instanceof-escaped.js")]
    public Task member_expr_ident_name_instanceof_escaped()
        => ExecutionTest("member-expr-ident-name-instanceof-escaped");

    [Fact(DisplayName = "member-expr-ident-name-interface-escaped.js")]
    public Task member_expr_ident_name_interface_escaped()
        => ExecutionTest("member-expr-ident-name-interface-escaped");

    [Fact(DisplayName = "member-expr-ident-name-let-escaped.js")]
    public Task member_expr_ident_name_let_escaped()
        => ExecutionTest("member-expr-ident-name-let-escaped");

    [Fact(DisplayName = "member-expr-ident-name-new-escaped.js")]
    public Task member_expr_ident_name_new_escaped()
        => ExecutionTest("member-expr-ident-name-new-escaped");

    [Fact(DisplayName = "member-expr-ident-name-package-escaped.js")]
    public Task member_expr_ident_name_package_escaped()
        => ExecutionTest("member-expr-ident-name-package-escaped");

    [Fact(DisplayName = "member-expr-ident-name-private-escaped.js")]
    public Task member_expr_ident_name_private_escaped()
        => ExecutionTest("member-expr-ident-name-private-escaped");

    [Fact(DisplayName = "member-expr-ident-name-protected-escaped.js")]
    public Task member_expr_ident_name_protected_escaped()
        => ExecutionTest("member-expr-ident-name-protected-escaped");

    [Fact(DisplayName = "member-expr-ident-name-public-escaped.js")]
    public Task member_expr_ident_name_public_escaped()
        => ExecutionTest("member-expr-ident-name-public-escaped");

    [Fact(DisplayName = "member-expr-ident-name-return-escaped.js")]
    public Task member_expr_ident_name_return_escaped()
        => ExecutionTest("member-expr-ident-name-return-escaped");

    [Fact(DisplayName = "member-expr-ident-name-static-escaped.js")]
    public Task member_expr_ident_name_static_escaped()
        => ExecutionTest("member-expr-ident-name-static-escaped");

    [Fact(DisplayName = "member-expr-ident-name-super-escaped.js")]
    public Task member_expr_ident_name_super_escaped()
        => ExecutionTest("member-expr-ident-name-super-escaped");

    [Fact(DisplayName = "member-expr-ident-name-switch-escaped.js")]
    public Task member_expr_ident_name_switch_escaped()
        => ExecutionTest("member-expr-ident-name-switch-escaped");

    [Fact(DisplayName = "member-expr-ident-name-this-escaped.js")]
    public Task member_expr_ident_name_this_escaped()
        => ExecutionTest("member-expr-ident-name-this-escaped");

    [Fact(DisplayName = "member-expr-ident-name-throw-escaped.js")]
    public Task member_expr_ident_name_throw_escaped()
        => ExecutionTest("member-expr-ident-name-throw-escaped");

    [Fact(DisplayName = "member-expr-ident-name-try-escaped.js")]
    public Task member_expr_ident_name_try_escaped()
        => ExecutionTest("member-expr-ident-name-try-escaped");

    [Fact(DisplayName = "member-expr-ident-name-typeof-escaped.js")]
    public Task member_expr_ident_name_typeof_escaped()
        => ExecutionTest("member-expr-ident-name-typeof-escaped");

    [Fact(DisplayName = "member-expr-ident-name-var-escaped.js")]
    public Task member_expr_ident_name_var_escaped()
        => ExecutionTest("member-expr-ident-name-var-escaped");

    [Fact(DisplayName = "member-expr-ident-name-void-escaped.js")]
    public Task member_expr_ident_name_void_escaped()
        => ExecutionTest("member-expr-ident-name-void-escaped");

    [Fact(DisplayName = "member-expr-ident-name-while-escaped.js")]
    public Task member_expr_ident_name_while_escaped()
        => ExecutionTest("member-expr-ident-name-while-escaped");

    [Fact(DisplayName = "member-expr-ident-name-with-escaped.js")]
    public Task member_expr_ident_name_with_escaped()
        => ExecutionTest("member-expr-ident-name-with-escaped");

    [Fact(DisplayName = "target-cover-id.js")]
    public Task target_cover_id()
        => ExecutionTest("target-cover-id");
}
