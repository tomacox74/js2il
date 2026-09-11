using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.call;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.call") { }

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A2.js")]
    public Task test_S11_2_3_A2()
        => ExecutionTest("S11.2.3_A2");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A3_T1.js")]
    public Task test_S11_2_3_A3_T1()
        => ExecutionTest("S11.2.3_A3_T1");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A3_T2.js")]
    public Task test_S11_2_3_A3_T2()
        => ExecutionTest("S11.2.3_A3_T2");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A3_T3.js")]
    public Task test_S11_2_3_A3_T3()
        => ExecutionTest("S11.2.3_A3_T3");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A3_T5.js")]
    public Task test_S11_2_3_A3_T5()
        => ExecutionTest("S11.2.3_A3_T5");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A4_T1.js")]
    public Task test_S11_2_3_A4_T1()
        => ExecutionTest("S11.2.3_A4_T1");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A4_T2.js")]
    public Task test_S11_2_3_A4_T2()
        => ExecutionTest("S11.2.3_A4_T2");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A4_T3.js")]
    public Task test_S11_2_3_A4_T3()
        => ExecutionTest("S11.2.3_A4_T3");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A4_T4.js")]
    public Task test_S11_2_3_A4_T4()
        => ExecutionTest("S11.2.3_A4_T4");

    [Fact(DisplayName = "language/expressions/call/S11.2.3_A4_T5.js")]
    public Task test_S11_2_3_A4_T5()
        => ExecutionTest("S11.2.3_A4_T5");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.1_T1.js")]
    public Task test_S11_2_4_A1_1_T1()
        => ExecutionTest("S11.2.4_A1.1_T1");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.1_T2.js")]
    public Task test_S11_2_4_A1_1_T2()
        => ExecutionTest("S11.2.4_A1.1_T2");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.2_T1.js")]
    public Task test_S11_2_4_A1_2_T1()
        => ExecutionTest("S11.2.4_A1.2_T1");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.2_T2.js")]
    public Task test_S11_2_4_A1_2_T2()
        => ExecutionTest("S11.2.4_A1.2_T2");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.4_T1.js")]
    public Task test_S11_2_4_A1_4_T1()
        => ExecutionTest("S11.2.4_A1.4_T1");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.4_T2.js")]
    public Task test_S11_2_4_A1_4_T2()
        => ExecutionTest("S11.2.4_A1.4_T2");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.4_T3.js")]
    public Task test_S11_2_4_A1_4_T3()
        => ExecutionTest("S11.2.4_A1.4_T3");

    [Fact(DisplayName = "language/expressions/call/S11.2.4_A1.4_T4.js")]
    public Task test_S11_2_4_A1_4_T4()
        => ExecutionTest("S11.2.4_A1.4_T4");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-expr-throws.js")]
    public Task test_spread_err_mult_err_expr_throws()
        => ExecutionTest("spread-err-mult-err-expr-throws");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-iter-get-value.js")]
    public Task test_spread_err_mult_err_iter_get_value()
        => ExecutionTest("spread-err-mult-err-iter-get-value");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-itr-get-call.js")]
    public Task test_spread_err_mult_err_itr_get_call()
        => ExecutionTest("spread-err-mult-err-itr-get-call");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-itr-get-get.js")]
    public Task test_spread_err_mult_err_itr_get_get()
        => ExecutionTest("spread-err-mult-err-itr-get-get");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-itr-step.js")]
    public Task test_spread_err_mult_err_itr_step()
        => ExecutionTest("spread-err-mult-err-itr-step");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-itr-value.js")]
    public Task test_spread_err_mult_err_itr_value()
        => ExecutionTest("spread-err-mult-err-itr-value");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-obj-unresolvable.js")]
    public Task test_spread_err_mult_err_obj_unresolvable()
        => ExecutionTest("spread-err-mult-err-obj-unresolvable");

    [Fact(DisplayName = "language/expressions/call/spread-err-mult-err-unresolvable.js")]
    public Task test_spread_err_mult_err_unresolvable()
        => ExecutionTest("spread-err-mult-err-unresolvable");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-expr-throws.js")]
    public Task test_spread_err_sngl_err_expr_throws()
        => ExecutionTest("spread-err-sngl-err-expr-throws");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-itr-get-call.js")]
    public Task test_spread_err_sngl_err_itr_get_call()
        => ExecutionTest("spread-err-sngl-err-itr-get-call");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-itr-get-get.js")]
    public Task test_spread_err_sngl_err_itr_get_get()
        => ExecutionTest("spread-err-sngl-err-itr-get-get");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-itr-get-value.js")]
    public Task test_spread_err_sngl_err_itr_get_value()
        => ExecutionTest("spread-err-sngl-err-itr-get-value");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-itr-step.js")]
    public Task test_spread_err_sngl_err_itr_step()
        => ExecutionTest("spread-err-sngl-err-itr-step");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-itr-value.js")]
    public Task test_spread_err_sngl_err_itr_value()
        => ExecutionTest("spread-err-sngl-err-itr-value");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-obj-unresolvable.js")]
    public Task test_spread_err_sngl_err_obj_unresolvable()
        => ExecutionTest("spread-err-sngl-err-obj-unresolvable");

    [Fact(DisplayName = "language/expressions/call/spread-err-sngl-err-unresolvable.js")]
    public Task test_spread_err_sngl_err_unresolvable()
        => ExecutionTest("spread-err-sngl-err-unresolvable");

    [Fact(DisplayName = "language/expressions/call/spread-mult-obj-ident.js")]
    public Task test_spread_mult_obj_ident()
        => ExecutionTest("spread-mult-obj-ident");

    [Fact(DisplayName = "language/expressions/call/spread-mult-obj-null.js")]
    public Task test_spread_mult_obj_null()
        => ExecutionTest("spread-mult-obj-null");

    [Fact(DisplayName = "language/expressions/call/spread-mult-obj-undefined.js")]
    public Task test_spread_mult_obj_undefined()
        => ExecutionTest("spread-mult-obj-undefined");

    [Fact(DisplayName = "language/expressions/call/spread-obj-getter-descriptor.js")]
    public Task test_spread_obj_getter_descriptor()
        => ExecutionTest("spread-obj-getter-descriptor");

    [Fact(DisplayName = "language/expressions/call/spread-obj-getter-init.js")]
    public Task test_spread_obj_getter_init()
        => ExecutionTest("spread-obj-getter-init");

    [Fact(DisplayName = "language/expressions/call/spread-obj-manipulate-outter-obj-in-getter.js")]
    public Task test_spread_obj_manipulate_outter_obj_in_getter()
        => ExecutionTest("spread-obj-manipulate-outter-obj-in-getter");

    [Fact(DisplayName = "language/expressions/call/spread-obj-mult-spread-getter.js")]
    public Task test_spread_obj_mult_spread_getter()
        => ExecutionTest("spread-obj-mult-spread-getter");

    [Fact(DisplayName = "language/expressions/call/spread-obj-mult-spread.js")]
    public Task test_spread_obj_mult_spread()
        => ExecutionTest("spread-obj-mult-spread");

    [Fact(DisplayName = "language/expressions/call/spread-obj-null.js")]
    public Task test_spread_obj_null()
        => ExecutionTest("spread-obj-null");

    [Fact(DisplayName = "language/expressions/call/spread-obj-override-immutable.js")]
    public Task test_spread_obj_override_immutable()
        => ExecutionTest("spread-obj-override-immutable");

    [Fact(DisplayName = "language/expressions/call/spread-obj-overrides-prev-properties.js")]
    public Task test_spread_obj_overrides_prev_properties()
        => ExecutionTest("spread-obj-overrides-prev-properties");

    [Fact(DisplayName = "language/expressions/call/spread-obj-skip-non-enumerable.js")]
    public Task test_spread_obj_skip_non_enumerable()
        => ExecutionTest("spread-obj-skip-non-enumerable");

    [Fact(DisplayName = "language/expressions/call/spread-obj-spread-order.js")]
    public Task test_spread_obj_spread_order()
        => ExecutionTest("spread-obj-spread-order");

    [Fact(DisplayName = "language/expressions/call/spread-obj-symbol-property.js")]
    public Task test_spread_obj_symbol_property()
        => ExecutionTest("spread-obj-symbol-property");

    [Fact(DisplayName = "language/expressions/call/spread-obj-undefined.js")]
    public Task test_spread_obj_undefined()
        => ExecutionTest("spread-obj-undefined");

    [Fact(DisplayName = "language/expressions/call/spread-obj-with-overrides.js")]
    public Task test_spread_obj_with_overrides()
        => ExecutionTest("spread-obj-with-overrides");

    [Fact(DisplayName = "language/expressions/call/spread-sngl-literal.js")]
    public Task test_spread_sngl_literal()
        => ExecutionTest("spread-sngl-literal");

    [Fact(DisplayName = "language/expressions/call/spread-sngl-obj-ident.js")]
    public Task test_spread_sngl_obj_ident()
        => ExecutionTest("spread-sngl-obj-ident");

    [Fact(DisplayName = "language/expressions/call/trailing-comma.js")]
    public Task test_trailing_comma()
        => ExecutionTest("trailing-comma");
}
