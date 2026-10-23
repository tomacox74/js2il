namespace Jroc.Test262.Tests.language.expressions.dynamic_import.syntax.valid;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.syntax.valid") { }

    [Fact(DisplayName = "callexpression-arguments.js")]
    public Task callexpression_arguments()
        => ExecutionTest("callexpression-arguments");

    [Fact(DisplayName = "callexpression-templateliteral.js")]
    public Task callexpression_templateliteral()
        => ExecutionTest("callexpression-templateliteral");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-attributes-trailing-comma-first.js")]
    public Task nested_arrow_assignment_expression_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-arrow-assignment-expression-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-attributes-trailing-comma-second.js")]
    public Task nested_arrow_assignment_expression_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-arrow-assignment-expression-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_arrow_assignment_expression_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-arrow-assignment-expression-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-defer-script-code-valid.js")]
    public Task nested_arrow_assignment_expression_import_defer_script_code_valid()
        => ExecutionTest("nested-arrow-assignment-expression-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_arrow_assignment_expression_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-arrow-assignment-expression-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-arrow-assignment-expression-import-source-script-code-valid.js")]
    public Task nested_arrow_assignment_expression_import_source_script_code_valid()
        => ExecutionTest("nested-arrow-assignment-expression-import-source-script-code-valid");

    [Fact(DisplayName = "nested-arrow-assignment-expression-nested-imports.js")]
    public Task nested_arrow_assignment_expression_nested_imports()
        => ExecutionTest("nested-arrow-assignment-expression-nested-imports");

    [Fact(DisplayName = "nested-arrow-assignment-expression-script-code-valid.js")]
    public Task nested_arrow_assignment_expression_script_code_valid()
        => ExecutionTest("nested-arrow-assignment-expression-script-code-valid");

    [Fact(DisplayName = "nested-arrow-import-attributes-trailing-comma-first.js")]
    public Task nested_arrow_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-arrow-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-arrow-import-attributes-trailing-comma-second.js")]
    public Task nested_arrow_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-arrow-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-arrow-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_arrow_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-arrow-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-arrow-import-defer-script-code-valid.js")]
    public Task nested_arrow_import_defer_script_code_valid()
        => ExecutionTest("nested-arrow-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-arrow-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_arrow_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-arrow-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-arrow-import-source-script-code-valid.js")]
    public Task nested_arrow_import_source_script_code_valid()
        => ExecutionTest("nested-arrow-import-source-script-code-valid");

    [Fact(DisplayName = "nested-arrow-nested-imports.js")]
    public Task nested_arrow_nested_imports()
        => ExecutionTest("nested-arrow-nested-imports");

    [Fact(DisplayName = "nested-arrow-script-code-valid.js")]
    public Task nested_arrow_script_code_valid()
        => ExecutionTest("nested-arrow-script-code-valid");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-attributes-trailing-comma-first.js")]
    public Task nested_async_arrow_function_await_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-async-arrow-function-await-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-attributes-trailing-comma-second.js")]
    public Task nested_async_arrow_function_await_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-async-arrow-function-await-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_arrow_function_await_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-arrow-function-await-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-defer-script-code-valid.js")]
    public Task nested_async_arrow_function_await_import_defer_script_code_valid()
        => ExecutionTest("nested-async-arrow-function-await-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_arrow_function_await_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-arrow-function-await-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-script-code-valid.js")]
    public Task nested_async_arrow_function_await_import_source_script_code_valid()
        => ExecutionTest("nested-async-arrow-function-await-import-source-script-code-valid");

    [Fact(DisplayName = "nested-async-arrow-function-await-nested-imports.js")]
    public Task nested_async_arrow_function_await_nested_imports()
        => ExecutionTest("nested-async-arrow-function-await-nested-imports");

    [Fact(DisplayName = "nested-async-arrow-function-await-script-code-valid.js")]
    public Task nested_async_arrow_function_await_script_code_valid()
        => ExecutionTest("nested-async-arrow-function-await-script-code-valid");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-attributes-trailing-comma-first.js")]
    public Task nested_async_arrow_function_return_await_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-async-arrow-function-return-await-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-attributes-trailing-comma-second.js")]
    public Task nested_async_arrow_function_return_await_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-async-arrow-function-return-await-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_arrow_function_return_await_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-arrow-function-return-await-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-defer-script-code-valid.js")]
    public Task nested_async_arrow_function_return_await_import_defer_script_code_valid()
        => ExecutionTest("nested-async-arrow-function-return-await-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_arrow_function_return_await_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-arrow-function-return-await-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-script-code-valid.js")]
    public Task nested_async_arrow_function_return_await_import_source_script_code_valid()
        => ExecutionTest("nested-async-arrow-function-return-await-import-source-script-code-valid");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-nested-imports.js")]
    public Task nested_async_arrow_function_return_await_nested_imports()
        => ExecutionTest("nested-async-arrow-function-return-await-nested-imports");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-script-code-valid.js")]
    public Task nested_async_arrow_function_return_await_script_code_valid()
        => ExecutionTest("nested-async-arrow-function-return-await-script-code-valid");

    [Fact(DisplayName = "nested-async-function-await-import-attributes-trailing-comma-first.js")]
    public Task nested_async_function_await_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-async-function-await-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-async-function-await-import-attributes-trailing-comma-second.js")]
    public Task nested_async_function_await_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-async-function-await-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-async-function-await-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_function_await_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-await-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-await-import-defer-script-code-valid.js")]
    public Task nested_async_function_await_import_defer_script_code_valid()
        => ExecutionTest("nested-async-function-await-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-async-function-await-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_function_await_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-await-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-await-import-source-script-code-valid.js")]
    public Task nested_async_function_await_import_source_script_code_valid()
        => ExecutionTest("nested-async-function-await-import-source-script-code-valid");

    [Fact(DisplayName = "nested-async-function-await-nested-imports.js")]
    public Task nested_async_function_await_nested_imports()
        => ExecutionTest("nested-async-function-await-nested-imports");

    [Fact(DisplayName = "nested-async-function-await-script-code-valid.js")]
    public Task nested_async_function_await_script_code_valid()
        => ExecutionTest("nested-async-function-await-script-code-valid");

    [Fact(DisplayName = "nested-async-function-import-attributes-trailing-comma-first.js")]
    public Task nested_async_function_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-async-function-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-async-function-import-attributes-trailing-comma-second.js")]
    public Task nested_async_function_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-async-function-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-async-function-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_function_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-import-defer-script-code-valid.js")]
    public Task nested_async_function_import_defer_script_code_valid()
        => ExecutionTest("nested-async-function-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-async-function-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_function_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-import-source-script-code-valid.js")]
    public Task nested_async_function_import_source_script_code_valid()
        => ExecutionTest("nested-async-function-import-source-script-code-valid");

    [Fact(DisplayName = "nested-async-function-nested-imports.js")]
    public Task nested_async_function_nested_imports()
        => ExecutionTest("nested-async-function-nested-imports");

    [Fact(DisplayName = "nested-async-function-return-await-import-attributes-trailing-comma-first.js")]
    public Task nested_async_function_return_await_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-async-function-return-await-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-async-function-return-await-import-attributes-trailing-comma-second.js")]
    public Task nested_async_function_return_await_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-async-function-return-await-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-async-function-return-await-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_function_return_await_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-return-await-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-return-await-import-defer-script-code-valid.js")]
    public Task nested_async_function_return_await_import_defer_script_code_valid()
        => ExecutionTest("nested-async-function-return-await-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_function_return_await_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-return-await-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-script-code-valid.js")]
    public Task nested_async_function_return_await_import_source_script_code_valid()
        => ExecutionTest("nested-async-function-return-await-import-source-script-code-valid");

    [Fact(DisplayName = "nested-async-function-return-await-nested-imports.js")]
    public Task nested_async_function_return_await_nested_imports()
        => ExecutionTest("nested-async-function-return-await-nested-imports");

    [Fact(DisplayName = "nested-async-function-return-await-script-code-valid.js")]
    public Task nested_async_function_return_await_script_code_valid()
        => ExecutionTest("nested-async-function-return-await-script-code-valid");

    [Fact(DisplayName = "nested-async-function-script-code-valid.js")]
    public Task nested_async_function_script_code_valid()
        => ExecutionTest("nested-async-function-script-code-valid");

    [Fact(DisplayName = "nested-async-gen-await-import-attributes-trailing-comma-first.js")]
    public Task nested_async_gen_await_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-async-gen-await-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-async-gen-await-import-attributes-trailing-comma-second.js")]
    public Task nested_async_gen_await_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-async-gen-await-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-async-gen-await-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_gen_await_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-gen-await-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-gen-await-import-defer-script-code-valid.js")]
    public Task nested_async_gen_await_import_defer_script_code_valid()
        => ExecutionTest("nested-async-gen-await-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-async-gen-await-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_async_gen_await_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-gen-await-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-gen-await-import-source-script-code-valid.js")]
    public Task nested_async_gen_await_import_source_script_code_valid()
        => ExecutionTest("nested-async-gen-await-import-source-script-code-valid");

    [Fact(DisplayName = "nested-async-gen-await-nested-imports.js")]
    public Task nested_async_gen_await_nested_imports()
        => ExecutionTest("nested-async-gen-await-nested-imports");

    [Fact(DisplayName = "nested-async-gen-await-script-code-valid.js")]
    public Task nested_async_gen_await_script_code_valid()
        => ExecutionTest("nested-async-gen-await-script-code-valid");

    [Fact(DisplayName = "nested-block-empty-str-is-valid-assign-expr.js")]
    public Task nested_block_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-block-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-block-import-attributes-trailing-comma-first.js")]
    public Task nested_block_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-block-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-block-import-attributes-trailing-comma-second.js")]
    public Task nested_block_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-block-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-block-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_block_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-block-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-block-import-defer-script-code-valid.js")]
    public Task nested_block_import_defer_script_code_valid()
        => ExecutionTest("nested-block-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-block-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_block_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-block-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-block-import-source-script-code-valid.js")]
    public Task nested_block_import_source_script_code_valid()
        => ExecutionTest("nested-block-import-source-script-code-valid");

    [Fact(DisplayName = "nested-block-labeled-empty-str-is-valid-assign-expr.js")]
    public Task nested_block_labeled_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-block-labeled-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-block-labeled-import-attributes-trailing-comma-first.js")]
    public Task nested_block_labeled_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-block-labeled-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-block-labeled-import-attributes-trailing-comma-second.js")]
    public Task nested_block_labeled_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-block-labeled-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-block-labeled-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_block_labeled_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-block-labeled-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-block-labeled-import-defer-script-code-valid.js")]
    public Task nested_block_labeled_import_defer_script_code_valid()
        => ExecutionTest("nested-block-labeled-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-block-labeled-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_block_labeled_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-block-labeled-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-block-labeled-import-source-script-code-valid.js")]
    public Task nested_block_labeled_import_source_script_code_valid()
        => ExecutionTest("nested-block-labeled-import-source-script-code-valid");

    [Fact(DisplayName = "nested-block-labeled-nested-imports.js")]
    public Task nested_block_labeled_nested_imports()
        => ExecutionTest("nested-block-labeled-nested-imports");

    [Fact(DisplayName = "nested-block-labeled-script-code-valid.js")]
    public Task nested_block_labeled_script_code_valid()
        => ExecutionTest("nested-block-labeled-script-code-valid");

    [Fact(DisplayName = "nested-block-nested-imports.js")]
    public Task nested_block_nested_imports()
        => ExecutionTest("nested-block-nested-imports");

    [Fact(DisplayName = "nested-block-script-code-valid.js")]
    public Task nested_block_script_code_valid()
        => ExecutionTest("nested-block-script-code-valid");

    [Fact(DisplayName = "nested-do-while-empty-str-is-valid-assign-expr.js")]
    public Task nested_do_while_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-do-while-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-do-while-import-attributes-trailing-comma-first.js")]
    public Task nested_do_while_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-do-while-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-do-while-import-attributes-trailing-comma-second.js")]
    public Task nested_do_while_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-do-while-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-do-while-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_do_while_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-do-while-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-do-while-import-defer-script-code-valid.js")]
    public Task nested_do_while_import_defer_script_code_valid()
        => ExecutionTest("nested-do-while-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-do-while-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_do_while_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-do-while-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-do-while-import-source-script-code-valid.js")]
    public Task nested_do_while_import_source_script_code_valid()
        => ExecutionTest("nested-do-while-import-source-script-code-valid");

    [Fact(DisplayName = "nested-do-while-nested-imports.js")]
    public Task nested_do_while_nested_imports()
        => ExecutionTest("nested-do-while-nested-imports");

    [Fact(DisplayName = "nested-do-while-script-code-valid.js")]
    public Task nested_do_while_script_code_valid()
        => ExecutionTest("nested-do-while-script-code-valid");

    [Fact(DisplayName = "nested-else-braceless-empty-str-is-valid-assign-expr.js")]
    public Task nested_else_braceless_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-else-braceless-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-else-braceless-import-attributes-trailing-comma-first.js")]
    public Task nested_else_braceless_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-else-braceless-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-else-braceless-import-attributes-trailing-comma-second.js")]
    public Task nested_else_braceless_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-else-braceless-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-else-braceless-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_else_braceless_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-else-braceless-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-else-braceless-import-defer-script-code-valid.js")]
    public Task nested_else_braceless_import_defer_script_code_valid()
        => ExecutionTest("nested-else-braceless-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-else-braceless-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_else_braceless_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-else-braceless-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-else-braceless-import-source-script-code-valid.js")]
    public Task nested_else_braceless_import_source_script_code_valid()
        => ExecutionTest("nested-else-braceless-import-source-script-code-valid");

    [Fact(DisplayName = "nested-else-braceless-nested-imports.js")]
    public Task nested_else_braceless_nested_imports()
        => ExecutionTest("nested-else-braceless-nested-imports");

    [Fact(DisplayName = "nested-else-braceless-script-code-valid.js")]
    public Task nested_else_braceless_script_code_valid()
        => ExecutionTest("nested-else-braceless-script-code-valid");

    [Fact(DisplayName = "nested-else-empty-str-is-valid-assign-expr.js")]
    public Task nested_else_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-else-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-else-import-attributes-trailing-comma-first.js")]
    public Task nested_else_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-else-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-else-import-attributes-trailing-comma-second.js")]
    public Task nested_else_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-else-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-else-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_else_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-else-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-else-import-defer-script-code-valid.js")]
    public Task nested_else_import_defer_script_code_valid()
        => ExecutionTest("nested-else-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-else-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_else_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-else-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-else-import-source-script-code-valid.js")]
    public Task nested_else_import_source_script_code_valid()
        => ExecutionTest("nested-else-import-source-script-code-valid");

    [Fact(DisplayName = "nested-else-nested-imports.js")]
    public Task nested_else_nested_imports()
        => ExecutionTest("nested-else-nested-imports");

    [Fact(DisplayName = "nested-else-script-code-valid.js")]
    public Task nested_else_script_code_valid()
        => ExecutionTest("nested-else-script-code-valid");

    [Fact(DisplayName = "nested-function-import-attributes-trailing-comma-first.js")]
    public Task nested_function_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-function-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-function-import-attributes-trailing-comma-second.js")]
    public Task nested_function_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-function-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-function-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_function_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-function-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-function-import-defer-script-code-valid.js")]
    public Task nested_function_import_defer_script_code_valid()
        => ExecutionTest("nested-function-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-function-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_function_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-function-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-function-import-source-script-code-valid.js")]
    public Task nested_function_import_source_script_code_valid()
        => ExecutionTest("nested-function-import-source-script-code-valid");

    [Fact(DisplayName = "nested-function-nested-imports.js")]
    public Task nested_function_nested_imports()
        => ExecutionTest("nested-function-nested-imports");

    [Fact(DisplayName = "nested-function-return-import-attributes-trailing-comma-first.js")]
    public Task nested_function_return_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-function-return-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-function-return-import-attributes-trailing-comma-second.js")]
    public Task nested_function_return_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-function-return-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-function-return-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_function_return_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-function-return-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-function-return-import-defer-script-code-valid.js")]
    public Task nested_function_return_import_defer_script_code_valid()
        => ExecutionTest("nested-function-return-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-function-return-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_function_return_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-function-return-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-function-return-import-source-script-code-valid.js")]
    public Task nested_function_return_import_source_script_code_valid()
        => ExecutionTest("nested-function-return-import-source-script-code-valid");

    [Fact(DisplayName = "nested-function-return-nested-imports.js")]
    public Task nested_function_return_nested_imports()
        => ExecutionTest("nested-function-return-nested-imports");

    [Fact(DisplayName = "nested-function-return-script-code-valid.js")]
    public Task nested_function_return_script_code_valid()
        => ExecutionTest("nested-function-return-script-code-valid");

    [Fact(DisplayName = "nested-function-script-code-valid.js")]
    public Task nested_function_script_code_valid()
        => ExecutionTest("nested-function-script-code-valid");

    [Fact(DisplayName = "nested-if-braceless-empty-str-is-valid-assign-expr.js")]
    public Task nested_if_braceless_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-if-braceless-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-if-braceless-import-attributes-trailing-comma-first.js")]
    public Task nested_if_braceless_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-if-braceless-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-if-braceless-import-attributes-trailing-comma-second.js")]
    public Task nested_if_braceless_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-if-braceless-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-if-braceless-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_if_braceless_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-if-braceless-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-if-braceless-import-defer-script-code-valid.js")]
    public Task nested_if_braceless_import_defer_script_code_valid()
        => ExecutionTest("nested-if-braceless-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-if-braceless-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_if_braceless_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-if-braceless-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-if-braceless-import-source-script-code-valid.js")]
    public Task nested_if_braceless_import_source_script_code_valid()
        => ExecutionTest("nested-if-braceless-import-source-script-code-valid");

    [Fact(DisplayName = "nested-if-braceless-nested-imports.js")]
    public Task nested_if_braceless_nested_imports()
        => ExecutionTest("nested-if-braceless-nested-imports");

    [Fact(DisplayName = "nested-if-braceless-script-code-valid.js")]
    public Task nested_if_braceless_script_code_valid()
        => ExecutionTest("nested-if-braceless-script-code-valid");

    [Fact(DisplayName = "nested-if-empty-str-is-valid-assign-expr.js")]
    public Task nested_if_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-if-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-if-import-attributes-trailing-comma-first.js")]
    public Task nested_if_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-if-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-if-import-attributes-trailing-comma-second.js")]
    public Task nested_if_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-if-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-if-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_if_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-if-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-if-import-defer-script-code-valid.js")]
    public Task nested_if_import_defer_script_code_valid()
        => ExecutionTest("nested-if-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-if-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_if_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-if-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-if-import-source-script-code-valid.js")]
    public Task nested_if_import_source_script_code_valid()
        => ExecutionTest("nested-if-import-source-script-code-valid");

    [Fact(DisplayName = "nested-if-nested-imports.js")]
    public Task nested_if_nested_imports()
        => ExecutionTest("nested-if-nested-imports");

    [Fact(DisplayName = "nested-if-script-code-valid.js")]
    public Task nested_if_script_code_valid()
        => ExecutionTest("nested-if-script-code-valid");

    [Fact(DisplayName = "nested-while-empty-str-is-valid-assign-expr.js")]
    public Task nested_while_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-while-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-while-import-attributes-trailing-comma-first.js")]
    public Task nested_while_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-while-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-while-import-attributes-trailing-comma-second.js")]
    public Task nested_while_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-while-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-while-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_while_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-while-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-while-import-defer-script-code-valid.js")]
    public Task nested_while_import_defer_script_code_valid()
        => ExecutionTest("nested-while-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-while-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_while_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-while-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-while-import-source-script-code-valid.js")]
    public Task nested_while_import_source_script_code_valid()
        => ExecutionTest("nested-while-import-source-script-code-valid");

    [Fact(DisplayName = "nested-while-nested-imports.js")]
    public Task nested_while_nested_imports()
        => ExecutionTest("nested-while-nested-imports");

    [Fact(DisplayName = "nested-while-script-code-valid.js")]
    public Task nested_while_script_code_valid()
        => ExecutionTest("nested-while-script-code-valid");

    [Fact(DisplayName = "nested-with-empty-str-is-valid-assign-expr.js")]
    public Task nested_with_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-with-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-with-expression-empty-str-is-valid-assign-expr.js")]
    public Task nested_with_expression_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-with-expression-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-with-expression-import-attributes-trailing-comma-first.js")]
    public Task nested_with_expression_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-with-expression-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-with-expression-import-attributes-trailing-comma-second.js")]
    public Task nested_with_expression_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-with-expression-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-with-expression-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_with_expression_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-with-expression-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-with-expression-import-defer-script-code-valid.js")]
    public Task nested_with_expression_import_defer_script_code_valid()
        => ExecutionTest("nested-with-expression-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-with-expression-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_with_expression_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-with-expression-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-with-expression-import-source-script-code-valid.js")]
    public Task nested_with_expression_import_source_script_code_valid()
        => ExecutionTest("nested-with-expression-import-source-script-code-valid");

    [Fact(DisplayName = "nested-with-expression-nested-imports.js")]
    public Task nested_with_expression_nested_imports()
        => ExecutionTest("nested-with-expression-nested-imports");

    [Fact(DisplayName = "nested-with-expression-script-code-valid.js")]
    public Task nested_with_expression_script_code_valid()
        => ExecutionTest("nested-with-expression-script-code-valid");

    [Fact(DisplayName = "nested-with-import-attributes-trailing-comma-first.js")]
    public Task nested_with_import_attributes_trailing_comma_first()
        => ExecutionTest("nested-with-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "nested-with-import-attributes-trailing-comma-second.js")]
    public Task nested_with_import_attributes_trailing_comma_second()
        => ExecutionTest("nested-with-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "nested-with-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task nested_with_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-with-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-with-import-defer-script-code-valid.js")]
    public Task nested_with_import_defer_script_code_valid()
        => ExecutionTest("nested-with-import-defer-script-code-valid");

    [Fact(DisplayName = "nested-with-import-source-empty-str-is-valid-assign-expr.js")]
    public Task nested_with_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-with-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-with-import-source-script-code-valid.js")]
    public Task nested_with_import_source_script_code_valid()
        => ExecutionTest("nested-with-import-source-script-code-valid");

    [Fact(DisplayName = "nested-with-nested-imports.js")]
    public Task nested_with_nested_imports()
        => ExecutionTest("nested-with-nested-imports");

    [Fact(DisplayName = "nested-with-script-code-valid.js")]
    public Task nested_with_script_code_valid()
        => ExecutionTest("nested-with-script-code-valid");

    [Fact(DisplayName = "new-covered-expression-is-valid.js")]
    public Task new_covered_expression_is_valid()
        => ExecutionTest("new-covered-expression-is-valid");

    [Fact(DisplayName = "top-level-empty-str-is-valid-assign-expr.js")]
    public Task top_level_empty_str_is_valid_assign_expr()
        => ExecutionTest("top-level-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "top-level-import-attributes-trailing-comma-first.js")]
    public Task top_level_import_attributes_trailing_comma_first()
        => ExecutionTest("top-level-import-attributes-trailing-comma-first");

    [Fact(DisplayName = "top-level-import-attributes-trailing-comma-second.js")]
    public Task top_level_import_attributes_trailing_comma_second()
        => ExecutionTest("top-level-import-attributes-trailing-comma-second");

    [Fact(DisplayName = "top-level-import-defer-empty-str-is-valid-assign-expr.js")]
    public Task top_level_import_defer_empty_str_is_valid_assign_expr()
        => ExecutionTest("top-level-import-defer-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "top-level-import-defer-script-code-valid.js")]
    public Task top_level_import_defer_script_code_valid()
        => ExecutionTest("top-level-import-defer-script-code-valid");

    [Fact(DisplayName = "top-level-import-source-empty-str-is-valid-assign-expr.js")]
    public Task top_level_import_source_empty_str_is_valid_assign_expr()
        => ExecutionTest("top-level-import-source-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "top-level-import-source-script-code-valid.js")]
    public Task top_level_import_source_script_code_valid()
        => ExecutionTest("top-level-import-source-script-code-valid");

    [Fact(DisplayName = "top-level-nested-imports.js")]
    public Task top_level_nested_imports()
        => ExecutionTest("top-level-nested-imports");

    [Fact(DisplayName = "top-level-script-code-valid.js")]
    public Task top_level_script_code_valid()
        => ExecutionTest("top-level-script-code-valid");
}
