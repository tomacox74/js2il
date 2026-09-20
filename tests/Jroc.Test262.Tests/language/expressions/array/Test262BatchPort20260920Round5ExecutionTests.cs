using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.array;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.array") { }

    [Fact(DisplayName = "S11.1.4_A1.4")]
    public Task S11_1_4_A1_4()
        => ExecutionTest("S11.1.4_A1.4");

    [Fact(DisplayName = "S11.1.4_A1.5")]
    public Task S11_1_4_A1_5()
        => ExecutionTest("S11.1.4_A1.5");

    [Fact(DisplayName = "S11.1.4_A1.6")]
    public Task S11_1_4_A1_6()
        => ExecutionTest("S11.1.4_A1.6");

    [Fact(DisplayName = "S11.1.4_A1.7")]
    public Task S11_1_4_A1_7()
        => ExecutionTest("S11.1.4_A1.7");

    [Fact(DisplayName = "S11.1.4_A2")]
    public Task S11_1_4_A2()
        => ExecutionTest("S11.1.4_A2");

    [Fact(DisplayName = "spread-err-mult-err-expr-throws")]
    public Task spread_err_mult_err_expr_throws()
        => ExecutionTest("spread-err-mult-err-expr-throws");

    [Fact(DisplayName = "spread-err-mult-err-iter-get-value")]
    public Task spread_err_mult_err_iter_get_value()
        => ExecutionTest("spread-err-mult-err-iter-get-value");

    [Fact(DisplayName = "spread-err-mult-err-itr-get-call")]
    public Task spread_err_mult_err_itr_get_call()
        => ExecutionTest("spread-err-mult-err-itr-get-call");

    [Fact(DisplayName = "spread-err-mult-err-itr-get-get")]
    public Task spread_err_mult_err_itr_get_get()
        => ExecutionTest("spread-err-mult-err-itr-get-get");

    [Fact(DisplayName = "spread-err-mult-err-itr-step")]
    public Task spread_err_mult_err_itr_step()
        => ExecutionTest("spread-err-mult-err-itr-step");

    [Fact(DisplayName = "spread-err-mult-err-itr-value")]
    public Task spread_err_mult_err_itr_value()
        => ExecutionTest("spread-err-mult-err-itr-value");

    [Fact(DisplayName = "spread-err-mult-err-obj-unresolvable")]
    public Task spread_err_mult_err_obj_unresolvable()
        => ExecutionTest("spread-err-mult-err-obj-unresolvable");

    [Fact(DisplayName = "spread-err-mult-err-unresolvable")]
    public Task spread_err_mult_err_unresolvable()
        => ExecutionTest("spread-err-mult-err-unresolvable");

    [Fact(DisplayName = "spread-err-sngl-err-expr-throws")]
    public Task spread_err_sngl_err_expr_throws()
        => ExecutionTest("spread-err-sngl-err-expr-throws");

    [Fact(DisplayName = "spread-err-sngl-err-itr-get-call")]
    public Task spread_err_sngl_err_itr_get_call()
        => ExecutionTest("spread-err-sngl-err-itr-get-call");

    [Fact(DisplayName = "spread-err-sngl-err-itr-get-get")]
    public Task spread_err_sngl_err_itr_get_get()
        => ExecutionTest("spread-err-sngl-err-itr-get-get");

    [Fact(DisplayName = "spread-err-sngl-err-itr-get-value")]
    public Task spread_err_sngl_err_itr_get_value()
        => ExecutionTest("spread-err-sngl-err-itr-get-value");

    [Fact(DisplayName = "spread-err-sngl-err-itr-step")]
    public Task spread_err_sngl_err_itr_step()
        => ExecutionTest("spread-err-sngl-err-itr-step");

    [Fact(DisplayName = "spread-err-sngl-err-itr-value")]
    public Task spread_err_sngl_err_itr_value()
        => ExecutionTest("spread-err-sngl-err-itr-value");

    [Fact(DisplayName = "spread-err-sngl-err-obj-unresolvable")]
    public Task spread_err_sngl_err_obj_unresolvable()
        => ExecutionTest("spread-err-sngl-err-obj-unresolvable");

    [Fact(DisplayName = "spread-err-sngl-err-unresolvable")]
    public Task spread_err_sngl_err_unresolvable()
        => ExecutionTest("spread-err-sngl-err-unresolvable");

    [Fact(DisplayName = "spread-mult-obj-ident")]
    public Task spread_mult_obj_ident()
        => ExecutionTest("spread-mult-obj-ident");

    [Fact(DisplayName = "spread-mult-obj-null")]
    public Task spread_mult_obj_null()
        => ExecutionTest("spread-mult-obj-null");

    [Fact(DisplayName = "spread-mult-obj-undefined")]
    public Task spread_mult_obj_undefined()
        => ExecutionTest("spread-mult-obj-undefined");

    [Fact(DisplayName = "spread-obj-getter-descriptor")]
    public Task spread_obj_getter_descriptor()
        => ExecutionTest("spread-obj-getter-descriptor");

    [Fact(DisplayName = "spread-obj-getter-init")]
    public Task spread_obj_getter_init()
        => ExecutionTest("spread-obj-getter-init");

    [Fact(DisplayName = "spread-obj-manipulate-outter-obj-in-getter")]
    public Task spread_obj_manipulate_outter_obj_in_getter()
        => ExecutionTest("spread-obj-manipulate-outter-obj-in-getter");

    [Fact(DisplayName = "spread-obj-mult-spread-getter")]
    public Task spread_obj_mult_spread_getter()
        => ExecutionTest("spread-obj-mult-spread-getter");

    [Fact(DisplayName = "spread-obj-mult-spread")]
    public Task spread_obj_mult_spread()
        => ExecutionTest("spread-obj-mult-spread");

    [Fact(DisplayName = "spread-obj-null")]
    public Task spread_obj_null()
        => ExecutionTest("spread-obj-null");

    [Fact(DisplayName = "spread-obj-override-immutable")]
    public Task spread_obj_override_immutable()
        => ExecutionTest("spread-obj-override-immutable");

    [Fact(DisplayName = "spread-obj-overrides-prev-properties")]
    public Task spread_obj_overrides_prev_properties()
        => ExecutionTest("spread-obj-overrides-prev-properties");

    [Fact(DisplayName = "spread-obj-skip-non-enumerable")]
    public Task spread_obj_skip_non_enumerable()
        => ExecutionTest("spread-obj-skip-non-enumerable");

    [Fact(DisplayName = "spread-obj-spread-order")]
    public Task spread_obj_spread_order()
        => ExecutionTest("spread-obj-spread-order");

    [Fact(DisplayName = "spread-obj-symbol-property")]
    public Task spread_obj_symbol_property()
        => ExecutionTest("spread-obj-symbol-property");

    [Fact(DisplayName = "spread-obj-undefined")]
    public Task spread_obj_undefined()
        => ExecutionTest("spread-obj-undefined");

    [Fact(DisplayName = "spread-obj-with-overrides")]
    public Task spread_obj_with_overrides()
        => ExecutionTest("spread-obj-with-overrides");

    [Fact(DisplayName = "spread-sngl-obj-ident")]
    public Task spread_sngl_obj_ident()
        => ExecutionTest("spread-sngl-obj-ident");

}
