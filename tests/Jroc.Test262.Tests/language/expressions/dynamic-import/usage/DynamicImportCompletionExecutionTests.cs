namespace Jroc.Test262.Tests.language.expressions.dynamic_import.usage;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.usage") { }

    [Fact(DisplayName = "nested-arrow-assignment-expression-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_arrow_assignment_expression_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-arrow-assignment-expression-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-arrow-assignment-expression-eval-gtbndng-indirect-update.js")]
    public Task nested_arrow_assignment_expression_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-arrow-assignment-expression-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-arrow-assignment-expression-eval-script-code-host-resolves-module-code.js")]
    public Task nested_arrow_assignment_expression_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-arrow-assignment-expression-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-arrow-assignment-expression-is-call-expression-square-brackets.js")]
    public Task nested_arrow_assignment_expression_is_call_expression_square_brackets()
        => ExecutionTest("nested-arrow-assignment-expression-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-arrow-assignment-expression-returns-thenable.js")]
    public Task nested_arrow_assignment_expression_returns_thenable()
        => ExecutionTest("nested-arrow-assignment-expression-returns-thenable");

    [Fact(DisplayName = "nested-arrow-assignment-expression-specifier-tostring.js")]
    public Task nested_arrow_assignment_expression_specifier_tostring()
        => ExecutionTest("nested-arrow-assignment-expression-specifier-tostring");

    [Fact(DisplayName = "nested-arrow-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_arrow_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-arrow-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-arrow-import-then-eval-gtbndng-indirect-update.js")]
    public Task nested_arrow_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-arrow-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-arrow-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task nested_arrow_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-arrow-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-arrow-import-then-is-call-expression-square-brackets.js")]
    public Task nested_arrow_import_then_is_call_expression_square_brackets()
        => ExecutionTest("nested-arrow-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-arrow-import-then-returns-thenable.js")]
    public Task nested_arrow_import_then_returns_thenable()
        => ExecutionTest("nested-arrow-import-then-returns-thenable");

    [Fact(DisplayName = "nested-arrow-import-then-specifier-tostring.js")]
    public Task nested_arrow_import_then_specifier_tostring()
        => ExecutionTest("nested-arrow-import-then-specifier-tostring");

    [Fact(DisplayName = "nested-async-arrow-function-await-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_arrow_function_await_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-arrow-function-await-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-arrow-function-await-eval-gtbndng-indirect-update.js")]
    public Task nested_async_arrow_function_await_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-arrow-function-await-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-arrow-function-await-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_arrow_function_await_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-arrow-function-await-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-arrow-function-await-is-call-expression-square-brackets.js")]
    public Task nested_async_arrow_function_await_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-arrow-function-await-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-arrow-function-await-returns-thenable.js")]
    public Task nested_async_arrow_function_await_returns_thenable()
        => ExecutionTest("nested-async-arrow-function-await-returns-thenable");

    [Fact(DisplayName = "nested-async-arrow-function-await-specifier-tostring.js")]
    public Task nested_async_arrow_function_await_specifier_tostring()
        => ExecutionTest("nested-async-arrow-function-await-specifier-tostring");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_arrow_function_return_await_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-arrow-function-return-await-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-eval-gtbndng-indirect-update.js")]
    public Task nested_async_arrow_function_return_await_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-arrow-function-return-await-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_arrow_function_return_await_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-arrow-function-return-await-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-is-call-expression-square-brackets.js")]
    public Task nested_async_arrow_function_return_await_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-arrow-function-return-await-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-returns-thenable.js")]
    public Task nested_async_arrow_function_return_await_returns_thenable()
        => ExecutionTest("nested-async-arrow-function-return-await-returns-thenable");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-specifier-tostring.js")]
    public Task nested_async_arrow_function_return_await_specifier_tostring()
        => ExecutionTest("nested-async-arrow-function-return-await-specifier-tostring");

    [Fact(DisplayName = "nested-async-function-await-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_function_await_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-function-await-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-function-await-eval-gtbndng-indirect-update.js")]
    public Task nested_async_function_await_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-function-await-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-function-await-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_function_await_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-function-await-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-function-await-is-call-expression-square-brackets.js")]
    public Task nested_async_function_await_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-function-await-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-function-await-returns-thenable.js")]
    public Task nested_async_function_await_returns_thenable()
        => ExecutionTest("nested-async-function-await-returns-thenable");

    [Fact(DisplayName = "nested-async-function-await-specifier-tostring.js")]
    public Task nested_async_function_await_specifier_tostring()
        => ExecutionTest("nested-async-function-await-specifier-tostring");

    [Fact(DisplayName = "nested-async-function-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_function_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-function-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-function-eval-gtbndng-indirect-update.js")]
    public Task nested_async_function_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-function-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-function-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_function_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-function-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-function-is-call-expression-square-brackets.js")]
    public Task nested_async_function_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-function-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-function-return-await-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_function_return_await_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-function-return-await-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-function-return-await-eval-gtbndng-indirect-update.js")]
    public Task nested_async_function_return_await_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-function-return-await-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-function-return-await-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_function_return_await_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-function-return-await-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-function-return-await-is-call-expression-square-brackets.js")]
    public Task nested_async_function_return_await_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-function-return-await-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-function-return-await-returns-thenable.js")]
    public Task nested_async_function_return_await_returns_thenable()
        => ExecutionTest("nested-async-function-return-await-returns-thenable");

    [Fact(DisplayName = "nested-async-function-return-await-specifier-tostring.js")]
    public Task nested_async_function_return_await_specifier_tostring()
        => ExecutionTest("nested-async-function-return-await-specifier-tostring");

    [Fact(DisplayName = "nested-async-function-returns-thenable.js")]
    public Task nested_async_function_returns_thenable()
        => ExecutionTest("nested-async-function-returns-thenable");

    [Fact(DisplayName = "nested-async-function-specifier-tostring.js")]
    public Task nested_async_function_specifier_tostring()
        => ExecutionTest("nested-async-function-specifier-tostring");

    [Fact(DisplayName = "nested-async-gen-await-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_gen_await_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-gen-await-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-gen-await-eval-gtbndng-indirect-update.js")]
    public Task nested_async_gen_await_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-gen-await-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-gen-await-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_gen_await_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-gen-await-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-gen-await-is-call-expression-square-brackets.js")]
    public Task nested_async_gen_await_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-gen-await-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-gen-await-returns-thenable.js")]
    public Task nested_async_gen_await_returns_thenable()
        => ExecutionTest("nested-async-gen-await-returns-thenable");

    [Fact(DisplayName = "nested-async-gen-await-specifier-tostring.js")]
    public Task nested_async_gen_await_specifier_tostring()
        => ExecutionTest("nested-async-gen-await-specifier-tostring");

    [Fact(DisplayName = "nested-async-gen-return-await-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_async_gen_return_await_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-async-gen-return-await-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-async-gen-return-await-eval-gtbndng-indirect-update.js")]
    public Task nested_async_gen_return_await_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-async-gen-return-await-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-async-gen-return-await-eval-script-code-host-resolves-module-code.js")]
    public Task nested_async_gen_return_await_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-async-gen-return-await-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-async-gen-return-await-is-call-expression-square-brackets.js")]
    public Task nested_async_gen_return_await_is_call_expression_square_brackets()
        => ExecutionTest("nested-async-gen-return-await-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-async-gen-return-await-returns-thenable.js")]
    public Task nested_async_gen_return_await_returns_thenable()
        => ExecutionTest("nested-async-gen-return-await-returns-thenable");

    [Fact(DisplayName = "nested-async-gen-return-await-specifier-tostring.js")]
    public Task nested_async_gen_return_await_specifier_tostring()
        => ExecutionTest("nested-async-gen-return-await-specifier-tostring");

    [Fact(DisplayName = "nested-block-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_block_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-block-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-block-import-then-eval-gtbndng-indirect-update.js")]
    public Task nested_block_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-block-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-block-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task nested_block_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-block-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-block-import-then-is-call-expression-square-brackets.js")]
    public Task nested_block_import_then_is_call_expression_square_brackets()
        => ExecutionTest("nested-block-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-block-import-then-returns-thenable.js")]
    public Task nested_block_import_then_returns_thenable()
        => ExecutionTest("nested-block-import-then-returns-thenable");

    [Fact(DisplayName = "nested-block-import-then-specifier-tostring.js")]
    public Task nested_block_import_then_specifier_tostring()
        => ExecutionTest("nested-block-import-then-specifier-tostring");

    [Fact(DisplayName = "nested-do-while-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_do_while_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-do-while-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-do-while-eval-gtbndng-indirect-update.js")]
    public Task nested_do_while_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-do-while-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-do-while-eval-script-code-host-resolves-module-code.js")]
    public Task nested_do_while_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-do-while-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-do-while-is-call-expression-square-brackets.js")]
    public Task nested_do_while_is_call_expression_square_brackets()
        => ExecutionTest("nested-do-while-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-do-while-returns-thenable.js")]
    public Task nested_do_while_returns_thenable()
        => ExecutionTest("nested-do-while-returns-thenable");

    [Fact(DisplayName = "nested-do-while-specifier-tostring.js")]
    public Task nested_do_while_specifier_tostring()
        => ExecutionTest("nested-do-while-specifier-tostring");

    [Fact(DisplayName = "nested-else-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_else_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-else-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-else-import-then-eval-gtbndng-indirect-update.js")]
    public Task nested_else_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-else-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-else-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task nested_else_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-else-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-else-import-then-is-call-expression-square-brackets.js")]
    public Task nested_else_import_then_is_call_expression_square_brackets()
        => ExecutionTest("nested-else-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-else-import-then-returns-thenable.js")]
    public Task nested_else_import_then_returns_thenable()
        => ExecutionTest("nested-else-import-then-returns-thenable");

    [Fact(DisplayName = "nested-else-import-then-specifier-tostring.js")]
    public Task nested_else_import_then_specifier_tostring()
        => ExecutionTest("nested-else-import-then-specifier-tostring");

    [Fact(DisplayName = "nested-function-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_function_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-function-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-function-import-then-eval-gtbndng-indirect-update.js")]
    public Task nested_function_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-function-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-function-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task nested_function_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-function-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-function-import-then-is-call-expression-square-brackets.js")]
    public Task nested_function_import_then_is_call_expression_square_brackets()
        => ExecutionTest("nested-function-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-function-import-then-returns-thenable.js")]
    public Task nested_function_import_then_returns_thenable()
        => ExecutionTest("nested-function-import-then-returns-thenable");

    [Fact(DisplayName = "nested-function-import-then-specifier-tostring.js")]
    public Task nested_function_import_then_specifier_tostring()
        => ExecutionTest("nested-function-import-then-specifier-tostring");

    [Fact(DisplayName = "nested-if-braceless-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_if_braceless_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-if-braceless-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-if-braceless-eval-gtbndng-indirect-update.js")]
    public Task nested_if_braceless_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-if-braceless-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-if-braceless-eval-script-code-host-resolves-module-code.js")]
    public Task nested_if_braceless_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-if-braceless-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-if-braceless-is-call-expression-square-brackets.js")]
    public Task nested_if_braceless_is_call_expression_square_brackets()
        => ExecutionTest("nested-if-braceless-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-if-braceless-returns-thenable.js")]
    public Task nested_if_braceless_returns_thenable()
        => ExecutionTest("nested-if-braceless-returns-thenable");

    [Fact(DisplayName = "nested-if-braceless-specifier-tostring.js")]
    public Task nested_if_braceless_specifier_tostring()
        => ExecutionTest("nested-if-braceless-specifier-tostring");

    [Fact(DisplayName = "nested-if-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_if_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-if-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-if-import-then-eval-gtbndng-indirect-update.js")]
    public Task nested_if_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-if-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-if-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task nested_if_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-if-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-if-import-then-is-call-expression-square-brackets.js")]
    public Task nested_if_import_then_is_call_expression_square_brackets()
        => ExecutionTest("nested-if-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-if-import-then-returns-thenable.js")]
    public Task nested_if_import_then_returns_thenable()
        => ExecutionTest("nested-if-import-then-returns-thenable");

    [Fact(DisplayName = "nested-if-import-then-specifier-tostring.js")]
    public Task nested_if_import_then_specifier_tostring()
        => ExecutionTest("nested-if-import-then-specifier-tostring");

    [Fact(DisplayName = "nested-while-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task nested_while_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("nested-while-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "nested-while-import-then-eval-gtbndng-indirect-update.js")]
    public Task nested_while_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("nested-while-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "nested-while-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task nested_while_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("nested-while-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "nested-while-import-then-is-call-expression-square-brackets.js")]
    public Task nested_while_import_then_is_call_expression_square_brackets()
        => ExecutionTest("nested-while-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "nested-while-import-then-returns-thenable.js")]
    public Task nested_while_import_then_returns_thenable()
        => ExecutionTest("nested-while-import-then-returns-thenable");

    [Fact(DisplayName = "nested-while-import-then-specifier-tostring.js")]
    public Task nested_while_import_then_specifier_tostring()
        => ExecutionTest("nested-while-import-then-specifier-tostring");

    [Fact(DisplayName = "syntax-nested-block-labeled-eval-gtbndng-indirect-update-dflt.js")]
    public Task syntax_nested_block_labeled_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("syntax-nested-block-labeled-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "syntax-nested-block-labeled-eval-gtbndng-indirect-update.js")]
    public Task syntax_nested_block_labeled_eval_gtbndng_indirect_update()
        => ExecutionTest("syntax-nested-block-labeled-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "syntax-nested-block-labeled-eval-script-code-host-resolves-module-code.js")]
    public Task syntax_nested_block_labeled_eval_script_code_host_resolves_module_code()
        => ExecutionTest("syntax-nested-block-labeled-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "syntax-nested-block-labeled-is-call-expression-square-brackets.js")]
    public Task syntax_nested_block_labeled_is_call_expression_square_brackets()
        => ExecutionTest("syntax-nested-block-labeled-is-call-expression-square-brackets");

    [Fact(DisplayName = "syntax-nested-block-labeled-returns-thenable.js")]
    public Task syntax_nested_block_labeled_returns_thenable()
        => ExecutionTest("syntax-nested-block-labeled-returns-thenable");

    [Fact(DisplayName = "syntax-nested-block-labeled-specifier-tostring.js")]
    public Task syntax_nested_block_labeled_specifier_tostring()
        => ExecutionTest("syntax-nested-block-labeled-specifier-tostring");

    [Fact(DisplayName = "top-level-import-then-eval-gtbndng-indirect-update-dflt.js")]
    public Task top_level_import_then_eval_gtbndng_indirect_update_dflt()
        => ExecutionTest("top-level-import-then-eval-gtbndng-indirect-update-dflt");

    [Fact(DisplayName = "top-level-import-then-eval-gtbndng-indirect-update.js")]
    public Task top_level_import_then_eval_gtbndng_indirect_update()
        => ExecutionTest("top-level-import-then-eval-gtbndng-indirect-update");

    [Fact(DisplayName = "top-level-import-then-eval-script-code-host-resolves-module-code.js")]
    public Task top_level_import_then_eval_script_code_host_resolves_module_code()
        => ExecutionTest("top-level-import-then-eval-script-code-host-resolves-module-code");

    [Fact(DisplayName = "top-level-import-then-is-call-expression-square-brackets.js")]
    public Task top_level_import_then_is_call_expression_square_brackets()
        => ExecutionTest("top-level-import-then-is-call-expression-square-brackets");

    [Fact(DisplayName = "top-level-import-then-returns-thenable.js")]
    public Task top_level_import_then_returns_thenable()
        => ExecutionTest("top-level-import-then-returns-thenable");

    [Fact(DisplayName = "top-level-import-then-specifier-tostring.js")]
    public Task top_level_import_then_specifier_tostring()
        => ExecutionTest("top-level-import-then-specifier-tostring");
}
