using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.async_generator;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.async_generator") { }

    [Fact(DisplayName = "expression-await-as-yield-operand")]
    public Task expression_await_as_yield_operand()
        => ExecutionTest("expression-await-as-yield-operand");

    [Fact(DisplayName = "expression-await-promise-as-yield-operand")]
    public Task expression_await_promise_as_yield_operand()
        => ExecutionTest("expression-await-promise-as-yield-operand");

    [Fact(DisplayName = "expression-await-thenable-as-yield-operand")]
    public Task expression_await_thenable_as_yield_operand()
        => ExecutionTest("expression-await-thenable-as-yield-operand");

    [Fact(DisplayName = "expression-yield-as-operand")]
    public Task expression_yield_as_operand()
        => ExecutionTest("expression-yield-as-operand");

    [Fact(DisplayName = "expression-yield-as-statement")]
    public Task expression_yield_as_statement()
        => ExecutionTest("expression-yield-as-statement");
}
