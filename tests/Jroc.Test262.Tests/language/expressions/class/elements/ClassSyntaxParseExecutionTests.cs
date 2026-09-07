using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.class_.elements;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/expressions/class/elements", "language.expressions.class_.elements") { }

    [Fact(DisplayName = "arrow-fnc-init-err-contains-arguments")]
    public Task arrow_fnc_init_err_contains_arguments()
        => CompilationFailureTest("arrow-fnc-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrow-fnc-init-err-contains-super")]
    public Task arrow_fnc_init_err_contains_super()
        => CompilationFailureTest("arrow-fnc-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "comp-name-init-err-contains-arguments")]
    public Task comp_name_init_err_contains_arguments()
        => CompilationFailureTest("comp-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "comp-name-init-err-contains-super")]
    public Task comp_name_init_err_contains_super()
        => CompilationFailureTest("comp-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "equality-init-err-contains-arguments")]
    public Task equality_init_err_contains_arguments()
        => CompilationFailureTest("equality-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "equality-init-err-contains-super")]
    public Task equality_init_err_contains_super()
        => CompilationFailureTest("equality-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-duplicate-privatenames")]
    public Task fields_duplicate_privatenames()
        => CompilationFailureTest("fields-duplicate-privatenames", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-literal-name-propname-constructor")]
    public Task fields_literal_name_propname_constructor()
        => CompilationFailureTest("fields-literal-name-propname-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-literal-name-static-propname-constructor")]
    public Task fields_literal_name_static_propname_constructor()
        => CompilationFailureTest("fields-literal-name-static-propname-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-literal-name-static-propname-prototype")]
    public Task fields_literal_name_static_propname_prototype()
        => CompilationFailureTest("fields-literal-name-static-propname-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-string-name-propname-constructor")]
    public Task fields_string_name_propname_constructor()
        => CompilationFailureTest("fields-string-name-propname-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-string-name-static-propname-constructor")]
    public Task fields_string_name_static_propname_constructor()
        => CompilationFailureTest("fields-string-name-static-propname-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-string-name-static-propname-prototype")]
    public Task fields_string_name_static_propname_prototype()
        => CompilationFailureTest("fields-string-name-static-propname-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "literal-name-init-err-contains-arguments")]
    public Task literal_name_init_err_contains_arguments()
        => CompilationFailureTest("literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "literal-name-init-err-contains-super")]
    public Task literal_name_init_err_contains_super()
        => CompilationFailureTest("literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-arrow-fnc-init-err-contains-arguments")]
    public Task nested_arrow_fnc_init_err_contains_arguments()
        => CompilationFailureTest("nested-arrow-fnc-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-arrow-fnc-init-err-contains-super")]
    public Task nested_arrow_fnc_init_err_contains_super()
        => CompilationFailureTest("nested-arrow-fnc-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-comp-name-init-err-contains-arguments")]
    public Task nested_comp_name_init_err_contains_arguments()
        => CompilationFailureTest("nested-comp-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-comp-name-init-err-contains-super")]
    public Task nested_comp_name_init_err_contains_super()
        => CompilationFailureTest("nested-comp-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-equality-init-err-contains-arguments")]
    public Task nested_equality_init_err_contains_arguments()
        => CompilationFailureTest("nested-equality-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-equality-init-err-contains-super")]
    public Task nested_equality_init_err_contains_super()
        => CompilationFailureTest("nested-equality-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-literal-name-init-err-contains-arguments")]
    public Task nested_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("nested-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-literal-name-init-err-contains-super")]
    public Task nested_literal_name_init_err_contains_super()
        => CompilationFailureTest("nested-literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-arrow-fnc-init-err-contains-arguments")]
    public Task nested_private_arrow_fnc_init_err_contains_arguments()
        => CompilationFailureTest("nested-private-arrow-fnc-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-arrow-fnc-init-err-contains-super")]
    public Task nested_private_arrow_fnc_init_err_contains_super()
        => CompilationFailureTest("nested-private-arrow-fnc-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-literal-name-init-err-contains-arguments")]
    public Task nested_private_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("nested-private-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-literal-name-init-err-contains-super")]
    public Task nested_private_literal_name_init_err_contains_super()
        => CompilationFailureTest("nested-private-literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-ternary-init-err-contains-arguments")]
    public Task nested_private_ternary_init_err_contains_arguments()
        => CompilationFailureTest("nested-private-ternary-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-ternary-init-err-contains-super")]
    public Task nested_private_ternary_init_err_contains_super()
        => CompilationFailureTest("nested-private-ternary-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-typeof-init-err-contains-arguments")]
    public Task nested_private_typeof_init_err_contains_arguments()
        => CompilationFailureTest("nested-private-typeof-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-private-typeof-init-err-contains-super")]
    public Task nested_private_typeof_init_err_contains_super()
        => CompilationFailureTest("nested-private-typeof-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-comp-name-init-err-contains-arguments")]
    public Task nested_static_comp_name_init_err_contains_arguments()
        => CompilationFailureTest("nested-static-comp-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-comp-name-init-err-contains-super")]
    public Task nested_static_comp_name_init_err_contains_super()
        => CompilationFailureTest("nested-static-comp-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-literal-init-err-contains-arguments")]
    public Task nested_static_literal_init_err_contains_arguments()
        => CompilationFailureTest("nested-static-literal-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-literal-init-err-contains-super")]
    public Task nested_static_literal_init_err_contains_super()
        => CompilationFailureTest("nested-static-literal-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-private-init-err-contains-arguments")]
    public Task nested_static_private_init_err_contains_arguments()
        => CompilationFailureTest("nested-static-private-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-private-init-err-contains-super")]
    public Task nested_static_private_init_err_contains_super()
        => CompilationFailureTest("nested-static-private-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-string-literal-name-init-err-contains-arguments")]
    public Task nested_static_string_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("nested-static-string-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-static-string-literal-name-init-err-contains-super")]
    public Task nested_static_string_literal_name_init_err_contains_super()
        => CompilationFailureTest("nested-static-string-literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-string-literal-name-init-err-contains-arguments")]
    public Task nested_string_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("nested-string-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-string-literal-name-init-err-contains-super")]
    public Task nested_string_literal_name_init_err_contains_super()
        => CompilationFailureTest("nested-string-literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-ternary-init-err-contains-arguments")]
    public Task nested_ternary_init_err_contains_arguments()
        => CompilationFailureTest("nested-ternary-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-ternary-init-err-contains-super")]
    public Task nested_ternary_init_err_contains_super()
        => CompilationFailureTest("nested-ternary-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-typeof-init-err-contains-arguments")]
    public Task nested_typeof_init_err_contains_arguments()
        => CompilationFailureTest("nested-typeof-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "nested-typeof-init-err-contains-super")]
    public Task nested_typeof_init_err_contains_super()
        => CompilationFailureTest("nested-typeof-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-arrow-fnc-init-err-contains-arguments")]
    public Task private_arrow_fnc_init_err_contains_arguments()
        => CompilationFailureTest("private-arrow-fnc-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-arrow-fnc-init-err-contains-super")]
    public Task private_arrow_fnc_init_err_contains_super()
        => CompilationFailureTest("private-arrow-fnc-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-literal-name-init-err-contains-arguments")]
    public Task private_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("private-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-literal-name-init-err-contains-super")]
    public Task private_literal_name_init_err_contains_super()
        => CompilationFailureTest("private-literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-ternary-init-err-contains-arguments")]
    public Task private_ternary_init_err_contains_arguments()
        => CompilationFailureTest("private-ternary-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-ternary-init-err-contains-super")]
    public Task private_ternary_init_err_contains_super()
        => CompilationFailureTest("private-ternary-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-typeof-init-err-contains-arguments")]
    public Task private_typeof_init_err_contains_arguments()
        => CompilationFailureTest("private-typeof-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-typeof-init-err-contains-super")]
    public Task private_typeof_init_err_contains_super()
        => CompilationFailureTest("private-typeof-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-comp-name-init-err-contains-arguments")]
    public Task static_comp_name_init_err_contains_arguments()
        => CompilationFailureTest("static-comp-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-comp-name-init-err-contains-super")]
    public Task static_comp_name_init_err_contains_super()
        => CompilationFailureTest("static-comp-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-literal-init-err-contains-arguments")]
    public Task static_literal_init_err_contains_arguments()
        => CompilationFailureTest("static-literal-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-literal-init-err-contains-super")]
    public Task static_literal_init_err_contains_super()
        => CompilationFailureTest("static-literal-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-private-init-err-contains-arguments")]
    public Task static_private_init_err_contains_arguments()
        => CompilationFailureTest("static-private-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-private-init-err-contains-super")]
    public Task static_private_init_err_contains_super()
        => CompilationFailureTest("static-private-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-string-literal-name-init-err-contains-arguments")]
    public Task static_string_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("static-string-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-string-literal-name-init-err-contains-super")]
    public Task static_string_literal_name_init_err_contains_super()
        => CompilationFailureTest("static-string-literal-name-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "string-literal-name-init-err-contains-arguments")]
    public Task string_literal_name_init_err_contains_arguments()
        => CompilationFailureTest("string-literal-name-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "string-literal-name-init-err-contains-super")]
    public Task string_literal_name_init_err_contains_super()
        => CompilationFailureTest("string-literal-name-init-err-contains-super", "Failed to parse JavaScript");
}
