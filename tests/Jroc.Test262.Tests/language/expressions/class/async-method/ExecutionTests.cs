using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.async_method;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.async-method") { }

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined.js")]
    public Task ported_dflt_params_arg_val_not_undefined() => ExecutionTest("dflt-params-arg-val-not-undefined");

}
