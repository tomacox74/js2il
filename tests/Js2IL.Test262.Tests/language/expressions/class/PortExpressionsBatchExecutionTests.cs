using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.class_;

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
}
