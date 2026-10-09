using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.async_method;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.async_method") { }

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task dflt_params_abrupt()
        => ExecutionTestFromFile("dflt-params-abrupt");
}
