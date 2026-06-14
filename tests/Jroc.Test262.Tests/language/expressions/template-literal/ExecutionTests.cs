using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.template_literal;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.template_literal") { }

    [Fact(DisplayName = "evaluation-order")]
    public Task evaluation_order()
        => ExecutionTest("evaluation-order");

    [Fact(DisplayName = "literal-expr-function")]
    public Task literal_expr_function()
        => ExecutionTest("literal-expr-function");

    [Fact(DisplayName = "literal-expr-method")]
    public Task literal_expr_method()
        => ExecutionTest("literal-expr-method");
}
