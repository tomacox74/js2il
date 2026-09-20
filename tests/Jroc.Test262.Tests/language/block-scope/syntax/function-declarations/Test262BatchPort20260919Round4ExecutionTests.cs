using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.syntax.function_declarations;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.block_scope.syntax.function_declarations") { }

    [Fact(DisplayName = "in-statement-position-default-statement-list")]
    public Task in_statement_position_default_statement_list()
        => ExecutionTest("in-statement-position-default-statement-list");

}
