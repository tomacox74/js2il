using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function.forbidden_ext.b1;

public sealed class ExpressionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ExpressionCompletionExecutionTests() : base("language.expressions.async_arrow_function.forbidden_ext.b1") { }

    [Fact(DisplayName = "async-arrow-function-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_async_arrow_function_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("async-arrow-function-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-arrow-function-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_async_arrow_function_forbidden_ext_direct_access_prop_caller() => ExecutionTest("async-arrow-function-forbidden-ext-direct-access-prop-caller");
}
