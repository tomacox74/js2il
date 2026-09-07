using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.function;

public class FunctionExpressionFunctionConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionFunctionConformanceBatchParseTests() : base("language/expressions/function", "language.expressions.function") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task array_destructuring_param_strict_body()
        => CompilationFailureTest("array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task dflt_params_duplicates()
        => CompilationFailureTest("dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task dflt_params_rest()
        => CompilationFailureTest("dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-ary.js")]
    public Task dstr_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-obj.js")]
    public Task dstr_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-body-super-call.js")]
    public Task early_body_super_call()
        => CompilationFailureTest("early-body-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-body-super-prop.js")]
    public Task early_body_super_prop()
        => CompilationFailureTest("early-body-super-prop", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-call-expression-bad-reference.js")]
    public Task early_errors_invalid_names_call_expression_bad_reference()
        => CompilationFailureTest("early-errors/invalid-names-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-call-expression-this.js")]
    public Task early_errors_invalid_names_call_expression_this()
        => CompilationFailureTest("early-errors/invalid-names-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-member-expression-bad-reference.js")]
    public Task early_errors_invalid_names_member_expression_bad_reference()
        => CompilationFailureTest("early-errors/invalid-names-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-names-member-expression-this.js")]
    public Task early_errors_invalid_names_member_expression_this()
        => CompilationFailureTest("early-errors/invalid-names-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-params-super-call.js")]
    public Task early_params_super_call()
        => CompilationFailureTest("early-params-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-params-super-prop.js")]
    public Task early_params_super_prop()
        => CompilationFailureTest("early-params-super-prop", "Failed to parse JavaScript");

    [Fact(DisplayName = "name-arguments-strict-body.js")]
    public Task name_arguments_strict_body()
        => CompilationFailureTest("name-arguments-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "name-arguments-strict.js")]
    public Task name_arguments_strict()
        => CompilationFailureTest("name-arguments-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task object_destructuring_param_strict_body()
        => CompilationFailureTest("object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "param-dflt-yield-strict.js")]
    public Task param_dflt_yield_strict()
        => CompilationFailureTest("param-dflt-yield-strict", "Failed to parse JavaScript");

}
