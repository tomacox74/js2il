using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.gen_method_static;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.gen-method-static") { }

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task ported_dflt_params_abrupt() => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task ported_dflt_params_ref_later() => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task ported_dflt_params_ref_self() => ExecutionTest("dflt-params-ref-self");

}
