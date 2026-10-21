namespace Jroc.Test262.Tests.language.statements.test_class;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class") { }

    [Fact(DisplayName = "class-name-ident-await-escaped-module.js")]
    public Task class_name_ident_await_escaped_module()
        => CompilationFailureTest("class-name-ident-await-escaped-module", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-await-module.js")]
    public Task class_name_ident_await_module()
        => CompilationFailureTest("class-name-ident-await-module", "Failed to parse JavaScript");

    [Fact(DisplayName = "classelementname-abrupt-completion.js")]
    public Task classelementname_abrupt_completion()
        => ExecutionTest("classelementname-abrupt-completion");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_decl_accessors_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_decl_accessors_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-async-arrow-function-expression.js")]
    public Task cpn_class_decl_accessors_computed_property_name_from_async_arrow_function_expression()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-await-expression.js")]
    public Task cpn_class_decl_accessors_computed_property_name_from_await_expression()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_decl_accessors_computed_property_name_from_generator_function_declaration()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_decl_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_decl_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-async-arrow-function-expression.js")]
    public Task cpn_class_decl_computed_property_name_from_async_arrow_function_expression()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-await-expression.js")]
    public Task cpn_class_decl_computed_property_name_from_await_expression()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_decl_computed_property_name_from_generator_function_declaration()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_decl_fields_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_decl_fields_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-await-expression.js")]
    public Task cpn_class_decl_fields_computed_property_name_from_await_expression()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_decl_fields_computed_property_name_from_generator_function_declaration()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-yield-expression.js")]
    public Task cpn_class_decl_fields_computed_property_name_from_yield_expression()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-logical-and.js")]
    public Task cpn_class_decl_fields_methods_computed_property_name_from_assignment_expression_logical_and()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-logical-and");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-logical-or.js")]
    public Task cpn_class_decl_fields_methods_computed_property_name_from_assignment_expression_logical_or()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-logical-or");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-await-expression.js")]
    public Task cpn_class_decl_fields_methods_computed_property_name_from_await_expression()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-await-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-generator-function-declaration.js")]
    public Task cpn_class_decl_fields_methods_computed_property_name_from_generator_function_declaration()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-generator-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-yield-expression.js")]
    public Task cpn_class_decl_fields_methods_computed_property_name_from_yield_expression()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "cptn-decl.js", Skip = "eval is not supported.")]
    public Task cptn_decl()
        => ExecutionTest("cptn-decl");

    [Fact(DisplayName = "gen-method-length-dflt.js")]
    public Task gen_method_length_dflt()
        => ExecutionTest("gen-method-length-dflt");

    [Fact(DisplayName = "ident-name-method-def-typeof-escaped.js")]
    public Task ident_name_method_def_typeof_escaped()
        => ExecutionTest("ident-name-method-def-typeof-escaped");

    [Fact(DisplayName = "ident-name-method-def-var-escaped.js")]
    public Task ident_name_method_def_var_escaped()
        => ExecutionTest("ident-name-method-def-var-escaped");

    [Fact(DisplayName = "ident-name-method-def-void-escaped.js")]
    public Task ident_name_method_def_void_escaped()
        => ExecutionTest("ident-name-method-def-void-escaped");

    [Fact(DisplayName = "ident-name-method-def-while-escaped.js")]
    public Task ident_name_method_def_while_escaped()
        => ExecutionTest("ident-name-method-def-while-escaped");

    [Fact(DisplayName = "ident-name-method-def-with-escaped.js")]
    public Task ident_name_method_def_with_escaped()
        => ExecutionTest("ident-name-method-def-with-escaped");

    [Fact(DisplayName = "method-length-dflt.js")]
    public Task method_length_dflt()
        => ExecutionTest("method-length-dflt");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "params-dflt-gen-meth-args-unmapped.js")]
    public Task params_dflt_gen_meth_args_unmapped()
        => ExecutionTest("params-dflt-gen-meth-args-unmapped");

    [Fact(DisplayName = "params-dflt-gen-meth-ref-arguments.js")]
    public Task params_dflt_gen_meth_ref_arguments()
        => ExecutionTest("params-dflt-gen-meth-ref-arguments");

    [Fact(DisplayName = "params-dflt-gen-meth-static-args-unmapped.js")]
    public Task params_dflt_gen_meth_static_args_unmapped()
        => ExecutionTest("params-dflt-gen-meth-static-args-unmapped");

    [Fact(DisplayName = "params-dflt-gen-meth-static-ref-arguments.js")]
    public Task params_dflt_gen_meth_static_ref_arguments()
        => ExecutionTest("params-dflt-gen-meth-static-ref-arguments");

    [Fact(DisplayName = "params-dflt-meth-args-unmapped.js")]
    public Task params_dflt_meth_args_unmapped()
        => ExecutionTest("params-dflt-meth-args-unmapped");

    [Fact(DisplayName = "params-dflt-meth-ref-arguments.js")]
    public Task params_dflt_meth_ref_arguments()
        => ExecutionTest("params-dflt-meth-ref-arguments");

    [Fact(DisplayName = "params-dflt-meth-static-args-unmapped.js")]
    public Task params_dflt_meth_static_args_unmapped()
        => ExecutionTest("params-dflt-meth-static-args-unmapped");

    [Fact(DisplayName = "params-dflt-meth-static-ref-arguments.js")]
    public Task params_dflt_meth_static_ref_arguments()
        => ExecutionTest("params-dflt-meth-static-ref-arguments");

    [Fact(DisplayName = "poisoned-underscore-proto.js")]
    public Task poisoned_underscore_proto()
        => ExecutionTest("poisoned-underscore-proto");

    [Fact(DisplayName = "restricted-properties.js")]
    public Task restricted_properties()
        => ExecutionTest("restricted-properties");

    [Fact(DisplayName = "scope-gen-meth-paramsbody-var-close.js")]
    public Task scope_gen_meth_paramsbody_var_close()
        => ExecutionTest("scope-gen-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-gen-meth-paramsbody-var-open.js")]
    public Task scope_gen_meth_paramsbody_var_open()
        => ExecutionTest("scope-gen-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-meth-paramsbody-var-close.js")]
    public Task scope_meth_paramsbody_var_close()
        => ExecutionTest("scope-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-meth-paramsbody-var-open.js")]
    public Task scope_meth_paramsbody_var_open()
        => ExecutionTest("scope-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-name-lex-close.js")]
    public Task scope_name_lex_close()
        => ExecutionTest("scope-name-lex-close");

    [Fact(DisplayName = "scope-name-lex-open-heritage.js")]
    public Task scope_name_lex_open_heritage()
        => ExecutionTest("scope-name-lex-open-heritage");

    [Fact(DisplayName = "scope-name-lex-open-no-heritage.js")]
    public Task scope_name_lex_open_no_heritage()
        => ExecutionTest("scope-name-lex-open-no-heritage");

    [Fact(DisplayName = "scope-setter-paramsbody-var-close.js")]
    public Task scope_setter_paramsbody_var_close()
        => ExecutionTest("scope-setter-paramsbody-var-close");

    [Fact(DisplayName = "scope-setter-paramsbody-var-open.js")]
    public Task scope_setter_paramsbody_var_open()
        => ExecutionTest("scope-setter-paramsbody-var-open");

    [Fact(DisplayName = "scope-static-gen-meth-paramsbody-var-close.js")]
    public Task scope_static_gen_meth_paramsbody_var_close()
        => ExecutionTest("scope-static-gen-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-static-gen-meth-paramsbody-var-open.js")]
    public Task scope_static_gen_meth_paramsbody_var_open()
        => ExecutionTest("scope-static-gen-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-static-meth-paramsbody-var-close.js")]
    public Task scope_static_meth_paramsbody_var_close()
        => ExecutionTest("scope-static-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-static-meth-paramsbody-var-open.js")]
    public Task scope_static_meth_paramsbody_var_open()
        => ExecutionTest("scope-static-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-static-setter-paramsbody-var-close.js")]
    public Task scope_static_setter_paramsbody_var_close()
        => ExecutionTest("scope-static-setter-paramsbody-var-close");

    [Fact(DisplayName = "scope-static-setter-paramsbody-var-open.js")]
    public Task scope_static_setter_paramsbody_var_open()
        => ExecutionTest("scope-static-setter-paramsbody-var-open");

    [Fact(DisplayName = "setter-length-dflt.js")]
    public Task setter_length_dflt()
        => ExecutionTest("setter-length-dflt");

    [Fact(DisplayName = "static-classelementname-abrupt-completion.js")]
    public Task static_classelementname_abrupt_completion()
        => ExecutionTest("static-classelementname-abrupt-completion");

    [Fact(DisplayName = "static-init-abrupt.js")]
    public Task static_init_abrupt()
        => ExecutionTest("static-init-abrupt");

    [Fact(DisplayName = "static-init-arguments-functions.js")]
    public Task static_init_arguments_functions()
        => ExecutionTest("static-init-arguments-functions");

    [Fact(DisplayName = "static-init-await-binding-valid.js")]
    public Task static_init_await_binding_valid()
        => ExecutionTest("static-init-await-binding-valid");

    [Fact(DisplayName = "static-init-expr-new-target.js")]
    public Task static_init_expr_new_target()
        => ExecutionTest("static-init-expr-new-target");

    [Fact(DisplayName = "static-init-expr-this.js")]
    public Task static_init_expr_this()
        => ExecutionTest("static-init-expr-this");

    [Fact(DisplayName = "static-init-scope-lex-close.js")]
    public Task static_init_scope_lex_close()
        => ExecutionTest("static-init-scope-lex-close");

    [Fact(DisplayName = "static-init-scope-lex-derived.js")]
    public Task static_init_scope_lex_derived()
        => ExecutionTest("static-init-scope-lex-derived");

    [Fact(DisplayName = "static-init-scope-lex-open.js")]
    public Task static_init_scope_lex_open()
        => ExecutionTest("static-init-scope-lex-open");

    [Fact(DisplayName = "static-init-scope-private.js")]
    public Task static_init_scope_private()
        => ExecutionTest("static-init-scope-private");

    [Fact(DisplayName = "static-init-scope-var-close.js")]
    public Task static_init_scope_var_close()
        => ExecutionTest("static-init-scope-var-close");

    [Fact(DisplayName = "static-init-scope-var-derived.js")]
    public Task static_init_scope_var_derived()
        => ExecutionTest("static-init-scope-var-derived");

    [Fact(DisplayName = "static-init-scope-var-open.js")]
    public Task static_init_scope_var_open()
        => ExecutionTest("static-init-scope-var-open");

    [Fact(DisplayName = "static-init-sequence.js")]
    public Task static_init_sequence()
        => ExecutionTest("static-init-sequence");

    [Fact(DisplayName = "static-init-statement-list-optional.js")]
    public Task static_init_statement_list_optional()
        => ExecutionTest("static-init-statement-list-optional");

    [Fact(DisplayName = "static-method-gen-non-configurable-err.js")]
    public Task static_method_gen_non_configurable_err()
        => ExecutionTest("static-method-gen-non-configurable-err");

    [Fact(DisplayName = "static-method-length-dflt.js")]
    public Task static_method_length_dflt()
        => ExecutionTest("static-method-length-dflt");

    [Fact(DisplayName = "static-method-non-configurable-err.js")]
    public Task static_method_non_configurable_err()
        => ExecutionTest("static-method-non-configurable-err");
}
