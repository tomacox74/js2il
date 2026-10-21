namespace Jroc.Test262.Tests.language.statements.test_class.gen_method_static;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.gen_method_static") { }

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined.js")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined.js")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-prior.js")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "dflt-params-trailing-comma.js")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "params-trailing-comma-multiple.js")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single.js")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "yield-spread-arr-multiple.js")]
    public Task yield_spread_arr_multiple()
        => ExecutionTest("yield-spread-arr-multiple");

    [Fact(DisplayName = "yield-spread-arr-single.js")]
    public Task yield_spread_arr_single()
        => ExecutionTest("yield-spread-arr-single");

    [Fact(DisplayName = "yield-spread-obj.js")]
    public Task yield_spread_obj()
        => ExecutionTest("yield-spread-obj");
}
