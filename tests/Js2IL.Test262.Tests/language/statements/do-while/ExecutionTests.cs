using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.do_while;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\do-while", "language.statements.do_while") { }

    [Fact(DisplayName = "S12.6.1_A10")]
    public Task S12_6_1_A10()
        => ExecutionTest("S12.6.1_A10");

    [Fact(DisplayName = "S12.6.1_A11")]
    public Task S12_6_1_A11()
        => ExecutionTest("S12.6.1_A11");

    [Fact(DisplayName = "S12.6.1_A14_T1")]
    public Task S12_6_1_A14_T1()
        => ExecutionTest("S12.6.1_A14_T1");

    [Fact(DisplayName = "S12.6.1_A14_T2")]
    public Task S12_6_1_A14_T2()
        => ExecutionTest("S12.6.1_A14_T2");

    [Fact(DisplayName = "S12.6.1_A4_T1")]
    public Task S12_6_1_A4_T1()
        => ExecutionTest("S12.6.1_A4_T1");
    [Fact(DisplayName = "S12.6.1_A1")]
    public Task S12_6_1_A1()
        => ExecutionTest("S12.6.1_A1");

    [Fact(DisplayName = "S12.6.1_A4_T2")]
    public Task S12_6_1_A4_T2()
        => ExecutionTest("S12.6.1_A4_T2");

    [Fact(DisplayName = "S12.6.1_A4_T3")]
    public Task S12_6_1_A4_T3()
        => ExecutionTest("S12.6.1_A4_T3");

    [Fact(DisplayName = "S12.6.1_A4_T4")]
    public Task S12_6_1_A4_T4()
        => ExecutionTest("S12.6.1_A4_T4");

}
