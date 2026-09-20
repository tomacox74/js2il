using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.syntax.redeclaration;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.block_scope.syntax.redeclaration") { }

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-var")]
    public Task fn_scope_var_name_redeclaration_attempt_with_var()
        => ExecutionTest("fn-scope-var-name-redeclaration-attempt-with-var");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-var")]
    public Task inner_block_var_name_redeclaration_attempt_with_var()
        => ExecutionTest("inner-block-var-name-redeclaration-attempt-with-var");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-var")]
    public Task var_name_redeclaration_attempt_with_var()
        => ExecutionTest("var-name-redeclaration-attempt-with-var");

}
