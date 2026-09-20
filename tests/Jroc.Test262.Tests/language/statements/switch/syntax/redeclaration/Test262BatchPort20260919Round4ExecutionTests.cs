using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.@switch.syntax.redeclaration;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.switch.syntax.redeclaration") { }

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-var")]
    public Task var_name_redeclaration_attempt_with_var()
        => ExecutionTest("var-name-redeclaration-attempt-with-var");

}
