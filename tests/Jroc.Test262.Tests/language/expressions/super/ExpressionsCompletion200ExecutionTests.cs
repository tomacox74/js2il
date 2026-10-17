namespace Jroc.Test262.Tests.language.expressions.super;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.super") { }

    [Fact(DisplayName = "call-expr-value.js")]
    public Task call_expr_value()
        => ExecutionTest("call-expr-value");

    [Fact(DisplayName = "call-proto-not-ctor.js")]
    public Task call_proto_not_ctor()
        => ExecutionTest("call-proto-not-ctor");

    [Fact(DisplayName = "call-spread-mult-empty.js")]
    public Task call_spread_mult_empty()
        => ExecutionTest("call-spread-mult-empty");

    [Fact(DisplayName = "call-spread-mult-expr.js")]
    public Task call_spread_mult_expr()
        => ExecutionTest("call-spread-mult-expr");

    [Fact(DisplayName = "call-spread-mult-iter.js")]
    public Task call_spread_mult_iter()
        => ExecutionTest("call-spread-mult-iter");

    [Fact(DisplayName = "call-spread-mult-literal.js")]
    public Task call_spread_mult_literal()
        => ExecutionTest("call-spread-mult-literal");

    [Fact(DisplayName = "call-spread-sngl-empty.js")]
    public Task call_spread_sngl_empty()
        => ExecutionTest("call-spread-sngl-empty");

    [Fact(DisplayName = "call-spread-sngl-expr.js")]
    public Task call_spread_sngl_expr()
        => ExecutionTest("call-spread-sngl-expr");

    [Fact(DisplayName = "call-spread-sngl-iter.js")]
    public Task call_spread_sngl_iter()
        => ExecutionTest("call-spread-sngl-iter");

    [Fact(DisplayName = "call-spread-sngl-literal.js")]
    public Task call_spread_sngl_literal()
        => ExecutionTest("call-spread-sngl-literal");

    [Fact(DisplayName = "prop-dot-cls-null-proto.js")]
    public Task prop_dot_cls_null_proto()
        => ExecutionTest("prop-dot-cls-null-proto");

    [Fact(DisplayName = "prop-dot-cls-ref-strict.js")]
    public Task prop_dot_cls_ref_strict()
        => ExecutionTest("prop-dot-cls-ref-strict");

    [Fact(DisplayName = "prop-dot-cls-ref-this.js")]
    public Task prop_dot_cls_ref_this()
        => ExecutionTest("prop-dot-cls-ref-this");

    [Fact(DisplayName = "prop-dot-cls-this-uninit.js")]
    public Task prop_dot_cls_this_uninit()
        => ExecutionTest("prop-dot-cls-this-uninit");

    [Fact(DisplayName = "prop-dot-cls-val-from-arrow.js")]
    public Task prop_dot_cls_val_from_arrow()
        => ExecutionTest("prop-dot-cls-val-from-arrow");

    [Fact(DisplayName = "prop-dot-cls-val-from-eval.js", Skip = "eval is not supported.")]
    public Task prop_dot_cls_val_from_eval()
        => ExecutionTest("prop-dot-cls-val-from-eval");

    [Fact(DisplayName = "prop-dot-cls-val.js")]
    public Task prop_dot_cls_val()
        => ExecutionTest("prop-dot-cls-val");

    [Fact(DisplayName = "prop-dot-obj-null-proto.js")]
    public Task prop_dot_obj_null_proto()
        => ExecutionTest("prop-dot-obj-null-proto");

    [Fact(DisplayName = "prop-dot-obj-ref-non-strict.js")]
    public Task prop_dot_obj_ref_non_strict()
        => ExecutionTest("prop-dot-obj-ref-non-strict");

    [Fact(DisplayName = "prop-dot-obj-ref-strict.js")]
    public Task prop_dot_obj_ref_strict()
        => ExecutionTest("prop-dot-obj-ref-strict");

    [Fact(DisplayName = "prop-dot-obj-val-from-arrow.js")]
    public Task prop_dot_obj_val_from_arrow()
        => ExecutionTest("prop-dot-obj-val-from-arrow");

    [Fact(DisplayName = "prop-dot-obj-val-from-eval.js", Skip = "eval is not supported.")]
    public Task prop_dot_obj_val_from_eval()
        => ExecutionTest("prop-dot-obj-val-from-eval");

    [Fact(DisplayName = "prop-expr-cls-err.js")]
    public Task prop_expr_cls_err()
        => ExecutionTest("prop-expr-cls-err");

    [Fact(DisplayName = "prop-expr-cls-key-err.js")]
    public Task prop_expr_cls_key_err()
        => ExecutionTest("prop-expr-cls-key-err");

    [Fact(DisplayName = "prop-expr-cls-null-proto.js")]
    public Task prop_expr_cls_null_proto()
        => ExecutionTest("prop-expr-cls-null-proto");

    [Fact(DisplayName = "prop-expr-cls-ref-strict.js")]
    public Task prop_expr_cls_ref_strict()
        => ExecutionTest("prop-expr-cls-ref-strict");

    [Fact(DisplayName = "prop-expr-cls-ref-this.js")]
    public Task prop_expr_cls_ref_this()
        => ExecutionTest("prop-expr-cls-ref-this");

    [Fact(DisplayName = "prop-expr-cls-this-uninit.js")]
    public Task prop_expr_cls_this_uninit()
        => ExecutionTest("prop-expr-cls-this-uninit");

    [Fact(DisplayName = "prop-expr-cls-unresolvable.js")]
    public Task prop_expr_cls_unresolvable()
        => ExecutionTest("prop-expr-cls-unresolvable");

    [Fact(DisplayName = "prop-expr-cls-val-from-arrow.js")]
    public Task prop_expr_cls_val_from_arrow()
        => ExecutionTest("prop-expr-cls-val-from-arrow");

    [Fact(DisplayName = "prop-expr-cls-val-from-eval.js", Skip = "eval is not supported.")]
    public Task prop_expr_cls_val_from_eval()
        => ExecutionTest("prop-expr-cls-val-from-eval");

    [Fact(DisplayName = "prop-expr-cls-val.js")]
    public Task prop_expr_cls_val()
        => ExecutionTest("prop-expr-cls-val");

    [Fact(DisplayName = "prop-expr-getsuperbase-before-topropertykey-getvalue.js")]
    public Task prop_expr_getsuperbase_before_topropertykey_getvalue()
        => ExecutionTest("prop-expr-getsuperbase-before-topropertykey-getvalue");

    [Fact(DisplayName = "prop-expr-getsuperbase-before-topropertykey-putvalue-compound-assign.js")]
    public Task prop_expr_getsuperbase_before_topropertykey_putvalue_compound_assign()
        => ExecutionTest("prop-expr-getsuperbase-before-topropertykey-putvalue-compound-assign");

    [Fact(DisplayName = "prop-expr-getsuperbase-before-topropertykey-putvalue-increment.js")]
    public Task prop_expr_getsuperbase_before_topropertykey_putvalue_increment()
        => ExecutionTest("prop-expr-getsuperbase-before-topropertykey-putvalue-increment");

    [Fact(DisplayName = "prop-expr-getsuperbase-before-topropertykey-putvalue.js")]
    public Task prop_expr_getsuperbase_before_topropertykey_putvalue()
        => ExecutionTest("prop-expr-getsuperbase-before-topropertykey-putvalue");

    [Fact(DisplayName = "prop-expr-obj-err.js")]
    public Task prop_expr_obj_err()
        => ExecutionTest("prop-expr-obj-err");

    [Fact(DisplayName = "prop-expr-obj-key-err.js")]
    public Task prop_expr_obj_key_err()
        => ExecutionTest("prop-expr-obj-key-err");

    [Fact(DisplayName = "prop-expr-obj-null-proto.js")]
    public Task prop_expr_obj_null_proto()
        => ExecutionTest("prop-expr-obj-null-proto");

    [Fact(DisplayName = "prop-expr-obj-ref-non-strict.js")]
    public Task prop_expr_obj_ref_non_strict()
        => ExecutionTest("prop-expr-obj-ref-non-strict");

    [Fact(DisplayName = "prop-expr-obj-ref-strict.js")]
    public Task prop_expr_obj_ref_strict()
        => ExecutionTest("prop-expr-obj-ref-strict");

    [Fact(DisplayName = "prop-expr-obj-ref-this.js")]
    public Task prop_expr_obj_ref_this()
        => ExecutionTest("prop-expr-obj-ref-this");

    [Fact(DisplayName = "prop-expr-obj-unresolvable.js")]
    public Task prop_expr_obj_unresolvable()
        => ExecutionTest("prop-expr-obj-unresolvable");

    [Fact(DisplayName = "prop-expr-obj-val-from-arrow.js")]
    public Task prop_expr_obj_val_from_arrow()
        => ExecutionTest("prop-expr-obj-val-from-arrow");

    [Fact(DisplayName = "prop-expr-obj-val-from-eval.js", Skip = "eval is not supported.")]
    public Task prop_expr_obj_val_from_eval()
        => ExecutionTest("prop-expr-obj-val-from-eval");

    [Fact(DisplayName = "prop-expr-obj-val.js")]
    public Task prop_expr_obj_val()
        => ExecutionTest("prop-expr-obj-val");

    [Fact(DisplayName = "prop-expr-uninitialized-this-getvalue.js")]
    public Task prop_expr_uninitialized_this_getvalue()
        => ExecutionTest("prop-expr-uninitialized-this-getvalue");

    [Fact(DisplayName = "prop-expr-uninitialized-this-putvalue-compound-assign.js")]
    public Task prop_expr_uninitialized_this_putvalue_compound_assign()
        => ExecutionTest("prop-expr-uninitialized-this-putvalue-compound-assign");

    [Fact(DisplayName = "prop-expr-uninitialized-this-putvalue-increment.js")]
    public Task prop_expr_uninitialized_this_putvalue_increment()
        => ExecutionTest("prop-expr-uninitialized-this-putvalue-increment");

    [Fact(DisplayName = "prop-expr-uninitialized-this-putvalue.js")]
    public Task prop_expr_uninitialized_this_putvalue()
        => ExecutionTest("prop-expr-uninitialized-this-putvalue");

    [Fact(DisplayName = "prop-poisoned-underscore-proto.js")]
    public Task prop_poisoned_underscore_proto()
        => ExecutionTest("prop-poisoned-underscore-proto");

    [Fact(DisplayName = "realm.js")]
    public Task realm()
        => ExecutionTest("realm");
}
