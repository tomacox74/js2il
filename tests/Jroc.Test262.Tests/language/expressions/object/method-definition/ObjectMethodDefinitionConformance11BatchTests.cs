using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.object_.method_definition;

public sealed class ObjectMethodDefinitionConformance11BatchTests : DiskExecutionTestsBase
{
    public ObjectMethodDefinitionConformance11BatchTests() : base("language.expressions.object_.method_definition") { }

    [Fact(DisplayName = "gen-yield-identifier-non-strict.js")]
    public Task test_gen_yield_identifier_non_strict()
        => ExecutionTest("gen-yield-identifier-non-strict");

    [Fact(DisplayName = "gen-yield-identifier-spread-non-strict.js")]
    public Task test_gen_yield_identifier_spread_non_strict()
        => ExecutionTest("gen-yield-identifier-spread-non-strict");

    [Fact(DisplayName = "gen-yield-identifier-spread-strict.js")]
    public Task test_gen_yield_identifier_spread_strict()
        => CompilationFailureTest("gen-yield-identifier-spread-strict");

    [Fact(DisplayName = "gen-yield-identifier-strict.js")]
    public Task test_gen_yield_identifier_strict()
        => CompilationFailureTest("gen-yield-identifier-strict");

    [Fact(DisplayName = "gen-yield-spread-arr-multiple.js")]
    public Task test_gen_yield_spread_arr_multiple()
        => ExecutionTest("gen-yield-spread-arr-multiple");

    [Fact(DisplayName = "gen-yield-spread-arr-single.js")]
    public Task test_gen_yield_spread_arr_single()
        => ExecutionTest("gen-yield-spread-arr-single");

    [Fact(DisplayName = "gen-yield-spread-obj.js")]
    public Task test_gen_yield_spread_obj()
        => ExecutionTest("gen-yield-spread-obj");

    [Fact(DisplayName = "generator-length-dflt.js")]
    public Task test_generator_length_dflt()
        => ExecutionTest("generator-length-dflt");

    [Fact(DisplayName = "generator-length.js")]
    public Task test_generator_length()
        => ExecutionTest("generator-length");

    [Fact(DisplayName = "generator-name-prop-string.js")]
    public Task test_generator_name_prop_string()
        => ExecutionTest("generator-name-prop-string");

    [Fact(DisplayName = "generator-name-prop-symbol.js")]
    public Task test_generator_name_prop_symbol()
        => ExecutionTest("generator-name-prop-symbol");

    [Fact(DisplayName = "generator-no-yield.js")]
    public Task test_generator_no_yield()
        => ExecutionTest("generator-no-yield");

    [Fact(DisplayName = "generator-param-id-yield.js")]
    public Task test_generator_param_id_yield()
        => CompilationFailureTest("generator-param-id-yield");

    [Fact(DisplayName = "generator-param-init-yield.js")]
    public Task test_generator_param_init_yield()
        => CompilationFailureTest("generator-param-init-yield");

    [Fact(DisplayName = "generator-param-redecl-const.js")]
    public Task test_generator_param_redecl_const()
        => CompilationFailureTest("generator-param-redecl-const");

    [Fact(DisplayName = "generator-param-redecl-let.js")]
    public Task test_generator_param_redecl_let()
        => CompilationFailureTest("generator-param-redecl-let");

    [Fact(DisplayName = "generator-params.js")]
    public Task test_generator_params()
        => ExecutionTest("generator-params");

    [Fact(DisplayName = "generator-prop-name-eval-error.js")]
    public Task test_generator_prop_name_eval_error()
        => ExecutionTest("generator-prop-name-eval-error");

    [Fact(DisplayName = "generator-prop-name-yield-expr.js")]
    public Task test_generator_prop_name_yield_expr()
        => ExecutionTest("generator-prop-name-yield-expr");

    [Fact(DisplayName = "generator-prop-name-yield-id.js")]
    public Task test_generator_prop_name_yield_id()
        => ExecutionTest("generator-prop-name-yield-id");

    [Fact(DisplayName = "generator-property-desc.js")]
    public Task test_generator_property_desc()
        => ExecutionTest("generator-property-desc");

    [Fact(DisplayName = "generator-prototype.js")]
    public Task test_generator_prototype()
        => ExecutionTest("generator-prototype");

