using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.function;

public class PortAdditionalFunctionCoverageExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalFunctionCoverageExecutionTests() : base("language.expressions.function") { }

    [Fact(DisplayName = "dflt-params-abrupt")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-later")]
    public Task dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-ref-self")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "dflt-params-trailing-comma")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body")]
    public Task named_no_strict_reassign_fn_name_in_body()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body")]
    public Task named_strict_error_reassign_fn_name_in_body()
        => ExecutionTest("named-strict-error-reassign-fn-name-in-body");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest("dstr/ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-rest-id")]
    public Task dstr_ary_ptrn_rest_id()
        => ExecutionTest("dstr/ary-ptrn-rest-id");

    [Fact(DisplayName = "ary-ptrn-empty")]
    public Task dstr_ary_ptrn_empty()
        => ExecutionTest("dstr/ary-ptrn-empty");
}
