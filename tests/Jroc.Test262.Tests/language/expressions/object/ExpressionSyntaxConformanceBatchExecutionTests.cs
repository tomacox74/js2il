using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions._object;

public class ExpressionSyntaxConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchExecutionTests() : base("language/expressions/object", "language.expressions.object") { }

    [Fact(DisplayName = "ident-name-method-def-implements-escaped.js")]
    public Task ident_name_method_def_implements_escaped()
        => ExecutionTest("ident-name-method-def-implements-escaped");

    [Fact(DisplayName = "ident-name-method-def-import-escaped.js")]
    public Task ident_name_method_def_import_escaped()
        => ExecutionTest("ident-name-method-def-import-escaped");

    [Fact(DisplayName = "ident-name-method-def-in-escaped.js")]
    public Task ident_name_method_def_in_escaped()
        => ExecutionTest("ident-name-method-def-in-escaped");

    [Fact(DisplayName = "ident-name-method-def-instanceof-escaped.js")]
    public Task ident_name_method_def_instanceof_escaped()
        => ExecutionTest("ident-name-method-def-instanceof-escaped");

    [Fact(DisplayName = "ident-name-method-def-interface-escaped.js")]
    public Task ident_name_method_def_interface_escaped()
        => ExecutionTest("ident-name-method-def-interface-escaped");

    [Fact(DisplayName = "ident-name-method-def-let-escaped.js")]
    public Task ident_name_method_def_let_escaped()
        => ExecutionTest("ident-name-method-def-let-escaped");

    [Fact(DisplayName = "ident-name-method-def-new-escaped.js")]
    public Task ident_name_method_def_new_escaped()
        => ExecutionTest("ident-name-method-def-new-escaped");

    [Fact(DisplayName = "ident-name-method-def-package-escaped.js")]
    public Task ident_name_method_def_package_escaped()
        => ExecutionTest("ident-name-method-def-package-escaped");

    [Fact(DisplayName = "ident-name-method-def-private-escaped.js")]
    public Task ident_name_method_def_private_escaped()
        => ExecutionTest("ident-name-method-def-private-escaped");

    [Fact(DisplayName = "ident-name-method-def-protected-escaped.js")]
    public Task ident_name_method_def_protected_escaped()
        => ExecutionTest("ident-name-method-def-protected-escaped");

    [Fact(DisplayName = "ident-name-method-def-public-escaped.js")]
    public Task ident_name_method_def_public_escaped()
        => ExecutionTest("ident-name-method-def-public-escaped");

    [Fact(DisplayName = "ident-name-method-def-return-escaped.js")]
    public Task ident_name_method_def_return_escaped()
        => ExecutionTest("ident-name-method-def-return-escaped");

    [Fact(DisplayName = "ident-name-method-def-static-escaped.js")]
    public Task ident_name_method_def_static_escaped()
        => ExecutionTest("ident-name-method-def-static-escaped");

    [Fact(DisplayName = "ident-name-method-def-super-escaped.js")]
    public Task ident_name_method_def_super_escaped()
        => ExecutionTest("ident-name-method-def-super-escaped");

    [Fact(DisplayName = "ident-name-method-def-switch-escaped.js")]
    public Task ident_name_method_def_switch_escaped()
        => ExecutionTest("ident-name-method-def-switch-escaped");

    [Fact(DisplayName = "ident-name-method-def-this-escaped.js")]
    public Task ident_name_method_def_this_escaped()
        => ExecutionTest("ident-name-method-def-this-escaped");

    [Fact(DisplayName = "ident-name-method-def-throw-escaped.js")]
    public Task ident_name_method_def_throw_escaped()
        => ExecutionTest("ident-name-method-def-throw-escaped");

    [Fact(DisplayName = "ident-name-method-def-try-escaped.js")]
    public Task ident_name_method_def_try_escaped()
        => ExecutionTest("ident-name-method-def-try-escaped");

    [Fact(DisplayName = "ident-name-method-def-typeof-escaped.js")]
    public Task ident_name_method_def_typeof_escaped()
        => ExecutionTest("ident-name-method-def-typeof-escaped");

    [Fact(DisplayName = "ident-name-method-def-var-escaped.js")]
    public Task ident_name_method_def_var_escaped()
        => ExecutionTest("ident-name-method-def-var-escaped");

    [Fact(DisplayName = "ident-name-method-def-void-escaped.js")]
    public Task ident_name_method_def_void_escaped()
        => ExecutionTest("ident-name-method-def-void-escaped");

    [Fact(DisplayName = "ident-name-method-def-while-escaped.js")]
    public Task ident_name_method_def_while_escaped()
        => ExecutionTest("ident-name-method-def-while-escaped");

    [Fact(DisplayName = "ident-name-method-def-with-escaped.js")]
    public Task ident_name_method_def_with_escaped()
        => ExecutionTest("ident-name-method-def-with-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-await-static-init.js")]
    public Task ident_name_prop_name_literal_await_static_init()
        => ExecutionTest("ident-name-prop-name-literal-await-static-init");

    [Fact(DisplayName = "ident-name-prop-name-literal-break-escaped.js")]
    public Task ident_name_prop_name_literal_break_escaped()
        => ExecutionTest("ident-name-prop-name-literal-break-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-case-escaped.js")]
    public Task ident_name_prop_name_literal_case_escaped()
        => ExecutionTest("ident-name-prop-name-literal-case-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-catch-escaped.js")]
    public Task ident_name_prop_name_literal_catch_escaped()
        => ExecutionTest("ident-name-prop-name-literal-catch-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-class-escaped.js")]
    public Task ident_name_prop_name_literal_class_escaped()
        => ExecutionTest("ident-name-prop-name-literal-class-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-const-escaped.js")]
    public Task ident_name_prop_name_literal_const_escaped()
        => ExecutionTest("ident-name-prop-name-literal-const-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-continue-escaped.js")]
    public Task ident_name_prop_name_literal_continue_escaped()
        => ExecutionTest("ident-name-prop-name-literal-continue-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-debugger-escaped.js")]
    public Task ident_name_prop_name_literal_debugger_escaped()
        => ExecutionTest("ident-name-prop-name-literal-debugger-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-default-escaped-ext.js")]
    public Task ident_name_prop_name_literal_default_escaped_ext()
        => ExecutionTest("ident-name-prop-name-literal-default-escaped-ext");

    [Fact(DisplayName = "ident-name-prop-name-literal-default-escaped.js")]
    public Task ident_name_prop_name_literal_default_escaped()
        => ExecutionTest("ident-name-prop-name-literal-default-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-default.js")]
    public Task ident_name_prop_name_literal_default()
        => ExecutionTest("ident-name-prop-name-literal-default");

    [Fact(DisplayName = "ident-name-prop-name-literal-delete-escaped.js")]
    public Task ident_name_prop_name_literal_delete_escaped()
        => ExecutionTest("ident-name-prop-name-literal-delete-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-do-escaped.js")]
    public Task ident_name_prop_name_literal_do_escaped()
        => ExecutionTest("ident-name-prop-name-literal-do-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-else-escaped.js")]
    public Task ident_name_prop_name_literal_else_escaped()
        => ExecutionTest("ident-name-prop-name-literal-else-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-enum-escaped.js")]
    public Task ident_name_prop_name_literal_enum_escaped()
        => ExecutionTest("ident-name-prop-name-literal-enum-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-export-escaped.js")]
    public Task ident_name_prop_name_literal_export_escaped()
        => ExecutionTest("ident-name-prop-name-literal-export-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-extends-escaped-ext.js")]
    public Task ident_name_prop_name_literal_extends_escaped_ext()
        => ExecutionTest("ident-name-prop-name-literal-extends-escaped-ext");

    [Fact(DisplayName = "ident-name-prop-name-literal-extends-escaped.js")]
    public Task ident_name_prop_name_literal_extends_escaped()
        => ExecutionTest("ident-name-prop-name-literal-extends-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-extends.js")]
    public Task ident_name_prop_name_literal_extends()
        => ExecutionTest("ident-name-prop-name-literal-extends");

    [Fact(DisplayName = "ident-name-prop-name-literal-finally-escaped.js")]
    public Task ident_name_prop_name_literal_finally_escaped()
        => ExecutionTest("ident-name-prop-name-literal-finally-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-for-escaped.js")]
    public Task ident_name_prop_name_literal_for_escaped()
        => ExecutionTest("ident-name-prop-name-literal-for-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-function-escaped.js")]
    public Task ident_name_prop_name_literal_function_escaped()
        => ExecutionTest("ident-name-prop-name-literal-function-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-if-escaped.js")]
    public Task ident_name_prop_name_literal_if_escaped()
        => ExecutionTest("ident-name-prop-name-literal-if-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-implements-escaped.js")]
    public Task ident_name_prop_name_literal_implements_escaped()
        => ExecutionTest("ident-name-prop-name-literal-implements-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-import-escaped.js")]
    public Task ident_name_prop_name_literal_import_escaped()
        => ExecutionTest("ident-name-prop-name-literal-import-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-in-escaped.js")]
    public Task ident_name_prop_name_literal_in_escaped()
        => ExecutionTest("ident-name-prop-name-literal-in-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-instanceof-escaped.js")]
    public Task ident_name_prop_name_literal_instanceof_escaped()
        => ExecutionTest("ident-name-prop-name-literal-instanceof-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-interface-escaped.js")]
    public Task ident_name_prop_name_literal_interface_escaped()
        => ExecutionTest("ident-name-prop-name-literal-interface-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-let-escaped.js")]
    public Task ident_name_prop_name_literal_let_escaped()
        => ExecutionTest("ident-name-prop-name-literal-let-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-new-escaped.js")]
    public Task ident_name_prop_name_literal_new_escaped()
        => ExecutionTest("ident-name-prop-name-literal-new-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-package-escaped.js")]
    public Task ident_name_prop_name_literal_package_escaped()
        => ExecutionTest("ident-name-prop-name-literal-package-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-private-escaped.js")]
    public Task ident_name_prop_name_literal_private_escaped()
        => ExecutionTest("ident-name-prop-name-literal-private-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-protected-escaped.js")]
    public Task ident_name_prop_name_literal_protected_escaped()
        => ExecutionTest("ident-name-prop-name-literal-protected-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-public-escaped.js")]
    public Task ident_name_prop_name_literal_public_escaped()
        => ExecutionTest("ident-name-prop-name-literal-public-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-return-escaped.js")]
    public Task ident_name_prop_name_literal_return_escaped()
        => ExecutionTest("ident-name-prop-name-literal-return-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-static-escaped.js")]
    public Task ident_name_prop_name_literal_static_escaped()
        => ExecutionTest("ident-name-prop-name-literal-static-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-super-escaped.js")]
    public Task ident_name_prop_name_literal_super_escaped()
        => ExecutionTest("ident-name-prop-name-literal-super-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-switch-escaped.js")]
    public Task ident_name_prop_name_literal_switch_escaped()
        => ExecutionTest("ident-name-prop-name-literal-switch-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-this-escaped.js")]
    public Task ident_name_prop_name_literal_this_escaped()
        => ExecutionTest("ident-name-prop-name-literal-this-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-throw-escaped.js")]
    public Task ident_name_prop_name_literal_throw_escaped()
        => ExecutionTest("ident-name-prop-name-literal-throw-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-try-escaped.js")]
    public Task ident_name_prop_name_literal_try_escaped()
        => ExecutionTest("ident-name-prop-name-literal-try-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-typeof-escaped.js")]
    public Task ident_name_prop_name_literal_typeof_escaped()
        => ExecutionTest("ident-name-prop-name-literal-typeof-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-var-escaped.js")]
    public Task ident_name_prop_name_literal_var_escaped()
        => ExecutionTest("ident-name-prop-name-literal-var-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-void-escaped.js")]
    public Task ident_name_prop_name_literal_void_escaped()
        => ExecutionTest("ident-name-prop-name-literal-void-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-while-escaped.js")]
    public Task ident_name_prop_name_literal_while_escaped()
        => ExecutionTest("ident-name-prop-name-literal-while-escaped");

    [Fact(DisplayName = "ident-name-prop-name-literal-with-escaped.js")]
    public Task ident_name_prop_name_literal_with_escaped()
        => ExecutionTest("ident-name-prop-name-literal-with-escaped");

    [Fact(DisplayName = "identifier-shorthand-await-strict-mode.js")]
    public Task identifier_shorthand_await_strict_mode()
        => ExecutionTest("identifier-shorthand-await-strict-mode");

    [Fact(DisplayName = "let-non-strict-access.js")]
    public Task let_non_strict_access()
        => ExecutionTest("let-non-strict-access");

    [Fact(DisplayName = "let-non-strict-syntax.js")]
    public Task let_non_strict_syntax()
        => ExecutionTest("let-non-strict-syntax");

    [Fact(DisplayName = "async-gen-meth-dflt-params-abrupt.js")]
    public Task method_definition_async_gen_meth_dflt_params_abrupt()
        => ExecutionTest("method-definition/async-gen-meth-dflt-params-abrupt");

    [Fact(DisplayName = "async-gen-meth-dflt-params-ref-later.js")]
    public Task method_definition_async_gen_meth_dflt_params_ref_later()
        => ExecutionTest("method-definition/async-gen-meth-dflt-params-ref-later");

    [Fact(DisplayName = "async-gen-meth-dflt-params-ref-self.js")]
    public Task method_definition_async_gen_meth_dflt_params_ref_self()
        => ExecutionTest("method-definition/async-gen-meth-dflt-params-ref-self");

    [Fact(DisplayName = "gen-meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task method_definition_forbidden_ext_b1_gen_meth_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("method-definition/forbidden-ext/b1/gen-meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "gen-meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task method_definition_forbidden_ext_b1_gen_meth_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("method-definition/forbidden-ext/b1/gen-meth-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "gen-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task method_definition_forbidden_ext_b2_gen_meth_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("method-definition/forbidden-ext/b2/gen-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "gen-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task method_definition_forbidden_ext_b2_gen_meth_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("method-definition/forbidden-ext/b2/gen-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "gen-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task method_definition_forbidden_ext_b2_gen_meth_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("method-definition/forbidden-ext/b2/gen-meth-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task method_definition_forbidden_ext_b2_meth_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("method-definition/forbidden-ext/b2/meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task method_definition_forbidden_ext_b2_meth_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("method-definition/forbidden-ext/b2/meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task method_definition_forbidden_ext_b2_meth_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("method-definition/forbidden-ext/b2/meth-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "gen-meth-dflt-params-arg-val-not-undefined.js")]
    public Task method_definition_gen_meth_dflt_params_arg_val_not_undefined()
        => ExecutionTest("method-definition/gen-meth-dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "gen-meth-dflt-params-arg-val-undefined.js")]
    public Task method_definition_gen_meth_dflt_params_arg_val_undefined()
        => ExecutionTest("method-definition/gen-meth-dflt-params-arg-val-undefined");

    [Fact(DisplayName = "gen-meth-dflt-params-ref-prior.js")]
    public Task method_definition_gen_meth_dflt_params_ref_prior()
        => ExecutionTest("method-definition/gen-meth-dflt-params-ref-prior");

    [Fact(DisplayName = "gen-meth-dflt-params-trailing-comma.js")]
    public Task method_definition_gen_meth_dflt_params_trailing_comma()
        => ExecutionTest("method-definition/gen-meth-dflt-params-trailing-comma");

    [Fact(DisplayName = "gen-meth-params-trailing-comma-multiple.js")]
    public Task method_definition_gen_meth_params_trailing_comma_multiple()
        => ExecutionTest("method-definition/gen-meth-params-trailing-comma-multiple");

    [Fact(DisplayName = "gen-meth-params-trailing-comma-single.js")]
    public Task method_definition_gen_meth_params_trailing_comma_single()
        => ExecutionTest("method-definition/gen-meth-params-trailing-comma-single");

}
