namespace Jroc.Test262.Tests.language.statements.test_class.name_binding;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.test_class.name_binding") { }

    [Fact(DisplayName = "in-extends-expression-assigned.js")]
    public Task in_extends_expression_assigned()
        => ExecutionTest("in-extends-expression-assigned");

    [Fact(DisplayName = "in-extends-expression-grouped.js")]
    public Task in_extends_expression_grouped()
        => ExecutionTest("in-extends-expression-grouped");

    [Fact(DisplayName = "in-extends-expression.js")]
    public Task in_extends_expression()
        => ExecutionTest("in-extends-expression");
}