    [Fact(DisplayName = "generator-return.js")]
    public Task test_generator_return()
        => ExecutionTest("generator-return");

    [Fact(DisplayName = "generator-super-call-body.js")]
    public Task test_generator_super_call_body()
        => CompilationFailureTest("generator-super-call-body");

    [Fact(DisplayName = "generator-super-call-param.js")]
    public Task test_generator_super_call_param()
        => CompilationFailureTest("generator-super-call-param");

    [Fact(DisplayName = "generator-super-prop-body.js")]
    public Task test_generator_super_prop_body()
        => ExecutionTest("generator-super-prop-body");

    [Fact(DisplayName = "generator-super-prop-param.js")]
    public Task test_generator_super_prop_param()
        => ExecutionTest("generator-super-prop-param");

    [Fact(DisplayName = "generator-use-strict-with-non-simple-param.js")]
    public Task test_generator_use_strict_with_non_simple_param()
        => CompilationFailureTest("generator-use-strict-with-non-simple-param");

    [Fact(DisplayName = "meth-array-destructuring-param-strict-body.js")]
    public Task test_meth_array_destructuring_param_strict_body()
        => CompilationFailureTest("meth-array-destructuring-param-strict-body");

    [Fact(DisplayName = "meth-dflt-params-abrupt.js")]
    public Task test_meth_dflt_params_abrupt()
        => ExecutionTest("meth-dflt-params-abrupt");

