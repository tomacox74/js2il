using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements;

public class StatementSyntaxConformance8Async_FunctionParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Async_FunctionParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "early-errors-declaration-NSPL-with-USD.js")]
    public Task test_async_function_early_errors_declaration_NSPL_with_USD()
        => CompilationFailureTest("async-function/early-errors-declaration-NSPL-with-USD", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-declaration-arguments-in-formal-parameters.js")]
    public Task test_async_function_early_errors_declaration_arguments_in_formal_parameters()
        => CompilationFailureTest("async-function/early-errors-declaration-arguments-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-declaration-binding-identifier-arguments.js")]
    public Task test_async_function_early_errors_declaration_binding_identifier_arguments()
        => CompilationFailureTest("async-function/early-errors-declaration-binding-identifier-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-declaration-binding-identifier-eval.js")]
    public Task test_async_function_early_errors_declaration_binding_identifier_eval()
        => CompilationFailureTest("async-function/early-errors-declaration-binding-identifier-eval", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-declaration-duplicate-parameters.js")]
    public Task test_async_function_early_errors_declaration_duplicate_parameters()
        => CompilationFailureTest("async-function/early-errors-declaration-duplicate-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-declaration-eval-in-formal-parameters.js")]
    public Task test_async_function_early_errors_declaration_eval_in_formal_parameters()
        => CompilationFailureTest("async-function/early-errors-declaration-eval-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-declaration-formals-contains-super-property.js")]
    public Task test_async_function_early_errors_declaration_formals_contains_super_property()
        => CompilationFailureTest("async-function/early-errors-declaration-formals-contains-super-property", "Failed to parse JavaScript");

    [Fact(DisplayName = "escaped-async.js")]
    public Task test_async_function_escaped_async()
        => CompilationFailureTest("async-function/escaped-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-newline-await-in-async-function.js")]
    public Task test_async_function_let_newline_await_in_async_function()
        => CompilationFailureTest("async-function/let-newline-await-in-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_async_function_object_destructuring_param_strict_body()
        => CompilationFailureTest("async-function/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_async_function_rest_param_strict_body()
        => CompilationFailureTest("async-function/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_async_function_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("async-function/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Async_GeneratorParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Async_GeneratorParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_async_generator_array_destructuring_param_strict_body()
        => CompilationFailureTest("async-generator/array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-binding-identifier-escaped.js")]
    public Task test_async_generator_await_as_binding_identifier_escaped()
        => CompilationFailureTest("async-generator/await-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-binding-identifier.js")]
    public Task test_async_generator_await_as_binding_identifier()
        => CompilationFailureTest("async-generator/await-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference-escaped.js")]
    public Task test_async_generator_await_as_identifier_reference_escaped()
        => CompilationFailureTest("async-generator/await-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference.js")]
    public Task test_async_generator_await_as_identifier_reference()
        => CompilationFailureTest("async-generator/await-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier-escaped.js")]
    public Task test_async_generator_await_as_label_identifier_escaped()
        => CompilationFailureTest("async-generator/await-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier.js")]
    public Task test_async_generator_await_as_label_identifier()
        => CompilationFailureTest("async-generator/await-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_async_generator_dflt_params_duplicates()
        => CompilationFailureTest("async-generator/dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_async_generator_dflt_params_rest()
        => CompilationFailureTest("async-generator/dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task test_async_generator_dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-generator/dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task test_async_generator_dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-generator/dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task test_async_generator_dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-generator/dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task test_async_generator_dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-generator/dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task test_async_generator_dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-generator/dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task test_async_generator_dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-generator/dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-ary.js")]
    public Task test_async_generator_dstr_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-generator/dstr/dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-id.js")]
    public Task test_async_generator_dstr_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-generator/dstr/dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-obj.js")]
    public Task test_async_generator_dstr_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-generator/dstr/dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-ary.js")]
    public Task test_async_generator_dstr_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-generator/dstr/dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-id.js")]
    public Task test_async_generator_dstr_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-generator/dstr/dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-obj.js")]
    public Task test_async_generator_dstr_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-generator/dstr/dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "escaped-async.js")]
    public Task test_async_generator_escaped_async()
        => CompilationFailureTest("async-generator/escaped-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_async_generator_object_destructuring_param_strict_body()
        => CompilationFailureTest("async-generator/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_async_generator_rest_param_strict_body()
        => CompilationFailureTest("async-generator/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_async_generator_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("async-generator/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_async_generator_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("async-generator/yield-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_async_generator_yield_as_binding_identifier()
        => CompilationFailureTest("async-generator/yield-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_async_generator_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("async-generator/yield-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_async_generator_yield_as_identifier_reference()
        => CompilationFailureTest("async-generator/yield-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_async_generator_yield_as_label_identifier_escaped()
        => CompilationFailureTest("async-generator/yield-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_async_generator_yield_as_label_identifier()
        => CompilationFailureTest("async-generator/yield-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_async_generator_yield_identifier_spread_strict()
        => CompilationFailureTest("async-generator/yield-identifier-spread-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_async_generator_yield_identifier_strict()
        => CompilationFailureTest("async-generator/yield-identifier-strict", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Await_UsingParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Await_UsingParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "redeclaration-error-from-within-strict-mode-function-await-using.js")]
    public Task test_await_using_redeclaration_error_from_within_strict_mode_function_await_using()
        => CompilationFailureTest("await-using/redeclaration-error-from-within-strict-mode-function-await-using", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-invalid-arraybindingpattern-after-bindingidentifier.js")]
    public Task test_await_using_syntax_await_using_invalid_arraybindingpattern_after_bindingidentifier()
        => CompilationFailureTest("await-using/syntax/await-using-invalid-arraybindingpattern-after-bindingidentifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-invalid-arraybindingpattern.js")]
    public Task test_await_using_syntax_await_using_invalid_arraybindingpattern()
        => CompilationFailureTest("await-using/syntax/await-using-invalid-arraybindingpattern", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-invalid-objectbindingpattern-after-bindingidentifier.js")]
    public Task test_await_using_syntax_await_using_invalid_objectbindingpattern_after_bindingidentifier()
        => CompilationFailureTest("await-using/syntax/await-using-invalid-objectbindingpattern-after-bindingidentifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-invalid-objectbindingpattern.js")]
    public Task test_await_using_syntax_await_using_invalid_objectbindingpattern()
        => CompilationFailureTest("await-using/syntax/await-using-invalid-objectbindingpattern", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-invalid-switchstatement-caseclause.js")]
    public Task test_await_using_syntax_await_using_invalid_switchstatement_caseclause()
        => CompilationFailureTest("await-using/syntax/await-using-invalid-switchstatement-caseclause", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-invalid-switchstatement-defaultclause.js")]
    public Task test_await_using_syntax_await_using_invalid_switchstatement_defaultclause()
        => CompilationFailureTest("await-using/syntax/await-using-invalid-switchstatement-defaultclause", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-using-not-allowed-at-top-level-of-script.js")]
    public Task test_await_using_syntax_await_using_not_allowed_at_top_level_of_script()
        => CompilationFailureTest("await-using/syntax/await-using-not-allowed-at-top-level-of-script", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-await-using-declarations-mixed-with-without-initializer.js")]
    public Task test_await_using_syntax_block_scope_syntax_await_using_declarations_mixed_with_without_initializer()
        => CompilationFailureTest("await-using/syntax/block-scope-syntax-await-using-declarations-mixed-with-without-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-await-using-declarations-mixed-without-with-initializer.js")]
    public Task test_await_using_syntax_block_scope_syntax_await_using_declarations_mixed_without_with_initializer()
        => CompilationFailureTest("await-using/syntax/block-scope-syntax-await-using-declarations-mixed-without-with-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-await-using-declarations-without-initializer.js")]
    public Task test_await_using_syntax_block_scope_syntax_await_using_declarations_without_initializer()
        => CompilationFailureTest("await-using/syntax/block-scope-syntax-await-using-declarations-without-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-case-expression-statement-list.js")]
    public Task test_await_using_syntax_with_initializer_case_expression_statement_list()
        => CompilationFailureTest("await-using/syntax/with-initializer-case-expression-statement-list", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-default-statement-list.js")]
    public Task test_await_using_syntax_with_initializer_default_statement_list()
        => CompilationFailureTest("await-using/syntax/with-initializer-default-statement-list", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-do-statement-while-expression.js")]
    public Task test_await_using_syntax_with_initializer_do_statement_while_expression()
        => CompilationFailureTest("await-using/syntax/with-initializer-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-for-statement.js")]
    public Task test_await_using_syntax_with_initializer_for_statement()
        => CompilationFailureTest("await-using/syntax/with-initializer-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-if-expression-statement-else-statement.js")]
    public Task test_await_using_syntax_with_initializer_if_expression_statement_else_statement()
        => CompilationFailureTest("await-using/syntax/with-initializer-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-if-expression-statement.js")]
    public Task test_await_using_syntax_with_initializer_if_expression_statement()
        => CompilationFailureTest("await-using/syntax/with-initializer-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-label-statement.js")]
    public Task test_await_using_syntax_with_initializer_label_statement()
        => CompilationFailureTest("await-using/syntax/with-initializer-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-while-expression-statement.js")]
    public Task test_await_using_syntax_with_initializer_while_expression_statement()
        => CompilationFailureTest("await-using/syntax/with-initializer-while-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-do-statement-while-expression.js")]
    public Task test_await_using_syntax_without_initializer_do_statement_while_expression()
        => CompilationFailureTest("await-using/syntax/without-initializer-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-for-statement.js")]
    public Task test_await_using_syntax_without_initializer_for_statement()
        => CompilationFailureTest("await-using/syntax/without-initializer-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-if-expression-statement-else-statement.js")]
    public Task test_await_using_syntax_without_initializer_if_expression_statement_else_statement()
        => CompilationFailureTest("await-using/syntax/without-initializer-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-if-expression-statement.js")]
    public Task test_await_using_syntax_without_initializer_if_expression_statement()
        => CompilationFailureTest("await-using/syntax/without-initializer-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-label-statement.js")]
    public Task test_await_using_syntax_without_initializer_label_statement()
        => CompilationFailureTest("await-using/syntax/without-initializer-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-while-expression-statement.js")]
    public Task test_await_using_syntax_without_initializer_while_expression_statement()
        => CompilationFailureTest("await-using/syntax/without-initializer-while-expression-statement", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8BlockParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8BlockParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "invalid-names-call-expression-bad-reference.js")]
    public Task test_block_early_errors_invalid_names_call_expression_bad_reference()
        => CompilationFailureTest("block/early-errors/invalid-names-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-call-expression-this.js")]
    public Task test_block_early_errors_invalid_names_call_expression_this()
        => CompilationFailureTest("block/early-errors/invalid-names-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-member-expression-bad-reference.js")]
    public Task test_block_early_errors_invalid_names_member_expression_bad_reference()
        => CompilationFailureTest("block/early-errors/invalid-names-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-member-expression-this.js")]
    public Task test_block_early_errors_invalid_names_member_expression_this()
        => CompilationFailureTest("block/early-errors/invalid-names-member-expression-this", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8BreakParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8BreakParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "S12.8_A1_T3.js")]
    public Task test_break_S12_8_A1_T3()
        => CompilationFailureTest("break/S12.8_A1_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A1_T4.js")]
    public Task test_break_S12_8_A1_T4()
        => CompilationFailureTest("break/S12.8_A1_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A5_T3.js")]
    public Task test_break_S12_8_A5_T3()
        => CompilationFailureTest("break/S12.8_A5_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A8_T1.js")]
    public Task test_break_S12_8_A8_T1()
        => CompilationFailureTest("break/S12.8_A8_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A8_T2.js")]
    public Task test_break_S12_8_A8_T2()
        => CompilationFailureTest("break/S12.8_A8_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-without-label.js")]
    public Task test_break_static_init_without_label()
        => CompilationFailureTest("break/static-init-without-label", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Class_1ParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Class_1ParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-field-init.js")]
    public Task test_class_elements_syntax_early_errors_grammar_privatename_whitespace_error_static_field_init()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-privatename-whitespace-error-static-field-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-field.js")]
    public Task test_class_elements_syntax_early_errors_grammar_privatename_whitespace_error_static_field()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-privatename-whitespace-error-static-field", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-gen-meth.js")]
    public Task test_class_elements_syntax_early_errors_grammar_privatename_whitespace_error_static_gen_meth()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-privatename-whitespace-error-static-gen-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatename-whitespace-error-static-meth.js")]
    public Task test_class_elements_syntax_early_errors_grammar_privatename_whitespace_error_static_meth()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-privatename-whitespace-error-static-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-privatenames-same-line-error.js")]
    public Task test_class_elements_syntax_early_errors_grammar_privatenames_same_line_error()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-privatenames-same-line-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-async-gen.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_async_gen()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-async.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_async()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-gen.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_gen()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-get.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_get()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-method.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_method()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-async-gen.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_private_async_gen()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-private-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-async.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_private_async()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-private-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-gen.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_private_gen()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-private-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-private-method.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_private_method()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-private-method", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-contains-super-set.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_contains_super_set()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-contains-super-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-async-gen.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_ctor_async_gen()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-ctor-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-async-meth.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_ctor_async_meth()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-ctor-async-meth", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-gen.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_ctor_gen()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-ctor-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-get.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_ctor_get()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-ctor-get", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-special-meth-ctor-set.js")]
    public Task test_class_elements_syntax_early_errors_grammar_special_meth_ctor_set()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-special-meth-ctor-set", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-gen-meth-prototype.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_async_gen_meth_prototype()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-async-gen-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-gen-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_async_gen_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-async-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-meth-prototype.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_async_meth_prototype()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-async-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-async-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_async_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-async-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-gen-meth-prototype.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_gen_meth_prototype()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-gen-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-gen-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_gen_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-get-meth-prototype.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_get_meth_prototype()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-get-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-get-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_get_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-get-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-meth-prototype.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_meth_prototype()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-async-gen-meth-constructor.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_async_gen_meth_constructor()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-async-gen-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-async-gen-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_async_gen_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-async-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-async-meth-constructor.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_async_meth_constructor()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-async-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-async-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_async_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-async-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-gen-meth-constructor.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_gen_meth_constructor()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-gen-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-gen-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_gen_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-meth-constructor.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_meth_constructor()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-private-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_private_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-private-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-privatename-constructor.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_privatename_constructor()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-privatename-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-set-meth-prototype.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_set_meth_prototype()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-set-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "grammar-static-set-meth-super.js")]
    public Task test_class_elements_syntax_early_errors_grammar_static_set_meth_super()
        => CompilationFailureTest("class/elements/syntax/early-errors/grammar-static-set-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-fn-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_fn_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-fn-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-fn-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_fn_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-fn-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-fn-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_fn_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-fn-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-fn-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_fn_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-fn-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "field-init-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_field_init_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/field-init-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-init-heritage-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_fields_init_heritage_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/fields-init-heritage-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-init-heritage-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_fields_init_heritage_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/fields-init-heritage-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-init-heritage-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_fields_init_heritage_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/fields-init-heritage-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-init-heritage-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_fields_init_heritage_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/fields-init-heritage-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-fn-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_fn_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-fn-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-fn-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_fn_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-fn-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-fn-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_fn_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-fn-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-fn-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_fn_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-fn-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-heritage-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_heritage_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-heritage-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-heritage-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_heritage_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-heritage-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-heritage-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_heritage_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-heritage-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-heritage-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_heritage_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-heritage-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-outter-call-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_outter_call_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-outter-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-outter-call-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_outter_call_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-outter-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-outter-member-expression-bad-reference.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_outter_member_expression_bad_reference()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-outter-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-outter-member-expression-this.js")]
    public Task test_class_elements_syntax_early_errors_invalid_names_method_outter_member_expression_this()
        => CompilationFailureTest("class/elements/syntax/early-errors/invalid-names/method-outter-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-async-generator-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_async_generator_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-async-generator-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-async-method-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_async_method_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-async-method-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-call-exp-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_call_exp_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-call-exp-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-field-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_field_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-field-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-generator-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_generator_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-generator-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-member-exp-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_member_exp_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-member-exp-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-method-cannot-escape-token.js")]
    public Task test_class_elements_syntax_early_errors_private_method_cannot_escape_token()
        => CompilationFailureTest("class/elements/syntax/early-errors/private-method-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "super-private-access-invalid.js")]
    public Task test_class_elements_syntax_early_errors_super_private_access_invalid()
        => CompilationFailureTest("class/elements/syntax/early-errors/super-private-access-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "ternary-init-err-contains-arguments.js")]
    public Task test_class_elements_ternary_init_err_contains_arguments()
        => CompilationFailureTest("class/elements/ternary-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "ternary-init-err-contains-super.js")]
    public Task test_class_elements_ternary_init_err_contains_super()
        => CompilationFailureTest("class/elements/ternary-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "typeof-init-err-contains-arguments.js")]
    public Task test_class_elements_typeof_init_err_contains_arguments()
        => CompilationFailureTest("class/elements/typeof-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "typeof-init-err-contains-super.js")]
    public Task test_class_elements_typeof_init_err_contains_super()
        => CompilationFailureTest("class/elements/typeof-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-method-param-dflt-yield.js")]
    public Task test_class_gen_method_param_dflt_yield()
        => CompilationFailureTest("class/gen-method-param-dflt-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_static_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method-static/array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_gen_method_static_dflt_params_duplicates()
        => CompilationFailureTest("class/gen-method-static/dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_gen_method_static_dflt_params_rest()
        => CompilationFailureTest("class/gen-method-static/dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_static_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method-static/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_gen_method_static_rest_param_strict_body()
        => CompilationFailureTest("class/gen-method-static/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_gen_method_static_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/gen-method-static/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_class_gen_method_static_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("class/gen-method-static/yield-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_class_gen_method_static_yield_as_binding_identifier()
        => CompilationFailureTest("class/gen-method-static/yield-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_class_gen_method_static_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("class/gen-method-static/yield-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_class_gen_method_static_yield_as_identifier_reference()
        => CompilationFailureTest("class/gen-method-static/yield-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_class_gen_method_static_yield_as_label_identifier_escaped()
        => CompilationFailureTest("class/gen-method-static/yield-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_class_gen_method_static_yield_as_label_identifier()
        => CompilationFailureTest("class/gen-method-static/yield-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_class_gen_method_static_yield_identifier_spread_strict()
        => CompilationFailureTest("class/gen-method-static/yield-identifier-spread-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_class_gen_method_static_yield_identifier_strict()
        => CompilationFailureTest("class/gen-method-static/yield-identifier-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "getter-param-dflt.js")]
    public Task test_class_getter_param_dflt()
        => CompilationFailureTest("class/getter-param-dflt", "Failed to parse JavaScript");

    [Fact(DisplayName = "method-param-yield.js")]
    public Task test_class_method_param_yield()
        => CompilationFailureTest("class/method-param-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-non-static-getter-static-setter-early-error.js")]
    public Task test_class_private_non_static_getter_static_setter_early_error()
        => CompilationFailureTest("class/private-non-static-getter-static-setter-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-non-static-setter-static-getter-early-error.js")]
    public Task test_class_private_non_static_setter_static_getter_early_error()
        => CompilationFailureTest("class/private-non-static-setter-static-getter-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-static-getter-non-static-setter-early-error.js")]
    public Task test_class_private_static_getter_non_static_setter_early_error()
        => CompilationFailureTest("class/private-static-getter-non-static-setter-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-static-setter-non-static-getter-early-error.js")]
    public Task test_class_private_static_setter_non_static_getter_early_error()
        => CompilationFailureTest("class/private-static-setter-non-static-getter-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-gen-method-param-dflt-yield.js")]
    public Task test_class_static_gen_method_param_dflt_yield()
        => CompilationFailureTest("class/static-gen-method-param-dflt-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-invalid.js")]
    public Task test_class_static_init_await_binding_invalid()
        => CompilationFailureTest("class/static-init-await-binding-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-arguments.js")]
    public Task test_class_static_init_invalid_arguments()
        => CompilationFailureTest("class/static-init-invalid-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-await.js")]
    public Task test_class_static_init_invalid_await()
        => CompilationFailureTest("class/static-init-invalid-await", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-label-dup.js")]
    public Task test_class_static_init_invalid_label_dup()
        => CompilationFailureTest("class/static-init-invalid-label-dup", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-lex-dup.js")]
    public Task test_class_static_init_invalid_lex_dup()
        => CompilationFailureTest("class/static-init-invalid-lex-dup", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-lex-var.js")]
    public Task test_class_static_init_invalid_lex_var()
        => CompilationFailureTest("class/static-init-invalid-lex-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-return.js")]
    public Task test_class_static_init_invalid_return()
        => CompilationFailureTest("class/static-init-invalid-return", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-super-call.js")]
    public Task test_class_static_init_invalid_super_call()
        => CompilationFailureTest("class/static-init-invalid-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-undefined-break-target.js")]
    public Task test_class_static_init_invalid_undefined_break_target()
        => CompilationFailureTest("class/static-init-invalid-undefined-break-target", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-undefined-continue-target.js")]
    public Task test_class_static_init_invalid_undefined_continue_target()
        => CompilationFailureTest("class/static-init-invalid-undefined-continue-target", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-yield.js")]
    public Task test_class_static_init_invalid_yield()
        => CompilationFailureTest("class/static-init-invalid-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-method-param-yield.js")]
    public Task test_class_static_method_param_yield()
        => CompilationFailureTest("class/static-method-param-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "with.js")]
    public Task test_class_strict_mode_with()
        => CompilationFailureTest("class/strict-mode/with", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-definition-evaluation-block-duplicate-binding.js")]
    public Task test_class_syntax_early_errors_class_definition_evaluation_block_duplicate_binding()
        => CompilationFailureTest("class/syntax/early-errors/class-definition-evaluation-block-duplicate-binding", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-definition-evaluation-scriptbody-duplicate-binding.js")]
    public Task test_class_syntax_early_errors_class_definition_evaluation_scriptbody_duplicate_binding()
        => CompilationFailureTest("class/syntax/early-errors/class-definition-evaluation-scriptbody-duplicate-binding", "Failed to parse JavaScript");

    [Fact(DisplayName = "escaped-static.js")]
    public Task test_class_syntax_escaped_static()
        => CompilationFailureTest("class/syntax/escaped-static", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Class_2ParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Class_2ParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method/array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_gen_method_dflt_params_duplicates()
        => CompilationFailureTest("class/gen-method/dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_gen_method_dflt_params_rest()
        => CompilationFailureTest("class/gen-method/dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_gen_method_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/gen-method/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_gen_method_rest_param_strict_body()
        => CompilationFailureTest("class/gen-method/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_gen_method_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/gen-method/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_class_gen_method_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("class/gen-method/yield-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_class_gen_method_yield_as_binding_identifier()
        => CompilationFailureTest("class/gen-method/yield-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_class_gen_method_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("class/gen-method/yield-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_class_gen_method_yield_as_identifier_reference()
        => CompilationFailureTest("class/gen-method/yield-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_class_gen_method_yield_as_label_identifier_escaped()
        => CompilationFailureTest("class/gen-method/yield-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_class_gen_method_yield_as_label_identifier()
        => CompilationFailureTest("class/gen-method/yield-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_class_gen_method_yield_identifier_spread_strict()
        => CompilationFailureTest("class/gen-method/yield-identifier-spread-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_class_gen_method_yield_identifier_strict()
        => CompilationFailureTest("class/gen-method/yield-identifier-strict", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Class_3ParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Class_3ParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_method_static_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/method-static/array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_method_static_dflt_params_duplicates()
        => CompilationFailureTest("class/method-static/dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_method_static_dflt_params_rest()
        => CompilationFailureTest("class/method-static/dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_method_static_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/method-static/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_method_static_rest_param_strict_body()
        => CompilationFailureTest("class/method-static/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_method_static_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/method-static/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Class_4ParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Class_4ParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_class_method_array_destructuring_param_strict_body()
        => CompilationFailureTest("class/method/array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_class_method_dflt_params_duplicates()
        => CompilationFailureTest("class/method/dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_class_method_dflt_params_rest()
        => CompilationFailureTest("class/method/dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_class_method_object_destructuring_param_strict_body()
        => CompilationFailureTest("class/method/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_class_method_rest_param_strict_body()
        => CompilationFailureTest("class/method/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_class_method_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("class/method/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8ConstParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8ConstParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task test_const_dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("const/dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task test_const_dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("const/dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task test_const_dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("const/dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task test_const_dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("const/dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task test_const_dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("const/dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task test_const_dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("const/dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "redeclaration-error-from-within-strict-mode-function-const.js")]
    public Task test_const_redeclaration_error_from_within_strict_mode_function_const()
        => CompilationFailureTest("const/redeclaration-error-from-within-strict-mode-function-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-invalid.js")]
    public Task test_const_static_init_await_binding_invalid()
        => CompilationFailureTest("const/static-init-await-binding-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-const-declarations-mixed-with-without-initialiser.js")]
    public Task test_const_syntax_block_scope_syntax_const_declarations_mixed_with_without_initialiser()
        => CompilationFailureTest("const/syntax/block-scope-syntax-const-declarations-mixed-with-without-initialiser", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-const-declarations-mixed-without-with-initialiser.js")]
    public Task test_const_syntax_block_scope_syntax_const_declarations_mixed_without_with_initialiser()
        => CompilationFailureTest("const/syntax/block-scope-syntax-const-declarations-mixed-without-with-initialiser", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-const-declarations-without-initialiser.js")]
    public Task test_const_syntax_block_scope_syntax_const_declarations_without_initialiser()
        => CompilationFailureTest("const/syntax/block-scope-syntax-const-declarations-without-initialiser", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-declaring-let-split-across-two-lines.js")]
    public Task test_const_syntax_const_declaring_let_split_across_two_lines()
        => CompilationFailureTest("const/syntax/const-declaring-let-split-across-two-lines", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-do-statement-while-expression.js")]
    public Task test_const_syntax_with_initializer_do_statement_while_expression()
        => CompilationFailureTest("const/syntax/with-initializer-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-for-statement.js")]
    public Task test_const_syntax_with_initializer_for_statement()
        => CompilationFailureTest("const/syntax/with-initializer-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-if-expression-statement-else-statement.js")]
    public Task test_const_syntax_with_initializer_if_expression_statement_else_statement()
        => CompilationFailureTest("const/syntax/with-initializer-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-if-expression-statement.js")]
    public Task test_const_syntax_with_initializer_if_expression_statement()
        => CompilationFailureTest("const/syntax/with-initializer-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-label-statement.js")]
    public Task test_const_syntax_with_initializer_label_statement()
        => CompilationFailureTest("const/syntax/with-initializer-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-while-expression-statement.js")]
    public Task test_const_syntax_with_initializer_while_expression_statement()
        => CompilationFailureTest("const/syntax/with-initializer-while-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-case-expression-statement-list.js")]
    public Task test_const_syntax_without_initializer_case_expression_statement_list()
        => CompilationFailureTest("const/syntax/without-initializer-case-expression-statement-list", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-default-statement-list.js")]
    public Task test_const_syntax_without_initializer_default_statement_list()
        => CompilationFailureTest("const/syntax/without-initializer-default-statement-list", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-do-statement-while-expression.js")]
    public Task test_const_syntax_without_initializer_do_statement_while_expression()
        => CompilationFailureTest("const/syntax/without-initializer-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-for-statement.js")]
    public Task test_const_syntax_without_initializer_for_statement()
        => CompilationFailureTest("const/syntax/without-initializer-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-if-expression-statement-else-statement.js")]
    public Task test_const_syntax_without_initializer_if_expression_statement_else_statement()
        => CompilationFailureTest("const/syntax/without-initializer-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-if-expression-statement.js")]
    public Task test_const_syntax_without_initializer_if_expression_statement()
        => CompilationFailureTest("const/syntax/without-initializer-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-label-statement.js")]
    public Task test_const_syntax_without_initializer_label_statement()
        => CompilationFailureTest("const/syntax/without-initializer-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-while-expression-statement.js")]
    public Task test_const_syntax_without_initializer_while_expression_statement()
        => CompilationFailureTest("const/syntax/without-initializer-while-expression-statement", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8ContinueParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8ContinueParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "S12.7_A1_T3.js")]
    public Task test_continue_S12_7_A1_T3()
        => CompilationFailureTest("continue/S12.7_A1_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A1_T4.js")]
    public Task test_continue_S12_7_A1_T4()
        => CompilationFailureTest("continue/S12.7_A1_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A5_T3.js")]
    public Task test_continue_S12_7_A5_T3()
        => CompilationFailureTest("continue/S12.7_A5_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A8_T1.js")]
    public Task test_continue_S12_7_A8_T1()
        => CompilationFailureTest("continue/S12.7_A8_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A8_T2.js")]
    public Task test_continue_S12_7_A8_T2()
        => CompilationFailureTest("continue/S12.7_A8_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-with-label.js")]
    public Task test_continue_static_init_with_label()
        => CompilationFailureTest("continue/static-init-with-label", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-without-label.js")]
    public Task test_continue_static_init_without_label()
        => CompilationFailureTest("continue/static-init-without-label", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8DebuggerParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8DebuggerParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "expression.js")]
    public Task test_debugger_expression()
        => CompilationFailureTest("debugger/expression", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8Do_WhileParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8Do_WhileParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "decl-async-fun.js")]
    public Task test_do_while_decl_async_fun()
        => CompilationFailureTest("do-while/decl-async-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-async-gen.js")]
    public Task test_do_while_decl_async_gen()
        => CompilationFailureTest("do-while/decl-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-cls.js")]
    public Task test_do_while_decl_cls()
        => CompilationFailureTest("do-while/decl-cls", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-gen.js")]
    public Task test_do_while_decl_gen()
        => CompilationFailureTest("do-while/decl-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt.js")]
    public Task test_do_while_labelled_fn_stmt()
        => CompilationFailureTest("do-while/labelled-fn-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-array-with-newline.js")]
    public Task test_do_while_let_array_with_newline()
        => CompilationFailureTest("do-while/let-array-with-newline", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8ForParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8ForParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "S12.6.3_A11.1_T3.js")]
    public Task test_for_S12_6_3_A11_1_T3()
        => CompilationFailureTest("for/S12.6.3_A11.1_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A11_T3.js")]
    public Task test_for_S12_6_3_A11_T3()
        => CompilationFailureTest("for/S12.6.3_A11_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A12.1_T3.js")]
    public Task test_for_S12_6_3_A12_1_T3()
        => CompilationFailureTest("for/S12.6.3_A12.1_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A12_T3.js")]
    public Task test_for_S12_6_3_A12_T3()
        => CompilationFailureTest("for/S12.6.3_A12_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A4.1.js")]
    public Task test_for_S12_6_3_A4_1()
        => CompilationFailureTest("for/S12.6.3_A4.1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A4_T1.js")]
    public Task test_for_S12_6_3_A4_T1()
        => CompilationFailureTest("for/S12.6.3_A4_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A4_T2.js")]
    public Task test_for_S12_6_3_A4_T2()
        => CompilationFailureTest("for/S12.6.3_A4_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A7.1_T1.js")]
    public Task test_for_S12_6_3_A7_1_T1()
        => CompilationFailureTest("for/S12.6.3_A7.1_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A7.1_T2.js")]
    public Task test_for_S12_6_3_A7_1_T2()
        => CompilationFailureTest("for/S12.6.3_A7.1_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A7_T1.js")]
    public Task test_for_S12_6_3_A7_T1()
        => CompilationFailureTest("for/S12.6.3_A7_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A7_T2.js")]
    public Task test_for_S12_6_3_A7_T2()
        => CompilationFailureTest("for/S12.6.3_A7_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A8.1_T1.js")]
    public Task test_for_S12_6_3_A8_1_T1()
        => CompilationFailureTest("for/S12.6.3_A8.1_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A8.1_T2.js")]
    public Task test_for_S12_6_3_A8_1_T2()
        => CompilationFailureTest("for/S12.6.3_A8.1_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A8.1_T3.js")]
    public Task test_for_S12_6_3_A8_1_T3()
        => CompilationFailureTest("for/S12.6.3_A8.1_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A8_T1.js")]
    public Task test_for_S12_6_3_A8_T1()
        => CompilationFailureTest("for/S12.6.3_A8_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A8_T2.js")]
    public Task test_for_S12_6_3_A8_T2()
        => CompilationFailureTest("for/S12.6.3_A8_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.3_A8_T3.js")]
    public Task test_for_S12_6_3_A8_T3()
        => CompilationFailureTest("for/S12.6.3_A8_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-async-fun.js")]
    public Task test_for_decl_async_fun()
        => CompilationFailureTest("for/decl-async-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-async-gen.js")]
    public Task test_for_decl_async_gen()
        => CompilationFailureTest("for/decl-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-cls.js")]
    public Task test_for_decl_cls()
        => CompilationFailureTest("for/decl-cls", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-const.js")]
    public Task test_for_decl_const()
        => CompilationFailureTest("for/decl-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-fun.js")]
    public Task test_for_decl_fun()
        => CompilationFailureTest("for/decl-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-gen.js")]
    public Task test_for_decl_gen()
        => CompilationFailureTest("for/decl-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-let.js")]
    public Task test_for_decl_let()
        => CompilationFailureTest("for/decl-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-ary-ptrn-rest-init-ary.js")]
    public Task test_for_dstr_const_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("for/dstr/const-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-ary-ptrn-rest-init-id.js")]
    public Task test_for_dstr_const_ary_ptrn_rest_init_id()
        => CompilationFailureTest("for/dstr/const-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-ary-ptrn-rest-init-obj.js")]
    public Task test_for_dstr_const_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("for/dstr/const-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-ary-ptrn-rest-not-final-ary.js")]
    public Task test_for_dstr_const_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("for/dstr/const-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-ary-ptrn-rest-not-final-id.js")]
    public Task test_for_dstr_const_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("for/dstr/const-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-ary-ptrn-rest-not-final-obj.js")]
    public Task test_for_dstr_const_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("for/dstr/const-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-ary-ptrn-rest-init-ary.js")]
    public Task test_for_dstr_let_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("for/dstr/let-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-ary-ptrn-rest-init-id.js")]
    public Task test_for_dstr_let_ary_ptrn_rest_init_id()
        => CompilationFailureTest("for/dstr/let-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-ary-ptrn-rest-init-obj.js")]
    public Task test_for_dstr_let_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("for/dstr/let-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-ary-ptrn-rest-not-final-ary.js")]
    public Task test_for_dstr_let_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("for/dstr/let-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-ary-ptrn-rest-not-final-id.js")]
    public Task test_for_dstr_let_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("for/dstr/let-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-ary-ptrn-rest-not-final-obj.js")]
    public Task test_for_dstr_let_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("for/dstr/let-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-ary-ptrn-rest-init-ary.js")]
    public Task test_for_dstr_var_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("for/dstr/var-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-ary-ptrn-rest-init-id.js")]
    public Task test_for_dstr_var_ary_ptrn_rest_init_id()
        => CompilationFailureTest("for/dstr/var-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-ary-ptrn-rest-init-obj.js")]
    public Task test_for_dstr_var_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("for/dstr/var-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-ary-ptrn-rest-not-final-ary.js")]
    public Task test_for_dstr_var_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("for/dstr/var-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-ary-ptrn-rest-not-final-id.js")]
    public Task test_for_dstr_var_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("for/dstr/var-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-ary-ptrn-rest-not-final-obj.js")]
    public Task test_for_dstr_var_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("for/dstr/var-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "head-const-bound-names-in-stmt.js")]
    public Task test_for_head_const_bound_names_in_stmt()
        => CompilationFailureTest("for/head-const-bound-names-in-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "head-let-bound-names-in-stmt.js")]
    public Task test_for_head_let_bound_names_in_stmt()
        => CompilationFailureTest("for/head-let-bound-names-in-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-const.js")]
    public Task test_for_labelled_fn_stmt_const()
        => CompilationFailureTest("for/labelled-fn-stmt-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-expr.js")]
    public Task test_for_labelled_fn_stmt_expr()
        => CompilationFailureTest("for/labelled-fn-stmt-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-let.js")]
    public Task test_for_labelled_fn_stmt_let()
        => CompilationFailureTest("for/labelled-fn-stmt-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-var.js")]
    public Task test_for_labelled_fn_stmt_var()
        => CompilationFailureTest("for/labelled-fn-stmt-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-array-with-newline.js")]
    public Task test_for_let_array_with_newline()
        => CompilationFailureTest("for/let-array-with-newline", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8GeneratorsParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8GeneratorsParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task test_generators_array_destructuring_param_strict_body()
        => CompilationFailureTest("generators/array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task test_generators_dflt_params_duplicates()
        => CompilationFailureTest("generators/dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task test_generators_dflt_params_rest()
        => CompilationFailureTest("generators/dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task test_generators_dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("generators/dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task test_generators_dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("generators/dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task test_generators_dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("generators/dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task test_generators_dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("generators/dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task test_generators_dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("generators/dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task test_generators_dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("generators/dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-ary.js")]
    public Task test_generators_dstr_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("generators/dstr/dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-id.js")]
    public Task test_generators_dstr_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("generators/dstr/dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-obj.js")]
    public Task test_generators_dstr_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("generators/dstr/dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-ary.js")]
    public Task test_generators_dstr_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("generators/dstr/dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-id.js")]
    public Task test_generators_dstr_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("generators/dstr/dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-obj.js")]
    public Task test_generators_dstr_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("generators/dstr/dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task test_generators_object_destructuring_param_strict_body()
        => CompilationFailureTest("generators/object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "param-dflt-yield.js")]
    public Task test_generators_param_dflt_yield()
        => CompilationFailureTest("generators/param-dflt-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task test_generators_rest_param_strict_body()
        => CompilationFailureTest("generators/rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task test_generators_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("generators/rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "use-strict-with-non-simple-param.js")]
    public Task test_generators_use_strict_with_non_simple_param()
        => CompilationFailureTest("generators/use-strict-with-non-simple-param", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier-escaped.js")]
    public Task test_generators_yield_as_binding_identifier_escaped()
        => CompilationFailureTest("generators/yield-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier.js")]
    public Task test_generators_yield_as_binding_identifier()
        => CompilationFailureTest("generators/yield-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped.js")]
    public Task test_generators_yield_as_identifier_reference_escaped()
        => CompilationFailureTest("generators/yield-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference.js")]
    public Task test_generators_yield_as_identifier_reference()
        => CompilationFailureTest("generators/yield-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier-escaped.js")]
    public Task test_generators_yield_as_label_identifier_escaped()
        => CompilationFailureTest("generators/yield-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier.js")]
    public Task test_generators_yield_as_label_identifier()
        => CompilationFailureTest("generators/yield-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-logical-or-expression.js")]
    public Task test_generators_yield_as_logical_or_expression()
        => CompilationFailureTest("generators/yield-as-logical-or-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-parameter.js")]
    public Task test_generators_yield_as_parameter()
        => CompilationFailureTest("generators/yield-as-parameter", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-spread-strict.js")]
    public Task test_generators_yield_identifier_spread_strict()
        => CompilationFailureTest("generators/yield-identifier-spread-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-strict.js")]
    public Task test_generators_yield_identifier_strict()
        => CompilationFailureTest("generators/yield-identifier-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-star-after-newline.js")]
    public Task test_generators_yield_star_after_newline()
        => CompilationFailureTest("generators/yield-star-after-newline", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-weak-binding.js")]
    public Task test_generators_yield_weak_binding()
        => CompilationFailureTest("generators/yield-weak-binding", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8IfParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8IfParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "if-async-fun-else-async-fun.js")]
    public Task test_if_if_async_fun_else_async_fun()
        => CompilationFailureTest("if/if-async-fun-else-async-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-async-fun-else-stmt.js")]
    public Task test_if_if_async_fun_else_stmt()
        => CompilationFailureTest("if/if-async-fun-else-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-async-fun-no-else.js")]
    public Task test_if_if_async_fun_no_else()
        => CompilationFailureTest("if/if-async-fun-no-else", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-async-gen-else-async-gen.js")]
    public Task test_if_if_async_gen_else_async_gen()
        => CompilationFailureTest("if/if-async-gen-else-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-async-gen-else-stmt.js")]
    public Task test_if_if_async_gen_else_stmt()
        => CompilationFailureTest("if/if-async-gen-else-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-async-gen-no-else.js")]
    public Task test_if_if_async_gen_no_else()
        => CompilationFailureTest("if/if-async-gen-no-else", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-cls-else-cls.js")]
    public Task test_if_if_cls_else_cls()
        => CompilationFailureTest("if/if-cls-else-cls", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-cls-else-stmt.js")]
    public Task test_if_if_cls_else_stmt()
        => CompilationFailureTest("if/if-cls-else-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-const-else-const.js")]
    public Task test_if_if_const_else_const()
        => CompilationFailureTest("if/if-const-else-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-decl-else-decl-strict.js")]
    public Task test_if_if_decl_else_decl_strict()
        => CompilationFailureTest("if/if-decl-else-decl-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-decl-else-stmt-strict.js")]
    public Task test_if_if_decl_else_stmt_strict()
        => CompilationFailureTest("if/if-decl-else-stmt-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-decl-no-else-strict.js")]
    public Task test_if_if_decl_no_else_strict()
        => CompilationFailureTest("if/if-decl-no-else-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-fun-else-fun-strict.js")]
    public Task test_if_if_fun_else_fun_strict()
        => CompilationFailureTest("if/if-fun-else-fun-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-fun-else-stmt-strict.js")]
    public Task test_if_if_fun_else_stmt_strict()
        => CompilationFailureTest("if/if-fun-else-stmt-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-fun-no-else-strict.js")]
    public Task test_if_if_fun_no_else_strict()
        => CompilationFailureTest("if/if-fun-no-else-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-gen-else-gen.js")]
    public Task test_if_if_gen_else_gen()
        => CompilationFailureTest("if/if-gen-else-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-gen-else-stmt.js")]
    public Task test_if_if_gen_else_stmt()
        => CompilationFailureTest("if/if-gen-else-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-gen-no-else.js")]
    public Task test_if_if_gen_no_else()
        => CompilationFailureTest("if/if-gen-no-else", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-let-else-let.js")]
    public Task test_if_if_let_else_let()
        => CompilationFailureTest("if/if-let-else-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-stmt-else-async-fun.js")]
    public Task test_if_if_stmt_else_async_fun()
        => CompilationFailureTest("if/if-stmt-else-async-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-stmt-else-async-gen.js")]
    public Task test_if_if_stmt_else_async_gen()
        => CompilationFailureTest("if/if-stmt-else-async-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-stmt-else-cls.js")]
    public Task test_if_if_stmt_else_cls()
        => CompilationFailureTest("if/if-stmt-else-cls", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-stmt-else-decl-strict.js")]
    public Task test_if_if_stmt_else_decl_strict()
        => CompilationFailureTest("if/if-stmt-else-decl-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-stmt-else-fun-strict.js")]
    public Task test_if_if_stmt_else_fun_strict()
        => CompilationFailureTest("if/if-stmt-else-fun-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "if-stmt-else-gen.js")]
    public Task test_if_if_stmt_else_gen()
        => CompilationFailureTest("if/if-stmt-else-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-first.js")]
    public Task test_if_labelled_fn_stmt_first()
        => CompilationFailureTest("if/labelled-fn-stmt-first", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-lone.js")]
    public Task test_if_labelled_fn_stmt_lone()
        => CompilationFailureTest("if/labelled-fn-stmt-lone", "Failed to parse JavaScript");

    [Fact(DisplayName = "labelled-fn-stmt-second.js")]
    public Task test_if_labelled_fn_stmt_second()
        => CompilationFailureTest("if/labelled-fn-stmt-second", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-array-with-newline.js")]
    public Task test_if_let_array_with_newline()
        => CompilationFailureTest("if/let-array-with-newline", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8LabeledParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8LabeledParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "decl-async-function.js")]
    public Task test_labeled_decl_async_function()
        => CompilationFailureTest("labeled/decl-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-async-generator.js")]
    public Task test_labeled_decl_async_generator()
        => CompilationFailureTest("labeled/decl-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-cls.js")]
    public Task test_labeled_decl_cls()
        => CompilationFailureTest("labeled/decl-cls", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-fun-strict.js")]
    public Task test_labeled_decl_fun_strict()
        => CompilationFailureTest("labeled/decl-fun-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-gen.js")]
    public Task test_labeled_decl_gen()
        => CompilationFailureTest("labeled/decl-gen", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-let.js")]
    public Task test_labeled_decl_let()
        => CompilationFailureTest("labeled/decl-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-invalid-await.js")]
    public Task test_labeled_static_init_invalid_await()
        => CompilationFailureTest("labeled/static-init-invalid-await", "Failed to parse JavaScript");

    [Fact(DisplayName = "value-yield-strict-escaped.js")]
    public Task test_labeled_value_yield_strict_escaped()
        => CompilationFailureTest("labeled/value-yield-strict-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "value-yield-strict.js")]
    public Task test_labeled_value_yield_strict()
        => CompilationFailureTest("labeled/value-yield-strict", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8LetParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8LetParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task test_let_dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("let/dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task test_let_dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("let/dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task test_let_dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("let/dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task test_let_dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("let/dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task test_let_dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("let/dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task test_let_dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("let/dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "redeclaration-error-from-within-strict-mode-function.js")]
    public Task test_let_redeclaration_error_from_within_strict_mode_function()
        => CompilationFailureTest("let/redeclaration-error-from-within-strict-mode-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-invalid.js")]
    public Task test_let_static_init_await_binding_invalid()
        => CompilationFailureTest("let/static-init-await-binding-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "identifier-let-allowed-as-lefthandside-expression-strict.js")]
    public Task test_let_syntax_identifier_let_allowed_as_lefthandside_expression_strict()
        => CompilationFailureTest("let/syntax/identifier-let-allowed-as-lefthandside-expression-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "identifier-let-disallowed-as-boundname.js")]
    public Task test_let_syntax_identifier_let_disallowed_as_boundname()
        => CompilationFailureTest("let/syntax/identifier-let-disallowed-as-boundname", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-let-declaration-split-across-two-lines.js")]
    public Task test_let_syntax_let_let_declaration_split_across_two_lines()
        => CompilationFailureTest("let/syntax/let-let-declaration-split-across-two-lines", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-let-declaration-with-initializer-split-across-two-lines.js")]
    public Task test_let_syntax_let_let_declaration_with_initializer_split_across_two_lines()
        => CompilationFailureTest("let/syntax/let-let-declaration-with-initializer-split-across-two-lines", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-newline-await-in-normal-function.js")]
    public Task test_let_syntax_let_newline_await_in_normal_function()
        => CompilationFailureTest("let/syntax/let-newline-await-in-normal-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-newline-yield-in-generator-function.js")]
    public Task test_let_syntax_let_newline_yield_in_generator_function()
        => CompilationFailureTest("let/syntax/let-newline-yield-in-generator-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-newline-yield-in-normal-function.js")]
    public Task test_let_syntax_let_newline_yield_in_normal_function()
        => CompilationFailureTest("let/syntax/let-newline-yield-in-normal-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-do-statement-while-expression.js")]
    public Task test_let_syntax_with_initialisers_in_statement_positions_do_statement_while_expression()
        => CompilationFailureTest("let/syntax/with-initialisers-in-statement-positions-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-for-statement.js")]
    public Task test_let_syntax_with_initialisers_in_statement_positions_for_statement()
        => CompilationFailureTest("let/syntax/with-initialisers-in-statement-positions-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-if-expression-statement-else-statement.js")]
    public Task test_let_syntax_with_initialisers_in_statement_positions_if_expression_statement_else_statement()
        => CompilationFailureTest("let/syntax/with-initialisers-in-statement-positions-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-if-expression-statement.js")]
    public Task test_let_syntax_with_initialisers_in_statement_positions_if_expression_statement()
        => CompilationFailureTest("let/syntax/with-initialisers-in-statement-positions-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-label-statement.js")]
    public Task test_let_syntax_with_initialisers_in_statement_positions_label_statement()
        => CompilationFailureTest("let/syntax/with-initialisers-in-statement-positions-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-while-expression-statement.js")]
    public Task test_let_syntax_with_initialisers_in_statement_positions_while_expression_statement()
        => CompilationFailureTest("let/syntax/with-initialisers-in-statement-positions-while-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-do-statement-while-expression.js")]
    public Task test_let_syntax_without_initialisers_in_statement_positions_do_statement_while_expression()
        => CompilationFailureTest("let/syntax/without-initialisers-in-statement-positions-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-for-statement.js")]
    public Task test_let_syntax_without_initialisers_in_statement_positions_for_statement()
        => CompilationFailureTest("let/syntax/without-initialisers-in-statement-positions-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-if-expression-statement-else-statement.js")]
    public Task test_let_syntax_without_initialisers_in_statement_positions_if_expression_statement_else_statement()
        => CompilationFailureTest("let/syntax/without-initialisers-in-statement-positions-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-if-expression-statement.js")]
    public Task test_let_syntax_without_initialisers_in_statement_positions_if_expression_statement()
        => CompilationFailureTest("let/syntax/without-initialisers-in-statement-positions-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-label-statement.js")]
    public Task test_let_syntax_without_initialisers_in_statement_positions_label_statement()
        => CompilationFailureTest("let/syntax/without-initialisers-in-statement-positions-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-while-expression-statement.js")]
    public Task test_let_syntax_without_initialisers_in_statement_positions_while_expression_statement()
        => CompilationFailureTest("let/syntax/without-initialisers-in-statement-positions-while-expression-statement", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8SwitchParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8SwitchParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "S12.11_A3_T3.js")]
    public Task test_switch_S12_11_A3_T3()
        => CompilationFailureTest("switch/S12.11_A3_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.11_A3_T4.js")]
    public Task test_switch_S12_11_A3_T4()
        => CompilationFailureTest("switch/S12.11_A3_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.11_A3_T5.js")]
    public Task test_switch_S12_11_A3_T5()
        => CompilationFailureTest("switch/S12.11_A3_T5", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_async_function_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/async-function-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_async_generator_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/async-generator-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_class_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/class-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_const_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/const-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_function_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/function-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_generator_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/generator-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-var.js")]
    public Task test_switch_syntax_redeclaration_let_name_redeclaration_attempt_with_var()
        => CompilationFailureTest("switch/syntax/redeclaration/let-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-async-function.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_async_function()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-async-generator.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_async_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-class.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_class()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-const.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_const()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-function.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_function()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-generator.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_generator()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-let.js")]
    public Task test_switch_syntax_redeclaration_var_name_redeclaration_attempt_with_let()
        => CompilationFailureTest("switch/syntax/redeclaration/var-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8TryParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8TryParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "catch-parameter-boundnames-restriction-arguments-negative-early.js")]
    public Task test_try_catch_parameter_boundnames_restriction_arguments_negative_early()
        => CompilationFailureTest("try/catch-parameter-boundnames-restriction-arguments-negative-early", "Failed to parse JavaScript");

    [Fact(DisplayName = "catch-parameter-boundnames-restriction-eval-negative-early.js")]
    public Task test_try_catch_parameter_boundnames_restriction_eval_negative_early()
        => CompilationFailureTest("try/catch-parameter-boundnames-restriction-eval-negative-early", "Failed to parse JavaScript");

}

