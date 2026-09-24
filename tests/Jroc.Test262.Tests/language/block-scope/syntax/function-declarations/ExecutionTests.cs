using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.syntax.function_declarations;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.syntax.function_declarations") { }

    [Fact(DisplayName = "in-statement-position-case-expression-statement-list")]
    public Task in_statement_position_case_expression_statement_list()
        => ExecutionTest("in-statement-position-case-expression-statement-list");

    [Fact(DisplayName = "in-statement-position-do-statement-while-expression.js")]
    public Task in_statement_position_do_statement_while_expression() => CompilationFailureTest("in-statement-position-do-statement-while-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "in-statement-position-for-statement.js")]
    public Task in_statement_position_for_statement() => CompilationFailureTest("in-statement-position-for-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "in-statement-position-if-expression-statement-else-statement.js")]
    public Task in_statement_position_if_expression_statement_else_statement() => CompilationFailureTest("in-statement-position-if-expression-statement-else-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "in-statement-position-if-expression-statement.js")]
    public Task in_statement_position_if_expression_statement() => CompilationFailureTest("in-statement-position-if-expression-statement", "Failed to parse JavaScript");

    [Fact(DisplayName = "in-statement-position-while-expression-statement.js")]
    public Task in_statement_position_while_expression_statement() => CompilationFailureTest("in-statement-position-while-expression-statement", "Failed to parse JavaScript");
}
