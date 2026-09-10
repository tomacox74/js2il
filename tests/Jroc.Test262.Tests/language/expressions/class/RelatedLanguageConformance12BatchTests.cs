using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.class_;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.class_") { }

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-field-identifier-initializer.js")]
    public Task test_elements_wrapped_in_sc_rs_field_identifier_initializer()
        => ExecutionTest("elements/wrapped-in-sc-rs-field-identifier-initializer");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-field-identifier.js")]
    public Task test_elements_wrapped_in_sc_rs_field_identifier()
        => ExecutionTest("elements/wrapped-in-sc-rs-field-identifier");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-private-getter-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_private_getter_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-getter-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-private-getter.js")]
    public Task test_elements_wrapped_in_sc_rs_private_getter()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-getter");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-private-method-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_private_method_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-method-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-private-method.js")]
    public Task test_elements_wrapped_in_sc_rs_private_method()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-method");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-private-setter-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_private_setter_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-setter-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-private-setter.js")]
    public Task test_elements_wrapped_in_sc_rs_private_setter()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-setter");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-privatename-identifier-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_privatename_identifier_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-privatename-identifier-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-privatename-identifier-initializer.js")]
    public Task test_elements_wrapped_in_sc_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/wrapped-in-sc-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-privatename-identifier.js")]
    public Task test_elements_wrapped_in_sc_rs_privatename_identifier()
        => ExecutionTest("elements/wrapped-in-sc-rs-privatename-identifier");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_wrapped_in_sc_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/wrapped-in-sc-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-rs-static-method-privatename-identifier.js")]
    public Task test_elements_wrapped_in_sc_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/wrapped-in-sc-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "language/expressions/class/elements/wrapped-in-sc-static-private-methods.js")]
    public Task test_elements_wrapped_in_sc_static_private_methods()
        => ExecutionTest("elements/wrapped-in-sc-static-private-methods");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/dflt-params-arg-val-not-undefined.js")]
    public Task test_gen_method_static_dflt_params_arg_val_not_undefined()
        => ExecutionTest("gen-method-static/dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/dflt-params-arg-val-undefined.js")]
    public Task test_gen_method_static_dflt_params_arg_val_undefined()
        => ExecutionTest("gen-method-static/dflt-params-arg-val-undefined");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/dflt-params-ref-prior.js")]
    public Task test_gen_method_static_dflt_params_ref_prior()
        => ExecutionTest("gen-method-static/dflt-params-ref-prior");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/dflt-params-trailing-comma.js")]
    public Task test_gen_method_static_dflt_params_trailing_comma()
        => ExecutionTest("gen-method-static/dflt-params-trailing-comma");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/forbidden-ext/b1/cls-expr-gen-meth-static-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_gen_method_static_forbidden_ext_b1_cls_expr_gen_meth_static_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("gen-method-static/forbidden-ext/b1/cls-expr-gen-meth-static-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/forbidden-ext/b1/cls-expr-gen-meth-static-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_gen_method_static_forbidden_ext_b1_cls_expr_gen_meth_static_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("gen-method-static/forbidden-ext/b1/cls-expr-gen-meth-static-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/forbidden-ext/b2/cls-expr-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_gen_method_static_forbidden_ext_b2_cls_expr_gen_meth_static_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("gen-method-static/forbidden-ext/b2/cls-expr-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/forbidden-ext/b2/cls-expr-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_gen_method_static_forbidden_ext_b2_cls_expr_gen_meth_static_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("gen-method-static/forbidden-ext/b2/cls-expr-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/forbidden-ext/b2/cls-expr-gen-meth-static-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_gen_method_static_forbidden_ext_b2_cls_expr_gen_meth_static_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("gen-method-static/forbidden-ext/b2/cls-expr-gen-meth-static-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/params-trailing-comma-multiple.js")]
    public Task test_gen_method_static_params_trailing_comma_multiple()
        => ExecutionTest("gen-method-static/params-trailing-comma-multiple");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/params-trailing-comma-single.js")]
    public Task test_gen_method_static_params_trailing_comma_single()
        => ExecutionTest("gen-method-static/params-trailing-comma-single");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/yield-spread-arr-multiple.js")]
    public Task test_gen_method_static_yield_spread_arr_multiple()
        => ExecutionTest("gen-method-static/yield-spread-arr-multiple");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/yield-spread-arr-single.js")]
    public Task test_gen_method_static_yield_spread_arr_single()
        => ExecutionTest("gen-method-static/yield-spread-arr-single");

    [Fact(DisplayName = "language/expressions/class/gen-method-static/yield-spread-obj.js")]
    public Task test_gen_method_static_yield_spread_obj()
        => ExecutionTest("gen-method-static/yield-spread-obj");

    [Fact(DisplayName = "language/expressions/class/gen-method/dflt-params-arg-val-undefined.js")]
    public Task test_gen_method_dflt_params_arg_val_undefined()
        => ExecutionTest("gen-method/dflt-params-arg-val-undefined");

    [Fact(DisplayName = "language/expressions/class/gen-method/dflt-params-ref-prior.js")]
    public Task test_gen_method_dflt_params_ref_prior()
        => ExecutionTest("gen-method/dflt-params-ref-prior");

    [Fact(DisplayName = "language/expressions/class/gen-method/dflt-params-trailing-comma.js")]
    public Task test_gen_method_dflt_params_trailing_comma()
        => ExecutionTest("gen-method/dflt-params-trailing-comma");

    [Fact(DisplayName = "language/expressions/class/gen-method/forbidden-ext/b1/cls-expr-gen-meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_gen_method_forbidden_ext_b1_cls_expr_gen_meth_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("gen-method/forbidden-ext/b1/cls-expr-gen-meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "language/expressions/class/gen-method/forbidden-ext/b1/cls-expr-gen-meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_gen_method_forbidden_ext_b1_cls_expr_gen_meth_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("gen-method/forbidden-ext/b1/cls-expr-gen-meth-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "language/expressions/class/gen-method/forbidden-ext/b2/cls-expr-gen-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_gen_method_forbidden_ext_b2_cls_expr_gen_meth_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("gen-method/forbidden-ext/b2/cls-expr-gen-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "language/expressions/class/gen-method/forbidden-ext/b2/cls-expr-gen-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_gen_method_forbidden_ext_b2_cls_expr_gen_meth_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("gen-method/forbidden-ext/b2/cls-expr-gen-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "language/expressions/class/gen-method/forbidden-ext/b2/cls-expr-gen-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_gen_method_forbidden_ext_b2_cls_expr_gen_meth_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("gen-method/forbidden-ext/b2/cls-expr-gen-meth-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "language/expressions/class/gen-method/params-trailing-comma-multiple.js")]
    public Task test_gen_method_params_trailing_comma_multiple()
        => ExecutionTest("gen-method/params-trailing-comma-multiple");

    [Fact(DisplayName = "language/expressions/class/gen-method/params-trailing-comma-single.js")]
    public Task test_gen_method_params_trailing_comma_single()
        => ExecutionTest("gen-method/params-trailing-comma-single");

    [Fact(DisplayName = "language/expressions/class/heritage-async-arrow-function.js")]
    public Task test_heritage_async_arrow_function()
        => ExecutionTest("heritage-async-arrow-function");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-break-escaped.js")]
    public Task test_ident_name_method_def_break_escaped()
        => ExecutionTest("ident-name-method-def-break-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-case-escaped.js")]
    public Task test_ident_name_method_def_case_escaped()
        => ExecutionTest("ident-name-method-def-case-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-catch-escaped.js")]
    public Task test_ident_name_method_def_catch_escaped()
        => ExecutionTest("ident-name-method-def-catch-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-class-escaped.js")]
    public Task test_ident_name_method_def_class_escaped()
        => ExecutionTest("ident-name-method-def-class-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-const-escaped.js")]
    public Task test_ident_name_method_def_const_escaped()
        => ExecutionTest("ident-name-method-def-const-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-continue-escaped.js")]
    public Task test_ident_name_method_def_continue_escaped()
        => ExecutionTest("ident-name-method-def-continue-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-debugger-escaped.js")]
    public Task test_ident_name_method_def_debugger_escaped()
        => ExecutionTest("ident-name-method-def-debugger-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-default-escaped-ext.js")]
    public Task test_ident_name_method_def_default_escaped_ext()
        => ExecutionTest("ident-name-method-def-default-escaped-ext");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-default-escaped.js")]
    public Task test_ident_name_method_def_default_escaped()
        => ExecutionTest("ident-name-method-def-default-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-default.js")]
    public Task test_ident_name_method_def_default()
        => ExecutionTest("ident-name-method-def-default");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-delete-escaped.js")]
    public Task test_ident_name_method_def_delete_escaped()
        => ExecutionTest("ident-name-method-def-delete-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-do-escaped.js")]
    public Task test_ident_name_method_def_do_escaped()
        => ExecutionTest("ident-name-method-def-do-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-else-escaped.js")]
    public Task test_ident_name_method_def_else_escaped()
        => ExecutionTest("ident-name-method-def-else-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-enum-escaped.js")]
    public Task test_ident_name_method_def_enum_escaped()
        => ExecutionTest("ident-name-method-def-enum-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-export-escaped.js")]
    public Task test_ident_name_method_def_export_escaped()
        => ExecutionTest("ident-name-method-def-export-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-extends-escaped-ext.js")]
    public Task test_ident_name_method_def_extends_escaped_ext()
        => ExecutionTest("ident-name-method-def-extends-escaped-ext");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-extends-escaped.js")]
    public Task test_ident_name_method_def_extends_escaped()
        => ExecutionTest("ident-name-method-def-extends-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-extends.js")]
    public Task test_ident_name_method_def_extends()
        => ExecutionTest("ident-name-method-def-extends");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-finally-escaped.js")]
    public Task test_ident_name_method_def_finally_escaped()
        => ExecutionTest("ident-name-method-def-finally-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-for-escaped.js")]
    public Task test_ident_name_method_def_for_escaped()
        => ExecutionTest("ident-name-method-def-for-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-function-escaped.js")]
    public Task test_ident_name_method_def_function_escaped()
        => ExecutionTest("ident-name-method-def-function-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-if-escaped.js")]
    public Task test_ident_name_method_def_if_escaped()
        => ExecutionTest("ident-name-method-def-if-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-implements-escaped.js")]
    public Task test_ident_name_method_def_implements_escaped()
        => ExecutionTest("ident-name-method-def-implements-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-import-escaped.js")]
    public Task test_ident_name_method_def_import_escaped()
        => ExecutionTest("ident-name-method-def-import-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-in-escaped.js")]
    public Task test_ident_name_method_def_in_escaped()
        => ExecutionTest("ident-name-method-def-in-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-instanceof-escaped.js")]
    public Task test_ident_name_method_def_instanceof_escaped()
        => ExecutionTest("ident-name-method-def-instanceof-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-interface-escaped.js")]
    public Task test_ident_name_method_def_interface_escaped()
        => ExecutionTest("ident-name-method-def-interface-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-let-escaped.js")]
    public Task test_ident_name_method_def_let_escaped()
        => ExecutionTest("ident-name-method-def-let-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-new-escaped.js")]
    public Task test_ident_name_method_def_new_escaped()
        => ExecutionTest("ident-name-method-def-new-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-package-escaped.js")]
    public Task test_ident_name_method_def_package_escaped()
        => ExecutionTest("ident-name-method-def-package-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-private-escaped.js")]
    public Task test_ident_name_method_def_private_escaped()
        => ExecutionTest("ident-name-method-def-private-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-protected-escaped.js")]
    public Task test_ident_name_method_def_protected_escaped()
        => ExecutionTest("ident-name-method-def-protected-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-public-escaped.js")]
    public Task test_ident_name_method_def_public_escaped()
        => ExecutionTest("ident-name-method-def-public-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-return-escaped.js")]
    public Task test_ident_name_method_def_return_escaped()
        => ExecutionTest("ident-name-method-def-return-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-static-escaped.js")]
    public Task test_ident_name_method_def_static_escaped()
        => ExecutionTest("ident-name-method-def-static-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-super-escaped.js")]
    public Task test_ident_name_method_def_super_escaped()
        => ExecutionTest("ident-name-method-def-super-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-switch-escaped.js")]
    public Task test_ident_name_method_def_switch_escaped()
        => ExecutionTest("ident-name-method-def-switch-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-this-escaped.js")]
    public Task test_ident_name_method_def_this_escaped()
        => ExecutionTest("ident-name-method-def-this-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-throw-escaped.js")]
    public Task test_ident_name_method_def_throw_escaped()
        => ExecutionTest("ident-name-method-def-throw-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-try-escaped.js")]
    public Task test_ident_name_method_def_try_escaped()
        => ExecutionTest("ident-name-method-def-try-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-typeof-escaped.js")]
    public Task test_ident_name_method_def_typeof_escaped()
        => ExecutionTest("ident-name-method-def-typeof-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-var-escaped.js")]
    public Task test_ident_name_method_def_var_escaped()
        => ExecutionTest("ident-name-method-def-var-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-void-escaped.js")]
    public Task test_ident_name_method_def_void_escaped()
        => ExecutionTest("ident-name-method-def-void-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-while-escaped.js")]
    public Task test_ident_name_method_def_while_escaped()
        => ExecutionTest("ident-name-method-def-while-escaped");

    [Fact(DisplayName = "language/expressions/class/ident-name-method-def-with-escaped.js")]
    public Task test_ident_name_method_def_with_escaped()
        => ExecutionTest("ident-name-method-def-with-escaped");

    [Fact(DisplayName = "language/expressions/class/method-length-dflt.js")]
    public Task test_method_length_dflt()
        => ExecutionTest("method-length-dflt");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-abrupt.js")]
    public Task test_method_static_dflt_params_abrupt()
        => ExecutionTest("method-static/dflt-params-abrupt");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-arg-val-not-undefined.js")]
    public Task test_method_static_dflt_params_arg_val_not_undefined()
        => ExecutionTest("method-static/dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-arg-val-undefined.js")]
    public Task test_method_static_dflt_params_arg_val_undefined()
        => ExecutionTest("method-static/dflt-params-arg-val-undefined");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-ref-later.js")]
    public Task test_method_static_dflt_params_ref_later()
        => ExecutionTest("method-static/dflt-params-ref-later");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-ref-prior.js")]
    public Task test_method_static_dflt_params_ref_prior()
        => ExecutionTest("method-static/dflt-params-ref-prior");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-ref-self.js")]
    public Task test_method_static_dflt_params_ref_self()
        => ExecutionTest("method-static/dflt-params-ref-self");

    [Fact(DisplayName = "language/expressions/class/method-static/dflt-params-trailing-comma.js")]
    public Task test_method_static_dflt_params_trailing_comma()
        => ExecutionTest("method-static/dflt-params-trailing-comma");

    [Fact(DisplayName = "language/expressions/class/method-static/forbidden-ext/b2/cls-expr-meth-static-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_method_static_forbidden_ext_b2_cls_expr_meth_static_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("method-static/forbidden-ext/b2/cls-expr-meth-static-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "language/expressions/class/method-static/forbidden-ext/b2/cls-expr-meth-static-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_method_static_forbidden_ext_b2_cls_expr_meth_static_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("method-static/forbidden-ext/b2/cls-expr-meth-static-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "language/expressions/class/method-static/forbidden-ext/b2/cls-expr-meth-static-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_method_static_forbidden_ext_b2_cls_expr_meth_static_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("method-static/forbidden-ext/b2/cls-expr-meth-static-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "language/expressions/class/method-static/params-trailing-comma-multiple.js")]
    public Task test_method_static_params_trailing_comma_multiple()
        => ExecutionTest("method-static/params-trailing-comma-multiple");

    [Fact(DisplayName = "language/expressions/class/method-static/params-trailing-comma-single.js")]
    public Task test_method_static_params_trailing_comma_single()
        => ExecutionTest("method-static/params-trailing-comma-single");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-abrupt.js")]
    public Task test_method_dflt_params_abrupt()
        => ExecutionTest("method/dflt-params-abrupt");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-arg-val-not-undefined.js")]
    public Task test_method_dflt_params_arg_val_not_undefined()
        => ExecutionTest("method/dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-arg-val-undefined.js")]
    public Task test_method_dflt_params_arg_val_undefined()
        => ExecutionTest("method/dflt-params-arg-val-undefined");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-ref-later.js")]
    public Task test_method_dflt_params_ref_later()
        => ExecutionTest("method/dflt-params-ref-later");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-ref-prior.js")]
    public Task test_method_dflt_params_ref_prior()
        => ExecutionTest("method/dflt-params-ref-prior");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-ref-self.js")]
    public Task test_method_dflt_params_ref_self()
        => ExecutionTest("method/dflt-params-ref-self");

    [Fact(DisplayName = "language/expressions/class/method/dflt-params-trailing-comma.js")]
    public Task test_method_dflt_params_trailing_comma()
        => ExecutionTest("method/dflt-params-trailing-comma");

    [Fact(DisplayName = "language/expressions/class/method/forbidden-ext/b2/cls-expr-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_method_forbidden_ext_b2_cls_expr_meth_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("method/forbidden-ext/b2/cls-expr-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "language/expressions/class/method/forbidden-ext/b2/cls-expr-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_method_forbidden_ext_b2_cls_expr_meth_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("method/forbidden-ext/b2/cls-expr-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "language/expressions/class/method/forbidden-ext/b2/cls-expr-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_method_forbidden_ext_b2_cls_expr_meth_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("method/forbidden-ext/b2/cls-expr-meth-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "language/expressions/class/method/params-trailing-comma-multiple.js")]
    public Task test_method_params_trailing_comma_multiple()
        => ExecutionTest("method/params-trailing-comma-multiple");

    [Fact(DisplayName = "language/expressions/class/method/params-trailing-comma-single.js")]
    public Task test_method_params_trailing_comma_single()
        => ExecutionTest("method/params-trailing-comma-single");

    [Fact(DisplayName = "language/expressions/class/params-dflt-gen-meth-args-unmapped.js")]
    public Task test_params_dflt_gen_meth_args_unmapped()
        => ExecutionTest("params-dflt-gen-meth-args-unmapped");

    [Fact(DisplayName = "language/expressions/class/params-dflt-gen-meth-ref-arguments.js")]
    public Task test_params_dflt_gen_meth_ref_arguments()
        => ExecutionTest("params-dflt-gen-meth-ref-arguments");

    [Fact(DisplayName = "language/expressions/class/params-dflt-gen-meth-static-args-unmapped.js")]
    public Task test_params_dflt_gen_meth_static_args_unmapped()
        => ExecutionTest("params-dflt-gen-meth-static-args-unmapped");

    [Fact(DisplayName = "language/expressions/class/params-dflt-gen-meth-static-ref-arguments.js")]
    public Task test_params_dflt_gen_meth_static_ref_arguments()
        => ExecutionTest("params-dflt-gen-meth-static-ref-arguments");

    [Fact(DisplayName = "language/expressions/class/params-dflt-meth-args-unmapped.js")]
    public Task test_params_dflt_meth_args_unmapped()
        => ExecutionTest("params-dflt-meth-args-unmapped");

    [Fact(DisplayName = "language/expressions/class/params-dflt-meth-ref-arguments.js")]
    public Task test_params_dflt_meth_ref_arguments()
        => ExecutionTest("params-dflt-meth-ref-arguments");

    [Fact(DisplayName = "language/expressions/class/params-dflt-meth-static-args-unmapped.js")]
    public Task test_params_dflt_meth_static_args_unmapped()
        => ExecutionTest("params-dflt-meth-static-args-unmapped");

    [Fact(DisplayName = "language/expressions/class/params-dflt-meth-static-ref-arguments.js")]
    public Task test_params_dflt_meth_static_ref_arguments()
        => ExecutionTest("params-dflt-meth-static-ref-arguments");

    [Fact(DisplayName = "language/expressions/class/private-static-getter-multiple-evaluations-of-class-factory.js")]
    public Task test_private_static_getter_multiple_evaluations_of_class_factory()
        => ExecutionTest("private-static-getter-multiple-evaluations-of-class-factory");

    [Fact(DisplayName = "language/expressions/class/private-static-method-brand-check-multiple-evaluations-of-class-factory.js")]
    public Task test_private_static_method_brand_check_multiple_evaluations_of_class_factory()
        => ExecutionTest("private-static-method-brand-check-multiple-evaluations-of-class-factory");

    [Fact(DisplayName = "language/expressions/class/scope-gen-meth-paramsbody-var-close.js")]
    public Task test_scope_gen_meth_paramsbody_var_close()
        => ExecutionTest("scope-gen-meth-paramsbody-var-close");

    [Fact(DisplayName = "language/expressions/class/scope-gen-meth-paramsbody-var-open.js")]
    public Task test_scope_gen_meth_paramsbody_var_open()
        => ExecutionTest("scope-gen-meth-paramsbody-var-open");

    [Fact(DisplayName = "language/expressions/class/scope-static-gen-meth-paramsbody-var-close.js")]
    public Task test_scope_static_gen_meth_paramsbody_var_close()
        => ExecutionTest("scope-static-gen-meth-paramsbody-var-close");

    [Fact(DisplayName = "language/expressions/class/scope-static-gen-meth-paramsbody-var-open.js")]
    public Task test_scope_static_gen_meth_paramsbody_var_open()
        => ExecutionTest("scope-static-gen-meth-paramsbody-var-open");

    [Fact(DisplayName = "language/expressions/class/scope-static-setter-paramsbody-var-close.js")]
    public Task test_scope_static_setter_paramsbody_var_close()
        => ExecutionTest("scope-static-setter-paramsbody-var-close");

    [Fact(DisplayName = "language/expressions/class/scope-static-setter-paramsbody-var-open.js")]
    public Task test_scope_static_setter_paramsbody_var_open()
        => ExecutionTest("scope-static-setter-paramsbody-var-open");

    [Fact(DisplayName = "language/expressions/class/setter-length-dflt.js")]
    public Task test_setter_length_dflt()
        => ExecutionTest("setter-length-dflt");

    [Fact(DisplayName = "language/expressions/class/static-method-length-dflt.js")]
    public Task test_static_method_length_dflt()
        => ExecutionTest("static-method-length-dflt");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-AggregateError.js")]
    public Task test_subclass_builtins_subclass_AggregateError()
        => ExecutionTest("subclass-builtins/subclass-AggregateError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Array.js")]
    public Task test_subclass_builtins_subclass_Array()
        => ExecutionTest("subclass-builtins/subclass-Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-ArrayBuffer.js")]
    public Task test_subclass_builtins_subclass_ArrayBuffer()
        => ExecutionTest("subclass-builtins/subclass-ArrayBuffer");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-BigInt64Array.js")]
    public Task test_subclass_builtins_subclass_BigInt64Array()
        => ExecutionTest("subclass-builtins/subclass-BigInt64Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-BigUint64Array.js")]
    public Task test_subclass_builtins_subclass_BigUint64Array()
        => ExecutionTest("subclass-builtins/subclass-BigUint64Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Boolean.js")]
    public Task test_subclass_builtins_subclass_Boolean()
        => ExecutionTest("subclass-builtins/subclass-Boolean");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-DataView.js")]
    public Task test_subclass_builtins_subclass_DataView()
        => ExecutionTest("subclass-builtins/subclass-DataView");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Date.js")]
    public Task test_subclass_builtins_subclass_Date()
        => ExecutionTest("subclass-builtins/subclass-Date");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Error.js")]
    public Task test_subclass_builtins_subclass_Error()
        => ExecutionTest("subclass-builtins/subclass-Error");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-EvalError.js")]
    public Task test_subclass_builtins_subclass_EvalError()
        => ExecutionTest("subclass-builtins/subclass-EvalError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Float32Array.js")]
    public Task test_subclass_builtins_subclass_Float32Array()
        => ExecutionTest("subclass-builtins/subclass-Float32Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Float64Array.js")]
    public Task test_subclass_builtins_subclass_Float64Array()
        => ExecutionTest("subclass-builtins/subclass-Float64Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Int16Array.js")]
    public Task test_subclass_builtins_subclass_Int16Array()
        => ExecutionTest("subclass-builtins/subclass-Int16Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Int32Array.js")]
    public Task test_subclass_builtins_subclass_Int32Array()
        => ExecutionTest("subclass-builtins/subclass-Int32Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Int8Array.js")]
    public Task test_subclass_builtins_subclass_Int8Array()
        => ExecutionTest("subclass-builtins/subclass-Int8Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Map.js")]
    public Task test_subclass_builtins_subclass_Map()
        => ExecutionTest("subclass-builtins/subclass-Map");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Number.js")]
    public Task test_subclass_builtins_subclass_Number()
        => ExecutionTest("subclass-builtins/subclass-Number");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Object.js")]
    public Task test_subclass_builtins_subclass_Object()
        => ExecutionTest("subclass-builtins/subclass-Object");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Promise.js")]
    public Task test_subclass_builtins_subclass_Promise()
        => ExecutionTest("subclass-builtins/subclass-Promise");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-RangeError.js")]
    public Task test_subclass_builtins_subclass_RangeError()
        => ExecutionTest("subclass-builtins/subclass-RangeError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-ReferenceError.js")]
    public Task test_subclass_builtins_subclass_ReferenceError()
        => ExecutionTest("subclass-builtins/subclass-ReferenceError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-RegExp.js")]
    public Task test_subclass_builtins_subclass_RegExp()
        => ExecutionTest("subclass-builtins/subclass-RegExp");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Set.js")]
    public Task test_subclass_builtins_subclass_Set()
        => ExecutionTest("subclass-builtins/subclass-Set");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-SharedArrayBuffer.js")]
    public Task test_subclass_builtins_subclass_SharedArrayBuffer()
        => ExecutionTest("subclass-builtins/subclass-SharedArrayBuffer");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-String.js")]
    public Task test_subclass_builtins_subclass_String()
        => ExecutionTest("subclass-builtins/subclass-String");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-SyntaxError.js")]
    public Task test_subclass_builtins_subclass_SyntaxError()
        => ExecutionTest("subclass-builtins/subclass-SyntaxError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-TypeError.js")]
    public Task test_subclass_builtins_subclass_TypeError()
        => ExecutionTest("subclass-builtins/subclass-TypeError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-URIError.js")]
    public Task test_subclass_builtins_subclass_URIError()
        => ExecutionTest("subclass-builtins/subclass-URIError");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Uint16Array.js")]
    public Task test_subclass_builtins_subclass_Uint16Array()
        => ExecutionTest("subclass-builtins/subclass-Uint16Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Uint32Array.js")]
    public Task test_subclass_builtins_subclass_Uint32Array()
        => ExecutionTest("subclass-builtins/subclass-Uint32Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Uint8Array.js")]
    public Task test_subclass_builtins_subclass_Uint8Array()
        => ExecutionTest("subclass-builtins/subclass-Uint8Array");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-Uint8ClampedArray.js")]
    public Task test_subclass_builtins_subclass_Uint8ClampedArray()
        => ExecutionTest("subclass-builtins/subclass-Uint8ClampedArray");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-WeakMap.js")]
    public Task test_subclass_builtins_subclass_WeakMap()
        => ExecutionTest("subclass-builtins/subclass-WeakMap");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-WeakRef.js")]
    public Task test_subclass_builtins_subclass_WeakRef()
        => ExecutionTest("subclass-builtins/subclass-WeakRef");

    [Fact(DisplayName = "language/expressions/class/subclass-builtins/subclass-WeakSet.js")]
    public Task test_subclass_builtins_subclass_WeakSet()
        => ExecutionTest("subclass-builtins/subclass-WeakSet");
}
