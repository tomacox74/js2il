using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.class_;

public class PortStatementsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortStatementsBatchExecutionTests() : base("language.statements.class_") { }

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-string-literal")]
    public Task cpn_class_decl_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-numeric-literal")]
    public Task cpn_class_decl_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-identifier")]
    public Task cpn_class_decl_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-null")]
    public Task cpn_class_decl_computed_property_name_from_null()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-identifier")]
    public Task cpn_class_decl_accessors_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-null")]
    public Task cpn_class_decl_accessors_computed_property_name_from_null()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-string-literal")]
    public Task cpn_class_decl_accessors_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-numeric-literal")]
    public Task cpn_class_decl_accessors_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-identifier")]
    public Task cpn_class_decl_fields_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-condition-expression-true")]
    public Task cpn_class_decl_fields_computed_property_name_from_condition_expression_true()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-condition-expression-true");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-null")]
    public Task cpn_class_decl_fields_computed_property_name_from_null()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-string-literal")]
    public Task cpn_class_decl_fields_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-numeric-literal")]
    public Task cpn_class_decl_fields_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-numeric-literal");
}
