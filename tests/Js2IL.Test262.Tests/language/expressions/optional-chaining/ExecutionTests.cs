using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.optional_chaining;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.optional_chaining") { }

    [Fact(DisplayName = "call-expression", Skip = "Known JS2IL defect")]
    public Task call_expression()
        => ExecutionTest("call-expression");

    [Fact(DisplayName = "iteration-statement-do")]
    public Task iteration_statement_do()
        => ExecutionTest("iteration-statement-do");

    [Fact(DisplayName = "iteration-statement-for-in")]
    public Task iteration_statement_for_in()
        => ExecutionTest("iteration-statement-for-in");
}