    [Fact(DisplayName = "meth-dflt-params-arg-val-not-undefined.js")]
    public Task test_meth_dflt_params_arg_val_not_undefined()
        => ExecutionTest("meth-dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "meth-dflt-params-arg-val-undefined.js")]
    public Task test_meth_dflt_params_arg_val_undefined()
        => ExecutionTest("meth-dflt-params-arg-val-undefined");

    [Fact(DisplayName = "meth-dflt-params-duplicates.js")]
    public Task test_meth_dflt_params_duplicates()
        => CompilationFailureTest("meth-dflt-params-duplicates");

    [Fact(DisplayName = "meth-dflt-params-ref-later.js")]
    public Task test_meth_dflt_params_ref_later()
        => ExecutionTest("meth-dflt-params-ref-later");

    [Fact(DisplayName = "meth-dflt-params-ref-prior.js")]
    public Task test_meth_dflt_params_ref_prior()
        => ExecutionTest("meth-dflt-params-ref-prior");

    [Fact(DisplayName = "meth-dflt-params-ref-self.js")]
    public Task test_meth_dflt_params_ref_self()
        => ExecutionTest("meth-dflt-params-ref-self");

    [Fact(DisplayName = "meth-dflt-params-rest.js")]
    public Task test_meth_dflt_params_rest()
        => CompilationFailureTest("meth-dflt-params-rest");

    [Fact(DisplayName = "meth-dflt-params-trailing-comma.js")]
    public Task test_meth_dflt_params_trailing_comma()
        => ExecutionTest("meth-dflt-params-trailing-comma");

    [Fact(DisplayName = "meth-object-destructuring-param-strict-body.js")]
    public Task test_meth_object_destructuring_param_strict_body()
        => CompilationFailureTest("meth-object-destructuring-param-strict-body");

    [Fact(DisplayName = "meth-params-trailing-comma-multiple.js")]
    public Task test_meth_params_trailing_comma_multiple()
        => ExecutionTest("meth-params-trailing-comma-multiple");

    [Fact(DisplayName = "meth-params-trailing-comma-single.js")]
    public Task test_meth_params_trailing_comma_single()
        => ExecutionTest("meth-params-trailing-comma-single");

    [Fact(DisplayName = "meth-rest-param-strict-body.js")]
    public Task test_meth_rest_param_strict_body()
        => CompilationFailureTest("meth-rest-param-strict-body");

    [Fact(DisplayName = "meth-rest-params-trailing-comma-early-error.js")]
    public Task test_meth_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("meth-rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "name-length-dflt.js")]
    public Task test_name_length_dflt()
        => ExecutionTest("name-length-dflt");

    [Fact(DisplayName = "name-length.js")]
    public Task test_name_length()
        => ExecutionTest("name-length");

    [Fact(DisplayName = "name-name-prop-string.js")]
    public Task test_name_name_prop_string()
        => ExecutionTest("name-name-prop-string");

    [Fact(DisplayName = "name-name-prop-symbol.js")]
    public Task test_name_name_prop_symbol()
        => ExecutionTest("name-name-prop-symbol");

    [Fact(DisplayName = "name-param-id-yield.js")]
    public Task test_name_param_id_yield()
        => ExecutionTest("name-param-id-yield");

    [Fact(DisplayName = "name-param-init-yield.js")]
    public Task test_name_param_init_yield()
        => ExecutionTest("name-param-init-yield");

    [Fact(DisplayName = "name-param-redecl.js")]
    public Task test_name_param_redecl()
        => CompilationFailureTest("name-param-redecl");

    [Fact(DisplayName = "name-prop-name-eval-error.js")]
    public Task test_name_prop_name_eval_error()
        => ExecutionTest("name-prop-name-eval-error");

    [Fact(DisplayName = "name-prop-name-yield-expr.js")]
    public Task test_name_prop_name_yield_expr()
        => ExecutionTest("name-prop-name-yield-expr");

    [Fact(DisplayName = "name-prop-name-yield-id.js")]
    public Task test_name_prop_name_yield_id()
        => ExecutionTest("name-prop-name-yield-id");

    [Fact(DisplayName = "name-property-desc.js")]
    public Task test_name_property_desc()
        => ExecutionTest("name-property-desc");

    [Fact(DisplayName = "name-super-call-body.js")]
    public Task test_name_super_call_body()
        => CompilationFailureTest("name-super-call-body");

    [Fact(DisplayName = "name-super-call-param.js")]
    public Task test_name_super_call_param()
        => CompilationFailureTest("name-super-call-param");

    [Fact(DisplayName = "name-super-prop-body.js")]
    public Task test_name_super_prop_body()
        => ExecutionTest("name-super-prop-body");

    [Fact(DisplayName = "name-super-prop-param.js")]
    public Task test_name_super_prop_param()
        => ExecutionTest("name-super-prop-param");

    [Fact(DisplayName = "object-method-returns-promise.js")]
    public Task test_object_method_returns_promise()
        => ExecutionTest("object-method-returns-promise");

    [Fact(DisplayName = "params-dflt-gen-meth-args-unmapped.js")]
    public Task test_params_dflt_gen_meth_args_unmapped()
        => ExecutionTest("params-dflt-gen-meth-args-unmapped");

    [Fact(DisplayName = "params-dflt-gen-meth-ref-arguments.js")]
    public Task test_params_dflt_gen_meth_ref_arguments()
        => ExecutionTest("params-dflt-gen-meth-ref-arguments");

    [Fact(DisplayName = "params-dflt-meth-args-unmapped.js")]
    public Task test_params_dflt_meth_args_unmapped()
        => ExecutionTest("params-dflt-meth-args-unmapped");

    [Fact(DisplayName = "params-dflt-meth-ref-arguments.js")]
    public Task test_params_dflt_meth_ref_arguments()
        => ExecutionTest("params-dflt-meth-ref-arguments");

    [Fact(DisplayName = "private-name-early-error-async-fn-inside-class.js")]
    public Task test_private_name_early_error_async_fn_inside_class()
        => CompilationFailureTest("private-name-early-error-async-fn-inside-class");

    [Fact(DisplayName = "private-name-early-error-async-fn.js")]
    public Task test_private_name_early_error_async_fn()
        => CompilationFailureTest("private-name-early-error-async-fn");

    [Fact(DisplayName = "private-name-early-error-async-gen-inside-class.js")]
    public Task test_private_name_early_error_async_gen_inside_class()
        => CompilationFailureTest("private-name-early-error-async-gen-inside-class");

    [Fact(DisplayName = "private-name-early-error-async-gen.js")]
    public Task test_private_name_early_error_async_gen()
        => CompilationFailureTest("private-name-early-error-async-gen");

    [Fact(DisplayName = "private-name-early-error-gen-inside-class.js")]
    public Task test_private_name_early_error_gen_inside_class()
        => CompilationFailureTest("private-name-early-error-gen-inside-class");

    [Fact(DisplayName = "private-name-early-error-gen.js")]
    public Task test_private_name_early_error_gen()
        => CompilationFailureTest("private-name-early-error-gen");

    [Fact(DisplayName = "private-name-early-error-get-method-inside-class.js")]
    public Task test_private_name_early_error_get_method_inside_class()
        => CompilationFailureTest("private-name-early-error-get-method-inside-class");

    [Fact(DisplayName = "private-name-early-error-get-method.js")]
    public Task test_private_name_early_error_get_method()
        => CompilationFailureTest("private-name-early-error-get-method");

    [Fact(DisplayName = "private-name-early-error-method-inside-class.js")]
    public Task test_private_name_early_error_method_inside_class()
        => CompilationFailureTest("private-name-early-error-method-inside-class");

    [Fact(DisplayName = "private-name-early-error-method.js")]
    public Task test_private_name_early_error_method()
        => CompilationFailureTest("private-name-early-error-method");

    [Fact(DisplayName = "private-name-early-error-set-method-inside-class.js")]
    public Task test_private_name_early_error_set_method_inside_class()
        => CompilationFailureTest("private-name-early-error-set-method-inside-class");

    [Fact(DisplayName = "private-name-early-error-set-method.js")]
    public Task test_private_name_early_error_set_method()
        => CompilationFailureTest("private-name-early-error-set-method");

    [Fact(DisplayName = "setter-use-strict-with-non-simple-param.js")]
    public Task test_setter_use_strict_with_non_simple_param()
        => CompilationFailureTest("setter-use-strict-with-non-simple-param");

    [Fact(DisplayName = "use-strict-with-non-simple-param.js")]
    public Task test_use_strict_with_non_simple_param()
        => CompilationFailureTest("use-strict-with-non-simple-param");

    [Fact(DisplayName = "yield-as-expression-with-rhs.js")]
    public Task test_yield_as_expression_with_rhs()
        => ExecutionTest("yield-as-expression-with-rhs");

    [Fact(DisplayName = "yield-as-expression-without-rhs.js")]
    public Task test_yield_as_expression_without_rhs()
        => ExecutionTest("yield-as-expression-without-rhs");

    [Fact(DisplayName = "yield-as-function-expression-binding-identifier.js")]
    public Task test_yield_as_function_expression_binding_identifier()
        => ExecutionTest("yield-as-function-expression-binding-identifier");

    [Fact(DisplayName = "yield-as-generator-method-binding-identifier.js")]
    public Task test_yield_as_generator_method_binding_identifier()
        => ExecutionTest("yield-as-generator-method-binding-identifier");

    [Fact(DisplayName = "yield-as-identifier-in-nested-function.js")]
    public Task test_yield_as_identifier_in_nested_function()
        => ExecutionTest("yield-as-identifier-in-nested-function");

    [Fact(DisplayName = "yield-as-literal-property-name.js")]
    public Task test_yield_as_literal_property_name()
        => ExecutionTest("yield-as-literal-property-name");

    [Fact(DisplayName = "yield-as-logical-or-expression.js")]
    public Task test_yield_as_logical_or_expression()
        => CompilationFailureTest("yield-as-logical-or-expression");

    [Fact(DisplayName = "yield-as-parameter.js")]
    public Task test_yield_as_parameter()
        => CompilationFailureTest("yield-as-parameter");

    [Fact(DisplayName = "yield-as-property-name.js")]
    public Task test_yield_as_property_name()
        => ExecutionTest("yield-as-property-name");

    [Fact(DisplayName = "yield-as-statement.js")]
    public Task test_yield_as_statement()
        => ExecutionTest("yield-as-statement");

    [Fact(DisplayName = "yield-as-yield-operand.js")]
    public Task test_yield_as_yield_operand()
        => ExecutionTest("yield-as-yield-operand");

    [Fact(DisplayName = "yield-newline.js")]
    public Task test_yield_newline()
        => ExecutionTest("yield-newline");

    [Fact(DisplayName = "yield-return.js")]
    public Task test_yield_return()
        => ExecutionTest("yield-return");

    [Fact(DisplayName = "yield-star-after-newline.js")]
    public Task test_yield_star_after_newline()
        => CompilationFailureTest("yield-star-after-newline");

    [Fact(DisplayName = "yield-star-before-newline.js")]
    public Task test_yield_star_before_newline()
        => ExecutionTest("yield-star-before-newline");

    [Fact(DisplayName = "yield-weak-binding.js")]
    public Task test_yield_weak_binding()
        => CompilationFailureTest("yield-weak-binding");

}
