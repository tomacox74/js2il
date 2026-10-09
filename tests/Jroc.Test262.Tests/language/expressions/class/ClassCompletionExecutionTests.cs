using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class") { }

    [Fact(DisplayName = "class-name-ident-await-escaped-module.js")]
    public Task class_name_ident_await_escaped_module()
        => CompilationFailureTest("class-name-ident-await-escaped-module", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-await-module.js")]
    public Task class_name_ident_await_module()
        => CompilationFailureTest("class-name-ident-await-module", "Failed to parse JavaScript");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_expr_accessors_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTestFromFile("cpn-class-expr-accessors-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_expr_accessors_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTestFromFile("cpn-class-expr-accessors-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-await-expression.js")]
    public Task cpn_class_expr_accessors_computed_property_name_from_await_expression()
        => ExecutionTestFromFile("cpn-class-expr-accessors-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_expr_accessors_computed_property_name_from_generator_function_declaration()
        => ExecutionTestFromFile("cpn-class-expr-accessors-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_expr_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTestFromFile("cpn-class-expr-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_expr_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTestFromFile("cpn-class-expr-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-await-expression.js")]
    public Task cpn_class_expr_computed_property_name_from_await_expression()
        => ExecutionTestFromFile("cpn-class-expr-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_expr_computed_property_name_from_generator_function_declaration()
        => ExecutionTestFromFile("cpn-class-expr-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_expr_fields_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTestFromFile("cpn-class-expr-fields-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_expr_fields_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTestFromFile("cpn-class-expr-fields-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-await-expression.js")]
    public Task cpn_class_expr_fields_computed_property_name_from_await_expression()
        => ExecutionTestFromFile("cpn-class-expr-fields-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_expr_fields_computed_property_name_from_generator_function_declaration()
        => ExecutionTestFromFile("cpn-class-expr-fields-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-yield-expression.js")]
    public Task cpn_class_expr_fields_computed_property_name_from_yield_expression()
        => ExecutionTestFromFile("cpn-class-expr-fields-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "cpn-class-expr-fields-methods-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_expr_fields_methods_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTestFromFile("cpn-class-expr-fields-methods-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-fields-methods-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_expr_fields_methods_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTestFromFile("cpn-class-expr-fields-methods-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-expr-fields-methods-computed-property-name-from-await-expression.js")]
    public Task cpn_class_expr_fields_methods_computed_property_name_from_await_expression()
        => ExecutionTestFromFile("cpn-class-expr-fields-methods-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-expr-fields-methods-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_expr_fields_methods_computed_property_name_from_generator_function_declaration()
        => ExecutionTestFromFile("cpn-class-expr-fields-methods-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-expr-fields-methods-computed-property-name-from-yield-expression.js")]
    public Task cpn_class_expr_fields_methods_computed_property_name_from_yield_expression()
        => ExecutionTestFromFile("cpn-class-expr-fields-methods-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class-eval.js", Skip = "eval is not supported.")]
    public Task private_getter_brand_check_multiple_evaluations_of_class_eval()
        => ExecutionTestFromFile("private-getter-brand-check-multiple-evaluations-of-class-eval");

    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_getter_brand_check_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-getter-brand-check-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class-realm-function-ctor.js")]
    public Task private_getter_brand_check_multiple_evaluations_of_class_realm_function_ctor()
        => ExecutionTestFromFile("private-getter-brand-check-multiple-evaluations-of-class-realm-function-ctor");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class-eval.js", Skip = "eval is not supported.")]
    public Task private_method_brand_check_multiple_evaluations_of_class_eval()
        => ExecutionTestFromFile("private-method-brand-check-multiple-evaluations-of-class-eval");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_method_brand_check_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-method-brand-check-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class-realm-function-ctor.js")]
    public Task private_method_brand_check_multiple_evaluations_of_class_realm_function_ctor()
        => ExecutionTestFromFile("private-method-brand-check-multiple-evaluations-of-class-realm-function-ctor");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class-realm.js", Skip = "eval is not supported.")]
    public Task private_method_brand_check_multiple_evaluations_of_class_realm()
        => ExecutionTestFromFile("private-method-brand-check-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class-eval-indirect.js", Skip = "eval is not supported.")]
    public Task private_setter_brand_check_multiple_evaluations_of_class_eval_indirect()
        => ExecutionTestFromFile("private-setter-brand-check-multiple-evaluations-of-class-eval-indirect");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class-eval.js", Skip = "eval is not supported.")]
    public Task private_setter_brand_check_multiple_evaluations_of_class_eval()
        => ExecutionTestFromFile("private-setter-brand-check-multiple-evaluations-of-class-eval");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class-factory.js")]
    public Task private_setter_brand_check_multiple_evaluations_of_class_factory()
        => ExecutionTestFromFile("private-setter-brand-check-multiple-evaluations-of-class-factory");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_setter_brand_check_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-setter-brand-check-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class-realm-function-ctor.js")]
    public Task private_setter_brand_check_multiple_evaluations_of_class_realm_function_ctor()
        => ExecutionTestFromFile("private-setter-brand-check-multiple-evaluations-of-class-realm-function-ctor");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class-realm.js", Skip = "eval is not supported.")]
    public Task private_setter_brand_check_multiple_evaluations_of_class_realm()
        => ExecutionTestFromFile("private-setter-brand-check-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "private-static-field-multiple-evaluations-of-class-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_field_multiple_evaluations_of_class_direct_eval()
        => ExecutionTestFromFile("private-static-field-multiple-evaluations-of-class-direct-eval");

    [Fact(DisplayName = "private-static-field-multiple-evaluations-of-class-eval-indirect.js", Skip = "eval is not supported.")]
    public Task private_static_field_multiple_evaluations_of_class_eval_indirect()
        => ExecutionTestFromFile("private-static-field-multiple-evaluations-of-class-eval-indirect");

    [Fact(DisplayName = "private-static-field-multiple-evaluations-of-class-factory.js")]
    public Task private_static_field_multiple_evaluations_of_class_factory()
        => ExecutionTestFromFile("private-static-field-multiple-evaluations-of-class-factory");

    [Fact(DisplayName = "private-static-field-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_static_field_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-static-field-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-static-field-multiple-evaluations-of-class-realm.js", Skip = "eval is not supported.")]
    public Task private_static_field_multiple_evaluations_of_class_realm()
        => ExecutionTestFromFile("private-static-field-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "private-static-getter-multiple-evaluations-of-class-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_getter_multiple_evaluations_of_class_direct_eval()
        => ExecutionTestFromFile("private-static-getter-multiple-evaluations-of-class-direct-eval");

    [Fact(DisplayName = "private-static-getter-multiple-evaluations-of-class-eval-indirect.js", Skip = "eval is not supported.")]
    public Task private_static_getter_multiple_evaluations_of_class_eval_indirect()
        => ExecutionTestFromFile("private-static-getter-multiple-evaluations-of-class-eval-indirect");

    [Fact(DisplayName = "private-static-getter-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_static_getter_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-static-getter-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-static-getter-multiple-evaluations-of-class-realm.js", Skip = "eval is not supported.")]
    public Task private_static_getter_multiple_evaluations_of_class_realm()
        => ExecutionTestFromFile("private-static-getter-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "private-static-method-brand-check-multiple-evaluations-of-class-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_method_brand_check_multiple_evaluations_of_class_direct_eval()
        => ExecutionTestFromFile("private-static-method-brand-check-multiple-evaluations-of-class-direct-eval");

    [Fact(DisplayName = "private-static-method-brand-check-multiple-evaluations-of-class-eval-indirect.js", Skip = "eval is not supported.")]
    public Task private_static_method_brand_check_multiple_evaluations_of_class_eval_indirect()
        => ExecutionTestFromFile("private-static-method-brand-check-multiple-evaluations-of-class-eval-indirect");

    [Fact(DisplayName = "private-static-method-brand-check-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_static_method_brand_check_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-static-method-brand-check-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-static-method-brand-check-multiple-evaluations-of-class-realm.js", Skip = "eval is not supported.")]
    public Task private_static_method_brand_check_multiple_evaluations_of_class_realm()
        => ExecutionTestFromFile("private-static-method-brand-check-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "private-static-setter-multiple-evaluations-of-class-direct-eval.js", Skip = "eval is not supported.")]
    public Task private_static_setter_multiple_evaluations_of_class_direct_eval()
        => ExecutionTestFromFile("private-static-setter-multiple-evaluations-of-class-direct-eval");

    [Fact(DisplayName = "private-static-setter-multiple-evaluations-of-class-eval-indirect.js", Skip = "eval is not supported.")]
    public Task private_static_setter_multiple_evaluations_of_class_eval_indirect()
        => ExecutionTestFromFile("private-static-setter-multiple-evaluations-of-class-eval-indirect");

    [Fact(DisplayName = "private-static-setter-multiple-evaluations-of-class-function-ctor.js")]
    public Task private_static_setter_multiple_evaluations_of_class_function_ctor()
        => ExecutionTestFromFile("private-static-setter-multiple-evaluations-of-class-function-ctor");

    [Fact(DisplayName = "private-static-setter-multiple-evaluations-of-class-realm.js", Skip = "eval is not supported.")]
    public Task private_static_setter_multiple_evaluations_of_class_realm()
        => ExecutionTestFromFile("private-static-setter-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "static-init-await-reference.js")]
    public Task static_init_await_reference()
        => ExecutionTestFromFile("static-init-await-reference");
}
