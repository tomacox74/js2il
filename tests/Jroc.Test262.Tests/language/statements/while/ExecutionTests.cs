using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.while_;

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
    [Fact(DisplayName = "S12.6.2_A1")]
    public Task S12_6_2_A1()
        => ExecutionTest("S12.6.2_A1");



    [Fact(DisplayName = "S12.6.2_A2")]
    public Task S12_6_2_A2()
        => ExecutionTest("S12.6.2_A2");

    [Fact(DisplayName = "S12.6.2_A4_T2")]
    public Task S12_6_2_A4_T2()
        => ExecutionTest("S12.6.2_A4_T2");

    [Fact(DisplayName = "S12.6.2_A4_T3")]
    public Task S12_6_2_A4_T3()
        => ExecutionTest("S12.6.2_A4_T3");

    [Fact(DisplayName = "S12.6.2_A4_T4")]
    public Task S12_6_2_A4_T4()
        => ExecutionTest("S12.6.2_A4_T4");

    [Fact(DisplayName = "S12.6.2_A4_T5")]
    public Task S12_6_2_A4_T5()
        => ExecutionTest("S12.6.2_A4_T5");

    [Fact(DisplayName = "S12.6.2_A9")]
    public Task S12_6_2_A9()
        => ExecutionTest("S12.6.2_A9");

    [Fact(DisplayName = "S12.6.2_A6_T1")]
    public Task S12_6_2_A6_T1()
        => CompilationFailureTest("S12.6.2_A6_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.2_A6_T2")]
    public Task S12_6_2_A6_T2()
        => CompilationFailureTest("S12.6.2_A6_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.2_A6_T3")]
    public Task S12_6_2_A6_T3()
        => CompilationFailureTest("S12.6.2_A6_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.2_A6_T4")]
    public Task S12_6_2_A6_T4()
        => CompilationFailureTest("S12.6.2_A6_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.2_A6_T5")]
    public Task S12_6_2_A6_T5()
        => CompilationFailureTest("S12.6.2_A6_T5", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.2_A15")]
    public Task S12_6_2_A15()
        => CompilationFailureTest("S12.6.2_A15", "Failed to parse JavaScript");

}
