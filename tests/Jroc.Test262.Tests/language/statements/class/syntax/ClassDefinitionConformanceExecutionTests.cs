namespace Jroc.Test262.Tests.language.statements.test_class.syntax;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.syntax") { }

    [Fact(DisplayName = "class-body-method-definition-super-property.js")]
    public Task class_body_method_definition_super_property()
        => ExecutionTest("class-body-method-definition-super-property");

    [Fact(DisplayName = "class-declaration-binding-identifier-class-element-list.js")]
    public Task class_declaration_binding_identifier_class_element_list()
        => ExecutionTest("class-declaration-binding-identifier-class-element-list");

    [Fact(DisplayName = "class-declaration-computed-method-definition.js")]
    public Task class_declaration_computed_method_definition()
        => ExecutionTest("class-declaration-computed-method-definition");

    [Fact(DisplayName = "class-declaration-computed-method-generator-definition.js")]
    public Task class_declaration_computed_method_generator_definition()
        => ExecutionTest("class-declaration-computed-method-generator-definition");

    [Fact(DisplayName = "class-declaration-heritage-identifier-reference-class-element-list.js")]
    public Task class_declaration_heritage_identifier_reference_class_element_list()
        => ExecutionTest("class-declaration-heritage-identifier-reference-class-element-list");

    [Fact(DisplayName = "class-expression-binding-identifier-opt-class-element-list.js")]
    public Task class_expression_binding_identifier_opt_class_element_list()
        => ExecutionTest("class-expression-binding-identifier-opt-class-element-list");

    [Fact(DisplayName = "class-expression-heritage-identifier-reference.js")]
    public Task class_expression_heritage_identifier_reference()
        => ExecutionTest("class-expression-heritage-identifier-reference");

    [Fact(DisplayName = "class-expression.js")]
    public Task class_expression()
        => ExecutionTest("class-expression");

    [Fact(DisplayName = "class-method-propname-constructor.js")]
    public Task class_method_propname_constructor()
        => ExecutionTest("class-method-propname-constructor");
}
