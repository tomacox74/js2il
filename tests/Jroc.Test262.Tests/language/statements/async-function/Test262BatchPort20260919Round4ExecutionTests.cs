using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.async_function;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.async_function") { }

    [Fact(DisplayName = "syntax-declaration-line-terminators-allowed")]
    public Task syntax_declaration_line_terminators_allowed()
        => ExecutionTest("syntax-declaration-line-terminators-allowed");

    [Fact(DisplayName = "syntax-declaration-no-line-terminator")]
    public Task syntax_declaration_no_line_terminator()
        => ExecutionTest("syntax-declaration-no-line-terminator");

}
