using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.decorator.syntax.valid;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.decorator.syntax.valid") { }

    [Fact(DisplayName = "decorator-call-expr-identifier-reference-yield.js")]
    public Task decorator_call_expr_identifier_reference_yield()
        => ExecutionTestFromFile("decorator-call-expr-identifier-reference-yield");

    [Fact(DisplayName = "decorator-call-expr-identifier-reference.js")]
    public Task decorator_call_expr_identifier_reference()
        => ExecutionTestFromFile("decorator-call-expr-identifier-reference");

    [Fact(DisplayName = "decorator-member-expr-decorator-member-expr.js")]
    public Task decorator_member_expr_decorator_member_expr()
        => ExecutionTestFromFile("decorator-member-expr-decorator-member-expr");

    [Fact(DisplayName = "decorator-member-expr-identifier-reference-yield.js")]
    public Task decorator_member_expr_identifier_reference_yield()
        => ExecutionTestFromFile("decorator-member-expr-identifier-reference-yield");

    [Fact(DisplayName = "decorator-member-expr-identifier-reference.js")]
    public Task decorator_member_expr_identifier_reference()
        => ExecutionTestFromFile("decorator-member-expr-identifier-reference");

    [Fact(DisplayName = "decorator-parenthesized-expr-identifier-reference-yield.js")]
    public Task decorator_parenthesized_expr_identifier_reference_yield()
        => ExecutionTestFromFile("decorator-parenthesized-expr-identifier-reference-yield");

    [Fact(DisplayName = "decorator-parenthesized-expr-identifier-reference.js")]
    public Task decorator_parenthesized_expr_identifier_reference()
        => ExecutionTestFromFile("decorator-parenthesized-expr-identifier-reference");
}
