using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_generator;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.async_generator") { }

    [Fact(DisplayName = "expression-await-as-yield-operand")]
    public Task expression_await_as_yield_operand()
        => ExecutionTest("expression-await-as-yield-operand");

    [Fact(DisplayName = "expression-await-promise-as-yield-operand")]
    public Task expression_await_promise_as_yield_operand()
        => ExecutionTest("expression-await-promise-as-yield-operand");

    [Fact(DisplayName = "expression-await-thenable-as-yield-operand")]
    public Task expression_await_thenable_as_yield_operand()
        => ExecutionTest("expression-await-thenable-as-yield-operand");

    [Fact(DisplayName = "expression-yield-as-operand")]
    public Task expression_yield_as_operand()
        => ExecutionTest("expression-yield-as-operand");

    [Fact(DisplayName = "expression-yield-as-statement")]
    public Task expression_yield_as_statement()
        => ExecutionTest("expression-yield-as-statement");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-trailing-comma")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "expression-yield-newline")]
    public Task expression_yield_newline()
        => ExecutionTest("expression-yield-newline");

    [Fact(DisplayName = "expression-yield-star-before-newline")]
    public Task expression_yield_star_before_newline()
        => ExecutionTest("expression-yield-star-before-newline");

    [Fact(DisplayName = "generator-created-after-decl-inst")]
    public Task generator_created_after_decl_inst()
        => ExecutionTest("generator-created-after-decl-inst");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "yield-promise-reject-next")]
    public Task yield_promise_reject_next()
        => ExecutionTest("yield-promise-reject-next");

    [Fact(DisplayName = "yield-star-async-next")]
    public Task yield_star_async_next()
        => ExecutionTest("yield-star-async-next");

    [Fact(DisplayName = "dstr/named-dflt-obj-ptrn-prop-obj-value-undef")]
    public Task dstr_named_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-obj-value-undef");
}
