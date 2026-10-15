namespace Jroc.Test262.Tests.language.expressions.object_;

public sealed class ObjectExpressionCompletionTests : DiskExecutionTestsBase
{
    public ObjectExpressionCompletionTests() : base("language.expressions.object_") { }

    [Fact(DisplayName = "computed-property-name-topropertykey-before-value-evaluation.js")]
    public Task test_computed_property_name_topropertykey_before_value_evaluation() => ExecutionTest("computed-property-name-topropertykey-before-value-evaluation");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task test_cpn_obj_lit_computed_property_name_from_assignment_expression_logical_and() => ExecutionTest("cpn-obj-lit-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task test_cpn_obj_lit_computed_property_name_from_assignment_expression_logical_or() => ExecutionTest("cpn-obj-lit-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-await-expression.js")]
    public Task test_cpn_obj_lit_computed_property_name_from_await_expression() => ExecutionTest("cpn-obj-lit-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-generator-function-declaration.js")]
    public Task test_cpn_obj_lit_computed_property_name_from_generator_function_declaration() => ExecutionTest("cpn-obj-lit-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "fn-name-class.js")]
    public Task test_fn_name_class() => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover.js")]
    public Task test_fn_name_cover() => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "identifier-shorthand-static-init-await-valid.js")]
    public Task test_identifier_shorthand_static_init_await_valid() => ExecutionTest("identifier-shorthand-static-init-await-valid");

    [Fact(DisplayName = "object-spread-proxy-get-not-called-on-dontenum-keys.js")]
    public Task test_object_spread_proxy_get_not_called_on_dontenum_keys() => ExecutionTest("object-spread-proxy-get-not-called-on-dontenum-keys");

    [Fact(DisplayName = "object-spread-proxy-no-excluded-keys.js")]
    public Task test_object_spread_proxy_no_excluded_keys() => ExecutionTest("object-spread-proxy-no-excluded-keys");

    [Fact(DisplayName = "object-spread-proxy-ownkeys-returned-keys-order.js")]
    public Task test_object_spread_proxy_ownkeys_returned_keys_order() => ExecutionTest("object-spread-proxy-ownkeys-returned-keys-order");

    [Fact(DisplayName = "prop-def-id-eval-error-2.js")]
    public Task test_prop_def_id_eval_error_2() => ExecutionTest("prop-def-id-eval-error-2");

    [Fact(DisplayName = "prop-def-id-eval-error.js")]
    public Task test_prop_def_id_eval_error() => ExecutionTest("prop-def-id-eval-error");

    [Fact(DisplayName = "prop-def-id-get-error.js")]
    public Task test_prop_def_id_get_error() => ExecutionTest("prop-def-id-get-error");

    [Fact(DisplayName = "prop-def-invalid-async-prefix.js")]
    public Task test_prop_def_invalid_async_prefix() => CompilationFailureTest("prop-def-invalid-async-prefix");

    [Fact(DisplayName = "prop-def-invalid-star-prefix.js")]
    public Task test_prop_def_invalid_star_prefix() => CompilationFailureTest("prop-def-invalid-star-prefix");

    [Fact(DisplayName = "scope-gen-meth-body-lex-distinct.js", Skip = "eval is not supported.")]
    public Task test_scope_gen_meth_body_lex_distinct() => ExecutionTest("scope-gen-meth-body-lex-distinct");

    [Fact(DisplayName = "scope-gen-meth-param-elem-var-close.js", Skip = "eval is not supported.")]
    public Task test_scope_gen_meth_param_elem_var_close() => ExecutionTest("scope-gen-meth-param-elem-var-close");

    [Fact(DisplayName = "scope-gen-meth-param-elem-var-open.js", Skip = "eval is not supported.")]
    public Task test_scope_gen_meth_param_elem_var_open() => ExecutionTest("scope-gen-meth-param-elem-var-open");

    [Fact(DisplayName = "scope-gen-meth-param-rest-elem-var-close.js", Skip = "eval is not supported.")]
    public Task test_scope_gen_meth_param_rest_elem_var_close() => ExecutionTest("scope-gen-meth-param-rest-elem-var-close");

    [Fact(DisplayName = "scope-gen-meth-param-rest-elem-var-open.js", Skip = "eval is not supported.")]
    public Task test_scope_gen_meth_param_rest_elem_var_open() => ExecutionTest("scope-gen-meth-param-rest-elem-var-open");

    [Fact(DisplayName = "scope-getter-body-lex-distinc.js", Skip = "eval is not supported.")]
    public Task test_scope_getter_body_lex_distinc() => ExecutionTest("scope-getter-body-lex-distinc");

    [Fact(DisplayName = "scope-meth-body-lex-distinct.js", Skip = "eval is not supported.")]
    public Task test_scope_meth_body_lex_distinct() => ExecutionTest("scope-meth-body-lex-distinct");

    [Fact(DisplayName = "scope-meth-param-elem-var-close.js", Skip = "eval is not supported.")]
    public Task test_scope_meth_param_elem_var_close() => ExecutionTest("scope-meth-param-elem-var-close");

    [Fact(DisplayName = "scope-meth-param-elem-var-open.js", Skip = "eval is not supported.")]
    public Task test_scope_meth_param_elem_var_open() => ExecutionTest("scope-meth-param-elem-var-open");

    [Fact(DisplayName = "scope-meth-param-rest-elem-var-close.js", Skip = "eval is not supported.")]
    public Task test_scope_meth_param_rest_elem_var_close() => ExecutionTest("scope-meth-param-rest-elem-var-close");

    [Fact(DisplayName = "scope-meth-param-rest-elem-var-open.js", Skip = "eval is not supported.")]
    public Task test_scope_meth_param_rest_elem_var_open() => ExecutionTest("scope-meth-param-rest-elem-var-open");

    [Fact(DisplayName = "scope-setter-body-lex-distinc.js", Skip = "eval is not supported.")]
    public Task test_scope_setter_body_lex_distinc() => ExecutionTest("scope-setter-body-lex-distinc");

    [Fact(DisplayName = "setter-body-strict-inside.js")]
    public Task test_setter_body_strict_inside() => CompilationFailureTest("setter-body-strict-inside");

    [Fact(DisplayName = "setter-body-strict-outside.js")]
    public Task test_setter_body_strict_outside() => CompilationFailureTest("setter-body-strict-outside");

    [Fact(DisplayName = "setter-param-arguments-strict-inside.js")]
    public Task test_setter_param_arguments_strict_inside() => CompilationFailureTest("setter-param-arguments-strict-inside");

    [Fact(DisplayName = "setter-param-arguments-strict-outside.js")]
    public Task test_setter_param_arguments_strict_outside() => CompilationFailureTest("setter-param-arguments-strict-outside");

    [Fact(DisplayName = "setter-param-eval-strict-inside.js")]
    public Task test_setter_param_eval_strict_inside() => CompilationFailureTest("setter-param-eval-strict-inside");

    [Fact(DisplayName = "setter-param-eval-strict-outside.js")]
    public Task test_setter_param_eval_strict_outside() => CompilationFailureTest("setter-param-eval-strict-outside");

    [Fact(DisplayName = "setter-super-prop.js")]
    public Task test_setter_super_prop() => ExecutionTest("setter-super-prop");
}
