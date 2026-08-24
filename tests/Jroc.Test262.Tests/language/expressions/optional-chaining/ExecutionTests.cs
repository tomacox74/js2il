using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.optional_chaining;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.optional_chaining") { }

    [Fact(DisplayName = "call-expression")]
    public Task call_expression()
        => ExecutionTest("call-expression");

    [Fact(DisplayName = "iteration-statement-do")]
    public Task iteration_statement_do()
        => ExecutionTest("iteration-statement-do");

    [Fact(DisplayName = "iteration-statement-for-in")]
    public Task iteration_statement_for_in()
        => ExecutionTest("iteration-statement-for-in");

    [Fact(DisplayName = "optional-call-preserves-this")]
    public Task optional_call_preserves_this()
        => ExecutionTest("optional-call-preserves-this");

    [Fact(DisplayName = "short-circuiting")]
    public Task short_circuiting()
        => ExecutionTest("short-circuiting");

    [Fact(DisplayName = "super-property-optional-call")]
    public Task super_property_optional_call()
        => ExecutionTest("super-property-optional-call");
}
