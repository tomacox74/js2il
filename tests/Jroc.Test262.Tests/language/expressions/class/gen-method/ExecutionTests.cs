using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.gen_method;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.gen-method") { }

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task ported_dflt_params_abrupt() => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task ported_dflt_params_ref_later() => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task ported_dflt_params_ref_self() => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "yield-spread-arr-multiple.js")]
    public Task ported_yield_spread_arr_multiple() => ExecutionTest("yield-spread-arr-multiple");

    [Fact(DisplayName = "yield-spread-arr-single.js")]
    public Task ported_yield_spread_arr_single() => ExecutionTest("yield-spread-arr-single");

    [Fact(DisplayName = "yield-spread-obj.js")]
    public Task ported_yield_spread_obj() => ExecutionTest("yield-spread-obj");

}
