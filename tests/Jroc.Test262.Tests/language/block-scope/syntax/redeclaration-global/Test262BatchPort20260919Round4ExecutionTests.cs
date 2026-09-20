using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.syntax.redeclaration_global;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.block_scope.syntax.redeclaration_global") { }

    [Fact(DisplayName = "allowed-to-declare-function-with-function-declaration")]
    public Task allowed_to_declare_function_with_function_declaration()
        => ExecutionTest("allowed-to-declare-function-with-function-declaration");

    [Fact(DisplayName = "allowed-to-redeclare-function-declaration-with-var")]
    public Task allowed_to_redeclare_function_declaration_with_var()
        => ExecutionTest("allowed-to-redeclare-function-declaration-with-var");

    [Fact(DisplayName = "allowed-to-redeclare-var-with-function-declaration")]
    public Task allowed_to_redeclare_var_with_function_declaration()
        => ExecutionTest("allowed-to-redeclare-var-with-function-declaration");

}
