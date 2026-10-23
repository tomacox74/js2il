namespace Jroc.Test262.Tests.language.expressions.dynamic_import;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import") { }

    [Fact(DisplayName = "always-create-new-promise.js")]
    public Task always_create_new_promise()
        => ExecutionTest("always-create-new-promise");

    [Fact(DisplayName = "assign-expr-get-value-abrupt-throws.js")]
    public Task assign_expr_get_value_abrupt_throws()
        => ExecutionTest("assign-expr-get-value-abrupt-throws");

    [Fact(DisplayName = "await-import-evaluation.js")]
    public Task await_import_evaluation()
        => ExecutionTest("await-import-evaluation");

    [Fact(DisplayName = "custom-primitive.js")]
    public Task custom_primitive()
        => ExecutionTest("custom-primitive");

    [Fact(DisplayName = "escape-sequence-import.js")]
    public Task escape_sequence_import()
        => CompilationFailureTest("escape-sequence-import", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-export-dflt-cls-anon.js")]
    public Task eval_export_dflt_cls_anon()
        => ExecutionTest("eval-export-dflt-cls-anon");

    [Fact(DisplayName = "eval-export-dflt-cls-name-meth.js")]
    public Task eval_export_dflt_cls_name_meth()
        => ExecutionTest("eval-export-dflt-cls-name-meth");

    [Fact(DisplayName = "eval-export-dflt-cls-named.js")]
    public Task eval_export_dflt_cls_named()
        => ExecutionTest("eval-export-dflt-cls-named");

    [Fact(DisplayName = "eval-export-dflt-expr-cls-anon.js")]
    public Task eval_export_dflt_expr_cls_anon()
        => ExecutionTest("eval-export-dflt-expr-cls-anon");

    [Fact(DisplayName = "eval-export-dflt-expr-cls-name-meth.js")]
    public Task eval_export_dflt_expr_cls_name_meth()
        => ExecutionTest("eval-export-dflt-expr-cls-name-meth");

    [Fact(DisplayName = "eval-export-dflt-expr-cls-named.js")]
    public Task eval_export_dflt_expr_cls_named()
        => ExecutionTest("eval-export-dflt-expr-cls-named");

    [Fact(DisplayName = "eval-export-dflt-expr-fn-anon.js")]
    public Task eval_export_dflt_expr_fn_anon()
        => ExecutionTest("eval-export-dflt-expr-fn-anon");

    [Fact(DisplayName = "eval-export-dflt-expr-fn-named.js")]
    public Task eval_export_dflt_expr_fn_named()
        => ExecutionTest("eval-export-dflt-expr-fn-named");

    [Fact(DisplayName = "eval-export-dflt-expr-gen-anon.js")]
    public Task eval_export_dflt_expr_gen_anon()
        => ExecutionTest("eval-export-dflt-expr-gen-anon");

    [Fact(DisplayName = "eval-export-dflt-expr-gen-named.js")]
    public Task eval_export_dflt_expr_gen_named()
        => ExecutionTest("eval-export-dflt-expr-gen-named");

    [Fact(DisplayName = "eval-export-dflt-expr-in.js")]
    public Task eval_export_dflt_expr_in()
        => ExecutionTest("eval-export-dflt-expr-in");

    [Fact(DisplayName = "eval-rqstd-once.js")]
    public Task eval_rqstd_once()
        => ExecutionTest("eval-rqstd-once");

    [Fact(DisplayName = "eval-self-once-module.js")]
    public Task eval_self_once_module()
        => ExecutionTest("eval-self-once-module");

    [Fact(DisplayName = "eval-self-once-script.js")]
    public Task eval_self_once_script()
        => ExecutionTest("eval-self-once-script");

    [Fact(DisplayName = "for-await-resolution-and-error-agen-yield.js")]
    public Task for_await_resolution_and_error_agen_yield()
        => ExecutionTest("for-await-resolution-and-error-agen-yield");

    [Fact(DisplayName = "for-await-resolution-and-error-agen.js")]
    public Task for_await_resolution_and_error_agen()
        => ExecutionTest("for-await-resolution-and-error-agen");

    [Fact(DisplayName = "for-await-resolution-and-error.js")]
    public Task for_await_resolution_and_error()
        => ExecutionTest("for-await-resolution-and-error");

    [Fact(DisplayName = "import-errored-module.js")]
    public Task import_errored_module()
        => ExecutionTest("import-errored-module");

    [Fact(DisplayName = "imported-self-update.js")]
    public Task imported_self_update()
        => ExecutionTest("imported-self-update");

    [Fact(DisplayName = "indirect-resolution.js")]
    public Task indirect_resolution()
        => ExecutionTest("indirect-resolution");

    [Fact(DisplayName = "returns-promise.js")]
    public Task returns_promise()
        => ExecutionTest("returns-promise");

    [Fact(DisplayName = "reuse-namespace-object-from-import.js")]
    public Task reuse_namespace_object_from_import()
        => ExecutionTest("reuse-namespace-object-from-import");

    [Fact(DisplayName = "reuse-namespace-object-from-script.js")]
    public Task reuse_namespace_object_from_script()
        => ExecutionTest("reuse-namespace-object-from-script");

    [Fact(DisplayName = "reuse-namespace-object.js")]
    public Task reuse_namespace_object()
        => ExecutionTest("reuse-namespace-object");

    [Fact(DisplayName = "update-to-dynamic-import.js")]
    public Task update_to_dynamic_import()
        => ExecutionTest("update-to-dynamic-import");

    [Fact(DisplayName = "usage-from-eval.js", Skip = "eval is not supported.")]
    public Task usage_from_eval()
        => ExecutionTest("usage-from-eval");
}
