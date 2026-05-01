using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.block;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\block", "language.statements.block") { }

    [Fact(DisplayName = "S12.1_A2", Skip = "ReferenceError and unresolved global call checks are not supported yet")]
    public Task S12_1_A2()
        => ExecutionTest("S12.1_A2");

    [Fact(DisplayName = "S12.1_A5")]
    public Task S12_1_A5()
        => ExecutionTest("S12.1_A5");

    [Fact(DisplayName = "scope-lex-close")]
    public Task scope_lex_close()
        => ExecutionTest("scope-lex-close");

    [Fact(DisplayName = "scope-lex-open")]
    public Task scope_lex_open()
        => ExecutionTest("scope-lex-open");

    [Fact(DisplayName = "scope-var-none")]
    public Task scope_var_none()
        => ExecutionTest("scope-var-none");
}
