using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.async_function;

public class FunctionExpressionAsyncFunctionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionAsyncFunctionConformanceBatchExecutionTests() : base("language/expressions/async-function", "language.expressions.async_function") { }

    [Fact(DisplayName = "expression-returns-promise.js")]
    public Task expression_returns_promise()
        => ExecutionTest("expression-returns-promise");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "syntax-expression-is-PrimaryExpression.js")]
    public Task syntax_expression_is_PrimaryExpression()
        => ExecutionTest("syntax-expression-is-PrimaryExpression");

}
