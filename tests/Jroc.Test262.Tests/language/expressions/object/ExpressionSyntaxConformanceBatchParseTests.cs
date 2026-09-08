using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions._object;

public class ExpressionSyntaxConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchParseTests() : base("language/expressions/object", "language.expressions.object") { }

    [Fact(DisplayName = "identifier-shorthand-implements-invalid-strict-mode.js")]
    public Task identifier_shorthand_implements_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-implements-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-interface-invalid-strict-mode.js")]
    public Task identifier_shorthand_interface_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-interface-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-invalid-computed-name.js")]
    public Task identifier_shorthand_invalid_computed_name()
        => CompilationFailureTest("identifier-shorthand-invalid-computed-name");

    [Fact(DisplayName = "identifier-shorthand-invalid-zero.js")]
    public Task identifier_shorthand_invalid_zero()
        => CompilationFailureTest("identifier-shorthand-invalid-zero");

    [Fact(DisplayName = "identifier-shorthand-let-invalid-strict-mode.js")]
    public Task identifier_shorthand_let_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-let-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-package-invalid-strict-mode.js")]
    public Task identifier_shorthand_package_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-package-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-private-invalid-strict-mode.js")]
    public Task identifier_shorthand_private_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-private-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-protected-invalid-strict-mode.js")]
    public Task identifier_shorthand_protected_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-protected-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-public-invalid-strict-mode.js")]
    public Task identifier_shorthand_public_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-public-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-static-init-await-invalid.js")]
    public Task identifier_shorthand_static_init_await_invalid()
        => CompilationFailureTest("identifier-shorthand-static-init-await-invalid");

    [Fact(DisplayName = "identifier-shorthand-static-invalid-strict-mode.js")]
    public Task identifier_shorthand_static_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-static-invalid-strict-mode");

    [Fact(DisplayName = "identifier-shorthand-yield-invalid-strict-mode.js")]
    public Task identifier_shorthand_yield_invalid_strict_mode()
        => CompilationFailureTest("identifier-shorthand-yield-invalid-strict-mode");

    [Fact(DisplayName = "async-await-as-binding-identifier-escaped.js")]
    public Task method_definition_async_await_as_binding_identifier_escaped()
        => CompilationFailureTest("method-definition/async-await-as-binding-identifier-escaped");

    [Fact(DisplayName = "async-await-as-binding-identifier.js")]
    public Task method_definition_async_await_as_binding_identifier()
        => CompilationFailureTest("method-definition/async-await-as-binding-identifier");

    [Fact(DisplayName = "async-await-as-identifier-reference-escaped.js")]
    public Task method_definition_async_await_as_identifier_reference_escaped()
        => CompilationFailureTest("method-definition/async-await-as-identifier-reference-escaped");

    [Fact(DisplayName = "async-await-as-identifier-reference.js")]
    public Task method_definition_async_await_as_identifier_reference()
        => CompilationFailureTest("method-definition/async-await-as-identifier-reference");

    [Fact(DisplayName = "async-await-as-label-identifier-escaped.js")]
    public Task method_definition_async_await_as_label_identifier_escaped()
        => CompilationFailureTest("method-definition/async-await-as-label-identifier-escaped");

    [Fact(DisplayName = "async-await-as-label-identifier.js")]
    public Task method_definition_async_await_as_label_identifier()
        => CompilationFailureTest("method-definition/async-await-as-label-identifier");

    [Fact(DisplayName = "async-gen-await-as-binding-identifier-escaped.js")]
    public Task method_definition_async_gen_await_as_binding_identifier_escaped()
        => CompilationFailureTest("method-definition/async-gen-await-as-binding-identifier-escaped");

    [Fact(DisplayName = "async-gen-await-as-binding-identifier.js")]
    public Task method_definition_async_gen_await_as_binding_identifier()
        => CompilationFailureTest("method-definition/async-gen-await-as-binding-identifier");

    [Fact(DisplayName = "async-gen-await-as-identifier-reference-escaped.js")]
    public Task method_definition_async_gen_await_as_identifier_reference_escaped()
        => CompilationFailureTest("method-definition/async-gen-await-as-identifier-reference-escaped");

    [Fact(DisplayName = "async-gen-await-as-identifier-reference.js")]
    public Task method_definition_async_gen_await_as_identifier_reference()
        => CompilationFailureTest("method-definition/async-gen-await-as-identifier-reference");

    [Fact(DisplayName = "async-gen-await-as-label-identifier-escaped.js")]
    public Task method_definition_async_gen_await_as_label_identifier_escaped()
        => CompilationFailureTest("method-definition/async-gen-await-as-label-identifier-escaped");

    [Fact(DisplayName = "async-gen-await-as-label-identifier.js")]
    public Task method_definition_async_gen_await_as_label_identifier()
        => CompilationFailureTest("method-definition/async-gen-await-as-label-identifier");

    [Fact(DisplayName = "async-gen-meth-array-destructuring-param-strict-body.js")]
    public Task method_definition_async_gen_meth_array_destructuring_param_strict_body()
        => CompilationFailureTest("method-definition/async-gen-meth-array-destructuring-param-strict-body");

    [Fact(DisplayName = "async-gen-meth-dflt-params-duplicates.js")]
    public Task method_definition_async_gen_meth_dflt_params_duplicates()
        => CompilationFailureTest("method-definition/async-gen-meth-dflt-params-duplicates");

    [Fact(DisplayName = "async-gen-meth-dflt-params-rest.js")]
    public Task method_definition_async_gen_meth_dflt_params_rest()
        => CompilationFailureTest("method-definition/async-gen-meth-dflt-params-rest");

    [Fact(DisplayName = "async-gen-meth-escaped-async.js")]
    public Task method_definition_async_gen_meth_escaped_async()
        => CompilationFailureTest("method-definition/async-gen-meth-escaped-async");

    [Fact(DisplayName = "async-gen-meth-object-destructuring-param-strict-body.js")]
    public Task method_definition_async_gen_meth_object_destructuring_param_strict_body()
        => CompilationFailureTest("method-definition/async-gen-meth-object-destructuring-param-strict-body");

    [Fact(DisplayName = "async-gen-meth-rest-param-strict-body.js")]
    public Task method_definition_async_gen_meth_rest_param_strict_body()
        => CompilationFailureTest("method-definition/async-gen-meth-rest-param-strict-body");

    [Fact(DisplayName = "async-gen-meth-rest-params-trailing-comma-early-error.js")]
    public Task method_definition_async_gen_meth_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("method-definition/async-gen-meth-rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "async-gen-yield-as-binding-identifier-escaped.js")]
    public Task method_definition_async_gen_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("method-definition/async-gen-yield-as-binding-identifier-escaped");

    [Fact(DisplayName = "async-gen-yield-as-binding-identifier.js")]
    public Task method_definition_async_gen_yield_as_binding_identifier()
        => CompilationFailureTest("method-definition/async-gen-yield-as-binding-identifier");

    [Fact(DisplayName = "async-gen-yield-as-identifier-reference-escaped.js")]
    public Task method_definition_async_gen_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("method-definition/async-gen-yield-as-identifier-reference-escaped");

    [Fact(DisplayName = "async-gen-yield-as-identifier-reference.js")]
    public Task method_definition_async_gen_yield_as_identifier_reference()
        => CompilationFailureTest("method-definition/async-gen-yield-as-identifier-reference");

    [Fact(DisplayName = "async-gen-yield-as-label-identifier-escaped.js")]
    public Task method_definition_async_gen_yield_as_label_identifier_escaped()
        => CompilationFailureTest("method-definition/async-gen-yield-as-label-identifier-escaped");

    [Fact(DisplayName = "async-gen-yield-as-label-identifier.js")]
    public Task method_definition_async_gen_yield_as_label_identifier()
        => CompilationFailureTest("method-definition/async-gen-yield-as-label-identifier");

    [Fact(DisplayName = "async-gen-yield-identifier-spread-strict.js")]
    public Task method_definition_async_gen_yield_identifier_spread_strict()
        => CompilationFailureTest("method-definition/async-gen-yield-identifier-spread-strict");

    [Fact(DisplayName = "async-gen-yield-identifier-strict.js")]
    public Task method_definition_async_gen_yield_identifier_strict()
        => CompilationFailureTest("method-definition/async-gen-yield-identifier-strict");

    [Fact(DisplayName = "async-meth-array-destructuring-param-strict-body.js")]
    public Task method_definition_async_meth_array_destructuring_param_strict_body()
        => CompilationFailureTest("method-definition/async-meth-array-destructuring-param-strict-body");

    [Fact(DisplayName = "async-meth-dflt-params-duplicates.js")]
    public Task method_definition_async_meth_dflt_params_duplicates()
        => CompilationFailureTest("method-definition/async-meth-dflt-params-duplicates");

    [Fact(DisplayName = "async-meth-dflt-params-rest.js")]
    public Task method_definition_async_meth_dflt_params_rest()
        => CompilationFailureTest("method-definition/async-meth-dflt-params-rest");

    [Fact(DisplayName = "async-meth-escaped-async.js")]
    public Task method_definition_async_meth_escaped_async()
        => CompilationFailureTest("method-definition/async-meth-escaped-async");

    [Fact(DisplayName = "async-meth-object-destructuring-param-strict-body.js")]
    public Task method_definition_async_meth_object_destructuring_param_strict_body()
        => CompilationFailureTest("method-definition/async-meth-object-destructuring-param-strict-body");

    [Fact(DisplayName = "async-meth-rest-param-strict-body.js")]
    public Task method_definition_async_meth_rest_param_strict_body()
        => CompilationFailureTest("method-definition/async-meth-rest-param-strict-body");

    [Fact(DisplayName = "async-meth-rest-params-trailing-comma-early-error.js")]
    public Task method_definition_async_meth_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("method-definition/async-meth-rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "early-errors-object-async-method-duplicate-parameters.js")]
    public Task method_definition_early_errors_object_async_method_duplicate_parameters()
        => CompilationFailureTest("method-definition/early-errors-object-async-method-duplicate-parameters");

    [Fact(DisplayName = "early-errors-object-method-NSPL-with-USD.js")]
    public Task method_definition_early_errors_object_method_NSPL_with_USD()
        => CompilationFailureTest("method-definition/early-errors-object-method-NSPL-with-USD");

    [Fact(DisplayName = "early-errors-object-method-arguments-in-formal-parameters.js")]
    public Task method_definition_early_errors_object_method_arguments_in_formal_parameters()
        => CompilationFailureTest("method-definition/early-errors-object-method-arguments-in-formal-parameters");

    [Fact(DisplayName = "early-errors-object-method-async-lineterminator.js")]
    public Task method_definition_early_errors_object_method_async_lineterminator()
        => CompilationFailureTest("method-definition/early-errors-object-method-async-lineterminator");

    [Fact(DisplayName = "early-errors-object-method-await-in-formals-default.js")]
    public Task method_definition_early_errors_object_method_await_in_formals_default()
        => CompilationFailureTest("method-definition/early-errors-object-method-await-in-formals-default");

    [Fact(DisplayName = "early-errors-object-method-await-in-formals.js")]
    public Task method_definition_early_errors_object_method_await_in_formals()
        => CompilationFailureTest("method-definition/early-errors-object-method-await-in-formals");

    [Fact(DisplayName = "early-errors-object-method-body-contains-super-call.js")]
    public Task method_definition_early_errors_object_method_body_contains_super_call()
        => CompilationFailureTest("method-definition/early-errors-object-method-body-contains-super-call");

    [Fact(DisplayName = "early-errors-object-method-duplicate-parameters.js")]
    public Task method_definition_early_errors_object_method_duplicate_parameters()
        => CompilationFailureTest("method-definition/early-errors-object-method-duplicate-parameters");

    [Fact(DisplayName = "early-errors-object-method-eval-in-formal-parameters.js")]
    public Task method_definition_early_errors_object_method_eval_in_formal_parameters()
        => CompilationFailureTest("method-definition/early-errors-object-method-eval-in-formal-parameters");

    [Fact(DisplayName = "early-errors-object-method-formals-body-duplicate.js")]
    public Task method_definition_early_errors_object_method_formals_body_duplicate()
        => CompilationFailureTest("method-definition/early-errors-object-method-formals-body-duplicate");

    [Fact(DisplayName = "early-errors-object-method-formals-contains-super-call.js")]
    public Task method_definition_early_errors_object_method_formals_contains_super_call()
        => CompilationFailureTest("method-definition/early-errors-object-method-formals-contains-super-call");

    [Fact(DisplayName = "escaped-get-e.js")]
    public Task method_definition_escaped_get_e()
        => CompilationFailureTest("method-definition/escaped-get-e");

    [Fact(DisplayName = "escaped-get-g.js")]
    public Task method_definition_escaped_get_g()
        => CompilationFailureTest("method-definition/escaped-get-g");

    [Fact(DisplayName = "escaped-get-t.js")]
    public Task method_definition_escaped_get_t()
        => CompilationFailureTest("method-definition/escaped-get-t");

    [Fact(DisplayName = "escaped-get.js")]
    public Task method_definition_escaped_get()
        => CompilationFailureTest("method-definition/escaped-get");

    [Fact(DisplayName = "escaped-set-e.js")]
    public Task method_definition_escaped_set_e()
        => CompilationFailureTest("method-definition/escaped-set-e");

    [Fact(DisplayName = "escaped-set-s.js")]
    public Task method_definition_escaped_set_s()
        => CompilationFailureTest("method-definition/escaped-set-s");

    [Fact(DisplayName = "escaped-set-t.js")]
    public Task method_definition_escaped_set_t()
        => CompilationFailureTest("method-definition/escaped-set-t");

    [Fact(DisplayName = "escaped-set.js")]
    public Task method_definition_escaped_set()
        => CompilationFailureTest("method-definition/escaped-set");

    [Fact(DisplayName = "gen-meth-array-destructuring-param-strict-body.js")]
    public Task method_definition_gen_meth_array_destructuring_param_strict_body()
        => CompilationFailureTest("method-definition/gen-meth-array-destructuring-param-strict-body");

    [Fact(DisplayName = "gen-meth-dflt-params-duplicates.js")]
    public Task method_definition_gen_meth_dflt_params_duplicates()
        => CompilationFailureTest("method-definition/gen-meth-dflt-params-duplicates");

    [Fact(DisplayName = "gen-meth-dflt-params-rest.js")]
    public Task method_definition_gen_meth_dflt_params_rest()
        => CompilationFailureTest("method-definition/gen-meth-dflt-params-rest");

    [Fact(DisplayName = "gen-meth-object-destructuring-param-strict-body.js")]
    public Task method_definition_gen_meth_object_destructuring_param_strict_body()
        => CompilationFailureTest("method-definition/gen-meth-object-destructuring-param-strict-body");

    [Fact(DisplayName = "gen-meth-rest-param-strict-body.js")]
    public Task method_definition_gen_meth_rest_param_strict_body()
        => CompilationFailureTest("method-definition/gen-meth-rest-param-strict-body");

    [Fact(DisplayName = "gen-meth-rest-params-trailing-comma-early-error.js")]
    public Task method_definition_gen_meth_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("method-definition/gen-meth-rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "gen-yield-as-binding-identifier-escaped.js")]
    public Task method_definition_gen_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("method-definition/gen-yield-as-binding-identifier-escaped");

    [Fact(DisplayName = "gen-yield-as-binding-identifier.js")]
    public Task method_definition_gen_yield_as_binding_identifier()
        => CompilationFailureTest("method-definition/gen-yield-as-binding-identifier");

    [Fact(DisplayName = "gen-yield-as-identifier-reference-escaped.js")]
    public Task method_definition_gen_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("method-definition/gen-yield-as-identifier-reference-escaped");

    [Fact(DisplayName = "gen-yield-as-identifier-reference.js")]
    public Task method_definition_gen_yield_as_identifier_reference()
        => CompilationFailureTest("method-definition/gen-yield-as-identifier-reference");

    [Fact(DisplayName = "gen-yield-as-label-identifier-escaped.js")]
    public Task method_definition_gen_yield_as_label_identifier_escaped()
        => CompilationFailureTest("method-definition/gen-yield-as-label-identifier-escaped");

    [Fact(DisplayName = "gen-yield-as-label-identifier.js")]
    public Task method_definition_gen_yield_as_label_identifier()
        => CompilationFailureTest("method-definition/gen-yield-as-label-identifier");

}
