namespace Jroc.Test262.Tests.language.statements.test_class.elements;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.elements") { }

    [Fact(DisplayName = "derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTest("derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "derived-cls-indirect-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task derived_cls_indirect_eval_contains_superproperty_1()
        => ExecutionTest("derived-cls-indirect-eval-contains-superproperty-1");

    [Fact(DisplayName = "derived-cls-indirect-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task derived_cls_indirect_eval_contains_superproperty_2()
        => ExecutionTest("derived-cls-indirect-eval-contains-superproperty-2");

    [Fact(DisplayName = "derived-cls-indirect-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task derived_cls_indirect_eval_err_contains_supercall_1()
        => ExecutionTest("derived-cls-indirect-eval-err-contains-supercall-1");

    [Fact(DisplayName = "derived-cls-indirect-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task derived_cls_indirect_eval_err_contains_supercall_2()
        => ExecutionTest("derived-cls-indirect-eval-err-contains-supercall-2");

    [Fact(DisplayName = "derived-cls-indirect-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task derived_cls_indirect_eval_err_contains_supercall()
        => ExecutionTest("derived-cls-indirect-eval-err-contains-supercall");

    [Fact(DisplayName = "direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task direct_eval_err_contains_arguments()
        => ExecutionTest("direct-eval-err-contains-arguments");

    [Fact(DisplayName = "direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task direct_eval_err_contains_newtarget()
        => ExecutionTest("direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "indirect-eval-contains-arguments.js", Skip = "eval is not supported.")]
    public Task indirect_eval_contains_arguments()
        => ExecutionTest("indirect-eval-contains-arguments");

    [Fact(DisplayName = "indirect-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task indirect_eval_err_contains_newtarget()
        => ExecutionTest("indirect-eval-err-contains-newtarget");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTest("nested-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTest("nested-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTest("nested-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTest("nested-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTest("nested-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "nested-derived-cls-indirect-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_indirect_eval_contains_superproperty_1()
        => ExecutionTest("nested-derived-cls-indirect-eval-contains-superproperty-1");

    [Fact(DisplayName = "nested-derived-cls-indirect-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_indirect_eval_contains_superproperty_2()
        => ExecutionTest("nested-derived-cls-indirect-eval-contains-superproperty-2");

    [Fact(DisplayName = "nested-derived-cls-indirect-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_indirect_eval_err_contains_supercall_1()
        => ExecutionTest("nested-derived-cls-indirect-eval-err-contains-supercall-1");

    [Fact(DisplayName = "nested-derived-cls-indirect-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_indirect_eval_err_contains_supercall_2()
        => ExecutionTest("nested-derived-cls-indirect-eval-err-contains-supercall-2");

    [Fact(DisplayName = "nested-derived-cls-indirect-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_indirect_eval_err_contains_supercall()
        => ExecutionTest("nested-derived-cls-indirect-eval-err-contains-supercall");

    [Fact(DisplayName = "nested-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task nested_direct_eval_err_contains_arguments()
        => ExecutionTest("nested-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "nested-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task nested_direct_eval_err_contains_newtarget()
        => ExecutionTest("nested-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "nested-indirect-eval-contains-arguments.js", Skip = "eval is not supported.")]
    public Task nested_indirect_eval_contains_arguments()
        => ExecutionTest("nested-indirect-eval-contains-arguments");

    [Fact(DisplayName = "nested-indirect-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task nested_indirect_eval_err_contains_newtarget()
        => ExecutionTest("nested-indirect-eval-err-contains-newtarget");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTest("nested-private-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTest("nested-private-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTest("nested-private-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTest("nested-private-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTest("nested-private-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "nested-private-derived-cls-indirect-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_indirect_eval_contains_superproperty_1()
        => ExecutionTest("nested-private-derived-cls-indirect-eval-contains-superproperty-1");

    [Fact(DisplayName = "nested-private-derived-cls-indirect-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_indirect_eval_contains_superproperty_2()
        => ExecutionTest("nested-private-derived-cls-indirect-eval-contains-superproperty-2");

    [Fact(DisplayName = "nested-private-derived-cls-indirect-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_indirect_eval_err_contains_supercall_1()
        => ExecutionTest("nested-private-derived-cls-indirect-eval-err-contains-supercall-1");

    [Fact(DisplayName = "nested-private-derived-cls-indirect-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_indirect_eval_err_contains_supercall_2()
        => ExecutionTest("nested-private-derived-cls-indirect-eval-err-contains-supercall-2");

    [Fact(DisplayName = "nested-private-derived-cls-indirect-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_indirect_eval_err_contains_supercall()
        => ExecutionTest("nested-private-derived-cls-indirect-eval-err-contains-supercall");

    [Fact(DisplayName = "nested-private-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task nested_private_direct_eval_err_contains_arguments()
        => ExecutionTest("nested-private-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "nested-private-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task nested_private_direct_eval_err_contains_newtarget()
        => ExecutionTest("nested-private-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "nested-private-indirect-eval-contains-arguments.js", Skip = "eval is not supported.")]
    public Task nested_private_indirect_eval_contains_arguments()
        => ExecutionTest("nested-private-indirect-eval-contains-arguments");

    [Fact(DisplayName = "nested-private-indirect-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task nested_private_indirect_eval_err_contains_newtarget()
        => ExecutionTest("nested-private-indirect-eval-err-contains-newtarget");

    [Fact(DisplayName = "private-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTest("private-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "private-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTest("private-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "private-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTest("private-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "private-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTest("private-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "private-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTest("private-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "private-derived-cls-indirect-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_indirect_eval_contains_superproperty_1()
        => ExecutionTest("private-derived-cls-indirect-eval-contains-superproperty-1");

    [Fact(DisplayName = "private-derived-cls-indirect-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_indirect_eval_contains_superproperty_2()
        => ExecutionTest("private-derived-cls-indirect-eval-contains-superproperty-2");

    [Fact(DisplayName = "private-derived-cls-indirect-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_indirect_eval_err_contains_supercall_1()
        => ExecutionTest("private-derived-cls-indirect-eval-err-contains-supercall-1");

    [Fact(DisplayName = "private-derived-cls-indirect-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_indirect_eval_err_contains_supercall_2()
        => ExecutionTest("private-derived-cls-indirect-eval-err-contains-supercall-2");

    [Fact(DisplayName = "private-derived-cls-indirect-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_indirect_eval_err_contains_supercall()
        => ExecutionTest("private-derived-cls-indirect-eval-err-contains-supercall");

    [Fact(DisplayName = "private-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task private_direct_eval_err_contains_arguments()
        => ExecutionTest("private-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "private-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task private_direct_eval_err_contains_newtarget()
        => ExecutionTest("private-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "private-field-visible-to-direct-eval-on-initializer.js", Skip = "eval is not supported.")]
    public Task private_field_visible_to_direct_eval_on_initializer()
        => ExecutionTest("private-field-visible-to-direct-eval-on-initializer");

    [Fact(DisplayName = "private-field-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_field_visible_to_direct_eval()
        => ExecutionTest("private-field-visible-to-direct-eval");

    [Fact(DisplayName = "private-getter-visible-to-direct-eval-on-initializer.js", Skip = "eval is not supported.")]
    public Task private_getter_visible_to_direct_eval_on_initializer()
        => ExecutionTest("private-getter-visible-to-direct-eval-on-initializer");

    [Fact(DisplayName = "private-getter-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_getter_visible_to_direct_eval()
        => ExecutionTest("private-getter-visible-to-direct-eval");

    [Fact(DisplayName = "private-indirect-eval-contains-arguments.js", Skip = "eval is not supported.")]
    public Task private_indirect_eval_contains_arguments()
        => ExecutionTest("private-indirect-eval-contains-arguments");

    [Fact(DisplayName = "private-indirect-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task private_indirect_eval_err_contains_newtarget()
        => ExecutionTest("private-indirect-eval-err-contains-newtarget");

    [Fact(DisplayName = "private-method-visible-to-direct-eval-on-initializer.js", Skip = "eval is not supported.")]
    public Task private_method_visible_to_direct_eval_on_initializer()
        => ExecutionTest("private-method-visible-to-direct-eval-on-initializer");

    [Fact(DisplayName = "private-method-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_method_visible_to_direct_eval()
        => ExecutionTest("private-method-visible-to-direct-eval");

    [Fact(DisplayName = "private-setter-visible-to-direct-eval-on-initializer.js", Skip = "eval is not supported.")]
    public Task private_setter_visible_to_direct_eval_on_initializer()
        => ExecutionTest("private-setter-visible-to-direct-eval-on-initializer");

    [Fact(DisplayName = "private-setter-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_setter_visible_to_direct_eval()
        => ExecutionTest("private-setter-visible-to-direct-eval");

    [Fact(DisplayName = "private-static-field-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_field_visible_to_direct_eval()
        => ExecutionTest("private-static-field-visible-to-direct-eval");

    [Fact(DisplayName = "private-static-getter-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_getter_visible_to_direct_eval()
        => ExecutionTest("private-static-getter-visible-to-direct-eval");

    [Fact(DisplayName = "private-static-method-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_method_visible_to_direct_eval()
        => ExecutionTest("private-static-method-visible-to-direct-eval");

    [Fact(DisplayName = "private-static-setter-visible-to-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_setter_visible_to_direct_eval()
        => ExecutionTest("private-static-setter-visible-to-direct-eval");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-1.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_1()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-1");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-2.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_2()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-2");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-3.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_3()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-3");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-4.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_4()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-4");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-5.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_5()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-5");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-6.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_6()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-6");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-7.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_7()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-7");

    [Fact(DisplayName = "privatename-not-valid-eval-earlyerr-8.js", Skip = "eval is not supported.")]
    public Task privatename_not_valid_eval_earlyerr_8()
        => ExecutionTest("privatename-not-valid-eval-earlyerr-8");

    [Fact(DisplayName = "static-field-init-with-this.js", Skip = "eval is not supported.")]
    public Task static_field_init_with_this()
        => ExecutionTest("static-field-init-with-this");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-async-generator-method-privatename-identifier-alt.js")]
    public Task wrapped_in_sc_rs_static_async_generator_method_privatename_identifier_alt()
        => ExecutionTest("wrapped-in-sc-rs-static-async-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-async-generator-method-privatename-identifier.js")]
    public Task wrapped_in_sc_rs_static_async_generator_method_privatename_identifier()
        => ExecutionTest("wrapped-in-sc-rs-static-async-generator-method-privatename-identifier");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-async-method-privatename-identifier-alt.js")]
    public Task wrapped_in_sc_rs_static_async_method_privatename_identifier_alt()
        => ExecutionTest("wrapped-in-sc-rs-static-async-method-privatename-identifier-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-async-method-privatename-identifier.js")]
    public Task wrapped_in_sc_rs_static_async_method_privatename_identifier()
        => ExecutionTest("wrapped-in-sc-rs-static-async-method-privatename-identifier");
}
