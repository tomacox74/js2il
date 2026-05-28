using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.arrow_function;

public class PortAdditionalArrowCoverageExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalArrowCoverageExecutionTests() : base("language.expressions.arrow_function") { }

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

    [Fact(DisplayName = "length-dflt")]
    public Task length_dflt()
        => ExecutionTest("length-dflt");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "lexical-bindings-overriden-by-formal-parameters-non-strict")]
    public Task lexical_bindings_overriden_by_formal_parameters_non_strict()
        => ExecutionTest("lexical-bindings-overriden-by-formal-parameters-non-strict");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest("dstr/ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-rest-id")]
    public Task dstr_ary_ptrn_rest_id()
        => ExecutionTest("dstr/ary-ptrn-rest-id");

    [Fact(DisplayName = "low-precedence-expression-body-no-parens")]
    public Task low_precedence_expression_body_no_parens()
        => ExecutionTest("low-precedence-expression-body-no-parens");
    [Fact(DisplayName = "non-strict")]
    public Task non_strict()
        => ExecutionTest("non-strict");
    [Fact(DisplayName = "strict")]
    public Task strict()
        => ExecutionTest("strict");
    [Fact(DisplayName = "throw-new")]
    public Task throw_new()
        => ExecutionTest("throw-new");
    [Fact(DisplayName = "scope-paramsbody-var-close")]
    public Task scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");
    [Fact(DisplayName = "dflt-params-trailing-comma")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "statement-body-requires-braces-must-return-explicitly-missing")]
    public Task statement_body_requires_braces_must_return_explicitly_missing()
        => ExecutionTest("statement-body-requires-braces-must-return-explicitly-missing");
    [Fact(DisplayName = "statement-body-requires-braces-must-return-explicitly")]
    public Task statement_body_requires_braces_must_return_explicitly()
        => ExecutionTest("statement-body-requires-braces-must-return-explicitly");
}
