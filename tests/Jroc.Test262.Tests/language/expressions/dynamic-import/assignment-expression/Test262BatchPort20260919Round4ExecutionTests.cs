using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.dynamic_import.assignment_expression;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.expressions.dynamic_import.assignment_expression") { }

    [Fact(DisplayName = "yield-star")]
    public Task yield_star()
        => ExecutionTest("yield-star");

}
