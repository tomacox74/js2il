using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.empty;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\empty", "language.statements.empty") { }

    [Fact(DisplayName = "S12.3_A1")]
    public Task S12_3_A1()
        => ExecutionTest("S12.3_A1");
}
