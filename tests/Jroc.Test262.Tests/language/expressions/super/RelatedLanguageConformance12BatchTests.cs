using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.super;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.super") { }

    [Fact(DisplayName = "language/expressions/super/call-arg-evaluation-err.js")]
    public Task test_call_arg_evaluation_err()
        => ExecutionTest("call-arg-evaluation-err");

    [Fact(DisplayName = "language/expressions/super/call-bind-this-value-twice.js")]
    public Task test_call_bind_this_value_twice()
        => ExecutionTest("call-bind-this-value-twice");

    [Fact(DisplayName = "language/expressions/super/call-bind-this-value.js")]
    public Task test_call_bind_this_value()
        => ExecutionTest("call-bind-this-value");

    [Fact(DisplayName = "language/expressions/super/call-construct-error.js")]
    public Task test_call_construct_error()
        => ExecutionTest("call-construct-error");

    [Fact(DisplayName = "language/expressions/super/call-construct-invocation.js")]
    public Task test_call_construct_invocation()
        => ExecutionTest("call-construct-invocation");

    [Fact(DisplayName = "language/expressions/super/call-poisoned-underscore-proto.js")]
    public Task test_call_poisoned_underscore_proto()
        => ExecutionTest("call-poisoned-underscore-proto");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-expr-throws.js")]
    public Task test_call_spread_err_mult_err_expr_throws()
        => ExecutionTest("call-spread-err-mult-err-expr-throws");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-iter-get-value.js")]
    public Task test_call_spread_err_mult_err_iter_get_value()
        => ExecutionTest("call-spread-err-mult-err-iter-get-value");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-itr-get-call.js")]
    public Task test_call_spread_err_mult_err_itr_get_call()
        => ExecutionTest("call-spread-err-mult-err-itr-get-call");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-itr-get-get.js")]
    public Task test_call_spread_err_mult_err_itr_get_get()
        => ExecutionTest("call-spread-err-mult-err-itr-get-get");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-itr-step.js")]
    public Task test_call_spread_err_mult_err_itr_step()
        => ExecutionTest("call-spread-err-mult-err-itr-step");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-itr-value.js")]
    public Task test_call_spread_err_mult_err_itr_value()
        => ExecutionTest("call-spread-err-mult-err-itr-value");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-obj-unresolvable.js")]
    public Task test_call_spread_err_mult_err_obj_unresolvable()
        => ExecutionTest("call-spread-err-mult-err-obj-unresolvable");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-mult-err-unresolvable.js")]
    public Task test_call_spread_err_mult_err_unresolvable()
        => ExecutionTest("call-spread-err-mult-err-unresolvable");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-expr-throws.js")]
    public Task test_call_spread_err_sngl_err_expr_throws()
        => ExecutionTest("call-spread-err-sngl-err-expr-throws");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-itr-get-call.js")]
    public Task test_call_spread_err_sngl_err_itr_get_call()
        => ExecutionTest("call-spread-err-sngl-err-itr-get-call");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-itr-get-get.js")]
    public Task test_call_spread_err_sngl_err_itr_get_get()
        => ExecutionTest("call-spread-err-sngl-err-itr-get-get");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-itr-get-value.js")]
    public Task test_call_spread_err_sngl_err_itr_get_value()
        => ExecutionTest("call-spread-err-sngl-err-itr-get-value");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-itr-step.js")]
    public Task test_call_spread_err_sngl_err_itr_step()
        => ExecutionTest("call-spread-err-sngl-err-itr-step");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-itr-value.js")]
    public Task test_call_spread_err_sngl_err_itr_value()
        => ExecutionTest("call-spread-err-sngl-err-itr-value");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-obj-unresolvable.js")]
    public Task test_call_spread_err_sngl_err_obj_unresolvable()
        => ExecutionTest("call-spread-err-sngl-err-obj-unresolvable");

    [Fact(DisplayName = "language/expressions/super/call-spread-err-sngl-err-unresolvable.js")]
    public Task test_call_spread_err_sngl_err_unresolvable()
        => ExecutionTest("call-spread-err-sngl-err-unresolvable");

    [Fact(DisplayName = "language/expressions/super/call-spread-mult-obj-ident.js")]
    public Task test_call_spread_mult_obj_ident()
        => ExecutionTest("call-spread-mult-obj-ident");

    [Fact(DisplayName = "language/expressions/super/call-spread-mult-obj-null.js")]
    public Task test_call_spread_mult_obj_null()
        => ExecutionTest("call-spread-mult-obj-null");

    [Fact(DisplayName = "language/expressions/super/call-spread-mult-obj-undefined.js")]
    public Task test_call_spread_mult_obj_undefined()
        => ExecutionTest("call-spread-mult-obj-undefined");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-getter-descriptor.js")]
    public Task test_call_spread_obj_getter_descriptor()
        => ExecutionTest("call-spread-obj-getter-descriptor");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-getter-init.js")]
    public Task test_call_spread_obj_getter_init()
        => ExecutionTest("call-spread-obj-getter-init");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-manipulate-outter-obj-in-getter.js")]
    public Task test_call_spread_obj_manipulate_outter_obj_in_getter()
        => ExecutionTest("call-spread-obj-manipulate-outter-obj-in-getter");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-mult-spread-getter.js")]
    public Task test_call_spread_obj_mult_spread_getter()
        => ExecutionTest("call-spread-obj-mult-spread-getter");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-mult-spread.js")]
    public Task test_call_spread_obj_mult_spread()
        => ExecutionTest("call-spread-obj-mult-spread");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-null.js")]
    public Task test_call_spread_obj_null()
        => ExecutionTest("call-spread-obj-null");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-override-immutable.js")]
    public Task test_call_spread_obj_override_immutable()
        => ExecutionTest("call-spread-obj-override-immutable");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-overrides-prev-properties.js")]
    public Task test_call_spread_obj_overrides_prev_properties()
        => ExecutionTest("call-spread-obj-overrides-prev-properties");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-skip-non-enumerable.js")]
    public Task test_call_spread_obj_skip_non_enumerable()
        => ExecutionTest("call-spread-obj-skip-non-enumerable");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-spread-order.js")]
    public Task test_call_spread_obj_spread_order()
        => ExecutionTest("call-spread-obj-spread-order");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-symbol-property.js")]
    public Task test_call_spread_obj_symbol_property()
        => ExecutionTest("call-spread-obj-symbol-property");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-undefined.js")]
    public Task test_call_spread_obj_undefined()
        => ExecutionTest("call-spread-obj-undefined");

    [Fact(DisplayName = "language/expressions/super/call-spread-obj-with-overrides.js")]
    public Task test_call_spread_obj_with_overrides()
        => ExecutionTest("call-spread-obj-with-overrides");

    [Fact(DisplayName = "language/expressions/super/call-spread-sngl-obj-ident.js")]
    public Task test_call_spread_sngl_obj_ident()
        => ExecutionTest("call-spread-sngl-obj-ident");

    [Fact(DisplayName = "language/expressions/super/prop-dot-obj-ref-this.js")]
    public Task test_prop_dot_obj_ref_this()
        => ExecutionTest("prop-dot-obj-ref-this");

    [Fact(DisplayName = "language/expressions/super/prop-dot-obj-val.js")]
    public Task test_prop_dot_obj_val()
        => ExecutionTest("prop-dot-obj-val");

    [Fact(DisplayName = "language/expressions/super/super-reference-resolution.js")]
    public Task test_super_reference_resolution()
        => ExecutionTest("super-reference-resolution");
}
