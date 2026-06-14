using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.break_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\break", "language.statements.break_") { }

    [Fact(DisplayName = "12.8-1")]
    public Task _12_8_1()
        => ExecutionTest("12.8-1");

    [Fact(DisplayName = "S12.8_A9_T1")]
    public Task S12_8_A9_T1()
        => ExecutionTest("S12.8_A9_T1");

    [Fact(DisplayName = "S12.8_A9_T2")]
    public Task S12_8_A9_T2()
        => ExecutionTest("S12.8_A9_T2");

    [Fact(DisplayName = "line-terminators")]
    public Task line_terminators()
        => ExecutionTest("line-terminators");
    [Fact(DisplayName = "S12.8_A3")]
    public Task S12_8_A3()
        => ExecutionTest("S12.8_A3");



    [Fact(DisplayName = "S12.8_A4_T1")]
    public Task S12_8_A4_T1()
        => ExecutionTest("S12.8_A4_T1");

    [Fact(DisplayName = "S12.8_A4_T2")]
    public Task S12_8_A4_T2()
        => ExecutionTest("S12.8_A4_T2");

    [Fact(DisplayName = "S12.8_A4_T3")]
    public Task S12_8_A4_T3()
        => ExecutionTest("S12.8_A4_T3");

    [Fact(DisplayName = "S12.8_A1_T1")]
    public Task S12_8_A1_T1()
        => CompilationFailureTest("S12.8_A1_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A1_T2")]
    public Task S12_8_A1_T2()
        => CompilationFailureTest("S12.8_A1_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A5_T1")]
    public Task S12_8_A5_T1()
        => CompilationFailureTest("S12.8_A5_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A6")]
    public Task S12_8_A6()
        => CompilationFailureTest("S12.8_A6", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.8_A5_T2")]
    public Task S12_8_A5_T2()
        => CompilationFailureTest("S12.8_A5_T2", "Failed to parse JavaScript");

}
