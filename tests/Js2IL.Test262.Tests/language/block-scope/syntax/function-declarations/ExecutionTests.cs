using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.block_scope.syntax.function_declarations;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.syntax.function_declarations") { }

    [Fact(DisplayName = "in-statement-position-case-expression-statement-list")]
    public Task in_statement_position_case_expression_statement_list()
        => ExecutionTest("in-statement-position-case-expression-statement-list");
}
