using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.async_gen_method;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.async_gen_method") { }

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined.js")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTestFromFile("dflt-params-arg-val-not-undefined");
}
