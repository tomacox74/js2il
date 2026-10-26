namespace Jroc.Test262.Tests.language.statements.test_class.decorator.syntax.valid;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.test_class.decorator.syntax.valid") { }

    [Fact(DisplayName = "class-element-decorator-call-expr-identifier-reference.js")]
    public Task class_element_decorator_call_expr_identifier_reference()
        => ExecutionTest("class-element-decorator-call-expr-identifier-reference");

    [Fact(DisplayName = "class-element-decorator-member-expr-decorator-member-expr.js")]
    public Task class_element_decorator_member_expr_decorator_member_expr()
        => ExecutionTest("class-element-decorator-member-expr-decorator-member-expr");

    [Fact(DisplayName = "class-element-decorator-member-expr-identifier-reference.js")]
    public Task class_element_decorator_member_expr_identifier_reference()
        => ExecutionTest("class-element-decorator-member-expr-identifier-reference");

    [Fact(DisplayName = "class-element-decorator-parenthesized-expr-identifier-reference.js")]
    public Task class_element_decorator_parenthesized_expr_identifier_reference()
        => ExecutionTest("class-element-decorator-parenthesized-expr-identifier-reference");
}
