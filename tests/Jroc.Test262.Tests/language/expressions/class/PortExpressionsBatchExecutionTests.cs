using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.class_;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.class_") { }

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-string-literal")]
    public Task cpn_class_expr_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-numeric-literal")]
    public Task cpn_class_expr_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-identifier")]
    public Task cpn_class_expr_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-null")]
    public Task cpn_class_expr_computed_property_name_from_null()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-identifier")]
    public Task cpn_class_expr_accessors_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-null")]
    public Task cpn_class_expr_accessors_computed_property_name_from_null()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-string-literal")]
    public Task cpn_class_expr_accessors_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-numeric-literal")]
    public Task cpn_class_expr_accessors_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-identifier")]
    public Task cpn_class_expr_fields_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-null")]
    public Task cpn_class_expr_fields_computed_property_name_from_null()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-string-literal")]
    public Task cpn_class_expr_fields_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-numeric-literal")]
    public Task cpn_class_expr_fields_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "heritage-arrow-function")]
    public Task heritage_arrow_function()
        => ExecutionTest("heritage-arrow-function");

    [Fact(DisplayName = "poisoned-underscore-proto")]
    public Task poisoned_underscore_proto()
        => ExecutionTest("poisoned-underscore-proto");

    [Fact(DisplayName = "restricted-properties")]
    public Task restricted_properties()
        => ExecutionTest("restricted-properties");

    [Fact(DisplayName = "scope-meth-paramsbody-var-close")]
    public Task scope_meth_paramsbody_var_close()
        => ExecutionTest("scope-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-meth-paramsbody-var-open")]
    public Task scope_meth_paramsbody_var_open()
        => ExecutionTest("scope-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-name-lex-close")]
    public Task scope_name_lex_close()
        => ExecutionTest("scope-name-lex-close");

    [Fact(DisplayName = "scope-name-lex-open-heritage")]
    public Task scope_name_lex_open_heritage()
        => ExecutionTest("scope-name-lex-open-heritage");

    [Fact(DisplayName = "scope-name-lex-open-no-heritage")]
    public Task scope_name_lex_open_no_heritage()
        => ExecutionTest("scope-name-lex-open-no-heritage");

    [Fact(DisplayName = "scope-setter-paramsbody-var-close")]
    public Task scope_setter_paramsbody_var_close()
        => ExecutionTest("scope-setter-paramsbody-var-close");

    [Fact(DisplayName = "scope-setter-paramsbody-var-open")]
    public Task scope_setter_paramsbody_var_open()
        => ExecutionTest("scope-setter-paramsbody-var-open");

    [Fact(DisplayName = "scope-static-meth-paramsbody-var-close")]
    public Task scope_static_meth_paramsbody_var_close()
        => ExecutionTest("scope-static-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-static-meth-paramsbody-var-open")]
    public Task scope_static_meth_paramsbody_var_open()
        => ExecutionTest("scope-static-meth-paramsbody-var-open");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-additive-expression-add")]
    public Task cpn_class_expr_computed_property_name_from_additive_expression_add()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-additive-expression-add");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-condition-expression-false")]
    public Task cpn_class_expr_computed_property_name_from_condition_expression_false()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-condition-expression-false");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-condition-expression-true")]
    public Task cpn_class_expr_computed_property_name_from_condition_expression_true()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-condition-expression-true");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-expression-logical-and")]
    public Task cpn_class_expr_computed_property_name_from_expression_logical_and()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-additive-expression-add")]
    public Task cpn_class_expr_accessors_computed_property_name_from_additive_expression_add()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-additive-expression-add");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-condition-expression-true")]
    public Task cpn_class_expr_accessors_computed_property_name_from_condition_expression_true()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-condition-expression-true");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-expression-logical-and")]
    public Task cpn_class_expr_accessors_computed_property_name_from_expression_logical_and()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-additive-expression-add")]
    public Task cpn_class_expr_fields_computed_property_name_from_additive_expression_add()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-additive-expression-add");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-condition-expression-false")]
    public Task cpn_class_expr_fields_computed_property_name_from_condition_expression_false()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-condition-expression-false");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-expression-logical-and")]
    public Task cpn_class_expr_fields_computed_property_name_from_expression_logical_and()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-expression-logical-and");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-additive-expression-subtract")]
    public Task cpn_class_expr_computed_property_name_from_additive_expression_subtract()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-additive-expression-subtract");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-assignment-expression-assignment")]
    public Task cpn_class_expr_computed_property_name_from_assignment_expression_assignment()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-assignment-expression-assignment");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-assignment-expression-bitwise-or")]
    public Task cpn_class_expr_computed_property_name_from_assignment_expression_bitwise_or()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-assignment-expression-bitwise-or");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-decimal-literal")]
    public Task cpn_class_expr_computed_property_name_from_decimal_literal()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-decimal-literal");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-expression-coalesce")]
    public Task cpn_class_expr_computed_property_name_from_expression_coalesce()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-expression-coalesce");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-expression-logical-or")]
    public Task cpn_class_expr_computed_property_name_from_expression_logical_or()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-expression-logical-or");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-math")]
    public Task cpn_class_expr_computed_property_name_from_math()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-math");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-multiplicative-expression-div")]
    public Task cpn_class_expr_computed_property_name_from_multiplicative_expression_div()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-multiplicative-expression-div");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-additive-expression-subtract")]
    public Task cpn_class_expr_accessors_computed_property_name_from_additive_expression_subtract()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-additive-expression-subtract");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-condition-expression-false")]
    public Task cpn_class_expr_accessors_computed_property_name_from_condition_expression_false()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-condition-expression-false");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-expression-coalesce")]
    public Task cpn_class_expr_accessors_computed_property_name_from_expression_coalesce()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-expression-coalesce");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-additive-expression-subtract")]
    public Task cpn_class_expr_fields_computed_property_name_from_additive_expression_subtract()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-additive-expression-subtract");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-expression-coalesce")]
    public Task cpn_class_expr_fields_computed_property_name_from_expression_coalesce()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-expression-coalesce");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-multiplicative-expression-mult")]
    public Task cpn_class_expr_computed_property_name_from_multiplicative_expression_mult()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-multiplicative-expression-mult");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-integer-e-notational-literal")]
    public Task cpn_class_expr_computed_property_name_from_integer_e_notational_literal()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-integer-e-notational-literal");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-decimal-e-notational-literal")]
    public Task cpn_class_expr_computed_property_name_from_decimal_e_notational_literal()
        => ExecutionTest("cpn-class-expr-computed-property-name-from-decimal-e-notational-literal");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-decimal-literal")]
    public Task cpn_class_expr_accessors_computed_property_name_from_decimal_literal()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-decimal-literal");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-expression-logical-or")]
    public Task cpn_class_expr_accessors_computed_property_name_from_expression_logical_or()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-expression-logical-or");

    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-math")]
    public Task cpn_class_expr_accessors_computed_property_name_from_math()
        => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-math");

    [Fact(DisplayName = "cpn-class-expr-fields-computed-property-name-from-decimal-literal")]
    public Task cpn_class_expr_fields_computed_property_name_from_decimal_literal()
        => ExecutionTest("cpn-class-expr-fields-computed-property-name-from-decimal-literal");

}
