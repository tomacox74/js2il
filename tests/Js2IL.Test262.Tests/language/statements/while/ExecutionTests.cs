using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.while_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\while", "language.statements.while_") { }

    [Fact(DisplayName = "S12.6.2_A10")]
    public Task S12_6_2_A10()
        => ExecutionTest("S12.6.2_A10");

    [Fact(DisplayName = "S12.6.2_A11")]
    public Task S12_6_2_A11()
        => ExecutionTest("S12.6.2_A11");

    [Fact(DisplayName = "S12.6.2_A14_T1")]
    public Task S12_6_2_A14_T1()
        => ExecutionTest("S12.6.2_A14_T1");

    [Fact(DisplayName = "S12.6.2_A14_T2")]
    public Task S12_6_2_A14_T2()
        => ExecutionTest("S12.6.2_A14_T2");

    [Fact(DisplayName = "S12.6.2_A4_T1")]
    public Task S12_6_2_A4_T1()
        => ExecutionTest("S12.6.2_A4_T1");
}
