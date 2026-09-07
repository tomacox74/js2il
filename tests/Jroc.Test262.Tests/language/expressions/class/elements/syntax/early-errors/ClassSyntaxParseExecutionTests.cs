using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.class_.elements.syntax.early_errors;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/expressions/class/elements/syntax/early-errors", "language.expressions.class_.elements.syntax.early_errors") { }

    [Fact(DisplayName = "class-heritage-array-literal-arrow-heritage")]
    public Task class_heritage_array_literal_arrow_heritage()
        => CompilationFailureTest("class-heritage-array-literal-arrow-heritage", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-heritage-array-literal-async-arrow-heritage")]
    public Task class_heritage_array_literal_async_arrow_heritage()
        => CompilationFailureTest("class-heritage-array-literal-async-arrow-heritage", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-class-body-ctor-duplicate")]
    public Task grammar_class_body_ctor_duplicate()
        => CompilationFailureTest("grammar-class-body-ctor-duplicate", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-ctor-super-no-heritage")]
    public Task grammar_ctor_super_no_heritage()
        => CompilationFailureTest("grammar-ctor-super-no-heritage", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-field-identifier-invalid-ues-error")]
    public Task grammar_field_identifier_invalid_ues_error()
        => CompilationFailureTest("grammar-field-identifier-invalid-ues-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-field-identifier-invalid-zwj-error")]
    public Task grammar_field_identifier_invalid_zwj_error()
        => CompilationFailureTest("grammar-field-identifier-invalid-zwj-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-field-identifier-invalid-zwnj-error")]
    public Task grammar_field_identifier_invalid_zwnj_error()
        => CompilationFailureTest("grammar-field-identifier-invalid-zwnj-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-fields-same-line-error")]
    public Task grammar_fields_same_line_error()
        => CompilationFailureTest("grammar-fields-same-line-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-environment-on-class-heritage-array-literal")]
    public Task grammar_private_environment_on_class_heritage_array_literal()
        => CompilationFailureTest("grammar-private-environment-on-class-heritage-array-literal", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-environment-on-class-heritage-chained-usage")]
    public Task grammar_private_environment_on_class_heritage_chained_usage()
        => CompilationFailureTest("grammar-private-environment-on-class-heritage-chained-usage", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-environment-on-class-heritage-function-expression")]
    public Task grammar_private_environment_on_class_heritage_function_expression()
        => CompilationFailureTest("grammar-private-environment-on-class-heritage-function-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-environment-on-class-heritage-obj-literal")]
    public Task grammar_private_environment_on_class_heritage_obj_literal()
        => CompilationFailureTest("grammar-private-environment-on-class-heritage-obj-literal", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-environment-on-class-heritage-recursive")]
    public Task grammar_private_environment_on_class_heritage_recursive()
        => CompilationFailureTest("grammar-private-environment-on-class-heritage-recursive", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-environment-on-class-heritage")]
    public Task grammar_private_environment_on_class_heritage()
        => CompilationFailureTest("grammar-private-environment-on-class-heritage", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-field-on-object-destructuring")]
    public Task grammar_private_field_on_object_destructuring()
        => CompilationFailureTest("grammar-private-field-on-object-destructuring", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-private-field-super-access")]
    public Task grammar_private_field_super_access()
        => CompilationFailureTest("grammar-private-field-super-access", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-async-gen")]
    public Task grammar_privatemeth_duplicate_async_gen()
        => CompilationFailureTest("grammar-privatemeth-duplicate-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-async")]
    public Task grammar_privatemeth_duplicate_async()
        => CompilationFailureTest("grammar-privatemeth-duplicate-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-gen")]
    public Task grammar_privatemeth_duplicate_gen()
        => CompilationFailureTest("grammar-privatemeth-duplicate-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-get-field")]
    public Task grammar_privatemeth_duplicate_get_field()
        => CompilationFailureTest("grammar-privatemeth-duplicate-get-field", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-get-get")]
    public Task grammar_privatemeth_duplicate_get_get()
        => CompilationFailureTest("grammar-privatemeth-duplicate-get-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-field")]
    public Task grammar_privatemeth_duplicate_meth_field()
        => CompilationFailureTest("grammar-privatemeth-duplicate-meth-field", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-get")]
    public Task grammar_privatemeth_duplicate_meth_get()
        => CompilationFailureTest("grammar-privatemeth-duplicate-meth-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-meth")]
    public Task grammar_privatemeth_duplicate_meth_meth()
        => CompilationFailureTest("grammar-privatemeth-duplicate-meth-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-set")]
    public Task grammar_privatemeth_duplicate_meth_set()
        => CompilationFailureTest("grammar-privatemeth-duplicate-meth-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-staticfield")]
    public Task grammar_privatemeth_duplicate_meth_staticfield()
        => CompilationFailureTest("grammar-privatemeth-duplicate-meth-staticfield", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-staticmeth")]
    public Task grammar_privatemeth_duplicate_meth_staticmeth()
        => CompilationFailureTest("grammar-privatemeth-duplicate-meth-staticmeth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-set-field")]
    public Task grammar_privatemeth_duplicate_set_field()
        => CompilationFailureTest("grammar-privatemeth-duplicate-set-field", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-set-set")]
    public Task grammar_privatemeth_duplicate_set_set()
        => CompilationFailureTest("grammar-privatemeth-duplicate-set-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-constructor")]
    public Task grammar_privatename_constructor()
        => CompilationFailureTest("grammar-privatename-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-error")]
    public Task grammar_privatename_error()
        => CompilationFailureTest("grammar-privatename-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-identifier-invalid-ues")]
    public Task grammar_privatename_identifier_invalid_ues()
        => CompilationFailureTest("grammar-privatename-identifier-invalid-ues", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-identifier-invalid-zwj-error")]
    public Task grammar_privatename_identifier_invalid_zwj_error()
        => CompilationFailureTest("grammar-privatename-identifier-invalid-zwj-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-identifier-invalid-zwnj-error")]
    public Task grammar_privatename_identifier_invalid_zwnj_error()
        => CompilationFailureTest("grammar-privatename-identifier-invalid-zwnj-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-in-computed-property-missing")]
    public Task grammar_privatename_in_computed_property_missing()
        => CompilationFailureTest("grammar-privatename-in-computed-property-missing", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-accessor-get-meth")]
    public Task grammar_privatename_whitespace_error_accessor_get_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-accessor-get-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-accessor-set-meth")]
    public Task grammar_privatename_whitespace_error_accessor_set_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-accessor-set-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-async-gen-meth")]
    public Task grammar_privatename_whitespace_error_async_gen_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-async-gen-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-async-meth")]
    public Task grammar_privatename_whitespace_error_async_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-async-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-call-expr")]
    public Task grammar_privatename_whitespace_error_call_expr()
        => CompilationFailureTest("grammar-privatename-whitespace-error-call-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-field-init")]
    public Task grammar_privatename_whitespace_error_field_init()
        => CompilationFailureTest("grammar-privatename-whitespace-error-field-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-field")]
    public Task grammar_privatename_whitespace_error_field()
        => CompilationFailureTest("grammar-privatename-whitespace-error-field", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-gen-meth")]
    public Task grammar_privatename_whitespace_error_gen_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-gen-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-member-expr")]
    public Task grammar_privatename_whitespace_error_member_expr()
        => CompilationFailureTest("grammar-privatename-whitespace-error-member-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-meth")]
    public Task grammar_privatename_whitespace_error_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-accessor-get-meth")]
    public Task grammar_privatename_whitespace_error_static_accessor_get_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-accessor-get-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-accessor-set-meth")]
    public Task grammar_privatename_whitespace_error_static_accessor_set_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-accessor-set-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-async-gen-meth")]
    public Task grammar_privatename_whitespace_error_static_async_gen_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-async-gen-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-async-meth")]
    public Task grammar_privatename_whitespace_error_static_async_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-async-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-field-init")]
    public Task grammar_privatename_whitespace_error_static_field_init()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-field-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-field")]
    public Task grammar_privatename_whitespace_error_static_field()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-field", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-gen-meth")]
    public Task grammar_privatename_whitespace_error_static_gen_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-gen-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-meth")]
    public Task grammar_privatename_whitespace_error_static_meth()
        => CompilationFailureTest("grammar-privatename-whitespace-error-static-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatenames-same-line-error")]
    public Task grammar_privatenames_same_line_error()
        => CompilationFailureTest("grammar-privatenames-same-line-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-async-gen")]
    public Task grammar_special_meth_contains_super_async_gen()
        => CompilationFailureTest("grammar-special-meth-contains-super-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-async")]
    public Task grammar_special_meth_contains_super_async()
        => CompilationFailureTest("grammar-special-meth-contains-super-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-gen")]
    public Task grammar_special_meth_contains_super_gen()
        => CompilationFailureTest("grammar-special-meth-contains-super-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-get")]
    public Task grammar_special_meth_contains_super_get()
        => CompilationFailureTest("grammar-special-meth-contains-super-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-method")]
    public Task grammar_special_meth_contains_super_method()
        => CompilationFailureTest("grammar-special-meth-contains-super-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-async-gen")]
    public Task grammar_special_meth_contains_super_private_async_gen()
        => CompilationFailureTest("grammar-special-meth-contains-super-private-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-async")]
    public Task grammar_special_meth_contains_super_private_async()
        => CompilationFailureTest("grammar-special-meth-contains-super-private-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-gen")]
    public Task grammar_special_meth_contains_super_private_gen()
        => CompilationFailureTest("grammar-special-meth-contains-super-private-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-method")]
    public Task grammar_special_meth_contains_super_private_method()
        => CompilationFailureTest("grammar-special-meth-contains-super-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-set")]
    public Task grammar_special_meth_contains_super_set()
        => CompilationFailureTest("grammar-special-meth-contains-super-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-async-gen")]
    public Task grammar_special_meth_ctor_async_gen()
        => CompilationFailureTest("grammar-special-meth-ctor-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-async-meth")]
    public Task grammar_special_meth_ctor_async_meth()
        => CompilationFailureTest("grammar-special-meth-ctor-async-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-gen")]
    public Task grammar_special_meth_ctor_gen()
        => CompilationFailureTest("grammar-special-meth-ctor-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-get")]
    public Task grammar_special_meth_ctor_get()
        => CompilationFailureTest("grammar-special-meth-ctor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-set")]
    public Task grammar_special_meth_ctor_set()
        => CompilationFailureTest("grammar-special-meth-ctor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-gen-meth-prototype")]
    public Task grammar_static_async_gen_meth_prototype()
        => CompilationFailureTest("grammar-static-async-gen-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-gen-meth-super")]
    public Task grammar_static_async_gen_meth_super()
        => CompilationFailureTest("grammar-static-async-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-meth-prototype")]
    public Task grammar_static_async_meth_prototype()
        => CompilationFailureTest("grammar-static-async-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-meth-super")]
    public Task grammar_static_async_meth_super()
        => CompilationFailureTest("grammar-static-async-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-gen-meth-prototype")]
    public Task grammar_static_gen_meth_prototype()
        => CompilationFailureTest("grammar-static-gen-meth-prototype", "Failed to parse JavaScript");
}
