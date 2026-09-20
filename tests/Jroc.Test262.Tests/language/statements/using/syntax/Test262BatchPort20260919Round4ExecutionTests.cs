using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.@using.syntax;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.using.syntax") { }

    [Fact(DisplayName = "using-declaring-let-split-across-two-lines")]
    public Task using_declaring_let_split_across_two_lines()
        => ExecutionTest("using-declaring-let-split-across-two-lines");

    [Fact(DisplayName = "using-invalid-arraybindingpattern-does-not-break-element-access")]
    public Task using_invalid_arraybindingpattern_does_not_break_element_access()
        => ExecutionTest("using-invalid-arraybindingpattern-does-not-break-element-access");

}
