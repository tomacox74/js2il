namespace Jroc.Test262.Tests.language.expressions.dynamic_import.@catch;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.catch") { }

    [Fact(DisplayName = "nested-arrow-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_arrow_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-arrow-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-arrow-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task nested_arrow_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-arrow-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-arrow-import-catch-eval-script-code-target.js")]
    public Task nested_arrow_import_catch_eval_script_code_target()
        => ExecutionTest("nested-arrow-import-catch-eval-script-code-target");

    [Fact(DisplayName = "nested-arrow-import-catch-file-does-not-exist.js")]
    public Task nested_arrow_import_catch_file_does_not_exist()
        => ExecutionTest("nested-arrow-import-catch-file-does-not-exist");

    [Fact(DisplayName = "nested-arrow-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_arrow_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-arrow-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-arrow-import-catch-import-source-source-text-module.js")]
    public Task nested_arrow_import_catch_import_source_source_text_module()
        => ExecutionTest("nested-arrow-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "nested-arrow-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_arrow_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-arrow-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-arrow-import-catch-import-source-specifier-tostring.js")]
    public Task nested_arrow_import_catch_import_source_specifier_tostring()
        => ExecutionTest("nested-arrow-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-arrow-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task nested_arrow_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-arrow-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-arrow-import-catch-instn-iee-err-circular.js")]
    public Task nested_arrow_import_catch_instn_iee_err_circular()
        => ExecutionTest("nested-arrow-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "nested-arrow-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task nested_arrow_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-arrow-import-catch-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-arrow-function-await-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_arrow_function_await_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-arrow-function-await-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-arrow-function-await-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_arrow_function_await_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-arrow-function-await-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-arrow-function-await-eval-script-code-target.js")]
    public Task nested_async_arrow_function_await_eval_script_code_target()
        => ExecutionTest("nested-async-arrow-function-await-eval-script-code-target");

    [Fact(DisplayName = "nested-async-arrow-function-await-file-does-not-exist.js")]
    public Task nested_async_arrow_function_await_file_does_not_exist()
        => ExecutionTest("nested-async-arrow-function-await-file-does-not-exist");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_arrow_function_await_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-arrow-function-await-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-source-text-module.js")]
    public Task nested_async_arrow_function_await_import_source_source_text_module()
        => ExecutionTest("nested-async-arrow-function-await-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_arrow_function_await_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-arrow-function-await-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-arrow-function-await-import-source-specifier-tostring.js")]
    public Task nested_async_arrow_function_await_import_source_specifier_tostring()
        => ExecutionTest("nested-async-arrow-function-await-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-arrow-function-await-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_arrow_function_await_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-arrow-function-await-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-arrow-function-await-instn-iee-err-circular.js")]
    public Task nested_async_arrow_function_await_instn_iee_err_circular()
        => ExecutionTest("nested-async-arrow-function-await-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-arrow-function-await-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_arrow_function_await_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-arrow-function-await-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_arrow_function_return_await_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-arrow-function-return-await-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_arrow_function_return_await_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-arrow-function-return-await-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-eval-script-code-target.js")]
    public Task nested_async_arrow_function_return_await_eval_script_code_target()
        => ExecutionTest("nested-async-arrow-function-return-await-eval-script-code-target");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-file-does-not-exist.js")]
    public Task nested_async_arrow_function_return_await_file_does_not_exist()
        => ExecutionTest("nested-async-arrow-function-return-await-file-does-not-exist");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_arrow_function_return_await_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-arrow-function-return-await-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-source-text-module.js")]
    public Task nested_async_arrow_function_return_await_import_source_source_text_module()
        => ExecutionTest("nested-async-arrow-function-return-await-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_arrow_function_return_await_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-arrow-function-return-await-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-import-source-specifier-tostring.js")]
    public Task nested_async_arrow_function_return_await_import_source_specifier_tostring()
        => ExecutionTest("nested-async-arrow-function-return-await-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_arrow_function_return_await_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-arrow-function-return-await-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-instn-iee-err-circular.js")]
    public Task nested_async_arrow_function_return_await_instn_iee_err_circular()
        => ExecutionTest("nested-async-arrow-function-return-await-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_arrow_function_return_await_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-arrow-function-return-await-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-await-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_function_await_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-function-await-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-function-await-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_function_await_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-function-await-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-function-await-eval-script-code-target.js")]
    public Task nested_async_function_await_eval_script_code_target()
        => ExecutionTest("nested-async-function-await-eval-script-code-target");

    [Fact(DisplayName = "nested-async-function-await-file-does-not-exist.js")]
    public Task nested_async_function_await_file_does_not_exist()
        => ExecutionTest("nested-async-function-await-file-does-not-exist");

    [Fact(DisplayName = "nested-async-function-await-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_await_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-await-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-await-import-source-source-text-module.js")]
    public Task nested_async_function_await_import_source_source_text_module()
        => ExecutionTest("nested-async-function-await-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-function-await-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_await_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-await-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-await-import-source-specifier-tostring.js")]
    public Task nested_async_function_await_import_source_specifier_tostring()
        => ExecutionTest("nested-async-function-await-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-function-await-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_function_await_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-function-await-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-function-await-instn-iee-err-circular.js")]
    public Task nested_async_function_await_instn_iee_err_circular()
        => ExecutionTest("nested-async-function-await-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-function-await-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_await_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-await-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_function_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-function-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-function-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_function_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-function-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-function-eval-script-code-target.js")]
    public Task nested_async_function_eval_script_code_target()
        => ExecutionTest("nested-async-function-eval-script-code-target");

    [Fact(DisplayName = "nested-async-function-file-does-not-exist.js")]
    public Task nested_async_function_file_does_not_exist()
        => ExecutionTest("nested-async-function-file-does-not-exist");

    [Fact(DisplayName = "nested-async-function-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-import-source-source-text-module.js")]
    public Task nested_async_function_import_source_source_text_module()
        => ExecutionTest("nested-async-function-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-function-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-import-source-specifier-tostring.js")]
    public Task nested_async_function_import_source_specifier_tostring()
        => ExecutionTest("nested-async-function-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-function-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_function_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-function-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-function-instn-iee-err-circular.js")]
    public Task nested_async_function_instn_iee_err_circular()
        => ExecutionTest("nested-async-function-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-function-return-await-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_function_return_await_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-function-return-await-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-function-return-await-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_function_return_await_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-function-return-await-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-function-return-await-eval-script-code-target.js")]
    public Task nested_async_function_return_await_eval_script_code_target()
        => ExecutionTest("nested-async-function-return-await-eval-script-code-target");

    [Fact(DisplayName = "nested-async-function-return-await-file-does-not-exist.js")]
    public Task nested_async_function_return_await_file_does_not_exist()
        => ExecutionTest("nested-async-function-return-await-file-does-not-exist");

    [Fact(DisplayName = "nested-async-function-return-await-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_return_await_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-return-await-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-source-text-module.js")]
    public Task nested_async_function_return_await_import_source_source_text_module()
        => ExecutionTest("nested-async-function-return-await-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_return_await_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-return-await-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-return-await-import-source-specifier-tostring.js")]
    public Task nested_async_function_return_await_import_source_specifier_tostring()
        => ExecutionTest("nested-async-function-return-await-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-function-return-await-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_function_return_await_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-function-return-await-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-function-return-await-instn-iee-err-circular.js")]
    public Task nested_async_function_return_await_instn_iee_err_circular()
        => ExecutionTest("nested-async-function-return-await-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-function-return-await-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_return_await_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-return-await-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-function-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_function_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-function-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-gen-await-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_gen_await_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-gen-await-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-gen-await-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_gen_await_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-gen-await-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-gen-await-eval-script-code-target.js")]
    public Task nested_async_gen_await_eval_script_code_target()
        => ExecutionTest("nested-async-gen-await-eval-script-code-target");

    [Fact(DisplayName = "nested-async-gen-await-file-does-not-exist.js")]
    public Task nested_async_gen_await_file_does_not_exist()
        => ExecutionTest("nested-async-gen-await-file-does-not-exist");

    [Fact(DisplayName = "nested-async-gen-await-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_gen_await_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-gen-await-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-gen-await-import-source-source-text-module.js")]
    public Task nested_async_gen_await_import_source_source_text_module()
        => ExecutionTest("nested-async-gen-await-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-gen-await-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_gen_await_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-gen-await-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-gen-await-import-source-specifier-tostring.js")]
    public Task nested_async_gen_await_import_source_specifier_tostring()
        => ExecutionTest("nested-async-gen-await-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-gen-await-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_gen_await_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-gen-await-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-gen-await-instn-iee-err-circular.js")]
    public Task nested_async_gen_await_instn_iee_err_circular()
        => ExecutionTest("nested-async-gen-await-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-gen-await-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_gen_await_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-gen-await-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-gen-return-await-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_async_gen_return_await_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-async-gen-return-await-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-async-gen-return-await-eval-rqstd-abrupt-urierror.js")]
    public Task nested_async_gen_return_await_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-async-gen-return-await-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-async-gen-return-await-eval-script-code-target.js")]
    public Task nested_async_gen_return_await_eval_script_code_target()
        => ExecutionTest("nested-async-gen-return-await-eval-script-code-target");

    [Fact(DisplayName = "nested-async-gen-return-await-file-does-not-exist.js")]
    public Task nested_async_gen_return_await_file_does_not_exist()
        => ExecutionTest("nested-async-gen-return-await-file-does-not-exist");

    [Fact(DisplayName = "nested-async-gen-return-await-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_gen_return_await_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-gen-return-await-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-gen-return-await-import-source-source-text-module.js")]
    public Task nested_async_gen_return_await_import_source_source_text_module()
        => ExecutionTest("nested-async-gen-return-await-import-source-source-text-module");

    [Fact(DisplayName = "nested-async-gen-return-await-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_gen_return_await_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-gen-return-await-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-async-gen-return-await-import-source-specifier-tostring.js")]
    public Task nested_async_gen_return_await_import_source_specifier_tostring()
        => ExecutionTest("nested-async-gen-return-await-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-async-gen-return-await-instn-iee-err-ambiguous-import.js")]
    public Task nested_async_gen_return_await_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-async-gen-return-await-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-async-gen-return-await-instn-iee-err-circular.js")]
    public Task nested_async_gen_return_await_instn_iee_err_circular()
        => ExecutionTest("nested-async-gen-return-await-instn-iee-err-circular");

    [Fact(DisplayName = "nested-async-gen-return-await-specifier-tostring-abrupt-rejects.js")]
    public Task nested_async_gen_return_await_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-async-gen-return-await-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-block-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_block_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-block-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-block-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task nested_block_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-block-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-block-import-catch-eval-script-code-target.js")]
    public Task nested_block_import_catch_eval_script_code_target()
        => ExecutionTest("nested-block-import-catch-eval-script-code-target");

    [Fact(DisplayName = "nested-block-import-catch-file-does-not-exist.js")]
    public Task nested_block_import_catch_file_does_not_exist()
        => ExecutionTest("nested-block-import-catch-file-does-not-exist");

    [Fact(DisplayName = "nested-block-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_block_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-block-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-block-import-catch-import-source-source-text-module.js")]
    public Task nested_block_import_catch_import_source_source_text_module()
        => ExecutionTest("nested-block-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "nested-block-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_block_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-block-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-block-import-catch-import-source-specifier-tostring.js")]
    public Task nested_block_import_catch_import_source_specifier_tostring()
        => ExecutionTest("nested-block-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-block-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task nested_block_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-block-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-block-import-catch-instn-iee-err-circular.js")]
    public Task nested_block_import_catch_instn_iee_err_circular()
        => ExecutionTest("nested-block-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "nested-block-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task nested_block_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-block-import-catch-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-block-labeled-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_block_labeled_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-block-labeled-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-block-labeled-eval-rqstd-abrupt-urierror.js")]
    public Task nested_block_labeled_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-block-labeled-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-block-labeled-eval-script-code-target.js")]
    public Task nested_block_labeled_eval_script_code_target()
        => ExecutionTest("nested-block-labeled-eval-script-code-target");

    [Fact(DisplayName = "nested-block-labeled-file-does-not-exist.js")]
    public Task nested_block_labeled_file_does_not_exist()
        => ExecutionTest("nested-block-labeled-file-does-not-exist");

    [Fact(DisplayName = "nested-block-labeled-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_block_labeled_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-block-labeled-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-block-labeled-import-source-source-text-module.js")]
    public Task nested_block_labeled_import_source_source_text_module()
        => ExecutionTest("nested-block-labeled-import-source-source-text-module");

    [Fact(DisplayName = "nested-block-labeled-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_block_labeled_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-block-labeled-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-block-labeled-import-source-specifier-tostring.js")]
    public Task nested_block_labeled_import_source_specifier_tostring()
        => ExecutionTest("nested-block-labeled-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-block-labeled-instn-iee-err-ambiguous-import.js")]
    public Task nested_block_labeled_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-block-labeled-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-block-labeled-instn-iee-err-circular.js")]
    public Task nested_block_labeled_instn_iee_err_circular()
        => ExecutionTest("nested-block-labeled-instn-iee-err-circular");

    [Fact(DisplayName = "nested-block-labeled-specifier-tostring-abrupt-rejects.js")]
    public Task nested_block_labeled_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-block-labeled-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-do-while-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_do_while_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-do-while-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-do-while-eval-rqstd-abrupt-urierror.js")]
    public Task nested_do_while_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-do-while-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-do-while-eval-script-code-target.js")]
    public Task nested_do_while_eval_script_code_target()
        => ExecutionTest("nested-do-while-eval-script-code-target");

    [Fact(DisplayName = "nested-do-while-file-does-not-exist.js")]
    public Task nested_do_while_file_does_not_exist()
        => ExecutionTest("nested-do-while-file-does-not-exist");

    [Fact(DisplayName = "nested-do-while-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_do_while_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-do-while-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-do-while-import-source-source-text-module.js")]
    public Task nested_do_while_import_source_source_text_module()
        => ExecutionTest("nested-do-while-import-source-source-text-module");

    [Fact(DisplayName = "nested-do-while-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_do_while_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-do-while-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-do-while-import-source-specifier-tostring.js")]
    public Task nested_do_while_import_source_specifier_tostring()
        => ExecutionTest("nested-do-while-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-do-while-instn-iee-err-ambiguous-import.js")]
    public Task nested_do_while_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-do-while-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-do-while-instn-iee-err-circular.js")]
    public Task nested_do_while_instn_iee_err_circular()
        => ExecutionTest("nested-do-while-instn-iee-err-circular");

    [Fact(DisplayName = "nested-do-while-specifier-tostring-abrupt-rejects.js")]
    public Task nested_do_while_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-do-while-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-else-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_else_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-else-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-else-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task nested_else_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-else-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-else-import-catch-eval-script-code-target.js")]
    public Task nested_else_import_catch_eval_script_code_target()
        => ExecutionTest("nested-else-import-catch-eval-script-code-target");

    [Fact(DisplayName = "nested-else-import-catch-file-does-not-exist.js")]
    public Task nested_else_import_catch_file_does_not_exist()
        => ExecutionTest("nested-else-import-catch-file-does-not-exist");

    [Fact(DisplayName = "nested-else-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_else_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-else-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-else-import-catch-import-source-source-text-module.js")]
    public Task nested_else_import_catch_import_source_source_text_module()
        => ExecutionTest("nested-else-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "nested-else-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_else_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-else-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-else-import-catch-import-source-specifier-tostring.js")]
    public Task nested_else_import_catch_import_source_specifier_tostring()
        => ExecutionTest("nested-else-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-else-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task nested_else_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-else-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-else-import-catch-instn-iee-err-circular.js")]
    public Task nested_else_import_catch_instn_iee_err_circular()
        => ExecutionTest("nested-else-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "nested-else-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task nested_else_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-else-import-catch-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-function-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_function_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-function-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-function-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task nested_function_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-function-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-function-import-catch-eval-script-code-target.js")]
    public Task nested_function_import_catch_eval_script_code_target()
        => ExecutionTest("nested-function-import-catch-eval-script-code-target");

    [Fact(DisplayName = "nested-function-import-catch-file-does-not-exist.js")]
    public Task nested_function_import_catch_file_does_not_exist()
        => ExecutionTest("nested-function-import-catch-file-does-not-exist");

    [Fact(DisplayName = "nested-function-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_function_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-function-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-function-import-catch-import-source-source-text-module.js")]
    public Task nested_function_import_catch_import_source_source_text_module()
        => ExecutionTest("nested-function-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "nested-function-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_function_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-function-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-function-import-catch-import-source-specifier-tostring.js")]
    public Task nested_function_import_catch_import_source_specifier_tostring()
        => ExecutionTest("nested-function-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-function-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task nested_function_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-function-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-function-import-catch-instn-iee-err-circular.js")]
    public Task nested_function_import_catch_instn_iee_err_circular()
        => ExecutionTest("nested-function-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "nested-function-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task nested_function_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-function-import-catch-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-if-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_if_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-if-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-if-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task nested_if_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-if-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-if-import-catch-eval-script-code-target.js")]
    public Task nested_if_import_catch_eval_script_code_target()
        => ExecutionTest("nested-if-import-catch-eval-script-code-target");

    [Fact(DisplayName = "nested-if-import-catch-file-does-not-exist.js")]
    public Task nested_if_import_catch_file_does_not_exist()
        => ExecutionTest("nested-if-import-catch-file-does-not-exist");

    [Fact(DisplayName = "nested-if-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_if_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-if-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-if-import-catch-import-source-source-text-module.js")]
    public Task nested_if_import_catch_import_source_source_text_module()
        => ExecutionTest("nested-if-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "nested-if-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_if_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-if-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-if-import-catch-import-source-specifier-tostring.js")]
    public Task nested_if_import_catch_import_source_specifier_tostring()
        => ExecutionTest("nested-if-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-if-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task nested_if_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-if-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-if-import-catch-instn-iee-err-circular.js")]
    public Task nested_if_import_catch_instn_iee_err_circular()
        => ExecutionTest("nested-if-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "nested-if-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task nested_if_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-if-import-catch-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-while-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task nested_while_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("nested-while-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "nested-while-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task nested_while_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("nested-while-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "nested-while-import-catch-eval-script-code-target.js")]
    public Task nested_while_import_catch_eval_script_code_target()
        => ExecutionTest("nested-while-import-catch-eval-script-code-target");

    [Fact(DisplayName = "nested-while-import-catch-file-does-not-exist.js")]
    public Task nested_while_import_catch_file_does_not_exist()
        => ExecutionTest("nested-while-import-catch-file-does-not-exist");

    [Fact(DisplayName = "nested-while-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task nested_while_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-while-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-while-import-catch-import-source-source-text-module.js")]
    public Task nested_while_import_catch_import_source_source_text_module()
        => ExecutionTest("nested-while-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "nested-while-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task nested_while_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-while-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "nested-while-import-catch-import-source-specifier-tostring.js")]
    public Task nested_while_import_catch_import_source_specifier_tostring()
        => ExecutionTest("nested-while-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "nested-while-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task nested_while_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("nested-while-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "nested-while-import-catch-instn-iee-err-circular.js")]
    public Task nested_while_import_catch_instn_iee_err_circular()
        => ExecutionTest("nested-while-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "nested-while-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task nested_while_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("nested-while-import-catch-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "top-level-import-catch-eval-rqstd-abrupt-typeerror.js")]
    public Task top_level_import_catch_eval_rqstd_abrupt_typeerror()
        => ExecutionTest("top-level-import-catch-eval-rqstd-abrupt-typeerror");

    [Fact(DisplayName = "top-level-import-catch-eval-rqstd-abrupt-urierror.js")]
    public Task top_level_import_catch_eval_rqstd_abrupt_urierror()
        => ExecutionTest("top-level-import-catch-eval-rqstd-abrupt-urierror");

    [Fact(DisplayName = "top-level-import-catch-eval-script-code-target.js")]
    public Task top_level_import_catch_eval_script_code_target()
        => ExecutionTest("top-level-import-catch-eval-script-code-target");

    [Fact(DisplayName = "top-level-import-catch-file-does-not-exist.js")]
    public Task top_level_import_catch_file_does_not_exist()
        => ExecutionTest("top-level-import-catch-file-does-not-exist");

    [Fact(DisplayName = "top-level-import-catch-import-defer-specifier-tostring-abrupt-rejects.js")]
    public Task top_level_import_catch_import_defer_specifier_tostring_abrupt_rejects()
        => ExecutionTest("top-level-import-catch-import-defer-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "top-level-import-catch-import-source-source-text-module.js")]
    public Task top_level_import_catch_import_source_source_text_module()
        => ExecutionTest("top-level-import-catch-import-source-source-text-module");

    [Fact(DisplayName = "top-level-import-catch-import-source-specifier-tostring-abrupt-rejects.js")]
    public Task top_level_import_catch_import_source_specifier_tostring_abrupt_rejects()
        => ExecutionTest("top-level-import-catch-import-source-specifier-tostring-abrupt-rejects");

    [Fact(DisplayName = "top-level-import-catch-import-source-specifier-tostring.js")]
    public Task top_level_import_catch_import_source_specifier_tostring()
        => ExecutionTest("top-level-import-catch-import-source-specifier-tostring");

    [Fact(DisplayName = "top-level-import-catch-instn-iee-err-ambiguous-import.js")]
    public Task top_level_import_catch_instn_iee_err_ambiguous_import()
        => ExecutionTest("top-level-import-catch-instn-iee-err-ambiguous-import");

    [Fact(DisplayName = "top-level-import-catch-instn-iee-err-circular.js")]
    public Task top_level_import_catch_instn_iee_err_circular()
        => ExecutionTest("top-level-import-catch-instn-iee-err-circular");

    [Fact(DisplayName = "top-level-import-catch-specifier-tostring-abrupt-rejects.js")]
    public Task top_level_import_catch_specifier_tostring_abrupt_rejects()
        => ExecutionTest("top-level-import-catch-specifier-tostring-abrupt-rejects");
}
