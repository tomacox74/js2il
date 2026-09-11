using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.new_;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.new_") { }

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A2.js")]
    public Task test_S11_2_2_A2()
        => ExecutionTest("S11.2.2_A2");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A3_T1.js")]
    public Task test_S11_2_2_A3_T1()
        => ExecutionTest("S11.2.2_A3_T1");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A3_T2.js")]
    public Task test_S11_2_2_A3_T2()
        => ExecutionTest("S11.2.2_A3_T2");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A3_T3.js")]
    public Task test_S11_2_2_A3_T3()
        => ExecutionTest("S11.2.2_A3_T3");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A3_T4.js")]
    public Task test_S11_2_2_A3_T4()
        => ExecutionTest("S11.2.2_A3_T4");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A3_T5.js")]
    public Task test_S11_2_2_A3_T5()
        => ExecutionTest("S11.2.2_A3_T5");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A4_T1.js")]
    public Task test_S11_2_2_A4_T1()
        => ExecutionTest("S11.2.2_A4_T1");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A4_T2.js")]
    public Task test_S11_2_2_A4_T2()
        => ExecutionTest("S11.2.2_A4_T2");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A4_T3.js")]
    public Task test_S11_2_2_A4_T3()
        => ExecutionTest("S11.2.2_A4_T3");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A4_T4.js")]
    public Task test_S11_2_2_A4_T4()
        => ExecutionTest("S11.2.2_A4_T4");

    [Fact(DisplayName = "language/expressions/new/S11.2.2_A4_T5.js")]
    public Task test_S11_2_2_A4_T5()
        => ExecutionTest("S11.2.2_A4_T5");

    [Fact(DisplayName = "language/expressions/new/ctorExpr-isCtor-after-args-eval-fn-wrapup.js")]
    public Task test_ctorExpr_isCtor_after_args_eval_fn_wrapup()
        => ExecutionTest("ctorExpr-isCtor-after-args-eval-fn-wrapup");

    [Fact(DisplayName = "language/expressions/new/ctorExpr-isCtor-after-args-eval.js")]
    public Task test_ctorExpr_isCtor_after_args_eval()
        => ExecutionTest("ctorExpr-isCtor-after-args-eval");

    [Fact(DisplayName = "language/expressions/new/spread-err-mult-err-obj-unresolvable.js")]
    public Task test_spread_err_mult_err_obj_unresolvable()
        => ExecutionTest("spread-err-mult-err-obj-unresolvable");

    [Fact(DisplayName = "language/expressions/new/spread-err-sngl-err-obj-unresolvable.js")]
    public Task test_spread_err_sngl_err_obj_unresolvable()
        => ExecutionTest("spread-err-sngl-err-obj-unresolvable");

    [Fact(DisplayName = "language/expressions/new/spread-mult-obj-ident.js")]
    public Task test_spread_mult_obj_ident()
        => ExecutionTest("spread-mult-obj-ident");

    [Fact(DisplayName = "language/expressions/new/spread-mult-obj-null.js")]
    public Task test_spread_mult_obj_null()
        => ExecutionTest("spread-mult-obj-null");

    [Fact(DisplayName = "language/expressions/new/spread-mult-obj-undefined.js")]
    public Task test_spread_mult_obj_undefined()
        => ExecutionTest("spread-mult-obj-undefined");

    [Fact(DisplayName = "language/expressions/new/spread-obj-getter-descriptor.js")]
    public Task test_spread_obj_getter_descriptor()
        => ExecutionTest("spread-obj-getter-descriptor");

    [Fact(DisplayName = "language/expressions/new/spread-obj-getter-init.js")]
    public Task test_spread_obj_getter_init()
        => ExecutionTest("spread-obj-getter-init");

    [Fact(DisplayName = "language/expressions/new/spread-obj-manipulate-outter-obj-in-getter.js")]
    public Task test_spread_obj_manipulate_outter_obj_in_getter()
        => ExecutionTest("spread-obj-manipulate-outter-obj-in-getter");

    [Fact(DisplayName = "language/expressions/new/spread-obj-mult-spread-getter.js")]
    public Task test_spread_obj_mult_spread_getter()
        => ExecutionTest("spread-obj-mult-spread-getter");

    [Fact(DisplayName = "language/expressions/new/spread-obj-mult-spread.js")]
    public Task test_spread_obj_mult_spread()
        => ExecutionTest("spread-obj-mult-spread");

    [Fact(DisplayName = "language/expressions/new/spread-obj-null.js")]
    public Task test_spread_obj_null()
        => ExecutionTest("spread-obj-null");

    [Fact(DisplayName = "language/expressions/new/spread-obj-override-immutable.js")]
    public Task test_spread_obj_override_immutable()
        => ExecutionTest("spread-obj-override-immutable");

    [Fact(DisplayName = "language/expressions/new/spread-obj-overrides-prev-properties.js")]
    public Task test_spread_obj_overrides_prev_properties()
        => ExecutionTest("spread-obj-overrides-prev-properties");

    [Fact(DisplayName = "language/expressions/new/spread-obj-skip-non-enumerable.js")]
    public Task test_spread_obj_skip_non_enumerable()
        => ExecutionTest("spread-obj-skip-non-enumerable");

    [Fact(DisplayName = "language/expressions/new/spread-obj-spread-order.js")]
    public Task test_spread_obj_spread_order()
        => ExecutionTest("spread-obj-spread-order");

    [Fact(DisplayName = "language/expressions/new/spread-obj-symbol-property.js")]
    public Task test_spread_obj_symbol_property()
        => ExecutionTest("spread-obj-symbol-property");

    [Fact(DisplayName = "language/expressions/new/spread-obj-undefined.js")]
    public Task test_spread_obj_undefined()
        => ExecutionTest("spread-obj-undefined");

    [Fact(DisplayName = "language/expressions/new/spread-obj-with-overrides.js")]
    public Task test_spread_obj_with_overrides()
        => ExecutionTest("spread-obj-with-overrides");

    [Fact(DisplayName = "language/expressions/new/spread-sngl-obj-ident.js")]
    public Task test_spread_sngl_obj_ident()
        => ExecutionTest("spread-sngl-obj-ident");
}
