namespace Jroc.Test262.Tests.language.statements.test_class.decorator.syntax.valid;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.decorator.syntax.valid") { }

    [Fact(DisplayName = "decorator-call-expr-identifier-reference-yield.js")]
    public Task decorator_call_expr_identifier_reference_yield()
        => ExecutionTest("decorator-call-expr-identifier-reference-yield");

    [Fact(DisplayName = "decorator-call-expr-identifier-reference.js")]
    public Task decorator_call_expr_identifier_reference()
        => ExecutionTest("decorator-call-expr-identifier-reference");

    [Fact(DisplayName = "decorator-member-expr-decorator-member-expr.js")]
    public Task decorator_member_expr_decorator_member_expr()
        => ExecutionTest("decorator-member-expr-decorator-member-expr");

    [Fact(DisplayName = "decorator-member-expr-identifier-reference-yield.js")]
    public Task decorator_member_expr_identifier_reference_yield()
        => ExecutionTest("decorator-member-expr-identifier-reference-yield");

    [Fact(DisplayName = "decorator-member-expr-identifier-reference.js")]
    public Task decorator_member_expr_identifier_reference()
        => ExecutionTest("decorator-member-expr-identifier-reference");

    [Fact(DisplayName = "decorator-parenthesized-expr-identifier-reference-yield.js")]
    public Task decorator_parenthesized_expr_identifier_reference_yield()
        => ExecutionTest("decorator-parenthesized-expr-identifier-reference-yield");

    [Fact(DisplayName = "decorator-parenthesized-expr-identifier-reference.js")]
    public Task decorator_parenthesized_expr_identifier_reference()
        => ExecutionTest("decorator-parenthesized-expr-identifier-reference");
}