public class StatementSyntaxConformance8UsingParseTests : FileSystemExecutionTestsBase
{
    public StatementSyntaxConformance8UsingParseTests() : base("language/statements", "language.statements") { }

    [Fact(DisplayName = "redeclaration-error-from-within-strict-mode-function-using.js")]
    public Task test_using_redeclaration_error_from_within_strict_mode_function_using()
        => CompilationFailureTest("using/redeclaration-error-from-within-strict-mode-function-using", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-invalid.js")]
    public Task test_using_static_init_await_binding_invalid()
        => CompilationFailureTest("using/static-init-await-binding-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-using-declarations-mixed-with-without-initializer.js")]
    public Task test_using_syntax_block_scope_syntax_using_declarations_mixed_with_without_initializer()
        => CompilationFailureTest("using/syntax/block-scope-syntax-using-declarations-mixed-with-without-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-using-declarations-mixed-without-with-initializer.js")]
    public Task test_using_syntax_block_scope_syntax_using_declarations_mixed_without_with_initializer()
        => CompilationFailureTest("using/syntax/block-scope-syntax-using-declarations-mixed-without-with-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "block-scope-syntax-using-declarations-without-initializer.js")]
    public Task test_using_syntax_block_scope_syntax_using_declarations_without_initializer()
        => CompilationFailureTest("using/syntax/block-scope-syntax-using-declarations-without-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-invalid-arraybindingpattern-after-bindingidentifier.js")]
    public Task test_using_syntax_using_invalid_arraybindingpattern_after_bindingidentifier()
        => CompilationFailureTest("using/syntax/using-invalid-arraybindingpattern-after-bindingidentifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-invalid-arraybindingpattern.js")]
    public Task test_using_syntax_using_invalid_arraybindingpattern()
        => CompilationFailureTest("using/syntax/using-invalid-arraybindingpattern", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-invalid-objectbindingpattern-after-bindingidentifier.js")]
    public Task test_using_syntax_using_invalid_objectbindingpattern_after_bindingidentifier()
        => CompilationFailureTest("using/syntax/using-invalid-objectbindingpattern-after-bindingidentifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-invalid-objectbindingpattern.js")]
    public Task test_using_syntax_using_invalid_objectbindingpattern()
        => CompilationFailureTest("using/syntax/using-invalid-objectbindingpattern", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-invalid-switchstatement-caseclause.js")]
    public Task test_using_syntax_using_invalid_switchstatement_caseclause()
        => CompilationFailureTest("using/syntax/using-invalid-switchstatement-caseclause", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-invalid-switchstatement-defaultclause.js")]
    public Task test_using_syntax_using_invalid_switchstatement_defaultclause()
        => CompilationFailureTest("using/syntax/using-invalid-switchstatement-defaultclause", "Failed to parse JavaScript");

    [Fact(DisplayName = "using-not-allowed-at-top-level-of-script.js")]
    public Task test_using_syntax_using_not_allowed_at_top_level_of_script()
        => CompilationFailureTest("using/syntax/using-not-allowed-at-top-level-of-script", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-case-expression-statement-list.js")]
    public Task test_using_syntax_with_initializer_case_expression_statement_list()
        => CompilationFailureTest("using/syntax/with-initializer-case-expression-statement-list", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-default-statement-list.js")]
    public Task test_using_syntax_with_initializer_default_statement_list()
        => CompilationFailureTest("using/syntax/with-initializer-default-statement-list", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-do-statement-while-expression.js")]
    public Task test_using_syntax_with_initializer_do_statement_while_expression()
        => CompilationFailureTest("using/syntax/with-initializer-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-for-statement.js")]
    public Task test_using_syntax_with_initializer_for_statement()
        => CompilationFailureTest("using/syntax/with-initializer-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-if-expression-statement-else-statement.js")]
    public Task test_using_syntax_with_initializer_if_expression_statement_else_statement()
        => CompilationFailureTest("using/syntax/with-initializer-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-if-expression-statement.js")]
    public Task test_using_syntax_with_initializer_if_expression_statement()
        => CompilationFailureTest("using/syntax/with-initializer-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-label-statement.js")]
    public Task test_using_syntax_with_initializer_label_statement()
        => CompilationFailureTest("using/syntax/with-initializer-label-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "with-initializer-while-expression-statement.js")]
    public Task test_using_syntax_with_initializer_while_expression_statement()
        => CompilationFailureTest("using/syntax/with-initializer-while-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "without-initializer-do-statement-while-expression.js")]
    public Task test_using_syntax_without_initializer_do_statement_while_expression()
        => CompilationFailureTest("using/syntax/without-initializer-do-statement-while-expression", "Failed to parse JavaScript");

}
