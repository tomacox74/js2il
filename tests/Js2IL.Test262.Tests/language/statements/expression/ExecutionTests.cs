using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.expression;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\expression", "language.statements.expression") { }

    [Fact(DisplayName = "S12.4_A1")]
    public Task S12_4_A1()
        => CompilationFailureTest("S12.4_A1", "Unexpected token");
}
