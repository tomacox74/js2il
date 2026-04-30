using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.expression;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\expression", "language.statements.expression") { }

    [Fact(DisplayName = "S12.4_A2_T1", Skip = "eval is not supported by JS2IL yet")]
    public Task S12_4_A2_T1()
        => ExecutionTest("S12.4_A2_T1");

    [Fact(DisplayName = "S12.4_A2_T2", Skip = "eval is not supported by JS2IL yet")]
    public Task S12_4_A2_T2()
        => ExecutionTest("S12.4_A2_T2");
}
