using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function.forbidden_ext.b2;

public sealed class ExpressionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ExpressionCompletionExecutionTests() : base("language.expressions.async_arrow_function.forbidden_ext.b2") { }

    [Fact(DisplayName = "async-arrow-function-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_async_arrow_function_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTest("async-arrow-function-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-arrow-function-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_async_arrow_function_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTest("async-arrow-function-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-arrow-function-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_async_arrow_function_forbidden_ext_indirect_access_prop_caller() => ExecutionTest("async-arrow-function-forbidden-ext-indirect-access-prop-caller");
}
