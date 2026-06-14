using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.do_while;

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



    [Fact(DisplayName = "S12.6.1_A2")]
    public Task S12_6_1_A2()
        => ExecutionTest("S12.6.1_A2");

    [Fact(DisplayName = "S12.6.1_A4_T5")]
    public Task S12_6_1_A4_T5()
        => ExecutionTest("S12.6.1_A4_T5");

    [Fact(DisplayName = "S12.6.1_A9")]
    public Task S12_6_1_A9()
        => ExecutionTest("S12.6.1_A9");

    [Fact(DisplayName = "S12.6.1_A6_T1")]
    public Task S12_6_1_A6_T1()
        => CompilationFailureTest("S12.6.1_A6_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A6_T2")]
    public Task S12_6_1_A6_T2()
        => CompilationFailureTest("S12.6.1_A6_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A6_T3")]
    public Task S12_6_1_A6_T3()
        => CompilationFailureTest("S12.6.1_A6_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A6_T4")]
    public Task S12_6_1_A6_T4()
        => CompilationFailureTest("S12.6.1_A6_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A6_T5")]
    public Task S12_6_1_A6_T5()
        => CompilationFailureTest("S12.6.1_A6_T5", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A12")]
    public Task S12_6_1_A12()
        => CompilationFailureTest("S12.6.1_A12", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A15")]
    public Task S12_6_1_A15()
        => CompilationFailureTest("S12.6.1_A15", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.6.1_A6_T6")]
    public Task S12_6_1_A6_T6()
        => CompilationFailureTest("S12.6.1_A6_T6", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-const")]
    public Task decl_const()
        => CompilationFailureTest("decl-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-fun")]
    public Task decl_fun()
        => CompilationFailureTest("decl-fun", "Failed to parse JavaScript");

    [Fact(DisplayName = "decl-let")]
    public Task decl_let()
        => CompilationFailureTest("decl-let", "Failed to parse JavaScript");

}
